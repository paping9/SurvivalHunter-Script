using GameLogic.Installers;
using VContainer;
using VContainer.Unity;

namespace GameLogic
{
    public class GameDiContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ====== Plugin Layer (Reusable Framework) ======
            new PluginInstaller().Install(builder);

            // ====== Logic Layer (Game Specific) ======
            new BootstrapInstaller().Install(builder);
            new DomainInstaller().Install(builder);
            new UIInstaller().Install(builder);
            new GameServicesInstaller().Install(builder);
            new MonoBehaviourInstaller().Install(builder);
        }
    }
}