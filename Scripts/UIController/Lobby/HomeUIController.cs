using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;
using UI;
using VContainer;

namespace UIController
{
    public class HomeUIController : IUIController
    {
        private IUIManager _uiManager;
        
        /// <summary>
        /// Receives the injected UI manager and stores it for the controller's use.
        /// </summary>
        /// <param name="uiManager">The UI manager instance to assign to the controller.</param>
        [Inject]
        private void Construct(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public UIControllerType ControllerType { get => UIControllerType.Home; }

        /// <summary>
        /// Performs initialization when the controller becomes active.
        /// </summary>
        /// <param name="param">Contextual parameters for entering the controller.</param>
        /// <returns>A UniTask that completes when the enter operation has finished.</returns>
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