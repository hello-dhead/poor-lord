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
        /// <summary>
        /// 최대 체력
        /// </summary>
        public int MaxHP { get; protected set; }

        /// <summary>
        /// 현재 체력
        /// </summary>
        public int HP { get; protected set; }

        /// <summary>
        /// 기본 공격력
        /// </summary>
        public int Damage { get; protected set; }

        /// <summary>
        /// 데미지 배율
        /// </summary>
        public int DamageMultiplier { get; protected set; }

        /// <summary>
        /// 데미지 추가 값
        /// </summary>
        public int AdditionalDamage { get; protected set; }

        /// <summary>
        /// 공격 딜레이
        /// </summary>
        public float AttackDelay { get; protected set; }

        /// <summary>
        /// 현재 공격 딜레이
        /// </summary>
        public float CurrentAttackDelay { get; protected set; }

        /// <summary>
        /// 공격 타겟
        /// </summary>
        public Unit Target { get; protected set; }

        /// <summary>
        /// 속한 타일의 포지션
        /// </summary>
        public Vector3Int UnitPosition { get; protected set; }

        /// <summary>
        /// 애니메이터
        /// </summary>
        public Animator UnitAnimator { get; protected set; }

        /// <summary>
        /// 유닛명
        /// </summary>
        protected string unitName;

        public abstract void UpdateFrame(float dt);
        public abstract bool OnEvent(IEvent e);
        public abstract void Attack();
        public abstract IEnumerator Dead();
        public abstract void Dispose(bool isRelease);

        protected List<Vector3Int> rangeTile = new List<Vector3Int>();

        // 데미지 배율, 추가 공격력 처리가 끝난 최종 데미지
        public int CalculateDamage()
        {
            return (Damage + AdditionalDamage) * DamageMultiplier;
        }

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

        public void AddDamageMultiplier(int amount)
        {
            DamageMultiplier *= amount;
        }

        public void AddAdditionalDamage(int amount)
        {
            AdditionalDamage += amount;
        }

        public void SubDamageMultiplier(int amount)
        {
            DamageMultiplier /= amount;
        }

        public void SubAdditionalDamage(int amount)
        {
            if (AdditionalDamage < amount)
            {
                AdditionalDamage = 0;
            }
            else
            {
                AdditionalDamage -= amount;
            }
        }
    }
}