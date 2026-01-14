using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UI;
using VContainer;

namespace UIController
{
    public class TitleUIController : IUIController
    {
        private IUIManager _uiManager;
        
        public UIControllerType ControllerType { get => UIControllerType.Title; }
        
        /// <summary>
        /// Initializes the controller's UI manager dependency.
        /// </summary>
        /// <param name="uiManager">The UI manager instance to use for opening and managing UI windows.</param>
        [Inject]
        private void Construct(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        
        /// <summary>
        /// Opens the title window and then yields for one frame.
        /// </summary>
        /// <param name="param">Parameters supplied when entering this controller; currently unused.</param>
        /// <returns>Completes after the title window is opened and one frame has elapsed.</returns>
        public async UniTask OnEnter(UIControllerParam param)
        {
            await _uiManager.OpenAsync(UIID.TitleWindow, new UIParam() { });

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