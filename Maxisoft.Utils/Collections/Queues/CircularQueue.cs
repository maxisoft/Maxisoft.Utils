using System;
using System.Collections;
using System.Collections.Generic;

namespace Maxisoft.Utils.Collections.Queues
{
    public class CircularQueue<T> : ICollection<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _size;

        public CircularQueue(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "must be positive number");
            }

            _queue = new Queue<T>(size);
            _size = size;
        }

        public void Add(T obj)
        {
            if (_queue.Count == _size)
            {
                _queue.Dequeue();
                _queue.Enqueue(obj);
            }
            else
            {
                _queue.Enqueue(obj);
            }
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public bool Contains(T item)
        {
            return _queue.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _queue.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (Count <= 0)
            {
                return false;
            }

            if (Peek().Equals(item))
            {
                Read();
                return true;
            }

            if (Contains(item))
            {
                throw new NotSupportedException();
            }

            return false;
        }

        public int Count => _queue.Count;
        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public T Read()
        {
            return _queue.Dequeue();
        }

        public T Peek()
        {
            return _queue.Peek();
        }
    }
}