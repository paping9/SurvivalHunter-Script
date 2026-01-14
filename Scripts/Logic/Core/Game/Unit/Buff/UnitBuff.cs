using System.Collections.Generic;
using Defs;

namespace Game
{
    using BuffContainer = Dictionary<int, BuffElement>;
    
    public class UnitBuff : IUnitComponent
    {
        private BaseUnit _owner = null;

        private Dictionary<BuffType, BuffContainer> _dicBuffContainer = new Dictionary<BuffType, BuffContainer>();
        
        public void Init(BaseUnit unit)
        {
            _owner = unit;
        }
        
        public void Release()
        {
            _owner = null;
        }

        public void AddBuff(BuffElement element)
        {
            _dicBuffContainer.TryAdd(element.BuffType, new BuffContainer());
            _dicBuffContainer[element.BuffType].Add(element.BuffInstanceId, element);
        }

        public void RemoveBuff(BuffType buffType, int buffInstanceId)
        {
            _dicBuffContainer.TryAdd(buffType, new BuffContainer());
            _dicBuffContainer[buffType].Remove(buffInstanceId);
        }
        
        public void GameUpdate(float elapsedTime)
        {
            using var buffContainer = _dicBuffContainer.GetEnumerator();
            while (buffContainer.MoveNext())
            {
                using var elementIter = buffContainer.Current.Value.GetEnumerator();
                while (elementIter.MoveNext())
                {
                    if (elementIter.Current.Value.IsNotDuration)
                    {
                                
                    }
                }
            }
        }
        public void FixedUpdate(float elapsedTime)
        {
            
        }
    }
}