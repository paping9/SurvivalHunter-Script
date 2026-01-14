using UnityEngine;

namespace UIController
{
    /// <summary>
    /// 게임 특화 UIController Factory 구현
    /// UIControllerType enum을 알고 구체적인 Controller 인스턴스 생성
    /// </summary>
    public class UIControllerFactory : IUIControllerFactory
    {
        public IUIController Create(int controllerTypeId)
        {
            var type = (UIControllerType)controllerTypeId;

            IUIController controller = type switch
            {
                UIControllerType.Title          => new TitleUIController(),
                UIControllerType.Home           => new HomeUIController(),
                // UIControllerType.Inventory   => new InventoryUIController(),
                // UIControllerType.SkillBook   => new SkillBookUIController(),
                // ... 필요한 Controller 추가
                _ => null
            };

            if (controller == null)
            {
                Debug.LogWarning($"[UIControllerFactory] Unknown controller type: {type} ({controllerTypeId})");
            }

            return controller;
        }
    }
}
