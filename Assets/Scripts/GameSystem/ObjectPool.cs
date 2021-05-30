using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    // 포함을 위한 인터페이스
    public interface ObjectPoolBase { }

    /// <summary>
    /// 컴포넌트 인스턴스만 저장하는 오브젝트풀
    /// </summary>
    public class ObjectPool<T> : ObjectPoolBase where T : UnityEngine.Component, new()
    {
        private List<T> disabledObjectList = new List<T>();
        private HashSet<T> enabledObjectHashSet = new HashSet<T>();

        // disable + enable 풀 크기
        private int maxPoolCount;
        private GameObject prefabObject;

        public ObjectPool(int count, GameObject prefab = null)
        {
            if (prefab.GetComponent<T>() != null)
                prefabObject = prefab;
            else
                prefabObject = null;

            maxPoolCount = count;
            for (int i = 0; i < count; ++i)
            {
                GameObject newObject;
                if (prefabObject == null)
                {
                    newObject = new GameObject();
                    disabledObjectList.Add(newObject.AddComponent<T>());
                }
                else
                {
                    newObject = GameObject.Instantiate(prefabObject);
                    disabledObjectList.Add(newObject.GetComponent<T>());
                }
                newObject.SetActive(false);
            }
        }

        // disabledObjectList에 오브젝트를 꺼내 enabledHashSet에 넣음
        public T Create()
        {
            if (disabledObjectList.Count == 0)
            {
                Expand(5);
            }

            T newObject = disabledObjectList[0];
            disabledObjectList.RemoveAt(0);
            enabledObjectHashSet.Add(newObject);
            newObject.gameObject.SetActive(true);
            return newObject;
        }

        // 오브젝트 비활성화
        public bool Release(T target)
        {
            if (enabledObjectHashSet.Remove(target) == false)
                return false;

            target.gameObject.SetActive(false);
            disabledObjectList.Add(target);
            return true;
        }

        // 오브젝트 파괴
        public bool Destroy(T target)
        {
            disabledObjectList.Remove(target);
            enabledObjectHashSet.Remove(target);
            GameObject.Destroy(target.gameObject);
            --maxPoolCount;
            return true;
        }

        // 모든 오브젝트풀 삭제
        public void DestoryAll()
        {
            for (int i = 0; i < disabledObjectList.Count; ++i)
                Destroy(disabledObjectList[i]);
            foreach (var target in enabledObjectHashSet)
                Destroy(target);
            disabledObjectList.Clear();
            enabledObjectHashSet.Clear();
        }

        // disabledObjectList 부족하면 추가로 증설
        private void Expand(int count)
        {
            maxPoolCount += count;
            for (int i = 0; i < count; ++i)
            {
                GameObject newObject;
                if (prefabObject == null)
                {
                    newObject = new GameObject();
                    disabledObjectList.Add(newObject.AddComponent<T>());
                }
                else
                {
                    newObject = GameObject.Instantiate(prefabObject);
                    disabledObjectList.Add(newObject.GetComponent<T>());
                }
                newObject.SetActive(false);
            }
        }
    }
}