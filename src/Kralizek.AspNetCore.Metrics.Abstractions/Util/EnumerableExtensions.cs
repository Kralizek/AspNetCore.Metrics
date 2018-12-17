using System;
using System.Collections.Generic;
using System.Linq;

namespace Kralizek.AspNetCore.Metrics.Abstractions.Util
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> enumerable, int chunkSize)
        {
            if (chunkSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize), $"{nameof(chunkSize)} can't be less than 1");
            }

            using (var e = enumerable.GetEnumerator())
            {
                int currentIndex = 0;
                T[] items = new T[chunkSize];

                while (e.MoveNext())
                {
                    items[currentIndex++] = e.Current;

                    if (currentIndex == chunkSize)
                    {
                        currentIndex = 0;
                        yield return items;
                        items = new T[chunkSize];
                    }
                }

                if (currentIndex > 0)
                {
                    yield return items.Take(currentIndex).ToArray();
                }
            }
        }
    }
}