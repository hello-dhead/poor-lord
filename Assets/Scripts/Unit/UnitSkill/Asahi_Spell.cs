using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class Asahi_Spell : Skill
    {
        public override sealed void ExecuteSkill(Unit caster, Unit target, int damage)
        {
            StartCoroutine(Spell_Attack(caster, target, damage));
            return;
        }

        private IEnumerator Spell_Attack(Unit caster, Unit target, int damage)
        {
            SoundManager.Instance.PlaySfx("Explosion8", 0.3f);
            ParticleSystem effect = EffectManager.Instance.CreateEffect("PoisonMissileGreen", caster.gameObject.transform.transform.position + new Vector3(-0.3f, 0.5f, 0.4f)
                , new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(0, 0, 0)));

            Vector3 startPos = effect.gameObject.transform.position;
            Vector3 targetPos = target.UnitPosition;
            float height = 0.35f * (startPos - targetPos).magnitude;
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
            SoundManager.Instance.PlaySfx("ExplosionGas", 0.3f);

            List<MonsterUnit> targetList = new List<MonsterUnit>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    List<MonsterUnit> m = TileManager.Instance.GetContainMonsterUnitList((int)targetPos.x + i, (int)targetPos.z + j);
                    if (m != null)
                    {
                        EffectManager.Instance.CreateEffect("DeathGreen", new Vector3((int)targetPos.x + i, 0, (int)targetPos.z + j), new Vector3(0.15f, 0.15f, 0.15f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
                        for (int k = 0; k < m.Count; k++)
                            targetList.Add(m[k]);
                    }
                }
            }

            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i].HP > 0)
                    GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(caster, targetList[i], damage));
            }

            EffectManager.Instance.ReleaseEffect("PoisonMissileGreen", effect);
            PoolManager.Instance.Release<Asahi_Spell>("Prefabs/Asahi_Spell", this);
        }
    }
}