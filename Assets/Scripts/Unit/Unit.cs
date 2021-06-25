using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    /// <summary>
    /// 몬스터와 플레이어 유닛이 둘다 상속받는 인터페이스
    /// </summary>
    public abstract class Unit : MonoBehaviour, IEventListener, IUpdatable
    {
        public int MaxHP { get; protected set; }
        public int HP { get; protected set; }
        public int Damage { get; protected set; }
        public float AttackDelay { get; protected set; }
        public float CurrentAttackDelay { get; protected set; }
        public Animator UnitAnimator { get; protected set; }
        public Unit Target { get; protected set; }

        // 속한 타일의 포지션
        public Vector3Int UnitPosition { get; protected set; }

        public abstract void UpdateFrame(float dt);
        public abstract bool OnEvent(IEvent e);
        public abstract void Attack();
        public abstract IEnumerator Dead();
        public abstract void Dispose(bool isRelease);

        protected List<Vector3Int> rangeTile = new List<Vector3Int>();

        // 자기의 이름
        protected string unitName;

        public void SetMaxHP(int amount)
        {
            MaxHP = amount;
        }

        public void SetHP(int amount)
        {
            HP = amount;
        }

        public void SetDamage(int amount)
        {
            Damage = amount;
        }
    }
}