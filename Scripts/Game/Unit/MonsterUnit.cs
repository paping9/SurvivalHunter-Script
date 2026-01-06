namespace Game
{
    public class MonsterUnit : BaseUnit
    {
        public override void Initialize(int id)
        {
            base.Initialize(id);
            
            AddComponent(new UnitSkill());
            AddComponent(new UnitMove());
            AddComponent(new UnitParts());
            AddComponent(new UnitState());
            AddComponent(new UnitAnimation());
            AddComponent(new UnitBuff());
            
            Get<UnitParts>().SetUp(_root, _animator);
        }
    }
}