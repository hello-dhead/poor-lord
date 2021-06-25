using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Commons
{
    public class CoroutineHandler : MonoBehaviour
    {
        IEnumerator enumerator = null;

        private void Coroutine(IEnumerator coro)
        {
            enumerator = coro;
            StartCoroutine(coro);
        }

        void Update()
        {
            if (enumerator != null)
            {
                if (enumerator.Current == null)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Stop()
        {
            StopCoroutine(enumerator.ToString());
            Destroy(gameObject);
        }

        public static CoroutineHandler Start_Coroutine(IEnumerator coro)
        {
            GameObject obj = new GameObject("CoroutineHandler");
            CoroutineHandler handler = obj.AddComponent<CoroutineHandler>();
            if (handler)
            {
                handler.Coroutine(coro);
            }
            return handler;
        }
    }
}