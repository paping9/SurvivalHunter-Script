using System.Collections.Generic;
using UnityEngine.Pool;

namespace Utils.Pool
{
    public class GenericPool<T> : IPoolClearable where T : class
    {
        private readonly LinkedList<T> _activeList = new();
        private readonly LinkedList<T> _inactiveList = new();
        private readonly IGenericPoolStrategy<T> _strategy;
        private readonly int _maxCount;

        public IReadOnlyCollection<T> ActiveItems => _activeList;
        public int ActiveCount => _activeList.Count;
        public int InactiveCount => _inactiveList.Count;
        public int TotalCount => _activeList.Count + _inactiveList.Count;
        
        public GenericPool(IGenericPoolStrategy<T> strategy, int maxCount)
        {
            _strategy = strategy;
            _maxCount = maxCount;
        }

        public T Get(PopOptionForNotEnough option = PopOptionForNotEnough.None)
        {
            T item = null;

            if (_inactiveList.Count > 0)
            {
                item = _inactiveList.First.Value;
                _inactiveList.RemoveFirst();
            }
            else if (TotalCount < _maxCount)
            {
                item = _strategy.Create();
            }
            else
            {
                // maxCount 넘었을 때의 처리 방식
                switch (option)
                {
                    case PopOptionForNotEnough.ForceReuse:
                        if (_activeList.Count > 0)
                        {
                            item = _activeList.First.Value;
                            _activeList.RemoveFirst();
                            _strategy.OnReuse(item);
                        }
                        break;

                    case PopOptionForNotEnough.ForceCreate:
                        item = _strategy.Create();
                        break;

                    case PopOptionForNotEnough.None:
                    default:
                        return null;
                }
            }

            if (item != null)
            {
                _strategy.OnGet(item);
                _activeList.AddLast(item);
            }

            return item;
        }

        public void Release(T item)
        {
            _activeList.Remove(item);
            _strategy.OnRelease(item);
            _inactiveList.AddLast(item);
        }

        public void ForceRemove(T item)
        {
            if (_inactiveList.Remove(item))
            {
                _strategy.OnDestroy(item);
            }
        }

        public void Clear()
        {
            foreach (var item in _inactiveList)
                _strategy.OnDestroy(item);
            _inactiveList.Clear();

            foreach (var item in _activeList)
                _strategy.OnDestroy(item);
            _activeList.Clear();
        }
    }
}