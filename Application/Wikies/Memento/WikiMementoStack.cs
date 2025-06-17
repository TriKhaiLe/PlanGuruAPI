using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Application.Wikies.Memento
{
    // Singleton stack for storing mementos per wiki
    public class WikiMementoStack
    {
        private static readonly Lazy<WikiMementoStack> _instance = new(() => new WikiMementoStack());
        public static WikiMementoStack Instance => _instance.Value;

        // Key: WikiId, Value: Stack of mementos
        private readonly ConcurrentDictionary<Guid, Stack<WikiMemento>> _mementos = new();

        private WikiMementoStack() { }

        public void Push(Guid wikiId, WikiMemento memento)
        {
            var stack = _mementos.GetOrAdd(wikiId, _ => new Stack<WikiMemento>());
            lock (stack)
            {
                // Limit to 5 mementos per wikiId
                while (stack.Count >= 5)
                {
                    // reverse stack to remove the oldest memento
                    var temp = new Stack<WikiMemento>();
                    while (stack.Count > 0) temp.Push(stack.Pop());

                    temp.Pop();
                    while (temp.Count > 0) stack.Push(temp.Pop());
                }
                stack.Push(memento);
            }
        }

        public WikiMemento? Pop(Guid wikiId)
        {
            if (_mementos.TryGetValue(wikiId, out var stack))
            {
                lock (stack)
                {
                    if (stack.Count > 0)
                        return stack.Pop();
                }
            }
            return null;
        }

        public bool HasUndo(Guid wikiId)
        {
            return _mementos.TryGetValue(wikiId, out var stack) && stack.Count > 0;
        }
    }
}