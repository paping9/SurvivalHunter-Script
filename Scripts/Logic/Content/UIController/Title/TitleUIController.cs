using Cysharp.Threading.Tasks;

using UI;
using VContainer;

namespace UIController
{
    public class TitleUIController : IUIController
    {
        private IUIManager _uiManager;

        [Inject]
        private void Construct(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        
        public async UniTask OnEnter(UIControllerParam param)
        {
            await _uiManager.OpenAsync(ContentUIID.TitleWindow, new UIParam() { });

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
