using UnityEngine;
using Utils.Extension;

namespace Game
{
    public class UnitHairItem : UnitEquipItem
    {
        [SerializeField] private GameObject _fullHair;
        [SerializeField] private GameObject _halfHair;

        public void SetFullHair()
        {
            UnityComponentEx.SetActive(_fullHair, true);
            UnityComponentEx.SetActive(_halfHair, false);
        }

        public void SetHalfHair()
        {
            UnityComponentEx.SetActive(_fullHair, false);
            UnityComponentEx.SetActive(_halfHair, true);
        }
    }
}