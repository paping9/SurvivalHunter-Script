using System;
using System.Collections;
using UnityEngine;

using Cysharp.Threading.Tasks;
using System.Threading;
using Utils.Extension;

namespace UI
{
    public enum UIState
    {
        None,
        Opening,
        Open,
        Closing,
        Close
    }

    public abstract class UIBase : MonoBehaviour
    {
        [SerializeField] private OpenType   _openType       = OpenType.Menu;

        [Header("UI Order 순서")]
        [SerializeField] private DepthType  _depthType      = UI.DepthType.Low;
        [SerializeField] private int        _priority       = 1;
        [SerializeField] private bool       _isforceTop     = false;

        private UiData _baseData = null;
        public UiData UIData => _baseData;

        private UIState _state = UIState.None;
        private CancellationTokenSource _cancelToken;

        public bool             IsActivate => _state == UIState.Open;
        public UIID             Id => _baseData.UiId;
        public OpenType         UIOpenType => _openType;
        public bool             IsForceTop => _isforceTop;
        public int              Priority => _priority;
        public DepthType        DepthType => _depthType;

        public abstract void Init();
        public abstract void Remove();

        public void InitUi(UiData uidata)
        {
            _baseData = uidata;
        }

        public virtual void OnUpdateUI() { }
        public virtual void OnUpdateUI(UiData updateData) { }

        public virtual void OnOpenStart(UIParam param = null)
        {
            if (_state == UIState.Opening)
                return;

            _state = UIState.Opening;
            DisposeCancelToken();

            UtilityEx.SetActive(this.gameObject, true);

            UIData?.CallBackOpenStart?.Invoke(this);

            OpenEndAsync(0f).Forget();
        }

        private async UniTask OpenEndAsync(float endTime)
        {
            _cancelToken = new CancellationTokenSource();

            await UniTask.Delay(endTime.ConvertTimeToMs(), false, PlayerLoopTiming.Update, _cancelToken.Token);
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _cancelToken.Token);

            DisposeCancelToken();
            OnOpenEnd();
        }

        public virtual void OnOpenEnd()
        {
            UIData?.CallBackOpenEnd?.Invoke(this);

            _state = UIState.Open;
        }

        public virtual void OnCloseStart(bool bForceHide /* hide 모션을 강제로 시키지 않는다*/ = false)
        {
            if (_state == UIState.Closing)
                return;

            DisposeCancelToken();

            _state = UIState.Closing;

            UIData?.CallBackCloseStart?.Invoke(this);

            CloseEndAsync(0f).Forget();
        }

        private async UniTask CloseEndAsync(float endTime)
        {
            _cancelToken = new CancellationTokenSource();

            await UniTask.Delay(endTime.ConvertTimeToMs(), false, PlayerLoopTiming.Update, _cancelToken.Token);
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _cancelToken.Token);

            DisposeCancelToken();
            OnCloseEnd();
        }


        public virtual void OnCloseEnd()
        {
            UIData?.CallBackCloseEnd?.Invoke(this);
            _state = UIState.Close;

            UtilityEx.SetActive(this.gameObject, false);

            _baseData = null;

            if (UIData != null)
            {
                UIData.Remove();
            }
        }

        public virtual bool BackKey()
        {
            Close();

            return true;
        }

        public virtual void Close()
        {
            UIManager.Instance.Close(_baseData.UiId);
        }

        public virtual void Show()
        {
            UtilityEx.SetActive(this.gameObject, true);
        }

        public virtual void Hide()
        {
            UtilityEx.SetActive(this.gameObject, false);
        }

        public virtual void Execute(float fElapsedTime)
        {

        }

        private void DisposeCancelToken()
        {
            if (_cancelToken == null) return;

            if (_cancelToken.IsCancellationRequested == false) _cancelToken.Cancel();

            _cancelToken.Dispose();
            _cancelToken = null;
        }

    }
}
