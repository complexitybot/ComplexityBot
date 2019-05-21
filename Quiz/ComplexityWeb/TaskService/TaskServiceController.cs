using System;
using System.Collections.Generic;
using System.Linq;
using Application.TaskService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuizWebApp.TaskService.DTO;

namespace QuizWebApp.TaskService
{
    [Route("service")]
    [ApiController]
    public class TaskServiceController : ControllerBase
    {
        private readonly ITaskService applicationApi;

        public TaskServiceController(ITaskService applicationApi)
        {
            this.applicationApi = applicationApi;
        }

        /// <summary>
        ///     Получить список тем
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     GET service/topics
        ///     </code>
        /// </remarks>
        /// <response code="200"> Возвращает список тем</response>
        [HttpGet("topics")]
        public ActionResult<IEnumerable<AdminTopicDTO>> GetTopics()
        {
            var topics = applicationApi.GetAllTopics();
            return Ok(topics.Select(Mapper.Map<AdminTopicDTO>));
        }

        /// <summary>
        ///     Получить уровень по id
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     GET service/0/level/1
        ///     </code>
        /// </remarks>
        /// <response code="200"> Возвращает уровень</response>
        /// <response code="404"> Id темы или уровня не найдены</response>
        [HttpGet("{topicId}/level/{levelId}")]
        public ActionResult<AdminLevelDTO> GetLevel(Guid topicId, Guid levelId)
        {
            var (_, isFailure, level, error) = applicationApi.GetLevel(topicId, levelId);
            if (isFailure)
                return NotFound(error.Message);
            return Ok(level);
        }

        /// <summary>
        ///     Получить генератор по id
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     GET service/0/1/templateGenerator/2
        ///     </code>
        /// </remarks>
        /// <response code="200"> Возвращает генератор</response>
        /// <response code="404"> Id темы, уровня или генератора не найдены</response>
        [HttpGet("{topicId}/{levelId}/templateGenerator/{generatorId}")]
        public ActionResult<AdminTaskGeneratorDTO> GetTemplateGenerator(Guid topicId, Guid levelId, Guid generatorId)
        {
            var (_, isFailure, generator, error) = applicationApi.GetTemplateGenerator(topicId, levelId, generatorId);
            if (isFailure)
                return NotFound(error.Message);
            return Ok(generator);
        }

        /// <summary>
        ///     Добавляет в сервис новый пустую тему
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     POST service/addTopic
        ///     {
        ///         "name": "Сложность алгоритмов",
        ///         "description": "Оценка сложностей алгоритмов"
        ///     }
        ///     </code>
        /// </remarks>
        /// <response code="200"> Возвращает Id от новой темы</response>
        [HttpPost("topic")]
        public ActionResult<Guid> AddEmptyTopic([FromBody] EmptyTopicDTO topic)
        {
            var topicGuid = applicationApi.AddEmptyTopic(topic.Name, topic.Description);
            return Ok(topicGuid);
        }

        /// <summary>
        ///     Удаляет тему из сервиса
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     DELETE service/deleteTopic/1
        ///     </code>
        /// </remarks>
        /// <response code="200"> Тема была успешно удалена</response>
        /// <response code="404"> Id темы не найден</response>
        [HttpDelete("topic/{topicId}")]
        public ActionResult DeleteTopic(Guid topicId)
        {
            var (_, isFailure, _, error) = applicationApi.DeleteTopic(topicId);
            if (isFailure)
                return NotFound(error.Message);
            return Ok();
        }

        /// <summary>
        ///     Добавляет в сервис пустой уровень
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     POST service/0/addLevel
        ///     {
        ///         "description": "Оценка сложности алгоритмов",
        ///         "nextLevels": [0, 1],
        ///         "previousLevels": [2, 3]
        ///     }
        ///     </code>
        /// </remarks>
        /// <response code="200"> Возвращает Id нового уровня</response>
        /// <response code="404"> Id темы не найден</response>
        [HttpPost("{topicId}/level")]
        public ActionResult<Guid> AddLevel(Guid topicId, [FromBody] EmptyLevelDTO level)
        {
            var (_, isFailure, id, error) = applicationApi
                .AddEmptyLevel(
                    topicId,
                    level.Description,
                    level.PreviousLevels,
                    level.NextLevels);
            if (isFailure)
                return NotFound(error.Message);
            return Ok(id);
        }

        /// <summary>
        ///     Удаляет уровень из сервиса.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     DELETE service/1/deleteLevel/1
        ///     </code>
        /// </remarks>
        /// <response code="200"> уровень был успешно удален</response>
        /// <response code="404"> Id темы или уровня не найдены</response>
        [HttpDelete("{topicId}/level/{levelId}")]
        public ActionResult DeleteLevel(Guid topicId, Guid levelId)
        {
            var (_, isFailure, _, error) = applicationApi.DeleteLevel(topicId, levelId);
            if (isFailure)
                return NotFound(error.Message);
            return Ok();
        }

        /// <summary>
        ///     Добавляет в сервис новый генератор
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     POST service/addTemplateGenerator/1/0
        ///     {
        ///        "question": "Оцените временную сложность алгоритма",
        ///        "text": "for (int i = {{from1}}; i &lt; {{to1}}; i += {{iter1}})\r\nc++\r\n",
        ///        "possibleAnswers": ["Θ(1)", "Θ(log({{to1}}))", "Θ({{to1}})"],
        ///        "answer": "Θ({{to1}})",
        ///        "hints": [],
        ///        "streak": 1
        ///     }
        ///     </code>
        /// </remarks>
        /// <response code="200"> Возвращает Id нового генератора</response>
        /// <response code="404"> Id темы или уровня не найдены</response>
        [HttpPost("{topicId}/{levelId}/templateGenerator")]
        public ActionResult<Guid> AddTemplateGenerator(
            Guid topicId,
            Guid levelId,
            [FromBody] TemplateGeneratorDTO templateGenerator)
        {
            var (_, isFailure, id, error) = applicationApi
                .AddTemplateGenerator(
                    topicId,
                    levelId,
                    templateGenerator.Text,
                    templateGenerator.PossibleAnswers,
                    templateGenerator.Answer, templateGenerator.Hints, templateGenerator.Streak,
                    templateGenerator.Question);
            if (isFailure)
                return NotFound(error.Message);
            return Ok(id);
        }

        /// <summary>
        ///     Удаляет генератор из сервиса.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     DELETE service/1/0/deleteGenerator/2
        ///     </code>
        /// </remarks>
        /// <response code="200"> Генератор был успешно удален</response>
        /// <response code="404"> Id темы, уровня или генератора не найдены</response>
        [HttpDelete("{topicId}/{levelId}/generator/{generatorId}")]
        public ActionResult DeleteGenerator(Guid topicId, Guid levelId, Guid generatorId)
        {
            var (_, isFailure, _, error) = applicationApi.DeleteGenerator(topicId, levelId, generatorId);
            if (isFailure)
                return NotFound(error.Message);
            return Ok();
        }

        /// <summary>
        ///     Рендерит и возвращает задачу по шаблону из запроса
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     <code>
        ///     POST service/taskToRender
        ///     {
        ///        "question": "Оцените временную сложность алгоритма",
        ///        "text": "for (int i = {{from1}}; i &lt; {{to1}}; i += {{iter1}})\r\nc++\r\n",
        ///        "possibleAnswers": ["Θ(1)", "Θ(log({{to1}}))", "Θ({{to1}})"],
        ///        "answer": "Θ({{to1}})",
        ///        "hints": []
        ///     }
        ///     </code>
        /// </remarks>
        /// <response code="200"> Возвращает отрендеренную задачу</response>
        [HttpPost("taskToRender")]
        public ActionResult RenderTask([FromBody] TemplateGeneratorForRenderDTO templateGenerator)
        {
            var task = applicationApi.RenderTask(
                templateGenerator.Text,
                templateGenerator.PossibleAnswers,
                templateGenerator.Answer,
                templateGenerator.Hints,
                templateGenerator.Question);

            return Ok(task);
        }
    }
}