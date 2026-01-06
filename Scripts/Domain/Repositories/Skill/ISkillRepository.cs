using System.Collections.Generic;
using Game.Domain;

namespace Game.Repositories
{
    public interface ISkillRepository
    {
        Skill GetSkillById(int id);
        IEnumerable<Skill> GetAllSkills();
        void AddSkill(Skill skill);
        void UpdateSkill(Skill skill);
        void DeleteSkill(int id);
    }
}