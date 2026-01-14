using System.Collections.Generic;
using Defs;
using UnityEngine;
using Utils.Extension;

namespace Game
{
    public class UnitParts : IUnitComponent
    {
        private BaseUnit _owner;
        private Dictionary<EquipSlot, UnitEquipItem> _equipItems = new();
        private Dictionary<string, Transform> _boneCache = new();
        private Transform _setRootBone;
        private Animator _animator;

        public bool IsEquipSlot(EquipSlot slot) => _equipItems.ContainsKey(slot);

        public void Init(BaseUnit unit)
        {
            _owner = unit;
        }

        public void Release()
        {
            _owner = null;
            _setRootBone = null;
            _equipItems.Clear();
        }

        public void GameUpdate(float elapsedTime) { }
        public void FixedUpdate(float elapsedTime) { }

        public void SetUp(Transform rootBone, Animator animator)
        {
            _setRootBone = rootBone;
            _animator = animator;
        }

        public void AttachEquipment(UnitEquipItem unitEquipItem, bool bForceChange = true)
        {
            var equipSlot = unitEquipItem.EquipSlot;
            if (IsEquipSlot(equipSlot))
            {
                if (bForceChange)
                {
                    DeAttachEquipment(equipSlot);
                }
                else
                {
                    return;
                }
            }
            
            Transform pivot = FindPivot(unitEquipItem);
            unitEquipItem.SetRoot(_setRootBone, pivot);
            _equipItems.Add(equipSlot, unitEquipItem);

            // ✅ 특정 장비(Head/Hair) 처리 로직 분리
            if (equipSlot == EquipSlot.Hair)
            {
                HandleHairEquip(unitEquipItem as UnitHairItem);
            }
            else if (equipSlot == EquipSlot.Head)
            {
                HandleHeadEquip(unitEquipItem);
            }
        }

        public void DeAttachEquipment(EquipSlot equipSlot)
        {
            if (!IsEquipSlot(equipSlot)) return;

            var unitEquipItem = _equipItems[equipSlot];

            // ✅ 특정 장비(Head/Hair) 해제 로직 분리
            if (equipSlot == EquipSlot.Head)
            {
                HandleHeadUnequip();
            }

            GameObject.DestroyImmediate(unitEquipItem.gameObject);
            _equipItems.Remove(equipSlot);
        }

        // ✅ 머리카락 장비(Hair) 처리
        private void HandleHairEquip(UnitHairItem hairEquipItem)
        {
            if (hairEquipItem == null) return;

            if (_equipItems.TryGetValue(EquipSlot.Head, out var headEquipItem) &&
                headEquipItem.EquipType != EquipType.Crown)
            {
                hairEquipItem.SetHalfHair();
            }
            else
            {
                hairEquipItem.SetFullHair();
            }
        }

        // ✅ 머리(Head) 장비 처리
        private void HandleHeadEquip(UnitEquipItem headEquipItem)
        {
            if (_equipItems.TryGetValue(EquipSlot.Hair, out var hairEquipItem))
            {
                var hair = hairEquipItem as UnitHairItem;
                if (hair != null)
                {
                    if (headEquipItem.EquipType != EquipType.Crown)
                    {
                        hair.SetHalfHair();
                    }
                    else
                    {
                        hair.SetFullHair();
                    }
                }
            }
        }

        // ✅ 머리(Head) 해제 시 머리카락 원상복구
        private void HandleHeadUnequip()
        {
            if (_equipItems.TryGetValue(EquipSlot.Hair, out var hairEquipItem))
            {
                var hair = hairEquipItem as UnitHairItem;
                if (hair != null)
                {
                    hair.SetFullHair();
                }
            }
        }
        
        private Transform FindPivot(UnitEquipItem unitEquipItem)
        {
            if (string.IsNullOrEmpty(unitEquipItem.PivotName))
            {
                return _animator.transform;
            }

            return _boneCache.TryGetValue(unitEquipItem.PivotName, out var pivot)
                ? pivot
                : UnityComponentEx.FindChildByName(unitEquipItem.PivotName, _animator.transform);
        }
    }
}