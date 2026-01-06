using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UI;

namespace UIController
{
    public class TitleUIController : IUIController
    {
        public UIControllerType ControllerType { get => UIControllerType.Title; }

        public async UniTask OnEnter(UIControllerParam param)
        {
            await UIManager.Instance.OpenAsync(UIID.TitleWindow, new UIParam() { });

            await UniTask.DelayFrame(1);
        }

        public async UniTask OnExit()
        {
            await UniTask.DelayFrame(1);
        }

        /// <summary>
        /// True 면 UIController 에서 이전 Stack 으로 이동.
        /// </summary>
        /// <returns></returns>
        public bool Back()
        {
            return false;
        }

        public void Release()
        {

        }

        public void Execute(float elapsedTime)
        {

        }
    }
}
