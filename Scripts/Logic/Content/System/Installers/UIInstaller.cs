using UI;
using UIController;
using VContainer;
using VContainer.Unity;

namespace GameLogic.Installers
{
    /// <summary>
    /// UI 관련 서비스 등록
    /// - UIControllerFactory
    /// - UIControllerContainer
    /// - MonoBehaviour UI Components
    /// </summary>
    public class UIInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // UI Controller System
            builder.Register<IUIControllerFactory, UIControllerFactory>(Lifetime.Singleton);
            builder.Register<IUIControllerContainer, UIControllerContainer>(Lifetime.Singleton);

            // MonoBehaviour UI Components
            builder.RegisterComponentInHierarchy<UIManager>().As<IUIManager>();
            
            // UI Handlers (필요 시)
            // builder.Register<IUIEventHandler, UIEventHandler>(Lifetime.Singleton);
        }
    }
}