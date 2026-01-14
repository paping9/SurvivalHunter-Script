using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace UI
{
    [ExecuteAlways]
    public class UGUICanvasHeightRatio : UIBehaviour
    {
        [SerializeField]
        private bool _bExcuteAwake = true;
        [SerializeField]
        private bool _bExcuteEnable = false;
        [SerializeField]
        private bool _bExcuteDimensionsChange = false;
        [SerializeField]
        [Range(0.3f, 1.0f)]
        private float _fRatio = 1.0f;

        [SerializeField]
        private RectTransform _canvasTrans = null;
        [SerializeField]
        private CanvasScaler _scaler = null;

        protected override void Awake()
        {
            base.Awake();

            var myRect = this.transform as RectTransform;
            if (myRect == null)
                return;

            var vAnchor = Vector2.one * 0.5f;
            myRect.anchorMin = vAnchor;
            myRect.anchorMax = vAnchor;

            if (_bExcuteAwake)
                UpdateSizeDelta();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_bExcuteEnable)
                UpdateSizeDelta();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            if (_bExcuteDimensionsChange)
                UpdateSizeDelta();
        }

        public void UpdateSizeDelta()
        {
            // Canvas의 RectTransform 찾기
            if (_canvasTrans == null)
            {
                var canvas = GetComponentInParent(typeof(Canvas)) as Canvas;
                if (canvas == null)
                    return;

                _canvasTrans = canvas.transform as RectTransform;
            }

            // CanvasScaler 찾기
            if (_scaler == null)
            {
                _scaler = GetComponentInParent(typeof(CanvasScaler)) as CanvasScaler;
                if (_scaler == null)
                    return;
            }

            // Holder의 Anchor 다시 잡기
            var myRect = this.transform as RectTransform;
            if (myRect == null)
                return;

            var canvasSize = _canvasTrans.sizeDelta;
            var mySize = myRect.sizeDelta;
            mySize.y = canvasSize.y;
            mySize.x = canvasSize.y * _fRatio;

            myRect.sizeDelta = mySize;
        }
    }


}

