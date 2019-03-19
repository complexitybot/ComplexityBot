﻿using System;
using System.Collections.Generic;

namespace Application
{
    public interface IApplicationApi
    {
        /// <summary>
        /// Получение списка названий тем
        /// </summary>
        /// <returns>
        /// Название и id темы
        /// </returns>
        IEnumerable<TopicInfo> GetTopicsInfo();

        /// <summary>
        /// Получение количество возможных уровней сложностей в данной теме 
        /// </summary>
        /// <returns>
        /// Количество уровней сложности
        /// </returns>
        int GetDifficultiesCount(Guid topicId);

        /// <summary>
        /// Получение номера максимального доступного уровня сложности в данной теме для данного уровня 
        /// </summary>
        /// <returns>
        /// Максимальный доступный уровень сложности для пользователя
        /// </returns>
        int GetMaxAvailableDifficulty(Guid userId, Guid topicId);


        /// <summary>
        /// Получение перечисления описаний заданий в данной теме в конкретном уровню сложности
        /// </summary>
        /// <returns>
        /// Перечисление информации о возможных заданиях
        /// </returns>
        IEnumerable<string> GetDifficultyDescription(Guid topicId, int difficulty);

        /// <summary>
        /// Прогресс пользователя в текущих теме и уровне сложности
        /// </summary>
        /// <returns>
        /// Прогресс в процентах
        /// </returns>
        int GetCurrentProgress(Guid userId);

        /// <summary>
        /// Получение задачи из конкретной темы по данной сложности
        /// </summary>
        /// <returns>
        /// Описание задачи с вариантами ответов
        /// </returns>
        TaskInfo GetTask(Guid userId, Guid topicId, int difficulty);

        /// <summary>
        /// Получение следующей задачи из текущих темы и уровня сложности
        /// </summary>
        /// <returns>
        /// Описание задачи с вариантами ответов
        /// </returns>
        TaskInfo GetNextTask(Guid userId);

        /// <summary>
        /// Получение подобной задачи от текущего генератора
        /// </summary>
        /// <returns>Описание задачи с вариантами ответов</returns>
        TaskInfo GetSimilarTask(Guid userId);

        /// <summary>
        /// Проверка правильности ответа на текущую задачу
        /// </summary>
        bool CheckAnswer(Guid userId, string answer);

        /// <summary>
        /// Получение подсказки для текущей задачи
        /// </summary>
        string GetHint(Guid userId);
    }
}