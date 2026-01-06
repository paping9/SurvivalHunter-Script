using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils.Extension
{
    public static class UtilityEx
    {
        public static void SetActive(GameObject obj, bool active)
        {
            if (obj == null || obj.activeSelf == active)
                return;

            obj.SetActive(active);
        }

        public static void SetActive(Component cmp, bool active)
        {
            if (cmp == null || cmp.gameObject == null || cmp.gameObject.activeSelf == active)
                return;

            cmp.gameObject.SetActive(active);
        }

        public static string GetHierarchyName(this Transform tr)
        {
            string parentName = null;
            if (tr.parent != null)
            {
                parentName = GetHierarchyName(tr.parent);
            }

            if (string.IsNullOrEmpty(parentName))
            {
                return tr.gameObject.name;
            }

            return string.Format("{0}/{1}", parentName, tr.gameObject.name);
        }
        public static Transform GetPivotTrasnform(Transform target, string pivotName)
        {
            if (string.IsNullOrEmpty(pivotName) == false)
            {
                var parent = target.Find(pivotName);

                if (parent == null)
                {
                    parent = GetFindRecursive(target, pivotName);
                }

                if (parent != null)
                {
                    return parent;
                }
            }

            return target;
        }
        public static Transform GetFindRecursive(Transform tr, string pivotName)
        {
            if (tr.name == pivotName)
                return tr;

            for (int i = 0; i < tr.childCount; i++)
            {
                Transform pivot = GetFindRecursive(tr.GetChild(i), pivotName);

                if (pivot != null)
                    return pivot;
            }

            return null;
        }

        public static float LinearFunc(float a, float b, float c, float d, float x)
        {
            return (d - c) / (b - a) * (x - a) + c;
        }


        public static bool IsTowardByAngle(Vector3 position, Vector3 forward, Vector3 targetPos)
        {
            var toTarget = targetPos - position;
            toTarget.Normalize();
            var angle = Vector3.Angle(toTarget, forward);

            return angle < 1f;
        }

        public static float GetTargetDistance(Vector3 vTo, Vector3 vFrom)
        {
            Vector3 to = vTo;
            to.y = 0.0f;

            Vector3 from = vFrom;
            from.y = 0.0f;


            return Vector3.Distance(to, from);
        }


        public static int ConvertTimeToMs(this float time)
        {
            return (int)(time * 1000);
        }

        public static float ConvertIntToTime(this int value)
        {
            return (float)value * 0.1f;
        }

        public static int ConvertRate(this int rate)
        {
            return (int)((float)rate * 0.01f);
        }

        public static float ConvertFrameToTime(this float frame) => frame * 0.033f;
        public static float ConvertFrameToTime(this int frame) => (float)frame * 0.033f;

        public static void SetRecursiveLayer(Transform target, int layerMask)
        {
            target.gameObject.layer = layerMask;

            for (int i = 0; i < target.childCount; i++)
            {
                Transform child = target.GetChild(i);
                SetRecursiveLayer(child, layerMask);
            }

            return;
        }

        public static void ShuffleList<T>(this List<T> list, int count) where T : class
        {
            for (int i = 0; i < count; i++)
            {
                int src = Random.Range(0, list.Count);
                int desc = Random.Range(0, list.Count);

                T temp = list[src];
                list[src] = list[desc];
                list[desc] = temp;

            }
        }
        
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        
        public static int ManhattanDistance(this Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
        
        public static bool AnyWithinDistance(this IEnumerable<Vector2Int> list, Vector2Int target, int distanceThreshold)
        {
            foreach (var item in list)
            {
                if (item.ManhattanDistance(target) <= distanceThreshold)
                    return true;
            }
            return false;
        }
    }
}
