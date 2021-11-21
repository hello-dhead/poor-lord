using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class Serika_Arrow : Skill
    {
        public override sealed void ExecuteSkill(Unit caster, Unit target, int damage)
        {
            StartCoroutine(Arrow_Attack(caster, target, damage));
            return;
        }

        private IEnumerator Arrow_Attack(Unit caster, Unit target, int damage)
        {
            ParticleSystem effect = GameManager.Instance.EffectSystem.CreateEffect("ShadowMissile", caster.gameObject.transform.transform.position + new Vector3(-0.3f, 0.5f, 0.2f)
                , new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(0, 0, 0)));

            Vector3 startPos = effect.gameObject.transform.position;
            Vector3 targetPos = target.gameObject.transform.position + new Vector3(0, 0.5f, -0.1f);
            float height = 0.25f * (startPos - targetPos).magnitude;
            float duration = 0.25f * (startPos - targetPos).magnitude;

            float timePassed = 0;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                float progress = timePassed / duration;

                float currentY = Mathf.Sin(Mathf.PI * progress) * height;
                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, progress);
                currentPos.y += currentY;
                effect.gameObject.transform.position = currentPos;
                yield return null;
            }

            if (target.HP > 0)
            {
                SoundManager.Instance.PlaySfx("StormExplosion", 0.3f);
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(caster, target, damage));
                GameManager.Instance.EffectSystem.CreateEffect("ShadowExplosion", effect.gameObject.transform.position, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
            }
            GameManager.Instance.EffectSystem.ReleaseEffect("ShadowMissile", effect);
            PoolManager.Instance.Release<Serika_Arrow>("Prefabs/Serika_Arrow", this);
        }
    }
}