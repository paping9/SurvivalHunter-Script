using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace UI
{
    [ExecuteAlways]
    public class UGUICanvasPerfect : UIBehaviour
    {
        [SerializeField]
        private bool _bExcuteAwake = true;
        [SerializeField]
        private bool _bExcuteEnable = false;
        [SerializeField]
        private bool _bExcuteDimensionsChange = false;

        [SerializeField]
        private RectTransform _canvasTrans = null;
        [SerializeField]
        private CanvasScaler _scaler = null;

        [SerializeField]
        private Vector2 resolution = Vector2.positiveInfinity;
        [SerializeField]
        private bool _bUseSafeArea = true;

        private const string PLAYERPREF_KEY_SAFEAREA = "OnSafeTest";

        //#if UNITY_EDITOR
        //        [UnityEditor.MenuItem("GameTools/Utils/SafeAreaTest/Safe Area Test On")]
        //        public static void OnSafeAreaTest()
        //        {
        //            PlayerPrefs.SetInt(PLAYERPREF_KEY_SAFEAREA, 0);
        //            PlayerPrefs.Save();
        //        }

        //        [UnityEditor.MenuItem("GameTools/Utils/SafeAreaTest/Safe Area Test Off")]
        //        public static void OffSafeAreaTest()
        //        {
        //            PlayerPrefs.DeleteKey(PLAYERPREF_KEY_SAFEAREA);
        //            PlayerPrefs.Save();
        //        }
        //#endif

        protected override void Awake()
        {
            base.Awake();

            var rttr_Mine = this.transform as RectTransform;
            if (rttr_Mine == null)
                return;

            var vAnchor = Vector2.one * 0.5f;
            rttr_Mine.anchorMin = vAnchor;
            rttr_Mine.anchorMax = vAnchor;

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
            var rttr_Mine = this.transform as RectTransform;
            if (rttr_Mine == null)
                return;

            var vCanvasPos = _canvasTrans.position;
            var vCanvasSize = _canvasTrans.sizeDelta;

            if (_bUseSafeArea)
            {
                var rt_ScreenArea = new Rect(0, 0, Screen.width, Screen.height);

                var factorX_ScreenToCanvas = vCanvasSize.x / rt_ScreenArea.width;
                var factorY_ScreenToCanvas = vCanvasSize.y / rt_ScreenArea.height;

                var rt_SafeArea = Screen.safeArea;

                //#if UNITY_EDITOR
                //                if(PlayerPrefs.HasKey(PLAYERPREF_KEY_SAFEAREA))
                //                    rt_SafeArea = new Rect(63, 132, 1062, 2172);
                //#endif

                var posOffset = rt_SafeArea.center - rt_ScreenArea.center;
                var newSizeDelta = rt_SafeArea.size;

                posOffset.x *= factorX_ScreenToCanvas;
                posOffset.y *= factorY_ScreenToCanvas;
                newSizeDelta.x *= factorX_ScreenToCanvas;
                newSizeDelta.y *= factorY_ScreenToCanvas;

                if (newSizeDelta.x >= 2340f)
                    newSizeDelta.x = 2340f;
                if (newSizeDelta.y >= resolution.y)
                    newSizeDelta.y = resolution.y;


                rttr_Mine.sizeDelta = newSizeDelta;
                rttr_Mine.position = vCanvasPos;
                rttr_Mine.anchoredPosition = posOffset;
            }
            else
            {
                if (vCanvasSize.x >= 2340f)
                    vCanvasSize.x = 2340f;
                if (vCanvasSize.y >= resolution.y)
                    vCanvasSize.y = resolution.y;

                rttr_Mine.sizeDelta = vCanvasSize;
                rttr_Mine.position = vCanvasPos;
            }
        }
    }


}

