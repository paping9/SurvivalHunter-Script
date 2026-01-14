using AssetBundle;
using Bootstrap;
using Data;
using Game;
using Game.Map;
using Message;
using Network;
using Scene;
using Sound;
using UI;
using UIController;
using Utils.Pool;
using VContainer;
using VContainer.Unity;

namespace System
{
    public class GameDiContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Core Services
            builder.Register<IUIControllerContainer, UIControllerContainer>(Lifetime.Singleton);
            builder.Register<ISceneManager, SceneManager>(Lifetime.Singleton);
            builder.Register<IAddressableManager, AddressableManager>(Lifetime.Singleton);
            builder.Register<IMessageBus, MessageBus>(Lifetime.Singleton);
            builder.Register<IGenericPoolManager, GenericPoolManager>(Lifetime.Singleton);
            builder.Register<ISignalHub, SignalHub>(Lifetime.Singleton);
            builder.Register<ITableDataManager, TableDataManager>(Lifetime.Singleton);

            // Bootstrap
            builder.Register<IBootstrapManager, BootstrapManager>(Lifetime.Singleton);

            // Game
            builder.Register<IPlayerManager, PlayerManager>(Lifetime.Singleton);
            builder.Register<IUnitManager, UnitManager>(Lifetime.Singleton);

            // Network
            builder.Register<IPacketQueue, PacketQueue>(Lifetime.Singleton);

            // MonoBehaviour Components
            builder.RegisterComponentInHierarchy<UIManager>().As<IUIManager>();
            builder.RegisterComponentInHierarchy<BGMManager>().As<IBGMManager>();
            builder.RegisterComponentInHierarchy<InputManager>().As<IInputManager>();

            // Map
            builder.Register<MapRoomCreator>(Lifetime.Singleton).As<IMapRoomCreator>();
        }
    }
}