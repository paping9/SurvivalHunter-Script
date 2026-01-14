using System.Collections.Generic;

namespace Game.Repositories
{
    using Domain;
    
    public class HeroRepository : IHeroRepository
    {
        private readonly Dictionary<int, Hero> _heroes = new Dictionary<int, Hero>();

        public Hero GetHeroById(int id)
        {
            _heroes.TryGetValue(id, out var hero);
            return hero;
        }

        public IEnumerable<Hero> GetAllHeroes() => _heroes.Values;

        public void AddHero(Hero hero) => _heroes[hero.Id] = hero;

        public void UpdateHero(Hero hero) => _heroes[hero.Id] = hero;

        public void DeleteHero(int id) => _heroes.Remove(id);
    }
}