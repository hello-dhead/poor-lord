using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    public sealed class Warrior_Fuyuko : PlayerUnit
    {
        private readonly int MAX_HP = 800;
        private readonly int DAMAGE = 75;
        private readonly float ATTACK_DELAY = 1f;

        /// <summary>
        /// 유닛명
        /// </summary>
        private readonly string UNIT_NAME = "Fuyuko";

        /// <summary>
        /// 공격에 사용 할 이펙트
        /// </summary>
        private readonly string ATTACK_EFFECT_NAME = "SpikyExplosionBlue";

        /// <summary>
        /// 이펙트 스케일
        /// </summary>
        private readonly Vector3 ATTACK_EFFECT_SCALE = new Vector3(0.3f, 0.3f, 0.3f);

        /// <summary>
        /// 이펙트 로테이션
        /// </summary>
        private readonly Quaternion ATTACK_EFFECT_ROTATE = Quaternion.Euler(new Vector3(-90, 90, 0));

        public override void Init(Vector3Int pos, List<Buff> buff)
        {
            // 애니메이터 초기화
            ResetAnimator();

            // 필수적인 구독 추가
            SubscribeEssentialEvent();

            // 공격 범위 리스트 추가
            rangeTile.Add(pos);
            rangeTile.Add(pos + new Vector3Int(-1, 0, 0));
            rangeTile.Add(pos + new Vector3Int(-2, 0, 0));

            // 필수적인 init
            SetEssentialInit(pos, buff, MAX_HP, DAMAGE, ATTACK_DELAY, UNIT_NAME);
        }

        public sealed override void Attack()
        {
            if(Target.HP > 0 && CheckMonsterInRange())
            {
                StartCoroutine(AttackRoutine());
                UnitAnimator.SetBool("attack", true);
            }
            else
            {
                UnitAnimator.SetBool("attack", false);
                Target = null;
                if (CheckRangeTileTarget() == false)
                {
                    CurrentAttackDelay = AttackDelay;
                    currentState = PlayerUnitState.Idle;
                }
            }
        }

        private IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(0.35f);

            if (Target != null && Target.HP > 0 && CheckMonsterInRange() && HP > 0)
            {
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(this, Target, CalculateDamage()));
                GameManager.Instance.EffectSystem.CreateEffect(ATTACK_EFFECT_NAME, Target.transform.position + new Vector3(-0.5f, 0.2f, 0.2f), ATTACK_EFFECT_SCALE, ATTACK_EFFECT_ROTATE, 2);
                SoundManager.Instance.PlaySfx("Fuyuko_Attack", 0.7f);

                yield return new WaitForSeconds(0.1f);

                if (Target != null && Target.HP > 0 && CheckMonsterInRange() && HP > 0)
                    GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(this, Target, CalculateDamage()));
                GameManager.Instance.EffectSystem.CreateEffect(ATTACK_EFFECT_NAME, Target.transform.position + new Vector3(0.5f, 0.2f, -0.2f), ATTACK_EFFECT_SCALE, ATTACK_EFFECT_ROTATE, 2);
            }
        }
    }
}