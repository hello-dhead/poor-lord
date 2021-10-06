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
    }
}