using System.Collections.Generic;
using Message;

namespace DotAlram
{
    public enum AlramLevel
    {
        Low,
        Middle,
        High
    }

    /// <summary>
    /// Tree 구조의 알림 요소
    /// 자식 노드에 알림이 추가되면 부모 노드까지 자동으로 알림이 표시됨
    /// </summary>
    public class DotAlramElement
    {
        public int          TypeId      { get; private set; }
        public AlramLevel   AlramLevel  { get; private set; }
        public int          Count       { get; private set; }
        public int          Value       { get; private set; }

        // Tree 구조
        public DotAlramElement          Parent      { get; private set; }
        public List<DotAlramElement>    Children    { get; private set; }

        private ISignalHub _signalHub;

        public DotAlramElement(int typeId, AlramLevel level, int count, int value, ISignalHub signalHub, DotAlramElement parent = null)
        {
            TypeId      = typeId;
            AlramLevel  = level;
            Count       = count;
            Value       = value;
            _signalHub  = signalHub;
            Parent      = parent;
            Children    = new List<DotAlramElement>();

            // 부모에 자식으로 등록
            if (Parent != null)
            {
                Parent.AddChild(this);
            }
        }

        /// <summary>
        /// 자식 노드 추가
        /// </summary>
        public void AddChild(DotAlramElement child)
        {
            if (!Children.Contains(child))
            {
                Children.Add(child);
            }
        }

        /// <summary>
        /// 자식 노드 제거
        /// </summary>
        public void RemoveChild(DotAlramElement child)
        {
            Children.Remove(child);
        }

        /// <summary>
        /// Count 업데이트 (자식 Count 합산 포함)
        /// </summary>
        public void UpdateCount(int count)
        {
            Count = count;
            _signalHub.Get<UpdateDotAlramMessage>().Dispatch(TypeId, Count, Value);

            // 부모 노드에게 자식 Count 변경을 알림 (Bubble up)
            Parent?.RecalculateCount();
        }

        /// <summary>
        /// 자식들의 Count를 합산하여 자신의 Count 재계산
        /// </summary>
        public void RecalculateCount()
        {
            int totalCount = 0;

            foreach (var child in Children)
            {
                totalCount += child.Count;
            }

            // 자신의 Count를 자식들의 합으로 업데이트
            if (Count != totalCount)
            {
                Count = totalCount;
                _signalHub.Get<UpdateDotAlramMessage>().Dispatch(TypeId, Count, Value);

                // 부모에게도 전파
                Parent?.RecalculateCount();
            }
        }

        /// <summary>
        /// 현재 노드가 루트인지 확인
        /// </summary>
        public bool IsRoot => Parent == null;

        /// <summary>
        /// 현재 노드가 리프(말단)인지 확인
        /// </summary>
        public bool IsLeaf => Children.Count == 0;

        /// <summary>
        /// 특정 TypeId를 가진 자식 찾기
        /// </summary>
        public DotAlramElement FindChild(int typeId, int value = 0)
        {
            foreach (var child in Children)
            {
                if (child.TypeId == typeId && child.Value == value)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// 특정 TypeId를 가진 자식을 재귀적으로 찾기
        /// </summary>
        public DotAlramElement FindDescendant(int typeId, int value = 0)
        {
            // 직접 자식 검색
            var directChild = FindChild(typeId, value);
            if (directChild != null)
            {
                return directChild;
            }

            // 재귀적으로 손자 검색
            foreach (var child in Children)
            {
                var descendant = child.FindDescendant(typeId, value);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        /// <summary>
        /// 루트 노드까지 거슬러 올라가기
        /// </summary>
        public DotAlramElement GetRoot()
        {
            var current = this;
            while (current.Parent != null)
            {
                current = current.Parent;
            }
            return current;
        }
    }
}