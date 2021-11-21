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

            gameObject.transform.GetChild(0).transform.position = new Vector3(0 + Random.Range(-0.1f, 0.1f), 0.1f, -0.3f + Random.Range(-0.1f, 0.1f));

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

            pathList.Clear();
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
                    if (Target.HP <= 0)
                    {
                        if (CheckPlayerUnit() == false)
                        {
                            Target = null;
                            CurrentAttackDelay = AttackDelay;
                            currentState = MonsterUnitState.Walk;
                        }
                        break;
                    }

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

        public sealed override void Attack()
        {
            GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(this, Target, CalculateDamage()));
            if (Target.HP - CalculateDamage() <= 0)
            {
                Target = null;
                CurrentAttackDelay = AttackDelay;
                currentState = MonsterUnitState.Walk;
            }

            Vector3 effect_pos = this.gameObject.transform.position + ((Target.transform.position - this.gameObject.transform.position).normalized * 0.3f);
            effect_pos.y = 0.2f;
            GameManager.Instance.EffectSystem.CreateEffect("PickupExplosionBlue", effect_pos, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            SoundManager.Instance.PlaySfx("SlimeHit", 0.2f);
        }
    }
}
