using Game;
using Game.Map;
using VContainer;
using VContainer.Unity;

namespace GameLogic.Installers
{
    /// <summary>
    /// 게임 로직 서비스 등록
    /// - UnitManager
    /// - MapRoomCreator
    /// - InputManager
    /// - GameServiceManager (필요 시)
    /// </summary>
    public class GameServicesInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Unit Management
            builder.Register<IUnitManager, UnitManager>(Lifetime.Singleton);

            // Map Generation
            builder.Register<IMapRoomCreator, MapRoomCreator>(Lifetime.Singleton);

            // Input
            builder.RegisterComponentInHierarchy<InputManager>().As<IInputManager>();

            // Game Services (In-Game Logic)
            builder.Register<IGameServiceManager, GameServiceManager>(Lifetime.Singleton);
        }
    }
}