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

        // 몬스터의 실제 트랜스폼ㄹ
        protected Transform monsterTransform;

        public abstract void Init( int hp, int damage, List<Vector3Int> path);
        protected abstract void Walk(float dt);
        protected abstract void SetPath();
        protected abstract bool CheckPlayerUnit();
    }
}