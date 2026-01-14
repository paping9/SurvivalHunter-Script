using System.Collections.Generic;
using System.Linq;
using Defs;
using UnityEngine;
using Utils.Extension;

namespace Game
{
    using Domain;
    
    public class UnitEquipItem : MonoBehaviour
    {
        private SkinnedMeshRenderer[] _skinnedMeshRenderers;
        private EquipItem _equipItem;

        public EquipSlot EquipSlot => _equipItem.EquipType.GetEquipSlot();
        public EquipType EquipType => _equipItem.EquipType;
        public string PivotName => _equipItem.PivotName;
        
        public void Initialized(EquipItem equipItem)
        {
            _skinnedMeshRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>();
            _equipItem = equipItem;
        }
        
        public bool SetRoot(Transform rootBone, Transform rootTransform)
        {
            SetSkinnedMeshParts(this.gameObject, rootBone);

            this.transform.SetParent(rootTransform);
            
            var rootPivot = UnityComponentEx.FindChildByName("root", this.transform);
            if(rootPivot != null)
                GameObject.DestroyImmediate(rootPivot.gameObject);
            
            UnityComponentEx.InitLocalTransform(this.transform);
            
            _skinnedMeshRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>();
            
            return true;
        }
        
        private void SetSkinnedMeshParts(GameObject partsObject, Transform root)
        {
            SkinnedMeshRenderer[] srcRenderers = partsObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var srcRenderer in srcRenderers)
            {
                SetSkinnedMeshRenderer(srcRenderer, root);
            }
        }

        private void SetSkinnedMeshRenderer(SkinnedMeshRenderer srcRenderer, Transform root)
        {
            if (srcRenderer == null)
            {
                return;
            }

            int nCount = srcRenderer.bones.Length;
            Transform[] destBones = new Transform[nCount];

            for (int j = 0; j < nCount; j++)
            {
                if(srcRenderer.bones[j] != null)
                    destBones[j] = UnityComponentEx.FindChildByName(srcRenderer.bones[j].name, root);
            }

            srcRenderer.bones = destBones;
            srcRenderer.rootBone = root;
                

            // GameObject part = GameObject.Instantiate(srcRenderer.gameObject);
            //
            // GameObject.DestroyImmediate(srcRenderer.gameObject);

            //return part;
        }
    }
}