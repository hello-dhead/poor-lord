using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    public sealed class Caster_Nian : PlayerUnit
    {
        private readonly int MAX_HP = 200;
        private readonly int DAMAGE = 400;
        private readonly float ATTACK_DELAY = 5;

        /// <summary>
        /// 유닛명
        /// </summary>
        private readonly string UNIT_NAME = "Nian";

        public override void Init(Vector3Int pos, List<Buff> buff)
        {
            // 애니메이터 초기화
            ResetAnimator();

            // 필수적인 구독 추가
            SubscribeEssentialEvent();

            // 공격 범위 리스트 추가
            for (int i = -1; i < 1; i++)
            {
                rangeTile.Add(pos + new Vector3Int(0, 0, i));
                rangeTile.Add(pos + new Vector3Int(-1, 0, i));
                rangeTile.Add(pos + new Vector3Int(-2, 0, i));
                rangeTile.Add(pos + new Vector3Int(-3, 0, i));
                rangeTile.Add(pos + new Vector3Int(-4, 0, i));
            }

            // 필수적인 init
            SetEssentialInit(pos, buff, MAX_HP, DAMAGE, ATTACK_DELAY, UNIT_NAME);
        }

        public sealed override void Attack()
        {
            if(Target.HP > 0 && CheckMonsterInRange())
            {
                StartCoroutine("AttackRoutine");
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
            yield return new WaitForSeconds(0.4f);

            if (HP > 0)
            {
                Nian_Spell spell = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Nian_Spell>("Prefabs/Nian_Spell");
                spell.ExecuteSkill(this, Target, CalculateDamage());
            }
        }
    }
}