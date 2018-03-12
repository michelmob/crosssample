using System;
using System.Collections.Generic;
using System.Linq;

namespace Gravity
{
    public static class Extensions
    {
        public static Dictionary<int, string> ToDictionary(this Enum enumType)
        {
            var type = enumType.GetType();
            return Enum.GetValues(type).Cast<int>().ToDictionary(e => e, e => Enum.GetName(type, e));
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            Func<TValue> valueFactory)
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            return dict.TryGetValue(key, out var value)
                ? value
                : dict[key] = valueFactory();
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childrenFunc)
        {
            items = items ?? throw new ArgumentNullException(nameof(items));
            childrenFunc = childrenFunc ?? throw new ArgumentNullException(nameof(childrenFunc));
            
            var stack = new Stack<T>(items);

            while (stack.Count > 0)
            {
                var item = stack.Pop();

                yield return item;

                var children = childrenFunc(item);

                if (children != null)
                {
                    foreach (var child in childrenFunc(item))
                    {
                        stack.Push(child);
                    }
                }
            }
        }

        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }
    }
}
