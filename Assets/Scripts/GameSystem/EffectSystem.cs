using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Commons;

namespace poorlord
{
    /// <summary>
    /// EffectSystem의 역할 : 게임 내 모든 이펙트 관리
    /// </summary>
    public class EffectSystem : IUpdatable
    {
        // 실행 중인 이펙트 리스트
        private List<Effect> effectList = new List<Effect>();

        // 이펙트 위치
        private readonly string EFFECT_PATH = "Effects/";

        public EffectSystem()
        {
            GameManager.Instance.AddUpdate(this);
        }

        public void UpdateFrame(float dt)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                if (effectList[i].currentTime < effectList[i].lifeTime)
                {
                    effectList[i].currentTime += dt;
                }
                else
                {
                    ReleaseEffect(effectList[i].key, effectList[i].particle);
                    effectList.RemoveAt(i);
                    i--;
                }
            }
        }

        // 이펙트 생성
        public ParticleSystem CreateEffect(string key, Vector3 pos, Vector3 scale, Quaternion rotate, float lifeTime = 999, int count = 5)
        {
            string path = EFFECT_PATH + key;
            ParticleSystem result = PoolManager.Instance.GetOrCreateObjectPoolFromPath<ParticleSystem>(path, path, count);

            Effect e = new Effect(key, result, lifeTime);
            effectList.Add(e);

            result.gameObject.transform.position = pos;
            result.gameObject.transform.localScale = scale;
            result.gameObject.transform.rotation = rotate;
            result.Play();
            return result;
        }

        // 이펙트 삭제
        public void ReleaseEffect(string key, ParticleSystem particleObject)
        {
            particleObject.Stop();
            string path = EFFECT_PATH + key;
            PoolManager.Instance.Release<ParticleSystem>(path, particleObject);
        }

        public void RemoveAllEffect()
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                ReleaseEffect(effectList[i].key, effectList[i].particle);
            }
            effectList.Clear();
        }

        public class Effect
        {
            public string key;
            public float lifeTime;
            public float currentTime = 0;
            public ParticleSystem particle;

            public Effect(string key, ParticleSystem particle, float lifeTime)
            {
                this.key = key;
                this.particle = particle;
                this.lifeTime = lifeTime;
            }
        }
    }
}