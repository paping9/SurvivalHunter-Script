using Defs;

namespace Game
{
    /// <summary>
    /// 기본 능력치 + Buff 로 획득한 능력치
    /// </summary>
    
    public class UnitStatus : IUnitComponent
    {
        public double this[StatusType key] => _statuses[(int)key];

        private double[] _statuses;
        private BaseUnit _owner = null;
        
        public void Init(BaseUnit unit)
        {
            _statuses = new double[(int)StatusType.Max];
            _owner = unit;
        }
        
        public void Release()
        {
            _owner = null;
        }
        
        public void GameUpdate(float elapsedTime)
        {

        }
        public void FixedUpdate(float elapsedTime)
        {

        }
    }

    internal class Dictionary<T>
    {
    }
}