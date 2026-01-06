using System;
using System.Collections.Generic;

namespace Utils
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private readonly List<T> _heap;

        public int Count => _heap.Count;

        public PriorityQueue()
        {
            _heap = new List<T>();
        }

        public PriorityQueue(List<T> heap)
        {
            _heap = heap;
        }

        public void Enqueue(T item)
        {
            _heap.Add(item);
            var childIndex = _heap.Count - 1;
            var parentIndex = (childIndex - 1) / 2;

            while (childIndex > 0 && _heap[childIndex].CompareTo(_heap[parentIndex]) > 0)
            {
                Swap(childIndex, parentIndex);
                childIndex = parentIndex;
                parentIndex = (childIndex - 1) / 2;
            }
        }

        public T Dequeue()
        {
            var lastIndex = _heap.Count - 1;
            var frontItem = _heap[0];
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            lastIndex--;

            var parentIndex = 0;

            while (true)
            {
                var leftChildIndex = parentIndex * 2 + 1;
                var rightChildIndex = parentIndex * 2 + 2;

                if (leftChildIndex > lastIndex)
                    break;

                var childIndex = leftChildIndex;

                if (rightChildIndex <= lastIndex && _heap[rightChildIndex].CompareTo(_heap[leftChildIndex]) > 0)
                    childIndex = rightChildIndex;

                if (_heap[parentIndex].CompareTo(_heap[childIndex]) >= 0)
                    break;

                Swap(parentIndex, childIndex);
                parentIndex = childIndex;
            }

            return frontItem;
        }

        public T Peek()
        {
            var frontItem = _heap[0];
            return frontItem;
        }

        public bool Contains(T item)
        {
            return _heap.Contains(item);
        }

        public void Clear()
        {
            _heap.Clear();
        }

        private void Swap(int index1, int index2)
        {
            (_heap[index1], _heap[index2]) = (_heap[index2], _heap[index1]);
        }
    }
}
