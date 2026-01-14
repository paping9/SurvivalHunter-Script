using Game.Repositories;
using VContainer;

namespace GameLogic.Installers
{
    /// <summary>
    /// Domain 레이어 서비스 등록
    /// - Repositories (Hero, Item, Mission, Skill)
    /// - Domain Services (필요 시 추가)
    /// </summary>
    public class DomainInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            // Repositories
            builder.Register<IHeroRepository, HeroRepository>(Lifetime.Singleton);
            builder.Register<IItemRepository, ItemRepository>(Lifetime.Singleton);
            builder.Register<IMissionRepository, MissionRepository>(Lifetime.Singleton);
            builder.Register<ISkillRepository, SkillRepository>(Lifetime.Singleton);

            // Domain Services (복합 비즈니스 로직 - 추후 추가 시)
            // builder.Register<IHeroService, HeroService>(Lifetime.Singleton);
            // builder.Register<IItemService, ItemService>(Lifetime.Singleton);
        }
    }
}