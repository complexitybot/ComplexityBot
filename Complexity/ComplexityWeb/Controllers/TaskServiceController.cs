using System;
using System.Collections.Generic;
using System.Linq;
using Application;
using AutoMapper;
using ComplexityWebApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ComplexityWebApi.Controllers
{
    [Route("service")]
    [ApiController]
    public class TaskServiceController : ControllerBase
    {
        private readonly ITaskService applicationApi;

        /// <summary>
        ///     Получить список всех Topic.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET service/topics
        /// </remarks>
        /// <response code="200"> Возвращает список тем</response>
        [HttpGet("topics")]
        public ActionResult<IEnumerable<TopicInfoDTO>> GetTopics()
        {
            var topics = applicationApi.GetAllTopics();
            //ToDo new DTO with Levels...
            return Ok(topics.Select(Mapper.Map<TopicInfoDTO>));
        }

        /// <summary>
        ///     Добавляет в сервис новый пустой Topic.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST service/addTopic
        ///     {
        ///         "name": "Сложность алгоритмов",
        ///         "description": "Оценка сложностей алгоритмов"
        ///     }
        /// </remarks>
        /// <response code="200"> Возвращает Guid от нового Topic</response>
        [HttpPost("addTopic")]
        public ActionResult<Guid> AddEmptyTopic([FromBody] TopicWithDescriptionDTO topic)
        {
            var topicGuid = applicationApi.AddEmptyTopic(topic.Name, topic.Description);
            return Ok(topicGuid);
        }

        /// <summary>
        ///     Удаляет Topic из сервиса.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     DELETE service/deleteTopic/1
        /// </remarks>
        /// <response code="200"> Topic был удален</response>
        [HttpDelete("deleteTopic/{topicId}")]
        public ActionResult DeleteTopic(Guid topicId)
        {
            var (_, _) = applicationApi.DeleteTopic(topicId);
            return Ok();
        }

        /// <summary>
        ///     Добавляет в сервис новый пустой Level.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST service/addLevel/0
        ///     {
        ///         "description": "Оценка сложностей алгоритмов",
        ///         "next_levels": [0, 1],
        ///         "previous_levels": [2, 3]
        ///     }
        /// </remarks>
        /// <response code="200"> Возвращает Guid от нового Level</response>
        [HttpPost("addLevel/{topicId}")]
        public ActionResult<Guid> AddLevel(Guid topicId, [FromBody] DataBaseLevelDTO level)
        {
            var (levelGuid, _) = applicationApi.AddEmptyLevel(topicId, level.Description, level.PreviousLevels, level.NextLevels);
            return Ok(levelGuid);
        }

        /// <summary>
        ///     Удаляет Level из сервиса.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     DELETE service/deleteLevel/1/0
        /// </remarks>
        /// <response code="200"> Level был удален</response>
        [HttpDelete("deleteLevel/{topicId}/{levelId}")]
        public ActionResult DeleteLevel(Guid topicId, Guid levelId)
        {
            var (_, _) = applicationApi.DeleteLevel(topicId, levelId);
            return Ok();
        }

        /// <summary>
        ///     Добавляет в сервис новый TemplateGenerator.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST service/addTemplateGenerator/1/0
        ///     {
        ///        "template": "for (int i = {{from1}}; i < {{to1}}; i += {{iter1}})\r\nc++\r\n",
        ///        "possibleAnswers": ["Θ(1)", "Θ(log(n))"],
        ///        "rightAnswer": "Θ(n)",
        ///        "hints": [],
        ///        "streak": 1
        ///     }
        /// </remarks>
        /// <response code="200"> Возвращает Guid от нового TemplateGenerator</response>
        [HttpPost("addTemplateGenerator/{topicId}/{levelId}")]
        public ActionResult<Guid> AddTemplateGenerator(Guid topicId, Guid levelId, [FromBody] DataBaseTemplateGeneratorWithStreakDTO templateGenerator)
        {
            var (generatorGuid, _) = applicationApi.AddTemplateGenerator(topicId, levelId, templateGenerator.Template, templateGenerator.PossibleAnswers,
                templateGenerator.RightAnswer, templateGenerator.Hints, templateGenerator.Streak);
            return Ok(generatorGuid);
        }
        
        /// <summary>
        ///     Удаляет Generator из сервиса.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     DELETE service/deleteGenerator/1/0/2
        /// </remarks>
        /// <response code="200"> Generator был удален</response>
        [HttpDelete("deleteGenerator/{topicId}/{levelId}/{generatorId}")]
        public ActionResult DeleteGenerator(Guid topicId, Guid levelId, Guid generatorId)
        {
            var (_, _) = applicationApi.DeleteGenerator(topicId, levelId, generatorId);
            return Ok();
        }

        /// <summary>
        ///     Рендерит и возвращает Task по шаблону полученому в запросе
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST service/renderTemplateGenerator
        ///     {
        ///        "template": "for (int i = {{from1}}; i < {{to1}}; i += {{iter1}})\r\nc++\r\n",
        ///        "possibleAnswers": ["Θ(1)", "Θ(log(n))"],
        ///        "rightAnswer": "Θ(n)",
        ///        "hints": []
        ///     }
        /// </remarks>
        /// <response code="200"> Возвращает отрендереный Task</response>
        [HttpPost("renderTemplateGenerator")]
        public ActionResult RenderTemplateGenerator([FromBody] DataBaseTemplateGeneratorDTO templateGenerator)
        {
            var task = applicationApi.RenderTask(templateGenerator.Template, templateGenerator.PossibleAnswers,
                templateGenerator.RightAnswer, templateGenerator.Hints);

            return Ok(task);
        }
    }
}