﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities.TaskGenerators;
using Infrastructure.Extensions;
using Infrastructure.Result;

namespace Application.Selectors
{
    public class RandomTaskGeneratorSelector : ITaskGeneratorSelector
    {
        private readonly Random random;

        public RandomTaskGeneratorSelector(Random random)
        {
            this.random = random;
        }

        public Result<TaskGenerator, Exception> SelectGenerator(
            IEnumerable<TaskGenerator> generators,
            Dictionary<Guid, int> streaks)
        {
            var generatorsArray = generators as TaskGenerator[] ?? generators.ToArray();
            return generatorsArray.Length == 0 
                ? new ArgumentOutOfRangeException(nameof(generators), $"{nameof(generators)} must be not empty") 
                : generatorsArray[random.Next(generatorsArray.Length)].Ok();
        }
    }
}