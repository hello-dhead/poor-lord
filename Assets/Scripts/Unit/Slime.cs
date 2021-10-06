using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class Slime : MonsterUnit
    {
        private readonly float ATTACK_DELAY = 5;
        private readonly float SPEED = 0.5f;

        public sealed override void Init(int HP, int damage,  List<Vector3Int> path) // 필요한 스탯 최대체력 체력 공격력 공격범위, 
        {
            if (UnitAnimator == null)
            {
                UnitAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
            }
            else
            {
                UnitAnimator.Rebind();
                UnitAnimator.SetBool("dead", false);
            }

            unitName = "Slime";

            spriteRenderer = gameObject.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenderer.material.color = new Color(1, 1, 1, 1);

            spriteRenderer.flipX = true;
            monsterTransform = gameObject.transform;

            DamageMultiplier = 1;
            AdditionalDamage = 0;
            MaxHP = HP;
            this.HP = HP;
            Damage = damage;
            AttackDelay = ATTACK_DELAY;
            speed = SPEED;
            CurrentAttackDelay = AttackDelay;

            foreach (var pos in path)
                pathList.Add(pos);

            this.gameObject.transform.position = pathList[0];
            UnitPosition = pathList[0];

            currentState = MonsterUnitState.SetPath;

            GameManager.Instance.AddUpdate(this);
            GameManager.Instance.MessageSystem.Subscribe(typeof(PlayerUnitSummonEvent), this);
            GameManager.Instance.MessageSystem.Subscribe(typeof(DamageEvent), this);

            rangeTile.Add(new Vector3Int(0,0,0));
        }

        public sealed override void Dispose(bool isReleaseImmediately)
        {
            pathList = null;

            // 업데이트에서 제거
            GameManager.Instance.RemoveUpdate(this);

            GameManager.Instance.MessageSystem.Unsubscribe(typeof(PlayerUnitSummonEvent), this);
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(DamageEvent), this);
            rangeTile.Clear();

            Target = null;

            if (isReleaseImmediately == true)
                FieldObjectManager.Instance.ReleaseUnit(unitName, this);
        }

        public sealed override void UpdateFrame(float dt)
        {
            switch (currentState)
            {
                case MonsterUnitState.Walk:
                    Walk(dt);
                    break;
                case MonsterUnitState.SetPath:
                    SetPath();
                    break;
                case MonsterUnitState.Attack:
                    if (CurrentAttackDelay > AttackDelay)
                    {
                        CurrentAttackDelay = 0;
                        Attack();
                    }
                    else
                    {
                        CurrentAttackDelay += Time.deltaTime;
                    }
                    break;
                case MonsterUnitState.Dead:
                    StartCoroutine("Dead");
                    break;
                default:
                    break;
            }
        }

        public sealed override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(DamageEvent))
            {
                DamageEvent DamageEvent = e as DamageEvent;
                if (DamageEvent.Target == (Unit)this)
                {
                    EffectManager.Instance.CreateEffect("SwordImpactRed", this.gameObject.transform.position + new Vector3(-0.1f, 0.4f, 0), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
                    DamageEvent newDamageEvent = DamageEvent.Create(DamageEvent.Publisher, DamageEvent.Target, DamageEvent.Damage);

                    HP -= newDamageEvent.Damage;
                    if (HP <= 0)
                        currentState = MonsterUnitState.Dead;
                }
            }
            else if (e.GetType() == typeof(PlayerUnitSummonEvent))
            {
                PlayerUnitSummonEvent summonEvent = e as PlayerUnitSummonEvent;
                if (currentState == MonsterUnitState.Walk)
                {
                    for (int i = 0; i < rangeTile.Count; i++)
                    {
                        if(summonEvent.SummonTilePos == UnitPosition + rangeTile[i])
                        {
                            Target = summonEvent.SummonUnit;
                            currentState = MonsterUnitState.Attack;
                        }
                    }
                }
            }
            return false;
        }

        protected sealed override void Walk(float dt)
        {
            monsterTransform.position = Vector3.MoveTowards(monsterTransform.position, pathList[0], dt * speed);
            // 거리가 특정 거리 이하 일때 발생
            if (Vector3.Distance(monsterTransform.position, pathList[0]) <= 0.1)
            {
                currentState = MonsterUnitState.SetPath;
            }
            else if (Vector3.Distance(monsterTransform.position, pathList[0]) <= 0.5 && pathList[0] != UnitPosition)
            {
                // 절반정도 진입했으면 TileEnter, TileLeave발행하고 포함된 타일 변경 + 적이 있는지 체크
                GameManager.Instance.MessageSystem.Publish(TileLeaveEvent.Create(UnitPosition, this));
                UnitPosition = pathList[0];
                GameManager.Instance.MessageSystem.Publish(TileEnterEvent.Create(UnitPosition, this));
                if(CheckPlayerUnit())
                {
                    currentState = MonsterUnitState.Attack;
                }
            }
        }

        protected sealed override void SetPath()
        {
            // 다음 노드로 이동
            if (pathList.Count > 1)
            {
                pathList.RemoveAt(0);
                direction = ((Vector3)pathList[0] - UnitPosition).normalized;
                if(UnitPosition.x <= pathList[0].x)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
                currentState = MonsterUnitState.Walk;
            }
            else
            {
                // TODO : dispose 이후 데미지 주는거 추가해야함
                Dispose(true);
            }
        }

        public sealed override void Attack()
        {
            if (Target.HP - CalculateDamage() > 0)
            {
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(this, Target, CalculateDamage()));
            }
            else
            {
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(this, Target, CalculateDamage()));
                Target = null;
                CurrentAttackDelay = AttackDelay;
                currentState = MonsterUnitState.Walk;
            }

            Vector3 effect_pos = this.gameObject.transform.position + ((Target.transform.position - this.gameObject.transform.position).normalized*0.3f);
            effect_pos.y = 0.2f;
            EffectManager.Instance.CreateEffect("PickupExplosionBlue", effect_pos, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
        }

        public sealed override IEnumerator Dead()
        {
            Dispose(false);

            GameManager.Instance.MessageSystem.Publish(MonsterDeadEvent.Create(this, UnitPosition));
            UnitAnimator.SetBool("dead", true);
            yield return new WaitForSeconds(0.2f);
            EffectManager.Instance.CreateEffect("DeathStandard", this.gameObject.transform.position + new Vector3(0, 0.1f, -0.1f), new Vector3(0.25f, 0.25f, 0.25f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            SoundManager.Instance.PlaySfx("Weird04", 0.1f);
            yield return new WaitForSeconds(0.1f);
            float alpha = spriteRenderer.material.color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
            {
                Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0, t));
                spriteRenderer.material.color = newColor;
                yield return null;
            }
            UnitAnimator.SetBool("dead", false);
            FieldObjectManager.Instance.ReleaseUnit(unitName, this);
        }

        protected sealed override bool CheckPlayerUnit()
        {
            for (int i = 0; i < rangeTile.Count; i++)
            {
                Target = TileManager.Instance.GetContainPlayerUnit(UnitPosition.x + rangeTile[i].x, UnitPosition.z + rangeTile[i].z );
                if (Target != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
