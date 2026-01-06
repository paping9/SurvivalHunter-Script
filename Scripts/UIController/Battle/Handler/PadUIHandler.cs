using Defs;
using UnityEngine;

namespace UIController
{
    /// <summary>
    /// UI Pad 에서 발생하는 Event 에 대한 처리
    /// </summary>
    public class PadUIHandler : IUIHandler
    {
        
        public void Init()
        {
            AddEventListener();
        }
        
        public void Start()
        {
            
        }
        public void End()
        {
            
        }
        
        public void Release()
        {
            RemoveEventListener();
        }

        #region EventMessage
        private void AddEventListener()
        {
            
        }

        private void RemoveEventListener()
        {
            
        }

        private void OnChangeDirection(Vector3 startPosition, Vector3 updatePosition)
        {
            
        }

        private void OnInvokeActionSkill(ActionButtonType actionType)
        {
            
        }
        
        /// <summary>
        /// 움직이지 못할 때
        /// UI 처리
        /// </summary>
        /// <param name="isBlock"></param>
        private void BlockMove(bool isBlock)
        {
            
        }
        
        /// <summary>
        /// 모든 스킬을 사용 할 수 없을 때
        /// UI 처리
        /// </summary>
        /// <param name="isBlock"></param>
        private void BlockAllSkill(bool isBlock)
        {
            
        }

        /// <summary>
        /// 특정 스킬만 사용 불가 일 때
        /// UI 처리
        /// </summary>
        /// <param name="actionButtonType"></param>
        /// <param name="isBlock"></param>
        private void BlockSkill(ActionButtonType actionButtonType, bool isBlock)
        {
            
        }
        
        #endregion
    }
}