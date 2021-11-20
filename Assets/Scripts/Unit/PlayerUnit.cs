using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    public enum PlayerUnitState
    {
        // 처음 들어온 상태
        Init,
        // 가만히 있는 상태
        Idle,
        // 범위에 몬스터가 있어서 전투 중
        Attack,
        // 죽는 연출 중
        Dead
    }

    /// <summary>
    /// 플레이어 유닛
    /// </summary>
    public abstract class PlayerUnit : Unit
    {
        protected List<Buff> buffList = new List<Buff>();

        protected PlayerUnitState currentState = PlayerUnitState.Idle;

        public abstract void Init(Vector3Int pos, List<Buff> buff);

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
                if (DamageEvent.Target == (Unit)this)
                {
                    DamageEvent newDamageEvent = DamageEvent.Create(DamageEvent.Publisher, DamageEvent.Target, DamageEvent.Damage);

                    if (HP > newDamageEvent.Damage)
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

        public override void UpdateFrame(float dt)
        {
            switch (currentState)
            {
                case PlayerUnitState.Init:
                    if (CheckRangeTileTarget() == true)
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
                    if (CurrentAttackDelay > AttackDelay)
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

            if (isRelease == true)
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

        // 자신의 타일안에 몬스터가 존재하는지 체크
        protected bool CheckRangeTileTarget()
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

        // 몬스터가 자신의 범위안에 있는지 체크
        protected bool CheckMonsterInRange()
        {
            for (int i = 0; i < rangeTile.Count; i++)
            {
                if (rangeTile[i] == Target.UnitPosition)
                    return true;
            }
            return false;
        }

        // 눈 깜박이는 애니메이션 카운트 추가 애니메이터에서 콜백으로 호출
        protected void AddBlinkCount()
        {
            if (UnitAnimator != null)
                UnitAnimator.SetInteger("blink", UnitAnimator.GetInteger("blink") + 1);
        }

        // 눈 깜박이는 애니메이션 카운트 초기화 애니메이터에서 콜백으로 호출
        protected void ResetBlinkCount()
        {
            UnitAnimator.SetInteger("blink", 0);
        }

        // 애니메이터 초기화
        protected void ResetAnimator()
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

            ResetBlinkCount();
        }

        // 필수 구독 추가
        protected void SubscribeEssentialEvent()
        {
            GameManager.Instance.MessageSystem.Subscribe(typeof(TileEnterEvent), this);
            GameManager.Instance.MessageSystem.Subscribe(typeof(DamageEvent), this);
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageEndEvent), this);
        }

        // 필수 init 처리
        protected void SetEssentialInit(Vector3Int pos, List<Buff> buff, int hp, int damage, float attack_delay, string name)
        {
            // 버프 적용
            for (int i = 0; i < buff.Count; i++)
            {
                Buff buffCopy = buff[i].Copy();
                buffCopy.Init(this);
                buffList.Add(buffCopy);
            }

            // 상태 초기화
            currentState = PlayerUnitState.Init;

            // 값 초기화
            MaxHP = hp;
            HP = hp;
            Damage = damage;
            AttackDelay = attack_delay;
            AdditionalDamage = 0;
            DamageMultiplier = 1;
            unitName = name;
            CurrentAttackDelay = 0;

            // 유닛 위치 초기화
            this.gameObject.transform.position = pos + new Vector3(0, 0, -0.4f);
            UnitPosition = pos;

            // 업데이트 추가
            GameManager.Instance.AddUpdate(this);

            GameManager.Instance.MessageSystem.Publish(PlayerUnitSummonEvent.Create(pos, this));
        }
    }
}