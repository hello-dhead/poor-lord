using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Commons;

namespace poorlord
{
    /// <summary>
    /// UnitManager의 역할 : 유닛의 프리팹, 생성 관리
    /// </summary>
    public enum UnitID
    {
        // 전사1
        Alice,
        // 플레이어 유닛
        PlayerUnitMax,
        // 슬라임
        Slime
    }

    public enum CardValue
    {
        // 1번째
        Bronze,
        // 2번째
        Gold,
        // 3번째
        Legend
    }

    public class UnitManager : Singleton<UnitManager>
    {
        // 유닛의 이름으로 생성
        public Unit CreateUnit(string unitString)
        {
            UnitID unitID = (UnitID)Enum.Parse(typeof(UnitID), unitString);

            return CreateUnit(unitID);
        }

        // 유닛의 타입으로 생성
        public Unit CreateUnit(UnitID unitID)
        {
            switch (unitID)
            {
                case UnitID.Alice:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Warrior_Alice>("Prefabs/Warrior_Alice", "Prefabs/Warrior_Alice");
                case UnitID.Slime:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Slime>("Prefabs/Monster_Slime", "Prefabs/Monster_Slime");
                default:
                    return null;
            }
        }

        // 유닛의 이름으로 생성
        public bool ReleaseUnit(string unitString, Unit unit)
        {
            UnitID unitID = (UnitID)Enum.Parse(typeof(UnitID), unitString);

            return ReleaseUnit(unitID, unit);
        }

        // 유닛의 타입으로 생성
        public bool ReleaseUnit(UnitID unitID, Unit unit)
        {
            switch (unitID)
            {
                case UnitID.Alice:
                    PoolManager.Instance.Release<Warrior_Alice>("Prefabs/Warrior_Alice", (Warrior_Alice)unit);
                    return true;
                case UnitID.Slime:
                    PoolManager.Instance.Release<Slime>("Prefabs/Monster_Slime", (Slime)unit);
                    return true;
                default:
                    return false;
            }
        }
    }
}