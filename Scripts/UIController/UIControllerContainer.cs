using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;
using VContainer;

namespace UIController
{
    public interface IUIControllerContainer
    {
        UniTask ChangeUIController(UIControllerParam param, bool addStack = true);
        void Push(UIControllerParam param);
        UniTask Back();
        void Clear();
    }
    public class UIControllerContainer : IUIControllerContainer
    {
        private Dictionary<UIControllerType, IUIController> _uiControllers = new Dictionary<UIControllerType, IUIController>();
        // Stack 쌓아 두고 Back Key 로 이동.
        private Stack<UIControllerParam> _stackControllerTypes = new Stack<UIControllerParam>();
        private IUIController _currentController = null;
        private IObjectResolver _objectResolver;

        public IUIController CurrentController { get => _currentController; }

        /// <summary>
        /// Stores the provided object resolver for use when injecting dependencies into created UI controllers.
        /// </summary>
        /// <param name="objectResolver">Resolver used to perform dependency injection into newly created controllers.</param>
        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        /// <summary>
        /// Switches the active UI controller to the one specified by <paramref name="param"/> and updates navigation state.
        /// </summary>
        /// <param name="param">Parameters that identify the target controller and provide contextual data for its entry.</param>
        /// <param name="addStack">If true, pushes <paramref name="param"/> onto the internal navigation stack.</param>
        public async UniTask ChangeUIController(UIControllerParam param, bool addStack = true)
        {
            IUIController controller = null;

            if(_uiControllers.TryGetValue(param.ControllerType, out controller) == false)
            {
                controller = CreateController(param.ControllerType);
                _uiControllers[param.ControllerType] = controller;
            }

            await controller.OnEnter(param);

            if(addStack)
                _stackControllerTypes.Push(param);

            if (_currentController != null)
            {
                await _currentController.OnExit();
            }

            _currentController = controller;
        }

        public void Push(UIControllerParam param)
        {
            _stackControllerTypes.Push(param);
        }

        // 이전 Stack 으로 이동
        public async UniTask Back()
        {
            if (_currentController != null && _currentController.Back())
            {
                // Stack 이 1개라면 .. 
                if (_stackControllerTypes.Count <= 1) return;

                var param = _stackControllerTypes.Pop();
                await ChangeUIController(param);
            }
        }
        

        // Scene 이동이나 Home 이동 시 모든 Stack 을 비워주는... 
        public void Clear()
        {
            _stackControllerTypes.Clear();
        }

        /// <summary>
        /// Creates a UI controller instance for the specified controller type and injects its dependencies.
        /// </summary>
        /// <param name="controllerType">The type of UI controller to create.</param>
        /// <returns>The created IUIController instance for the given type, or <c>null</c> if the type is not recognized.</returns>
        private IUIController CreateController(UIControllerType controllerType)
        {
            IUIController controller = controllerType switch
            {
                UIControllerType.Title => new TitleUIController(),
                UIControllerType.Home => new HomeUIController(),
                _ => null
            };

            if (controller != null)
            {
                _objectResolver.Inject(controller);
            }

            return controller;
        }
    }
}