using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class Rock : Skill
    {
        public override sealed void ExecuteSkill(Unit caster, Unit target, int damage)
        {
            StartCoroutine(RockAttack(caster, target, damage));
            return;
        }

        private IEnumerator RockAttack(Unit caster, Unit target, int damage)
        {
            this.gameObject.transform.position = caster.gameObject.transform.GetChild(0).transform.position + new Vector3(0, 0.5f, 0.6f);
            yield return new WaitForSeconds(0.3f);

            // sin을 그리며 날아가는 로직
            Vector3 startPos = this.gameObject.transform.position;
            Vector3 targetPos = target.UnitPosition + new Vector3(0, 0.8f, 0);
            float height = 0.25f * (startPos - targetPos).magnitude;
            float duration = 0.2f * (startPos - targetPos).magnitude;

            float timePassed = 0;
            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;
                float progress = timePassed / duration;
                float currentY = Mathf.Sin(Mathf.PI * progress) * height;
                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, progress);
                currentPos.y += currentY;

                this.gameObject.transform.position = currentPos;
                yield return null;
            }

            if (target.HP > 0)
            {
                SoundManager.Instance.PlaySfx("RockHit");
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(caster, target, damage));
                GameManager.Instance.EffectSystem.CreateEffect("SpikyExplosionYellow", this.gameObject.transform.position, new Vector3(0.2f, 0.2f, 0.2f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
            }

            PoolManager.Instance.Release<Rock>("Prefabs/Rock", this);
        }
    }
}