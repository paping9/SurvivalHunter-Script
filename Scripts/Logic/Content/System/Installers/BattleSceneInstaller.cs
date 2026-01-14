// Logic/Core/System/Installers/BattleSceneInstaller.cs
using VContainer;

namespace GameLogic.Installers
{
    /// <summary>
    /// Battle Scene에서만 필요한 서비스
    /// </summary>
    public class BattleSceneInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // builder.Register<IBattleManager, BattleManager>(Lifetime.Scoped);
            // builder.Register<IEnemySpawner, EnemySpawner>(Lifetime.Scoped);
            // builder.Register<IWaveManager, WaveManager>(Lifetime.Scoped);
        }
    }
}

// Battle Scene용 별도 LifetimeScope
// public class BattleSceneScope : LifetimeScope
// {
//     protected override void Configure(IContainerBuilder builder)
//     {
//         // Battle Scene 전용 서비스만 등록
//         new BattleSceneInstaller().Install(builder);
//     }
// }