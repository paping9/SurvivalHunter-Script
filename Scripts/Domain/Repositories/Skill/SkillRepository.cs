using System.Collections.Generic;
using Game.Domain;

namespace Game.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly Dictionary<int, Skill> _skills = new Dictionary<int, Skill>();

        public Skill GetSkillById(int id)
        {
            _skills.TryGetValue(id, out var skill);
            return skill;
        }

        public IEnumerable<Skill> GetAllSkills() => _skills.Values;

        public void AddSkill(Skill skill) => _skills[skill.Id] = skill;

        public void UpdateSkill(Skill skill) => _skills[skill.Id] = skill;

        public void DeleteSkill(int id) => _skills.Remove(id);
    }
}