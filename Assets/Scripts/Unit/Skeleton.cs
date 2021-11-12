using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class Skeleton : MonsterUnit
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

            gameObject.transform.GetChild(0).transform.position = new Vector3(-0.1f + Random.Range(-0.1f, 0.1f), 0.1f, -0.3f + Random.Range(-0.1f, 0.1f));

            unitName = "Skeleton";

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
                        Target = null;
                        CurrentAttackDelay = AttackDelay;
                        currentState = MonsterUnitState.Walk;
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
            StartCoroutine("SkeletonAttack");
        }

        private IEnumerator SkeletonAttack()
        {
            UnitAnimator.Play("Skeleton_Attack");

            yield return new WaitForSeconds(0.2f);

            if (Target.HP > 0)
            {
                SoundManager.Instance.PlaySfx("SkeletonHit", 0.2f);
                EffectManager.Instance.CreateEffect("SlashRed", gameObject.transform.position + new Vector3(0.5f, 0.5f, -0.2f), new Vector3(0.2f, 0.2f, 0.2f), Quaternion.Euler(new Vector3(90, 90, 0)), 2);
                GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(this, Target, CalculateDamage()));
            }
        }
    }
}
