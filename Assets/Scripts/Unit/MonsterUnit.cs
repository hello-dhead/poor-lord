using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public enum MonsterUnitState
    {
        // 적 성으로 가는 중
        Walk,
        // 경로 설정 중
        SetPath,
        // 범위에 유닛이 있어서 전투 중
        Attack,
        // 죽는 연출 중
        Dead
    }

    /// <summary>
    /// 몬스터 타입
    /// </summary>
    public abstract class MonsterUnit : Unit
    {
        protected List<Vector3Int> pathList = new List<Vector3Int>();

        protected MonsterUnitState currentState = MonsterUnitState.Walk;

        protected float speed;
        protected Vector3 direction;
        protected SpriteRenderer spriteRenderer;

        // 몬스터의 실제 트랜스폼
        protected Transform monsterTransform;

        public abstract void Init( int hp, int damage, List<Vector3Int> path);

        // 구독한 이벤트 처리
        public sealed override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(DamageEvent))
            {
                DamageEvent DamageEvent = e as DamageEvent;
                if (DamageEvent.Target == (Unit)this)
                {
                    GameManager.Instance.EffectSystem.CreateEffect("SwordImpactRed", this.gameObject.transform.position + new Vector3(-0.1f, 0.4f, 0), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
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
                        if (summonEvent.SummonTilePos == UnitPosition + rangeTile[i])
                        {
                            Target = summonEvent.SummonUnit;
                            currentState = MonsterUnitState.Attack;
                        }
                    }
                }
            }
            return false;
        }

        protected void Walk(float dt)
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
                if (CheckPlayerUnit())
                {
                    currentState = MonsterUnitState.Attack;
                }
            }
        }

        public sealed override IEnumerator Dead()
        {
            Dispose(false);

            GameManager.Instance.MessageSystem.Publish(MonsterDeadEvent.Create(this, UnitPosition));
            UnitAnimator.SetBool("dead", true);
            yield return new WaitForSeconds(0.2f);
            GameManager.Instance.EffectSystem.CreateEffect("DeathStandard", this.gameObject.transform.position + new Vector3(0, 0.1f, -0.1f), new Vector3(0.25f, 0.25f, 0.25f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            SoundManager.Instance.PlaySfx("Weird0" + Random.Range(1, 5), 0.1f);
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

        protected void SetPath()
        {
            // 다음 노드로 이동
            if (pathList.Count > 1)
            {
                pathList.RemoveAt(0);
                direction = ((Vector3)pathList[0] - UnitPosition).normalized;
                if (UnitPosition.x <= pathList[0].x)
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
                Dispose(true);
            }
        }

        protected bool CheckPlayerUnit()
        {
            for (int i = 0; i < rangeTile.Count; i++)
            {
                Target = TileManager.Instance.GetContainPlayerUnit(UnitPosition.x + rangeTile[i].x, UnitPosition.z + rangeTile[i].z);
                if (Target != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void ChangePath(List<Vector3Int> newPathList)
        {
            pathList = newPathList;
        }

        public List<Vector3Int> GetPathList()
        {
            return pathList;
        }
    }
}