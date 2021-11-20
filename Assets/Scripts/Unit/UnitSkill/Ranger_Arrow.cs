using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class Ranger_Arrow : Skill
    {
        public override sealed void ExecuteSkill(Unit caster, Unit target, int damage)
        {
            StartCoroutine(Arrow_Attack(caster, target, damage));
            return;
        }

        private IEnumerator Arrow_Attack(Unit caster, Unit target, int damage)
        {
            ParticleSystem effect = EffectManager.Instance.CreateEffect("RocketRed", caster.gameObject.transform.transform.position + new Vector3(-0.7f, 0.5f, 0.2f)
                , new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(0, 0, 0)));

            Vector3 startPos = effect.gameObject.transform.position;
            Vector3 targetPos = target.gameObject.transform.position + new Vector3(0, 0.5f, 0);
            float duration = 0.05f * (startPos - targetPos).magnitude;

            float timePassed = 0;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                float progress = timePassed / duration;

                effect.gameObject.transform.position = Vector3.Lerp(startPos, targetPos, progress);
                yield return null;
            }

            if (target.HP > 0)
            {
                SoundManager.Instance.PlaySfx("GunShot03", 0.7f);
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(caster, target, damage));
                EffectManager.Instance.CreateEffect("RocketExplosionRed", effect.gameObject.transform.position, new Vector3(0.2f, 0.2f, 0.2f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            }
            EffectManager.Instance.ReleaseEffect("RocketRed", effect);
            PoolManager.Instance.Release<Ranger_Arrow>("Prefabs/Ranger_Arrow", this);
        }
    }
}