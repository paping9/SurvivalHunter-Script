using System.Collections.Generic;
using UnityEngine;

namespace DotAlram
{
    /// <summary>
    /// 모든 알림 요소를 flat하게 관리하는 컨트롤러
    /// Tree 구조는 Logic 레이어에서 DotAlramElement 생성 시 구성
    /// </summary>
    public class DotAlramController
    {
        // 모든 알림 요소 관리 (Key: TypeId_Value)
        private Dictionary<string, DotAlramElement> _elements = new();

        /// <summary>
        /// TypeId와 Value로 고유 키 생성
        /// </summary>
        private string GetKey(int typeId, int value) => $"{typeId}_{value}";

        /// <summary>
        /// 알림 요소 등록
        /// </summary>
        public void RegisterElement(DotAlramElement element)
        {
            var key = GetKey(element.TypeId, element.Value);
            _elements[key] = element;
        }

        /// <summary>
        /// 알림 요소 조회
        /// </summary>
        public DotAlramElement GetElement(int alramTypeId, int value = 0)
        {
            var key = GetKey(alramTypeId, value);
            return _elements.TryGetValue(key, out var element) ? element : null;
        }

        /// <summary>
        /// 모든 알림 요소 조회
        /// </summary>
        public IEnumerable<DotAlramElement> GetAllElements()
        {
            return _elements.Values;
        }

        /// <summary>
        /// 알림 카운트 증가
        /// </summary>
        public void AddAlram(DotAlramElement element, int count = 1)
        {
            if (element == null)
            {
                Debug.LogWarning("[DotAlramController] Element is null!");
                return;
            }

            var newCount = element.Count + count;
            element.UpdateCount(newCount);
        }

        /// <summary>
        /// 알림 카운트 감소
        /// </summary>
        public void RemoveAlram(DotAlramElement element, int count = 1)
        {
            if (element == null)
            {
                Debug.LogWarning("[DotAlramController] Element is null!");
                return;
            }

            var newCount = Mathf.Max(0, element.Count - count);
            element.UpdateCount(newCount);
        }

        /// <summary>
        /// 알림 완전 제거 (카운트를 0으로)
        /// </summary>
        public void ClearAlram(DotAlramElement element)
        {
            if (element == null)
            {
                Debug.LogWarning("[DotAlramController] Element is null!");
                return;
            }

            element.UpdateCount(0);
        }

        /// <summary>
        /// 특정 알림 요소 제거
        /// </summary>
        public void RemoveElement(int typeId, int value = 0)
        {
            var key = GetKey(typeId, value);
            if (_elements.TryGetValue(key, out var element))
            {
                element.UpdateCount(0);
                _elements.Remove(key);
            }
        }

        /// <summary>
        /// 모든 알림 제거
        /// </summary>
        public void Clear()
        {
            foreach (var element in _elements.Values)
            {
                element.UpdateCount(0);
            }
            _elements.Clear();
        }
    }
}