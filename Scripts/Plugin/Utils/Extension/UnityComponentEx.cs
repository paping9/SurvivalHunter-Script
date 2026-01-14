using System;
using UnityEngine;

namespace Utils.Extension
{
    public static class UnityComponentEx
    {
        public static GameObject Instantiate(GameObject gameObject)
        {
            GameObject temp = GameObject.Instantiate(gameObject);
            InitLocalTransform(temp.transform);
            return temp;
        }

        public static GameObject Instantiate(GameObject gameObject, Transform parent)
        {
            GameObject temp = GameObject.Instantiate(gameObject, parent);
            InitLocalTransform(temp.transform);
            return temp;
        }

        public static T Instantiate<T>(T gameObject, Transform parent) where T : UnityEngine.Component
        {
            T temp = GameObject.Instantiate<T>(gameObject, parent);
            InitLocalTransform(temp.transform);

            return temp;
        }

        public static T Instantiate<T>(T gameObject) where T : UnityEngine.Component
        {
            if (null != gameObject)
            {
                T temp = GameObject.Instantiate<T>(gameObject);
                InitLocalTransform(temp.transform);

                return temp;
            }

            return null;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
#if UNITY_2019_2_OR_NEWER
            if (!gameObject.TryGetComponent<T>(out var component))
            {
                component = gameObject.AddComponent<T>();
            }
#else
        var component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
#endif

            return component;
        }

        public static T CreateMonoBehaviour<T>(string gameObjectName = null, Transform parent = null, bool worldPositionStays = false) where T : Component
        {
            GameObject newGameObject = new GameObject(string.IsNullOrEmpty(gameObjectName) ? typeof(T).Name : gameObjectName);
            if (parent != null)
            {
                newGameObject.transform.SetParent(parent, worldPositionStays);
            }
            return newGameObject.AddComponent<T>();
        }

        public static void ResetLocalTransform(Transform t)
        {
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
        }

        public static void DoRectTransformToStretch(RectTransform t)
        {
            t.anchorMin = Vector2.zero;
            t.anchorMax = Vector2.one;
            t.pivot = new Vector2(0.5f, 0.5f);

            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.sizeDelta = Vector2.zero;
            t.anchoredPosition = Vector2.zero;
        }
        /// <summary>
        /// Transform에 SetParent 할때 사용합니다.
        /// initTransform이 true면 worldPositionStays 값은 무시 됩니다.
        /// </summary>
        /// <param name="origin"> 가져다 붙일 Transform </param>
        /// <param name="parent"> origin이 붙을 대상 </param>
        /// <param name="sameWithParent"> true 면 origin을 parent에 붙인 후 LocalTransform 값을 Reset 시킨다. </param>
        /// <param name="worldPositionStays"> sameWithParent이 false일때만 작동, 
        ///                                 true 면 world Position값은 그대로 두고 local position값만 변경. 
        ///                                 false 면 local position값을 유지하고 그에 맞춰 world position값이 변경 됨. </param>
        public static void AttachTransform(Transform origin, Transform parent, bool sameWithParent = true, bool worldPositionStays = false)
        {
            if (sameWithParent)
            {
                origin.SetParent(parent);
                ResetLocalTransform(origin);
            }
            else
            {
                origin.SetParent(parent, worldPositionStays);
            }
        }

        public static void AttachTransform(Transform origin, Transform parent)
        {
            origin.SetParent(parent);
            InitLocalTransform(origin);
        }

        public static void AttachTransformSetWorldScaleInit(Transform origin, Transform parent)
        {
            if (origin.gameObject.activeSelf == false)
            {
                origin.localScale = Vector3.one;
                return;
            }

            origin.SetParent(null);
            origin.localScale = Vector3.one;
            origin.SetParent(parent);
        }

        public static void AttachRectTransform(RectTransform origin, RectTransform parent)
        {
            origin.SetParent(parent);
            InitRectTransform(origin);
        }

        public static void AttachGameObject(GameObject origin, GameObject parent)
        {
            AttachTransform(origin.transform, parent.transform);
        }

        public static void Reset(this Transform t)
        {
            InitLocalTransform(t);
        }

        public static void InitRectTransformOffset(RectTransform rect)
        {
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static void InitLocalTransform(Transform t)
        {
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
        }

        public static void InitPosRotLocalTransform(Transform t)
        {
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
        }

        public static void InitRectTransform(RectTransform t)
        {
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.sizeDelta = Vector2.zero;
            t.anchoredPosition3D = Vector3.zero;
        }

        public static bool CheckLayerInLayerMask(int layer, LayerMask layerMask)
        {
            return (layerMask & (1 << layer)) != 0;
        }

        /// <summary>
        /// 무거우니 가능하면 런타임에 쓰지 마세요.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newLayer"></param>
        /// <param name="withChilds"></param>
        public static void ChangeLayer(GameObject obj, int newLayer, bool withChilds = false)
        {
            obj.layer = newLayer;

            if (withChilds)
            {
                var childs = obj.GetComponentsInChildren<Transform>(true);
                foreach (var child in childs)
                {
                    child.gameObject.layer = newLayer;
                }
            }
        }

        /// <summary>
        /// 이거 엄청 느립니다. 가능하면 런타임에 사용하지 말고 로딩하는 타이밍에 씁시다.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursive(GameObject go, int layer)
        {
            Transform[] ts = go.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < ts.Length; i++)
            {
                ts[i].gameObject.layer = layer;
            }
        }

        public static void SetLayerRecursive(GameObject go, int targetLayer, int destLayer)
        {
            Transform[] ts = go.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i].gameObject.layer == targetLayer)
                    ts[i].gameObject.layer = destLayer;
            }
        }

        public static T[] Clone<T>(T[] source)
        {
            if (source == null)
            {
                return null;
            }
            T[] dest = (T[])Array.CreateInstance(typeof(T), source.Length);
            Array.Copy(source, dest, source.Length);
            return dest;
        }

        public static int GetLayerMask(string layer1 = "", string layer2 = "", string layer3 = "", string layer4 = "", string layer5 = "")
        {
            int retMask = 0;
            if (false == string.IsNullOrEmpty(layer1))
                retMask = retMask | 1 << LayerMask.NameToLayer(layer1);
            if (false == string.IsNullOrEmpty(layer2))
                retMask = retMask | 1 << LayerMask.NameToLayer(layer2);
            if (false == string.IsNullOrEmpty(layer3))
                retMask = retMask | 1 << LayerMask.NameToLayer(layer3);
            if (false == string.IsNullOrEmpty(layer4))
                retMask = retMask | 1 << LayerMask.NameToLayer(layer4);
            if (false == string.IsNullOrEmpty(layer5))
                retMask = retMask | 1 << LayerMask.NameToLayer(layer5);

            return retMask;
        }

        public static void SetLayerMaskChild(GameObject parents, string layer)
        {
            parents.layer = LayerMask.NameToLayer(layer);
            Transform[] trans = parents.GetComponentsInChildren<Transform>();
            for (int i = 0; i < trans.Length; ++i)
                trans[i].gameObject.layer = LayerMask.NameToLayer(layer);
        }

        public static Transform DestroyChild(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
            return transform;
        }

        public static void DestroyImmediateChild(Transform parents)
        {
            int childs = parents.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(parents.GetChild(i).gameObject);
            }
        }

        public static void DestoryComponentAll(Transform parents)
        {
            Component[] components = parents.GetComponents<Component>();

            for (int i = 0; i < components.Length; ++i)
            {
                if (components[i].GetType() != typeof(Transform))
                {
                    UnityEngine.Object.Destroy(components[i]);
                }
            }
        }
        
        public static Transform FindChildByName(string strName, Transform trans)
        {
            if (trans.name == strName)
            {
                return trans;
            }

            Transform ret = null;
            for (int i = 0; i < trans.childCount; i++)
            {
                ret = FindChildByName(strName, trans.GetChild(i));
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        public static void SetActive(GameObject gameObject, bool isActive)
        {
            if (gameObject == null || gameObject.activeSelf == isActive) return;
            
            gameObject.SetActive(isActive);
        }

    }
}
