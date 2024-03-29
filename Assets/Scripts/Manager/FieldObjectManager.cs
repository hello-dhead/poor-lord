﻿using System.Collections;
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
        // 전사2
        Fuyuko,
        // 전사3
        Kaho,
        // 궁수1
        Shiori,
        // 궁수2
        Ranger,
        // 궁수3
        Serika,
        // 법사1
        Yuni,
        // 법사2
        Nian,
        // 법사3
        Asahi,
        // 플레이어 유닛
        PlayerUnitMax,
        // 슬라임
        Slime,
        // 박쥐
        Bat,
        // 버섯
        Mushroom,
        // 해골
        Skeleton,
        // 고릴라
        Kong
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
                case UnitID.Fuyuko:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Warrior_Fuyuko>("Prefabs/Warrior_Fuyuko");
                case UnitID.Kaho:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Warrior_Kaho>("Prefabs/Warrior_Kaho");
                case UnitID.Shiori:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Archer_Shiori>("Prefabs/Archer_Shiori");
                case UnitID.Ranger:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Archer_Ranger>("Prefabs/Archer_Ranger");
                case UnitID.Serika:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Archer_Serika>("Prefabs/Archer_Serika");
                case UnitID.Yuni:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Caster_Yuni>("Prefabs/Caster_Yuni");
                case UnitID.Nian:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Caster_Nian>("Prefabs/Caster_Nian");
                case UnitID.Asahi:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Caster_Asahi>("Prefabs/Caster_Asahi");
                case UnitID.Slime:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Slime>("Prefabs/Monster_Slime");
                case UnitID.Bat:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Bat>("Prefabs/Monster_Bat");
                case UnitID.Mushroom:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Mushroom>("Prefabs/Monster_Mushroom");
                case UnitID.Skeleton:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Skeleton>("Prefabs/Monster_Skeleton");
                case UnitID.Kong:
                    return PoolManager.Instance.GetOrCreateObjectPoolFromPath<Kong>("Prefabs/Monster_Kong");
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
                case UnitID.Fuyuko:
                    PoolManager.Instance.Release<Warrior_Fuyuko>("Prefabs/Warrior_Fuyuko", (Warrior_Fuyuko)unit);
                    return true;
                case UnitID.Kaho:
                    PoolManager.Instance.Release<Warrior_Kaho>("Prefabs/Warrior_Kaho", (Warrior_Kaho)unit);
                    return true;
                case UnitID.Shiori:
                    PoolManager.Instance.Release<Archer_Shiori>("Prefabs/Archer_Shiori", (Archer_Shiori)unit);
                    return true;
                case UnitID.Ranger:
                    PoolManager.Instance.Release<Archer_Ranger>("Prefabs/Archer_Ranger", (Archer_Ranger)unit);
                    return true;
                case UnitID.Serika:
                    PoolManager.Instance.Release<Archer_Serika>("Prefabs/Archer_Serika", (Archer_Serika)unit);
                    return true;
                case UnitID.Yuni:
                    PoolManager.Instance.Release<Caster_Yuni>("Prefabs/Caster_Yuni", (Caster_Yuni)unit);
                    return true;
                case UnitID.Nian:
                    PoolManager.Instance.Release<Caster_Nian>("Prefabs/Caster_Nian", (Caster_Nian)unit);
                    return true;
                case UnitID.Asahi:
                    PoolManager.Instance.Release<Caster_Asahi>("Prefabs/Caster_Asahi", (Caster_Asahi)unit);
                    return true;
                case UnitID.Slime:
                    PoolManager.Instance.Release<Slime>("Prefabs/Monster_Slime", (Slime)unit);
                    return true;
                case UnitID.Bat:
                    PoolManager.Instance.Release<Bat>("Prefabs/Monster_Bat", (Bat)unit);
                    return true;
                case UnitID.Mushroom:
                    PoolManager.Instance.Release<Mushroom>("Prefabs/Monster_Mushroom", (Mushroom)unit);
                    return true;
                case UnitID.Skeleton:
                    PoolManager.Instance.Release<Skeleton>("Prefabs/Monster_Skeleton", (Skeleton)unit);
                    return true;
                case UnitID.Kong:
                    PoolManager.Instance.Release<Kong>("Prefabs/Monster_Kong", (Kong)unit);
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