using System;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class UnitSkill : IUnitComponent, IAssetPreLoader
    {
        private BaseUnit _owner;
        private SkillTimeline _skillTimeline = null;

        public void Init(BaseUnit unit)
        {
            _owner = unit;
            _skillTimeline = new SkillTimeline(new SkillTimelineNotiReceiver());
        }
        
        public void Release()
        {
            if (_skillTimeline != null)
            {
                _skillTimeline.Dispose();
                _skillTimeline = null;
            }
        }

        public void GameUpdate(float elapsedTime)
        {

        }
        public void FixedUpdate(float elapsedTime)
        {

        }
        public async UniTask Load()
        {
            await UniTask.DelayFrame(1);
        }

        public void UseSkill(int skillId)
        {

        }

        public void CancelSkill()
        {

        }

        public bool IsSkill()
        {
            return true;
        }
    }
}
