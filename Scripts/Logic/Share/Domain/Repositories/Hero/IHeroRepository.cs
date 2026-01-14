using System.Collections.Generic;

namespace Game.Repositories
{
    using Domain;
    
    public interface IHeroRepository
    {
        Hero GetHeroById(int id);
        IEnumerable<Hero> GetAllHeroes();
        void AddHero(Hero hero);
        void UpdateHero(Hero hero);
        void DeleteHero(int id);
    }
}