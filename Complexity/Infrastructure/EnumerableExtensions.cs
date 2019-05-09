﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure {
    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> Zip<T1, T2, T3, TResult>(
            this IEnumerable<T1> source,
            IEnumerable<T2> second,
            IEnumerable<T3> third,
            Func<T1, T2, T3, TResult> func)
        {
            using (var e1 = source.GetEnumerator())
                using (var e2 = second.GetEnumerator())
                    using (var e3 = third.GetEnumerator())
                    {
                        while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                            yield return func(e1.Current, e2.Current, e3.Current);
                    }
        }
        private static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, Random random)
        {
            random = random ?? new Random(42);
            return enumerable.OrderBy(_ => random.Next());
        }

        public static IEnumerable<T> TakeRandom<T>(
            this IEnumerable<T> enumerable,
            int amount = 1,
            Random random = default)
        {
            return enumerable.Shuffle(random).Take(amount);
        }

        public static T TakeRandomOne<T>(
            this IEnumerable<T> enumerable,
            Random random = default)
        {
            return enumerable.Shuffle(random).Take(1).First();
        }
    }
}