using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileGroup
{
    public string BasicTilePath = "Prefabs/Block";
    public TileSet GrassTileSet = new TileSet();
    public TileSet IceTileSet = new TileSet();
    public TileSet DesertTileSet = new TileSet();
    public List<DecoTileData> UserTileSet = new List<DecoTileData>();
}

[System.Serializable]
public class TileSet
{
    public List<BasicTileData> BasicTileData = new List<BasicTileData>();
    public List<DecoTileData> DecoTileData = new List<DecoTileData>();

    public BasicTileData WaterData = new BasicTileData();
    public DecoTileData BridgeData = new DecoTileData();
}

[System.Serializable]
public struct BasicTileData
{
    public string MaterialTop;
    public string MaterialSide;
}

[System.Serializable]
public struct DecoTileData
{
    public string PrefabPath;
}