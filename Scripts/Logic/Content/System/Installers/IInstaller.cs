using VContainer;

namespace GameLogic.Installers
{
    public interface IInstaller
    {
        void Install(IContainerBuilder builder);
    }
}