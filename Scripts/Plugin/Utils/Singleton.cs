using UnityEngine;

namespace Utils
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }

        public static bool CheckInstance()
        {
            return _instance != null ? true : false;
        }

        public virtual void OnDestroy()
        {
            _instance = null;
        }

        public static bool HasInstance()
        {
            return _instance != null ? true : false;
        }
    }

    public class SingletonMB<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                }

                return _instance;
            }
        }

        public static bool CheckInstance()
        {
            return _instance != null ? true : false;
        }

        public virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}