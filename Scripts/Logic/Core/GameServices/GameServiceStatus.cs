using System;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// 유닛에 상태 처리 
    /// Hp, Mana
    /// Buff, Debuff
    /// CC
    /// </summary>
    public class GameServiceStatus
    {
        public void Damage(int attackId, int defenceId, ulong damage)
        {

        }

        public void Heal(int casterId, int targetId, ulong heal)
        {

        }

        public void Buff(int casterId, int targetId, int buffId, int instanceId)
        {

        }

        public void Debuff(int casterId, int targetId, int debuffId, int instanceId)
        {

        }
    }
}
