using Game;
using Sound;
using UI;
using VContainer;
using VContainer.Unity;

namespace GameLogic.Installers
{
    /// <summary>
    /// Hierarchy에 존재하는 MonoBehaviour 컴포넌트 등록
    /// - UIManager
    /// - BGMManager
    /// - InputManager
    /// </summary>
    public class MonoBehaviourInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<UIManager>().As<IUIManager>();
            builder.RegisterComponentInHierarchy<BGMManager>().As<IBGMManager>();
            builder.RegisterComponentInHierarchy<InputManager>().As<IInputManager>();
        }
    }
}