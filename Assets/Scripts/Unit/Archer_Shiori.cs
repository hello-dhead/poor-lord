﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    public sealed class Archer_Shiori : PlayerUnit
    {
        private readonly int MAX_HP = 1000;
        private readonly int DAMAGE = 100;
        private readonly float ATTACK_DELAY = 2;

        /// <summary>
        /// 유닛명
        /// </summary>
        private readonly string UNIT_NAME = "Shiori";

        /// <summary>
        /// 공격에 사용 할 이펙트
        /// </summary>
        private readonly string ATTACK_EFFECT_NAME = "SlashYellow";

        /// <summary>
        /// 이펙트 위치
        /// </summary>
        private readonly Vector3 ATTACK_EFFECT_POS = new Vector3(-0.5f, 0.2f, 0);

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
            if (UnitAnimator == null)
            {
                UnitAnimator = this.gameObject.GetComponent<Animator>();
            }
            else
            {
                UnitAnimator.Rebind();
                UnitAnimator.Play("Idle");
            }
            gameObject.transform.GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 1);

            // 값 초기화
            MaxHP = MAX_HP;
            HP = MAX_HP;
            Damage = DAMAGE;
            AttackDelay = ATTACK_DELAY;
            AdditionalDamage = 0;
            DamageMultiplier = 1;
            unitName = UNIT_NAME;
            CurrentAttackDelay = 0;

            // 유닛 위치 초기화
            this.gameObject.transform.position = pos + new Vector3(0,0,-0.4f);
            UnitPosition = pos;

            // 상태 초기화
            currentState = PlayerUnitState.Init;

            // 애니메이션 초기화
            ResetBlinkCount();

            // 업데이트 추가
            GameManager.Instance.AddUpdate(this);

            // 필요한 구독 추가
            GameManager.Instance.MessageSystem.Subscribe(typeof(TileEnterEvent), this);
            GameManager.Instance.MessageSystem.Subscribe(typeof(DamageEvent), this);
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageEndEvent), this);
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageEndEvent), this);

            // 공격 범위 리스트 추가
            rangeTile.Add(pos);
            rangeTile.Add(pos + new Vector3Int(-1, 0, 0));
            rangeTile.Add(pos + new Vector3Int(0, 0, 1));
            rangeTile.Add(pos + new Vector3Int(0, 0, -1));

            // 버프 적용
            for (int i = 0; i < buff.Count; i++)
            {
                Buff buffCopy = buff[i].Copy();
                buffCopy.Init(this);
                buffList.Add(buffCopy);
            }

            GameManager.Instance.MessageSystem.Publish(PlayerUnitSummonEvent.Create(pos, this));
        }

        public sealed override void UpdateFrame(float dt)
        {
            switch (currentState)
            {
                case PlayerUnitState.Init:
                    if(CheckRangeTileTarget() == true)
                    {
                        currentState = PlayerUnitState.Attack;
                    }
                    else
                    {
                        currentState = PlayerUnitState.Idle;
                    }
                    break;
                case PlayerUnitState.Idle:
                    if (CurrentAttackDelay <= AttackDelay)
                    {
                        CurrentAttackDelay += dt;
                    }
                    break;
                case PlayerUnitState.Attack:
                    if(CurrentAttackDelay > AttackDelay)
                    {
                        ResetBlinkCount();
                        CurrentAttackDelay = 0;
                        Attack();
                    }
                    else
                    {
                        UnitAnimator.SetBool("attack", false);
                        CurrentAttackDelay += dt;
                    }
                    break;
                case PlayerUnitState.Dead:
                    StartCoroutine("Dead");
                    break;
                default:
                    break;
            }
        }

        public sealed override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(TileEnterEvent))
            {
                TileEnterEvent enterEvent = e as TileEnterEvent;
                if (currentState == PlayerUnitState.Idle)
                {
                    foreach (var rangeTilePos in rangeTile)
                    {
                        if (rangeTilePos == enterEvent.EnterTilePos)
                        {
                            currentState = PlayerUnitState.Attack;
                            Target = (MonsterUnit)enterEvent.EnterUnit;
                            return true;
                        }
                    }
                }
            }
            else if (e.GetType() == typeof(DamageEvent))
            {
                DamageEvent DamageEvent = e as DamageEvent;
                if(DamageEvent.Target == (Unit)this)
                {
                    DamageEvent newDamageEvent = DamageEvent.Create(DamageEvent.Publisher, DamageEvent.Target, DamageEvent.Damage);

                    if(HP > newDamageEvent.Damage)
                    {
                        HP -= newDamageEvent.Damage;
                    }
                    else
                    {
                        HP = 0;
                        currentState = PlayerUnitState.Dead;
                    }
                }
            }
            else if (e.GetType() == typeof(BattleStageEndEvent))
            {
                Dispose(true);
            }
            return false;
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
            yield return new WaitForSeconds(0.2f);

            if (Target != null && Target.HP > 0 && CheckMonsterInRange())
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(this, Target, CalculateDamage()));
            EffectManager.Instance.CreateEffect(ATTACK_EFFECT_NAME, UnitPosition + ATTACK_EFFECT_POS, ATTACK_EFFECT_SCALE, ATTACK_EFFECT_ROTATE, 2);
        }

        public sealed override void Dispose(bool isRelease)
        {
            // 업데이트에서 제거
            GameManager.Instance.RemoveUpdate(this);

            // 구독 해지
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(TileEnterEvent), this);
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(DamageEvent), this);
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(BattleStageEndEvent), this);

            // 특성 dispose
            for (int i = 0; i < buffList.Count; i++)
                buffList[i].Dispose();

            // 변수 초기화
            buffList.Clear();

            rangeTile.Clear();
            Target = null;

            if(isRelease == true)
                FieldObjectManager.Instance.ReleaseUnit(unitName, this);
        }

        public sealed override IEnumerator Dead()
        {
            Dispose(false);

            GameManager.Instance.MessageSystem.Publish(PlayerUnitDeadEvent.Create(UnitPosition, this));
            UnitAnimator.SetBool("dead", true);
            yield return new WaitForSeconds(1f);
            float alpha = gameObject.transform.GetComponent<SpriteRenderer>().material.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0, t));
                transform.GetComponent<SpriteRenderer>().material.color = newColor;
                yield return null;
            }
            UnitAnimator.SetBool("dead", false);
            FieldObjectManager.Instance.ReleaseUnit(unitName, this);
        }

        private bool CheckRangeTileTarget()
        {
            for (int i = 0; i < rangeTile.Count; i++)
            {
                List<MonsterUnit> m = TileManager.Instance.GetContainMonsterUnitList(rangeTile[i].x, rangeTile[i].z);
                if (m == null)
                    continue;

                if (m.Count > 0)
                {
                    Target = TileManager.Instance.GetContainMonsterUnitList(rangeTile[i].x, rangeTile[i].z)[0];
                    return true;
                }
            }
            return false;
        }

        private bool CheckMonsterInRange()
        {
            for (int i = 0; i < rangeTile.Count; i++)
            {
                if(rangeTile[i] == Target.UnitPosition)
                    return true;
            }
            return false;
        }

        private void AddBlinkCount()
        {
            if(UnitAnimator != null)
                UnitAnimator.SetInteger("blink", UnitAnimator.GetInteger("blink") + 1);
        }

        private void ResetBlinkCount()
        {
            UnitAnimator.SetInteger("blink", 0);
        }
    }
}