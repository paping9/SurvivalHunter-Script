using AssetBundle;
using Cysharp.Threading.Tasks;
using Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UIController;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extension;
using VContainer;
using VContainer.Unity;

namespace UI
{
    public class UIManager : SingletonMB<UIManager>
    {
        [SerializeField] private Canvas             _uiCanvas               = null;
        [SerializeField] private RectTransform      _uiCanvasTrans          = null;
        [SerializeField] private CanvasScaler       _canvasScaler           = null;
        [SerializeField] private RectTransform      _uiPanelParent          = null;
        [SerializeField] private RectTransform      _uiMenuParent           = null;
        [SerializeField] private RectTransform      _uiPopupParent          = null;
        [SerializeField] private RectTransform      _uiLoadingParent        = null;


        private Vector2 _canvasSize = Vector2.zero;
        private Vector2 _canvasHalfSize = Vector2.zero;

        private readonly bool                        _isBlockBackKey     = false;
        private readonly Dictionary<UIID, UIBase>    _dicUICache         = new();
        private readonly Dictionary<UIID, UIBase>    _dicUIOpenCache     = new();
        private readonly Stack<UIID>                 _popupStack         = new();

        private Camera _mainCamera = null;

        private IAddressableManager _addressableManager;

        [Inject]
        public void Construct(IAddressableManager addressableManager)
        {
            _addressableManager = addressableManager;
        }

        private void Start()
        {
            var fRateWidth        = (float)Screen.width / CommonConstValue.DefaultWidth;
            var fRateHeight       = (float)Screen.height / CommonConstValue.DefaultHeight;

            var fUIWidth          = CommonConstValue.DefaultWidth;
            var fUIHeight         = CommonConstValue.DefaultHeight;

            if (fRateWidth < fRateHeight)
            {
                _canvasScaler.matchWidthOrHeight = 0.0f;
                fUIHeight = CommonConstValue.DefaultWidth / (float)Screen.width * (float)Screen.height;
            }
            else
            {
                _canvasScaler.matchWidthOrHeight = 1.0f;
                fUIWidth = CommonConstValue.DefaultHeight / (float)Screen.height * (float)Screen.width;
            }

            _canvasSize       = new Vector2(fUIWidth, fUIHeight);
            _canvasHalfSize   = _canvasSize * 0.5f;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            RemoveAll();
        }

        public void RemoveAll()
        {

        }

        public void Remove(UIID uiId)
        {
            if (_dicUICache.ContainsKey(uiId) == false)
                return;

            _dicUICache[uiId].Remove();
            _addressableManager?.ReleaseInstantiate(_dicUICache[uiId].gameObject);
            _dicUICache.Remove(uiId);
            _dicUIOpenCache.Remove(uiId);
        }

        public async UniTask OpenAsync(UIID uiId, UIParam param = null, Action<UIBase> onOpenStart = null, Action<UIBase> onOpenEnd = null, Action<UIBase> onCloseStart = null, Action<UIBase> onCloseEnd = null)
        {
            var uidata = new UiData(uiId, param)
            {
                CallBackOpenStart = onOpenStart,
                CallBackOpenEnd = onOpenEnd,
                CallBackCloseStart = onCloseStart,
                CallBackCloseEnd = onCloseEnd,
            };

            var uiBase = await CreateInstance(uidata);

            if (uiBase != null)
            {
                uiBase.OnOpenStart(uidata.Param);
                uiBase.transform.SetAsLastSibling();
            }
        }

        public void Close(UIID uiId, bool bForceHide = false, bool bNextPopup = true)
        {
            if (_dicUIOpenCache.ContainsKey(uiId) == true)
            {
                if (_dicUIOpenCache[uiId].UIOpenType == OpenType.Menu)
                    bNextPopup = false;
                _dicUIOpenCache.Remove(uiId);
            }

            if (_dicUICache.ContainsKey(uiId) == true)
            {
                if (_dicUICache[uiId].UIOpenType == OpenType.Popup)
                    _popupStack.Pop();

                _dicUICache[uiId].OnCloseStart(bForceHide);
            }

            if (bNextPopup == true)
                DeQueuePopup();
        }

        public void Update()
        {
            UpdateBackKey();

            using var iter = _dicUIOpenCache.GetEnumerator();
            float fTime = Time.deltaTime;

            while (iter.MoveNext())
            {
                if (iter.Current.Value.IsActivate)
                    iter.Current.Value.Execute(fTime);
            }
        }

        private void UpdateBackKey()
        {
            if (_isBlockBackKey == true)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {

            }
        }

        private async UniTask<UIBase> CreateInstance(UiData uiData)
        {
            UIBase uiInstance = await CreateInstanceBasic(uiData);

            CheckPopUpUi(uiInstance);

            return uiInstance;
        }

        private async UniTask<UIBase> CreateInstanceBasic(UiData uiData)
        {
            UIBase uiInstance = null;

            if (_dicUICache.ContainsKey(uiData.UiId) == false) // cache 에 없는 UI만 로드
            {
                var instance = await _addressableManager.LoadInstantiate(uiData.UiId.Name, null);
                uiInstance = instance.GetComponent<UIBase>();

#if UNITY_EDITOR
                if (uiInstance == null)
                {
                    Debug.LogError("UIManager uiIntance == null");
                    return null;
                }
#endif
                
                uiInstance.Init();

                _dicUICache.Add(uiData.UiId, uiInstance);
            }
            else
            {
                uiInstance = _dicUICache[uiData.UiId];
            }

            _dicUIOpenCache.TryAdd(uiData.UiId, uiInstance);

            uiInstance.InitUi(uiData);
            uiInstance.transform.SetParent(GetParent(uiInstance.UIOpenType));
            UnityComponentEx.InitRectTransform(uiInstance.transform as RectTransform);

            return uiInstance;
        }

        private void CheckPopUpUi(UIBase uiInstance)
        {
            if (uiInstance == null)
                return;

            if (uiInstance.UIOpenType != OpenType.Popup) return;
            if (_popupStack.Count > 0)
            {
                var id = _popupStack.Peek();
                _dicUICache[id].Hide();
            }

            _popupStack.Push(uiInstance.UIData.UiId);
        }

        private void DeQueuePopup()
        {
            if (_popupStack.Count == 0)
            {
                return;
            }

            var uiId = _popupStack.Pop();

            if (_dicUIOpenCache.ContainsKey(uiId) == false)
            {
                DeQueuePopup();
                return;
            }

            _dicUIOpenCache[uiId].Show();
            _dicUIOpenCache[uiId].OnUpdateUI();
        }

        private RectTransform GetParent(OpenType openType)
        {
            return openType switch
            {
                OpenType.Menu => _uiMenuParent,
                OpenType.Popup => _uiPopupParent,
                _ => null
            };
        }

        #region Convert Point

        public Vector2 UiScreenSize()
        {
            return _canvasScaler.referenceResolution;
        }
        public Vector2 ConvertScreenToCanvasPoint(Vector2 vScreen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _uiCanvasTrans, vScreen, _uiCanvas.worldCamera, out var result);

            return result;
        }

        public Vector2 ConvertScreenToWorldPoint(Vector2 vScreen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _uiCanvasTrans, vScreen, _uiCanvas.worldCamera, out var result);

            return result;
        }

        public Vector2 ConvertWorldToCanvasPoint(Vector3 vWorld)
        {
            var vViewPort = _mainCamera.WorldToViewportPoint(vWorld);
            return ConvertViewportToCanvasPoint(vViewPort);
            //return _canvasRootTrans.InverseTransformPoint(vWorld);
        }

        public Vector2 ConvertViewportToCanvasPoint(Vector3 vViewport)
        {
            var vWorld = _uiCanvas.worldCamera.ViewportToWorldPoint(vViewport);
            return _uiCanvasTrans.InverseTransformPoint(vWorld);
        }

        public Vector2 ConvertViewportToWorldPoint(Vector3 vViewport)
        {
            return _uiCanvas.worldCamera.ViewportToWorldPoint(vViewport);
        }

        #endregion
    }
}
