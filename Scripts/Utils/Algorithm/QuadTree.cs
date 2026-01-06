using System;
using System.Collections.Generic;

using UnityEngine;

namespace Utils
{
    public class QuadTree<T> where T : MonoBehaviour
    {
        private readonly int _capacity;
        private readonly int _maxDepth;
        private readonly Rect _rect;
        private readonly QuadTree<T>[] _nodes;
        private readonly List<T> _objects;

        public QuadTree(Rect rect, int capacity, int maxDepth)
        {
            _capacity = capacity;
            _maxDepth = maxDepth;
            _rect = rect;
            _nodes = new QuadTree<T>[4];
            _objects = new List<T>(_capacity);
        }

        public void Insert(T obj, Rect rect)
        {
            if (!_rect.Overlaps(rect)) return;

            if(_objects.Count < _capacity || _maxDepth == 0)
            {
                _objects.Add(obj);
                return;
            }

            if(_nodes[0] == null)
            {
                Split();
            }

            for(int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i].Insert(obj, rect);
            }
        }

        public List<T> Query(Rect rect)
        {
            List<T> result = new List<T>();

            if (!_rect.Overlaps(rect)) return result;

            foreach(T obj in _objects)
            {
                if(rect.Contains(obj.transform.position))
                {
                    result.Add(obj);
                }
            }

            if(_nodes[0] != null)
            {
                for(int i = 0; i < _nodes.Length; i++)
                {
                    result.AddRange(_nodes[i].Query(rect));
                }
            }

            return result;
        }

        private void Split()
        {
            float subWidth = _rect.width / 2f;
            float subHeight = _rect.height / 2f;

            _nodes[0] = new QuadTree<T>(new Rect(_rect.x, _rect.y, subWidth, subHeight), _capacity, _maxDepth - 1);
            _nodes[1] = new QuadTree<T>(new Rect(_rect.x + subWidth, _rect.y, subWidth, subHeight), _capacity, _maxDepth - 1);
            _nodes[2] = new QuadTree<T>(new Rect(_rect.x, _rect.y + subHeight, subWidth, subHeight), _capacity, _maxDepth - 1);
            _nodes[3] = new QuadTree<T>(new Rect(_rect.x + subWidth, _rect.y + subHeight, subWidth, subHeight), _capacity, _maxDepth - 1);

        }
    }
}
