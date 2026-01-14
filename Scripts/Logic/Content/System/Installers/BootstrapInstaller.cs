using Bootstrap;
using VContainer;

namespace GameLogic.Installers
{
    /// <summary>
    /// Bootstrap 관련 서비스 등록
    /// - BootstrapManager
    /// - Bootstrap Steps (자동 등록)
    /// </summary>
    public class BootstrapInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Bootstrap Manager
            builder.Register<IBootstrapManager, BootstrapManager>(Lifetime.Singleton);

            // Bootstrap Steps는 BootstrapManager가 동적으로 생성하므로
            // DI에 등록하지 않음 (new로 생성 후 Inject 호출)
        }
    }
}