using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Commons;

namespace poorlord
{
    /// <summary>
    /// FieldObjectManager의 역할 : 필드오브젝트의 프리팹, 생성 관리
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

    public enum BlockID
    {
        // 블럭
        PlayerTile1x1,
        PlayerTile1x4,
        PlayerTile2x2,
        PlayerTile3x3,
        PlayerTile4x1,
        PlayerTilePlus,
        PlayerTileSickle01,
        PlayerTileSickle02,
        PlayerTileVane01,
        PlayerTileVane02,
        BlockMax
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

    public class FieldObjectManager : Singleton<FieldObjectManager>
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
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Warrior_Alice>("Prefabs/Warrior_Alice");
                case UnitID.Slime:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Slime>("Prefabs/Monster_Slime");
                default:
                    return null;
            }
        }

        // 유닛의 이름으로 해제
        public bool ReleaseUnit(string unitString, Unit unit)
        {
            UnitID unitID = (UnitID)Enum.Parse(typeof(UnitID), unitString);

            return ReleaseUnit(unitID, unit);
        }

        // 유닛의 타입으로 해제
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

        // 타일의 타입으로 생성
        public PlayerTile CreateTile(BlockID blockID)
        {
            switch (blockID)
            {
                case BlockID.PlayerTile1x1:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTile1x1>("Prefabs/PlayerTile1x1");
                case BlockID.PlayerTile1x4:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTile1x4>("Prefabs/PlayerTile1x4");
                case BlockID.PlayerTile4x1:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTile4x1>("Prefabs/PlayerTile4x1");
                case BlockID.PlayerTile2x2:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTile2x2>("Prefabs/PlayerTile2x2");
                case BlockID.PlayerTile3x3:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTile3x3>("Prefabs/PlayerTile3x3");
                case BlockID.PlayerTilePlus:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTilePlus>("Prefabs/PlayerTilePlus");
                case BlockID.PlayerTileSickle01:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTileSickle01>("Prefabs/PlayerTileSickle01");
                case BlockID.PlayerTileSickle02:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTileSickle02>("Prefabs/PlayerTileSickle02");
                case BlockID.PlayerTileVane01:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTileVane01>("Prefabs/PlayerTileVane01");
                case BlockID.PlayerTileVane02:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<PlayerTileVane02>("Prefabs/PlayerTileVane02");
                default:
                    return null;
            }
        }

        // 타일의 타입으로 해제
        public bool ReleaseTile(BlockID blockID, PlayerTile tile)
        {
            switch (blockID)
            {
                case BlockID.PlayerTile1x1:
                    PoolManager.Instance.Release<PlayerTile1x1>("Prefabs/PlayerTile1x1", (PlayerTile1x1)tile);
                    return true;
                case BlockID.PlayerTile1x4:
                    PoolManager.Instance.Release<PlayerTile1x4>("Prefabs/PlayerTile1x4", (PlayerTile1x4)tile);
                    return true;
                case BlockID.PlayerTile4x1:
                    PoolManager.Instance.Release<PlayerTile4x1>("Prefabs/PlayerTile4x1", (PlayerTile4x1)tile);
                    return true;
                case BlockID.PlayerTile2x2:
                    PoolManager.Instance.Release<PlayerTile2x2>("Prefabs/PlayerTile2x2", (PlayerTile2x2)tile);
                    return true;
                case BlockID.PlayerTile3x3:
                    PoolManager.Instance.Release<PlayerTile3x3>("Prefabs/PlayerTile3x3", (PlayerTile3x3)tile);
                    return true;
                case BlockID.PlayerTilePlus:
                    PoolManager.Instance.Release<PlayerTilePlus>("Prefabs/PlayerTilePlus", (PlayerTilePlus)tile);
                    return true;
                case BlockID.PlayerTileSickle01:
                    PoolManager.Instance.Release<PlayerTileSickle01>("Prefabs/PlayerTileSickle01", (PlayerTileSickle01)tile);
                    return true;
                case BlockID.PlayerTileSickle02:
                    PoolManager.Instance.Release<PlayerTileSickle02>("Prefabs/PlayerTileSickle02", (PlayerTileSickle02)tile);
                    return true;
                case BlockID.PlayerTileVane01:
                    PoolManager.Instance.Release<PlayerTileVane01>("Prefabs/PlayerTileVane01", (PlayerTileVane01)tile);
                    return true;
                case BlockID.PlayerTileVane02:
                    PoolManager.Instance.Release<PlayerTileVane02>("Prefabs/PlayerTileVane02", (PlayerTileVane02)tile);
                    return true;
                default:
                    return false;
            }
        }
    }
}