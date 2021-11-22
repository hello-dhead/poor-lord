using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Commons;
using System.IO;

namespace poorlord
{
    /// <summary>
    ///  생성할 타일맵 테마
    /// </summary>
    public enum TileTheme
    {
        /// 초원 테마
        Grass,
        /// 얼음 테마
        Ice,
        /// 사막 테마
        Desert
    }

    public enum TileMapSize
    {
        /// 작음
        Small = 10,
        /// 중간
        Middle = 15,
        /// 큼
        Large = 20
    }

    /// <summary>
    ///  TileManger의 역할 : 타일 관리, 랜덤 타일 생성 및 삭제 / 타일의 키값은 주소값
    /// </summary>
    public class TileManager : Singleton<TileManager>
    {
        // 타일의 정보가 들어가 있는 데이터셋
        private TileGroup tileGroupData = JsonUtility.FromJson<TileGroup>(File.ReadAllText("Assets/Json/TileGroupJson.json"));

        // 현재 타일셋과 매테리얼 캐싱
        private TileSet currentTileSet;
        private List<List<Material>> currentTileMaterial;
        private List<Material> currentWaterMaterial;

        // 최대 배치 타일 개수
        private readonly int MAX_TILE_NUMBER = 120;

        // 각 X좌표 끝에서부터 성의 랜덤 배치 가능 범위
        private readonly int CASTLE_MAX_TILE_RANGE = 2;

        // 바닥 타일맵
        private List<List<BasicTile>> tileList;

        // 장애물, 다리와 같은 타일맵 리스트에 포함되지 않는 장식용 타일
        private List<DecoTileListData> decoTileList;

        PathFinder pathFinder = new PathFinder();

        public Vector3Int monsterCastlePos { get; private set; }
        public Vector3Int playerCastlePos { get; private set; }

        // 새 타일맵 생성
        public void CreateTileMap(TileTheme theme, int horizontal, int vertical)
        {
            tileList = new List<List<BasicTile>>();
            decoTileList = new List<DecoTileListData>();

            if (horizontal * vertical > MAX_TILE_NUMBER)
            {
                Debug.Log("타일 최대값 초과!");
                return;
            }

            PoolManager.Instance.CreateObjectPoolFromPath<BasicTile>(tileGroupData.BasicTilePath, tileGroupData.BasicTilePath, 60);

            switch (theme)
            {
                case TileTheme.Grass:
                    currentTileSet = tileGroupData.GrassTileSet;
                    break;
                case TileTheme.Ice:
                    currentTileSet = tileGroupData.IceTileSet;
                    break;
                case TileTheme.Desert:
                    currentTileSet = tileGroupData.DesertTileSet;
                    break;
            }
            SetCurrentMaterialOnPath(currentTileSet);

            // 테마에 맞는 랜덤 바닥 타일맵 세팅
            for (int i = 0; i < horizontal; i++)
            {
                tileList.Add(new List<BasicTile>());
                for (int j = 0; j < vertical; j++)
                {
                    int rand = Random.Range(0, currentTileMaterial.Count);
                    BasicTile tile = PoolManager.Instance.Create<BasicTile>(tileGroupData.BasicTilePath);
                    tile.Init(TileState.None, new Vector3Int(i, 0, j), currentTileMaterial[rand][0], currentTileMaterial[rand][1]);
                    tileList[i].Add(tile);
                }
            }

            // 몬스터 성 생성
            CreateCastle(vertical, horizontal);

            // 무조건 성으로 가는 길은 하나 존재해야 하므로 None State를 잠시 바꿔둔다.
            List<Vector3Int> roadTilePosList = pathFinder.GetPath(monsterCastlePos, playerCastlePos, tileList);

            for (int i = 1; i < roadTilePosList.Count - 1; i++)
                tileList[roadTilePosList[i].x][roadTilePosList[i].z].SetState(TileState.Obstacle);

            int riverCreateGen;
            // 테마 별로 강 생성 확률이 다르다
            if (theme == TileTheme.Grass)
            {
                riverCreateGen = 100;
            }
            else if (theme == TileTheme.Ice)
            {
                riverCreateGen = 50;
            }
            else
            {
                riverCreateGen = 30;
            }

            if (riverCreateGen > Random.Range(1, 101))
                CreateRiver(horizontal, vertical);

            CreateObstacle(Random.Range(2, horizontal), horizontal, vertical);

            for (int i = 1; i < roadTilePosList.Count - 1; i++)
            {
                if (tileList[roadTilePosList[i].x][roadTilePosList[i].z].GetState() == TileState.Obstacle)
                    tileList[roadTilePosList[i].x][roadTilePosList[i].z].SetState(TileState.None);
            }
        }
        
        // 성 생성
        private void CreateCastle(int vertical, int horizontal)
        {
            monsterCastlePos = new Vector3Int(Random.Range(0, CASTLE_MAX_TILE_RANGE), 0, Random.Range(0, vertical));
            tileList[monsterCastlePos.x][monsterCastlePos.z].SetState(TileState.Castle);

            PoolManager.Instance.CreateObjectPoolFromPath<DecorationTile>("MonsterCastle", "Prefabs/Monster_Castle", 1);
            DecorationTile monsterCastle = PoolManager.Instance.Create<DecorationTile>("MonsterCastle");
            monsterCastle.init(new Vector3Int(monsterCastlePos.x, 0, monsterCastlePos.z));
            decoTileList.Add(new DecoTileListData("MonsterCastle", monsterCastle));

            playerCastlePos = new Vector3Int(Random.Range(horizontal - CASTLE_MAX_TILE_RANGE, horizontal), 0, Random.Range(0, vertical));
            tileList[playerCastlePos.x][playerCastlePos.z].SetState(TileState.Castle);

            PoolManager.Instance.CreateObjectPoolFromPath<DecorationTile>("PlayerCastle", "Prefabs/Player_Castle", 1);
            DecorationTile playerCastle = PoolManager.Instance.Create<DecorationTile>("PlayerCastle");
            playerCastle.init(new Vector3Int(playerCastlePos.x, 0, playerCastlePos.z));
            decoTileList.Add(new DecoTileListData("PlayerCastle", playerCastle));
        }

        // 현재 기본 타일, 물 매테리얼을 교체한다.
        private void SetCurrentMaterialOnPath(TileSet tileSet)
        {
            this.currentTileMaterial = new List<List<Material>>();
            this.currentWaterMaterial = new List<Material>();

            for (int i = 0; i < tileSet.BasicTileData.Count; i++)
            {
                currentTileMaterial.Add(new List<Material>());
                currentTileMaterial[i].Add(Resources.Load<Material>(tileSet.BasicTileData[i].MaterialTop));
                currentTileMaterial[i].Add(Resources.Load<Material>(tileSet.BasicTileData[i].MaterialSide));
            }
            currentWaterMaterial.Add(Resources.Load<Material>(tileSet.WaterData.MaterialTop));
            currentWaterMaterial.Add(Resources.Load<Material>(tileSet.WaterData.MaterialSide));
        }

        // 강 생성
        private void CreateRiver(int horizontal, int vertical)
        {
            // 강의 굵기 = 가로 10부터는 1줄 이후 5줄이 늘어날 때마다 굵어짐
            int riverArea = (horizontal / 5) - 1;
            int currentRiverX = Random.Range(CASTLE_MAX_TILE_RANGE, horizontal - CASTLE_MAX_TILE_RANGE - riverArea + 1);
            int currentRiverZ = vertical - 1;

            while (currentRiverZ > -1)
            {
                for (int i = 0; i < riverArea; i++)
                    CreateWaterAndBridge(currentRiverX + i, currentRiverZ);


                // 랜덤하게 중앙 유지, 왼쪽 or 오른쪽으로 물줄기 이동
                int randDirection = Random.Range(0, 100);
                if (randDirection < 20 && currentRiverX - 1 >= CASTLE_MAX_TILE_RANGE)
                {
                    currentRiverX--;
                    CreateWaterAndBridge(currentRiverX, currentRiverZ);
                }
                else if (randDirection < 40 && currentRiverX + riverArea < horizontal - CASTLE_MAX_TILE_RANGE)
                {
                    CreateWaterAndBridge(currentRiverX + riverArea, currentRiverZ);
                    currentRiverX++;
                }
                currentRiverZ--;
            }
        }

        // 물 and 다리 생성
        private void CreateWaterAndBridge(int x, int z)
        {
            if (tileList[x][z].GetState() == TileState.Obstacle)
            {
                PoolManager.Instance.CreateObjectPoolFromPath<DecorationTile>("Bridge", currentTileSet.BridgeData.PrefabPath);
                DecorationTile bridge = PoolManager.Instance.Create<DecorationTile>("Bridge");
                bridge.init(new Vector3Int(x, 0, z));
                decoTileList.Add(new DecoTileListData("Bridge", bridge));
                tileList[x][z].Init(TileState.Bridge, new Vector3(x, (float)-0.5, z), currentWaterMaterial[0], currentWaterMaterial[1], false);
            }
            else
            {
                tileList[x][z].Init(TileState.Water, new Vector3(x, (float)-0.5, z), currentWaterMaterial[0], currentWaterMaterial[1], false);
            }
        }

        // 장애물 생성
        private void CreateObstacle(int count, int horizontal, int vertical)
        {
            List<DecoTileData> decoTileData = currentTileSet.DecoTileData;
            for (int i = 0; i < count; i++)
            {
                int randX = Random.Range(0, horizontal);
                int randZ = Random.Range(0, vertical);
                string randDecoTile = decoTileData[Random.Range(0, decoTileData.Count)].PrefabPath;

                PoolManager.Instance.CreateObjectPoolFromPath<DecorationTile>(randDecoTile, randDecoTile);
                DecorationTile decoTile = PoolManager.Instance.Create<DecorationTile>(randDecoTile);

                if (CheckDecoTileCollision(decoTile, randX, randZ))
                {
                    decoTile.init(new Vector3Int(randX, 1, randZ));
                    decoTileList.Add(new DecoTileListData(randDecoTile, decoTile));
                    for (int j = 0; j < decoTile.gameObject.transform.childCount; j++)
                    {
                        Vector3 pos = decoTile.gameObject.transform.GetChild(j).transform.localPosition;
                        tileList[(int)pos.x + randX][(int)pos.z + randZ].SetState(TileState.Obstacle);
                    }
                }
                else
                {
                    PoolManager.Instance.Release<DecorationTile>(randDecoTile, decoTile);
                }
            }
        }

        // 데코 타일의 충돌 체크 / 게임오브젝트의 자식 = 각 타일 포지션 타일의 상태를 비교
        private bool CheckDecoTileCollision(DecorationTile decoTile, int x, int z)
        {
            Transform tileTransform = decoTile.gameObject.transform;
            for (int i = 0; i < tileTransform.childCount; i++)
            {
                int checkX = (int)tileTransform.GetChild(i).transform.localPosition.x + x;
                int checkZ = (int)tileTransform.GetChild(i).transform.localPosition.z + z;
                if (tileList.Count <= checkX || tileList[0].Count <= checkZ || tileList[(int)checkX][(int)checkZ].CheckBuildable() == false)
                {
                    return false;
                }
            }
            return true;
        }

        // 매개변수로 들어온 타일리스트가 모두 건축이 가능한 타일인지 체크
        public bool CheckBuildableTileList(List<Vector3Int> checkTileList)
        {
            for (int i = 0; i < checkTileList.Count; i++)
            {
                if (tileList.Count <= checkTileList[i].x || tileList[0].Count <= checkTileList[i].z || tileList[checkTileList[i].x][checkTileList[i].z].CheckBuildable() == false)
                {
                    return false;
                }
            }
            return true;
        }

        // 타일맵 해제
        public void ReleaseTileMap()
        {
            for (int i = 0; i < tileList.Count; i++)
            {
                for (int j = 0; j < tileList[i].Count; j++)
                {
                    tileList[i][j].Dispose();
                    PoolManager.Instance.Release<BasicTile>(tileGroupData.BasicTilePath, tileList[i][j]);
                }
            }

            for (int i = 0; i < decoTileList.Count; i++)
            {
                PoolManager.Instance.Release<DecorationTile>(decoTileList[i].key, decoTileList[i].decoTile);
            }

            tileList.Clear();
            decoTileList.Clear();
        }

        // 타일에 건물 지을 수 있는지 체크
        public bool CheckBuildable(int x, int z)
        {
            return tileList[x][z].CheckBuildable();
        }

        // 타일에 유닛을 놓을 수 있는지 체크
        public bool CheckBuildableUnit(int x, int z)
        {
            return tileList[x][z].CheckBuildableUnit();
        }

        // 타일에 존재하는 플레이어 유닛 받아오기
        public PlayerUnit GetContainPlayerUnit(int x, int z)
        {
            if (tileList.Count <= x || tileList[0].Count <= z || x < 0 || z < 0)
                return null;
            return tileList[x][z].GetContainPlayerUnit();
        }

        // 타일에 존재하는 몬스터 리스트 받아오기
        public List<MonsterUnit> GetContainMonsterUnitList(int x, int z)
        {
            if (tileList.Count <= x || tileList[0].Count <= z || x < 0 || z < 0)
                return null;
            return tileList[x][z].GetContainMonsterUnitList();
        }

        // 포지션에서 플레이어 성까지의 경로 리턴 못찾으면 비어있는 리스트 리턴
        public List<Vector3Int> GetPathFromPos(Vector3Int pos)
        {
            return pathFinder.GetPath(pos, playerCastlePos, tileList);
        }

        // 몬스터 성에서 플레이어 성까지의 경로 리턴 못찾으면 비어있는 리스트 리턴
        public List<Vector3Int> GetPathFromMonsterCastle()
        {
            return GetPathFromPos(monsterCastlePos);
        }

        public Vector3Int GetMonsterCastlePos()
        {
            return monsterCastlePos;
        }

        // 블락이 추가된다면 지나갈 수 있는 길이 있는지 체크
        public bool CheckOverlapPath(Vector3Int currentPosition, List<Vector3Int> newBlockPath)
        {
            // 맵을 벗어나는 타일이 있으면 무조건 false
            for (int i = 0; i < newBlockPath.Count; i++)
            {
                if (newBlockPath[i].x < 0 || newBlockPath[i].x >= tileList.Count || newBlockPath[i].z < 0 || newBlockPath[i].z >= tileList[0].Count)
                    return false;
            }

            // 현재 state 잠시 저장해놓음
            List<TileState> state = new List<TileState>();
            for (int i = 0; i < newBlockPath.Count; i++)
            {
                state.Add(tileList[newBlockPath[i].x][newBlockPath[i].z].GetState());
            }

            for (int i = 0; i < newBlockPath.Count; i++)
            {
                tileList[newBlockPath[i].x][newBlockPath[i].z].SetState(TileState.PlayerTile);
            }

            List<Vector3Int> roadTilePosList = pathFinder.GetPath(currentPosition, playerCastlePos, tileList);

            for (int i = 0; i < newBlockPath.Count; i++)
            {
                tileList[newBlockPath[i].x][newBlockPath[i].z].SetState(state[i]);
            }
            
            if (roadTilePosList.Count != 0)
                return true;
            return false;
        }

        // 타일의 상태 변경
        public void ChangeState(Vector3Int pos, TileState state)
        {
            tileList[pos.x][pos.z].SetState(state);
        }

        // 타일 리스트 길이 리턴
        public int GetTileListCount()
        {
            return tileList.Count;
        }
    }

    public class DecoTileListData
    {
        public string key;
        public DecorationTile decoTile;

        public DecoTileListData(string key, DecorationTile decoTile)
        {
            this.key = key;
            this.decoTile = decoTile;
        }
    }
}