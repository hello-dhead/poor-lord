using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class Nian_Spell : Skill
    {
        public override sealed void ExecuteSkill(Unit caster, Unit target, int damage)
        {
            StartCoroutine(Spell_Attack(caster, target, damage));
            return;
        }

        private IEnumerator Spell_Attack(Unit caster, Unit target, int damage)
        {
            SoundManager.Instance.PlaySfx("NukeLaunch", 0.2f);
            ParticleSystem effect = EffectManager.Instance.CreateEffect("NukeMissileRed", caster.gameObject.transform.transform.position + new Vector3(-0.3f, 0.5f, 0.4f)
                , new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(0, 0, 0)));

            Vector3 startPos = effect.gameObject.transform.position;
            Vector3 targetPos = target.gameObject.transform.position + new Vector3(0, 0.5f, 0);
            float duration = 0.2f * (startPos - targetPos).magnitude;

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
                SoundManager.Instance.PlaySfx("ExplosionNuke", 0.2f);
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(caster, target, damage));
                EffectManager.Instance.CreateEffect("NukeExplosionRed", effect.gameObject.transform.position, new Vector3(0.2f, 0.2f, 0.2f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
            }
            EffectManager.Instance.ReleaseEffect("NukeMissileRed", effect);
            PoolManager.Instance.Release<Nian_Spell>("Prefabs/Nian_Spell", this);
        }
    }
}