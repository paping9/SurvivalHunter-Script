using AssetBundle;
using Data;
using Message;
using Scene;
using Sound;
using UI;
using Utils.Pool;
using VContainer;
using VContainer.Unity;

namespace GameLogic.Installers
{
    /// <summary>
    /// Plugin(재사용 가능한 프레임워크) 서비스 등록
    /// - AddressableManager
    /// - MessageBus / SignalHub
    /// - TableDataManager
    /// - PoolManager
    /// - SceneManager
    /// - UIManager (Base)
    /// </summary>
    public class PluginInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Asset Management
            builder.Register<IAddressableManager, AddressableManager>(Lifetime.Singleton);

            // Messaging
            builder.Register<IMessageBus, MessageBus>(Lifetime.Singleton);
            builder.Register<ISignalHub, SignalHub>(Lifetime.Singleton);

            // Data
            builder.Register<ITableDataManager, TableDataManager>(Lifetime.Singleton);

            // Pooling
            builder.Register<IGenericPoolManager, GenericPoolManager>(Lifetime.Singleton);

            // Scene Management
            builder.Register<ISceneManager, SceneManager>(Lifetime.Singleton);

            // UI Framework
            builder.Register<IUIManager, UIManager>(Lifetime.Singleton);
            
            // Sound
            builder.RegisterComponentInHierarchy<BGMManager>().As<IBGMManager>();
        }
    }
}