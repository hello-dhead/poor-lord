using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Commons;

namespace poorlord
{
    /// <summary>
    /// PoolManager의 역할 : 게임 내 모든 풀을 생성, 삭제 / 요청한 데이터를 풀에서 꺼내주는 관리자
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private Dictionary<string, ObjectPoolBase> poolDictionary = new Dictionary<string, ObjectPoolBase>();

        // key의 값이 있으면 바로 리턴하고 없으면 경로의 프리팹 풀을 생성한 다음 리턴 
        public T GetOrCreateObjectPoolFromPath<T>(string path, string key = null, int count = 5) where T : Component, new()
        {
            // key를 따로 지정해주지 않으면 path가 key가 됨 
            if (key == null)
                key = path;

            if (poolDictionary.ContainsKey(key) == true)
                return Create<T>(key);

            GameObject prefab = Resources.Load<GameObject>(path);

            CreateObjectPool<T>(key, prefab, count);
            return Create<T>(key);
        }

        //특정 타입의 컴포넌트를 가진 프리팹을 등록
        public bool CreateObjectPool<T>(string key, GameObject prefab, int count = 5) where T : Component, new()
        {
            if (poolDictionary.ContainsKey(key) == true)
                return false;

            ObjectPool<T> newPool = new ObjectPool<T>(count, prefab);
            poolDictionary.Add(key, newPool);
            return true;
        }

        //경로로 부터 특정 타입의 컴포넌트를 가진 프리팹을 로드하여 등록
        public bool CreateObjectPoolFromPath<T>(string key, string path, int count = 5) where T : Component, new()
        {
            if (poolDictionary.ContainsKey(key) == true)
                return false;

            GameObject prefab = Resources.Load<GameObject>(path);

            return CreateObjectPool<T>(key, prefab, count);
        }

        public bool DestroyPool<T>(string key) where T : Component, new()
        {
            ObjectPoolBase pool = null;
            if (poolDictionary.TryGetValue(key, out pool) == false)
                return false;

            ObjectPool<T> realPool = pool as ObjectPool<T>;
            realPool.DestoryAll();
            return true;
        }

        public T Create<T>(string key) where T : Component, new()
        {
            ObjectPoolBase pool = null;
            if (poolDictionary.TryGetValue(key, out pool) == false)
                return null;

            Type poolType = pool.GetType();
            if (poolType == typeof(ObjectPool<T>))
            {
                ObjectPool<T> componentPool = pool as ObjectPool<T>;
                return componentPool.Create();
            }
            else
            {
                return null;
            }
        }

        public bool Release<T>(string key, T target) where T : Component, new()
        {
            ObjectPoolBase pool = null;
            if (poolDictionary.TryGetValue(key, out pool) == false)
                return false;
            ObjectPool<T> componentPool = pool as ObjectPool<T>;
            if (componentPool == null)
                return false;
            return componentPool.Release(target);
        }
    }
}