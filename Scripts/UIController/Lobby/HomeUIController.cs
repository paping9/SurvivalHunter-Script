using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

namespace UIController
{
    public class HomeUIController : IUIController
    {
        public UIControllerType ControllerType { get => UIControllerType.Home; }

        public async UniTask OnEnter(UIControllerParam param)
        {
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
