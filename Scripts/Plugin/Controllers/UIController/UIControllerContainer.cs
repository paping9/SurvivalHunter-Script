using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace UIController
{
    public interface IUIControllerContainer
    {
        UniTask ChangeUIController(int controllerTypeId, UIControllerParam param, bool addStack = true);
        void Push(int controllerTypeId, UIControllerParam param);
        UniTask Back();
        void Clear();
    }

    /// <summary>
    /// UIController를 관리하는 컨테이너
    /// Factory 패턴을 사용하여 Logic 레이어의 구체 타입에 의존하지 않음
    /// </summary>
    public class UIControllerContainer : IUIControllerContainer
    {
        private Dictionary<int, IUIController>  _uiControllers          = new Dictionary<int, IUIController>();
        private Stack<ControllerStackItem>      _stackControllerTypes   = new Stack<ControllerStackItem>();
        private IUIController                   _currentController      = null;

        private IObjectResolver         _objectResolver;
        private IUIControllerFactory    _factory;

        public IUIController CurrentController => _currentController;

        [Inject]
        public void Construct(IObjectResolver objectResolver, IUIControllerFactory factory)
        {
            _objectResolver = objectResolver;
            _factory        = factory;
        }

        /// <summary>
        /// UIController 변경
        /// </summary>
        public async UniTask ChangeUIController(int controllerTypeId, UIControllerParam param, bool addStack = true)
        {
            IUIController controller = null;

            if (_uiControllers.TryGetValue(controllerTypeId, out controller) == false)
            {
                controller = CreateController(controllerTypeId);
                if (controller == null)
                {
                    Debug.LogError($"ChangeUIController TypeId : {controllerTypeId} is null!!!!");
                    return;
                }
                _uiControllers[controllerTypeId] = controller;
            }

            await controller.OnEnter(param);

            if (addStack)
            {
                _stackControllerTypes.Push(new ControllerStackItem
                {
                    ControllerTypeId    = controllerTypeId,
                    Param               = param
                });
            }

            if (_currentController != null)
            {
                await _currentController.OnExit();
            }

            _currentController = controller;
        }

        /// <summary>
        /// Stack에 Controller 정보 추가
        /// </summary>
        public void Push(int controllerTypeId, UIControllerParam param)
        {
            _stackControllerTypes.Push(new ControllerStackItem
            {
                ControllerTypeId    = controllerTypeId,
                Param               = param
            });
        }

        /// <summary>
        /// 이전 Stack으로 이동
        /// </summary>
        public async UniTask Back()
        {
            if (_currentController != null && _currentController.Back())
            {
                // Stack이 1개 이하라면 돌아갈 곳이 없음
                if (_stackControllerTypes.Count <= 1) return;

                var item = _stackControllerTypes.Pop();
                await ChangeUIController(item.ControllerTypeId, item.Param);
            }
        }

        /// <summary>
        /// Scene 이동이나 Home 이동 시 모든 Stack 비우기
        /// </summary>
        public void Clear()
        {
            _stackControllerTypes.Clear();
        }

        /// <summary>
        /// Factory를 통해 Controller 생성
        /// </summary>
        private IUIController CreateController(int controllerTypeId)
        {
            var controller = _factory.Create(controllerTypeId);

            if (controller != null)
            {
                _objectResolver.Inject(controller);
            }

            return controller;
        }

        /// <summary>
        /// Stack에 저장되는 Controller 정보
        /// </summary>
        private struct ControllerStackItem
        {
            public int                  ControllerTypeId;
            public UIControllerParam    Param;
        }
    }
}
