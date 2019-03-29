﻿using System;
using System.Collections.Generic;
using System.Linq;
using Application.Exceptions;
using Application.Info;
using DataBase;
using DataBase.Entities;
using Domain.Values;

namespace Application
{
    public class Application : IApplicationApi
    {
        //TODO: build asp.net, database etc.
        static Application()
        {
            //Какое-то странное что-то, просто пример
        }

        private readonly ITaskRepository taskRepository;
        private readonly IUserRepository userRepository;
        private readonly ITaskGeneratorSelector generatorSelector;
        private readonly Random random = new Random();

        public Application(
            IUserRepository userRepository,
            ITaskRepository taskRepository,
            ITaskGeneratorSelector generatorSelector)
        {
            this.userRepository = userRepository;
            this.taskRepository = taskRepository;
            this.generatorSelector = generatorSelector;
        }

        public IEnumerable<TopicInfo> GetTopicsInfo()
        {
            return taskRepository.GetTopics().Select(topic => topic.ToInfo());
        }

        public IEnumerable<LevelInfo> GetLevels(Guid topicId)
        {
            CheckTopic(topicId);
            return taskRepository.GetLevelsFromTopic(topicId).Select(level => level.ToInfo());
        }

        public IEnumerable<LevelInfo> GetAvailableLevels(Guid userId, Guid topicId)
        {
            CheckTopic(topicId);
            return userRepository.FindOrInsertUser(userId, taskRepository)
                .UserProgressEntity
                .TopicsProgress[topicId]
                .LevelProgressEntities
                .Select(levelProgress =>
                    taskRepository
                        .FindLevel(topicId, levelProgress.Key)
                        .ToInfo());
        }

        public double GetCurrentProgress(Guid userId, Guid topicId, Guid levelId)
        {
            CheckLevel(topicId, levelId);
            var user = userRepository.FindOrInsertUser(userId, taskRepository);
            var solved = user
                .UserProgressEntity
                .TopicsProgress[topicId]
                .LevelProgressEntities[levelId]
                .CurrentLevelStreaks
                .Count(pair => IsGeneratorSolved(user, topicId, levelId, pair.Key));
            return (double) solved / taskRepository.GetGeneratorsFromLevel(topicId, levelId).Length;
        }

        public TaskInfo GetTask(Guid userId, Guid topicId, Guid levelId)
        {
            CheckLevel(topicId, levelId);
            var user = userRepository.FindOrInsertUser(userId, taskRepository);
            if (!GetAvailableLevels(userId, topicId).Select(info => info.Id).Contains(levelId))
                throw new AccessDeniedException(
                    $"User {userId} doesn't have access to level {levelId} in topic {topicId}");
            var task = generatorSelector
                .Select(taskRepository.GetGeneratorsFromLevel(topicId, levelId))
                .GetTask(random);
            UpdateUserCurrentTask(user, topicId, levelId, task);
            return task.ToInfo();
        }

        public TaskInfo GetNextTask(Guid userId)
        {
            var user = userRepository.FindOrInsertUser(userId, taskRepository);
            CheckCurrentTask(user);
            if (!user.UserProgressEntity.CurrentTask.IsSolved)
                throw new AccessDeniedException($"User {userId} should solve current task first");
            return GetTask(userId, user.UserProgressEntity.CurrentTopicId, user.UserProgressEntity.CurrentLevelId);
        }

        public bool CheckAnswer(Guid userId, string answer)
        {
            var user = userRepository.FindOrInsertUser(userId, taskRepository);
            CheckCurrentTask(user);
            if (user.UserProgressEntity.CurrentTask.Answer != answer)
            {
                UpdateStreakIfNotSolved(user, streak => 0);
                return false;
            }
            user.UserProgressEntity.CurrentTask.IsSolved = true;
            UpdateStreakIfNotSolved(user, streak => streak + 1);
            UpdateProgressIfLevelSolved(user);
            userRepository.Update(user);
            return true;
        }

        public string GetHint(Guid userId)
        {
            var user = userRepository.FindOrInsertUser(userId, taskRepository);
            CheckCurrentTask(user);
            var hints = user.UserProgressEntity.CurrentTask.Hints;
            var currentHintIndex = user.UserProgressEntity.CurrentTask.HintsTaken;
            if (currentHintIndex >= hints.Length)
                return null;
            user.UserProgressEntity.CurrentTask.HintsTaken++;
            userRepository.Update(user);
            return hints[currentHintIndex];
        }

        private void UpdateProgressIfLevelSolved(UserEntity user)
        {
            var topicId = user.UserProgressEntity.CurrentTopicId;
            var levelId = user.UserProgressEntity.CurrentLevelId;
            var allSolved = taskRepository
                .GetGeneratorsFromLevel(topicId, levelId)
                .All(generator => IsGeneratorSolved(user, topicId, levelId, generator.Id));
            if (!allSolved)
                return;
            var level = taskRepository
                .GetLevelsFromTopic(topicId)
                .SkipWhile(l => l.Id != levelId)
                .Skip(1)
                .FirstOrDefault();
            if (level is null)
                return;
            user.UserProgressEntity
                .TopicsProgress[topicId]
                .LevelProgressEntities[level.Id] = taskRepository.GetLevelProgressEntity(levelId, topicId);
        }

        private bool IsGeneratorSolved(UserEntity user, Guid topicId, Guid levelId, Guid generatorId)
        {
            var currentStreak = user
                .UserProgressEntity
                .TopicsProgress[topicId]
                .LevelProgressEntities[levelId]
                .CurrentLevelStreaks[generatorId];
            return currentStreak >= taskRepository.FindGenerator(topicId, levelId, generatorId).Streak;
        }

        private void UpdateUserCurrentTask(UserEntity user, Guid topicId, Guid levelId, Task task)
        {
            user.UserProgressEntity.CurrentTopicId = topicId;
            user.UserProgressEntity.CurrentLevelId = levelId;
            user.UserProgressEntity.CurrentTask = new TaskInfoEntity
            {
                Question = task.Question,
                Answer = task.Answer,
                Hints = task.Hints,
                HintsTaken = 0,
                IsSolved = false,
                ParentGeneratorId = task.ParentGeneratorId
            };
            userRepository.Update(user);
        }

        private void UpdateStreakIfNotSolved(UserEntity user, Func<int, int> updateFunc)
        {
            var topicId = user.UserProgressEntity.CurrentTopicId;
            var levelId = user.UserProgressEntity.CurrentLevelId;
            var generatorId = user.UserProgressEntity.CurrentTask.ParentGeneratorId;
            var currentStreak = user
                .UserProgressEntity
                .TopicsProgress[topicId]
                .LevelProgressEntities[levelId]
                .CurrentLevelStreaks[generatorId];
            if (!IsGeneratorSolved(user, topicId, levelId, generatorId))
            {
                user.UserProgressEntity
                    .TopicsProgress[topicId]
                    .LevelProgressEntities[levelId]
                    .CurrentLevelStreaks[generatorId] = updateFunc(currentStreak);
            }
        }

        private void CheckTopic(Guid topicId)
        {
            if (taskRepository.FindTopic(topicId) is null)
                throw new TopicNotFoundException();
        }

        private void CheckLevel(Guid topicId, Guid levelId)
        {
            CheckTopic(topicId);
            if (taskRepository.FindLevel(topicId, levelId) is null)
                throw new LevelNotFoundException();
        }

        private static void CheckCurrentTask(UserEntity user)
        {
            if (user.UserProgressEntity.CurrentTask is null)
                throw new AccessDeniedException($"User {user.Id} hadn't started any task");
        }
    }
}