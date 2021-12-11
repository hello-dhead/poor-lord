using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    public sealed class Archer_Serika : PlayerUnit
    {
        private readonly int MAX_HP = 500;
        private readonly int DAMAGE = 100;
        private readonly float ATTACK_DELAY = 2;

        /// <summary>
        /// 유닛명
        /// </summary>
        private readonly string UNIT_NAME = "Serika";

        private List<MonsterUnit> targetList = new List<MonsterUnit>();

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
            rangeTile.Add(pos + new Vector3Int(0, 0, 1));
            rangeTile.Add(pos + new Vector3Int(-1, 0, 1));
            rangeTile.Add(pos + new Vector3Int(-2, 0, 1));
            rangeTile.Add(pos + new Vector3Int(0, 0, -1));
            rangeTile.Add(pos + new Vector3Int(-1, 0, -1));
            rangeTile.Add(pos + new Vector3Int(-2, 0, -1));

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
            yield return new WaitForSeconds(0.3f);

            if (HP > 0)
            {
                for (int i = 0; i < rangeTile.Count; i++)
                {
                    List<MonsterUnit> m = TileManager.Instance.GetContainMonsterUnitList(rangeTile[i].x, rangeTile[i].z);
                    if (m != null)
                    {
                        for (int j = 0; j < m.Count; j++)
                            targetList.Add(m[j]);
                    }
                }

                if (targetList.Count > 0)
                    SoundManager.Instance.PlaySfx("SnowExplosion", 0.3f);
                for (int i = 0; i < targetList.Count; i++)
                {
                    Serika_Arrow arrow = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Serika_Arrow>("Prefabs/Serika_Arrow");
                    arrow.ExecuteSkill(this, targetList[i], CalculateDamage());
                }
                targetList.Clear();
            }
        }
    }
}