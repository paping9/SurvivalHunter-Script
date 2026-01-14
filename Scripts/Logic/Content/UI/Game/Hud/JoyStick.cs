using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class JoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _padInside;
        [SerializeField] private Image _padOutSide;

        private bool _isDown;
        private Vector3 _startPos;
        private Vector3 _curPos;
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            
            if (_isDown) return;
            
            _startPos = _curPos = eventData.position;
            _isDown = true;
            
            // pad point 를 start position 으로 위치
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDown) return;

            _curPos = eventData.position;

            var dir = (_curPos - _startPos).normalized;
            // Update position;
            
            // 움직이는 방향으로 outside point 를 위치. 거리가 멀면 최대 거리만큼만 이동.
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDown) return;
            
            _startPos = Vector3.zero;
            _curPos = Vector3.zero;
            _isDown = false;
        }
    }
}