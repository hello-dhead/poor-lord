using UnityEngine;

namespace Assets.Scripts.Commons
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new T();
                return _instance;
            }
        }

    }

    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        protected static T _instance;
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }


        protected virtual void Awake()
        {
            if (_instance == null)
                _instance = GetComponent<T>();
            else
                DestroyImmediate(this);
        }

    }
}