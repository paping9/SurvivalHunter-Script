using System;

namespace Defs
{
    public enum ItemType
    {
        Currency,
        Material,
        Exp,
        Equip
    }

    public enum EquipSlot
    {
        None        = 0,
        Head        ,  // Helm, Crown
        Hair        ,  // Hair
        Body        ,  // Cloth
        Hands       ,  // Glove
        Feet        ,  // Shoe
        Shoulder    ,  // ShoulderPad
        Waist       ,  // Belt
        Back        ,  // Backpack
        Face        ,  // Mask, Glasses
        Weapon      ,  //  Left-Hand Weapon
        Shield      ,  // Shield
    }

    // EquipType: 각 장비의 종류
    [Flags]
    public enum EquipType
    {
        None    = 0,
        // 🏆 머리 관련 (Head)
        _Head_   = EquipSlot.Head << 16,
        Helm    ,
        Crown   ,
        Hat     ,
        
        _Hair  = EquipSlot.Hair << 16,
        Hair    ,
        
        // 👕 방어구 (Body)
        _Body_ = EquipSlot.Body << 16,
        Cloth   ,

        // 🧤 장갑 (Hands)
        _Hands_ = EquipSlot.Hands << 16,
        Glove   ,

        // 👞 신발 (Feet)
        _Feet_ = EquipSlot.Feet << 16,
        Shoe    ,

        // 🛡 어깨 보호대 (Shoulder)
        _Shoulder_ = EquipSlot.Shoulder << 16,
        ShoulderPad,

        // 🏅 허리 장비 (Waist)
        _Waist_ = EquipSlot.Waist << 16,
        Belt    ,

        // 🎒 등 장비 (Back)
        _Back_ = EquipSlot.Back << 16,
        Backpack ,

        // 😷 얼굴 장비 (Face)
        _Face_ = EquipSlot.Face << 16,
        Mask    ,
        Glasses ,

        // ⚔️ 무기 (Weapon)
        _Weapon_ = EquipSlot.Weapon << 16,
        Sword   , 
        Wand    , 
        Axe     , 
        Hammer  , 
        Bow     , 
        Arrow   ,
        
        // 🛡 방패 (Shield)
        _Shield_ = EquipSlot.Shield << 16,
        Shield
    }

    public static class ItemExtension
    {
        public static EquipSlot GetEquipSlot(this EquipType equipType)
        {
            return (EquipSlot)((int)equipType >> 16);
        }
        
        public static EquipType GetPureEquipType(this EquipType equipType)
        {
            return (EquipType)((int)equipType & 0xFFFF); // 하위 16비트만 남김
        }
        
        public static bool IsSameEquipType(this EquipType a, EquipType b)
        {
            return GetPureEquipType(a) == GetPureEquipType(b);
        }
    }
}