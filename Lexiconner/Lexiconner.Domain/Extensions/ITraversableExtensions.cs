using Lexiconner.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexiconner.Domain.Extensions
{
    public static class ITraversableExtensions
    {
        public static IEnumerable<T> Flatten<T>(this ITraversable<T> root) where T : ITraversable<T>
        {
            var stack = new Stack<T>();
            stack.Push(root.Current);
            while(stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                foreach (var item in current.Children)
                {
                    stack.Push(item);
                }
            }
        }
    }
}
