using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class Shiori_Arrow : Skill
    {
        public override sealed void ExecuteSkill(Unit caster, Unit target, int damage)
        {
            StartCoroutine(Arrow_Attack(caster, target, damage));
            return;
        }

        private IEnumerator Arrow_Attack(Unit caster, Unit target, int damage)
        {
            ParticleSystem effect = GameManager.Instance.EffectSystem.CreateEffect("AcidMissileRed", caster.gameObject.transform.transform.position + new Vector3(-0.3f, 0.5f, 0.1f)
                , new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(0, 0, 0)));

            Vector3 startPos = effect.gameObject.transform.position;
            Vector3 targetPos = target.gameObject.transform.position + new Vector3(0, 0.5f, 0);
            float duration = 0.1f * (startPos - targetPos).magnitude;

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
                SoundManager.Instance.PlaySfx("GunShot05", 0.2f);
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(caster, target, damage));
                GameManager.Instance.EffectSystem.CreateEffect("AcidExplosionRed", effect.gameObject.transform.position, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
            }
            GameManager.Instance.EffectSystem.ReleaseEffect("AcidMissileRed", effect);
            PoolManager.Instance.Release<Shiori_Arrow>("Prefabs/Shiori_Arrow", this);
        }
    }
}