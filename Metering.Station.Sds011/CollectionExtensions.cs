using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metering.Station.Sds011
{
    internal static class CollectionExtensions
    {
        public static T GetValueOrDefault<T>(this T[] array, int index, T defaultValue)
        {
            if (index < array.Length)
                return array[index];
            return defaultValue;
        }

        public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var item in items)
                queue.Enqueue(item);
        }
    }
}
