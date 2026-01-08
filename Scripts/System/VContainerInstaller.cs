using VContainer;
using VContainer.Unity;
using AssetBundle;
using Game.Map;
using Message;
using UIController;
using Utils.Pool;
using Scene;

namespace System
{
    /// <summary>
    /// VContainer DI 컨테이너 설정
    /// 모든 시스템 서비스를 등록
    /// </summary>
    public class VContainerInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 싱글톤 서비스 등록 (최신 API)
            builder.Register<IUIControllerContainer, UIControllerContainer>(Lifetime.Singleton);
            builder.Register<ISceneManager, SceneManager>(Lifetime.Singleton);
            builder.Register<IAddressableManager, AddressableManager>(Lifetime.Singleton);
            builder.Register<IMessageBus, MessageBus>(Lifetime.Singleton);
            builder.Register<IGenericPoolManager, GenericPoolManager>(Lifetime.Singleton);
            
            // 의존성이 있는 서비스 (Factory 패턴)
            builder.Register<IMapRoomCreator>(
                (container) => new MapRoomCreator(container.Resolve<IGenericPoolManager>()),
                Lifetime.Singleton
            );
        }
    }
}