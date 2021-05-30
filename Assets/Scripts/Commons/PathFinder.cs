using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using poorlord;

namespace Assets.Scripts.Commons
{
    public class PathNode
    {
        public Vector3Int NodePos { get; private set; }
        public int CostG { get; private set; }
        public int CostH { get; private set; }
        public int CostF { get; private set; }
        public PathNode ParentNode { get; private set; }
        public PathNode(Vector3Int nodePos, int costG, Vector3Int destPos, PathNode parentNode)
        {
            nodePos.y = 0;
            NodePos = nodePos;

            CostG = costG;
            //현재지점에서 끝지점까지 거리의 절대 값 = 휴리스틱 Cost
            CostH = Mathf.Abs(destPos.x - NodePos.x) + Mathf.Abs(destPos.z - NodePos.z);
            //총 비용
            CostF = CostG + CostH;
            ParentNode = parentNode;
        }
    }

    public class PathFinder
    {
        private enum PathDirection
        {
            Up = 0,
            Right,
            Down,
            Left,
            DirectionMax
        }
    
        private List<PathNode> openedList = new List<PathNode>();
        private List<PathNode> closedList = new List<PathNode>();

        private List<List<BasicTile>> tileList;
        private int maxDepth = 400;
        private int stackDepth = 0;

        //인접 타일 체크 
        private bool CheckChildNode(Vector3Int curPos, PathDirection direction, out Vector3Int childNodePos)
        {
            switch (direction)
            {
                case PathDirection.Up:
                    curPos.z += 1;
                    break;
                case PathDirection.Down:
                    curPos.z -= 1;
                    break;
                case PathDirection.Left:
                    curPos.x -= 1;
                    break;
                case PathDirection.Right:
                    curPos.x += 1;
                    break;
            }
            childNodePos = curPos;

            if (curPos.x < 0 || curPos.x >= tileList.Count || curPos.z < 0 || curPos.z >= tileList[0].Count)
                return false;
            return tileList[curPos.x][curPos.z].CheckRoadTile(); // 검사한 노드가 갈수 있는 노드인지 체크해서 리턴
        }

        private bool InsertOpenList(PathNode node)
        {
            //열린 목록과 닫힌 목록에 동일한 노드가 있는지 탐색
            for (int i = 0; i < closedList.Count; i++)
            {
                if (node.NodePos == closedList[i].NodePos)
                {
                    return false;
                }
            }

            for (int i = 0; i < openedList.Count; i++)
            {
                if (node.NodePos == openedList[i].NodePos)
                {
                    return false;
                }
            }
            openedList.Add(node);
            return true;
        }

        //destPos부터 startPos까지의 경로가 담긴 PathNode 리턴 부모를 따라가면 startPos에 도착
        private PathNode FindPath(PathNode parentNode, Vector3Int destPos)
        {
            //재귀가 너무 깊이 들어가면 에러 또는 너무 먼거리이므로 탈출
            if (stackDepth > maxDepth)
                return null;

            stackDepth++;

            //부모 노드가 destPos면 길 찾았으므로 재귀 탈출
            if (parentNode.NodePos == destPos)
                return parentNode;

            //인접 노드 체크
            for (int dir = (int)PathDirection.Up; dir < (int)PathDirection.DirectionMax; dir++)
            {
                // 지나갈 수 있는지 체크
                Vector3Int childNodePos;
                if (CheckChildNode(parentNode.NodePos, (PathDirection)dir, out childNodePos))
                {
                    PathNode childNode = new PathNode(childNodePos, parentNode.CostG + 1, destPos, parentNode);
                    InsertOpenList(childNode);
                }
            }

            //열린 목록안에 노드들 중 가장 총 비용(F)이 작은 노드를 찾는다.
            if (openedList.Count > 0)
            {
                PathNode leastCostNode = openedList[0];
                int leastCostNodeIndex = 0;

                // 거리가 같은 경우 한쪽 길로만 가는 현상이 생겨서 추가
                int randomSelect = Random.Range(0, 2);
                for (int i = 0; i < openedList.Count; i++)
                {
                    if (randomSelect == 0)
                    {
                        if (leastCostNode.CostF >= openedList[i].CostF)
                        {
                            leastCostNode = openedList[i];
                            leastCostNodeIndex = i;
                        }
                    }
                    else
                    {
                        if (leastCostNode.CostF > openedList[i].CostF)
                        {
                            leastCostNode = openedList[i];
                            leastCostNodeIndex = i;
                        }
                    }
                }
                //열린 목록에서 가장 작은노드를 찾았으니 열린목록에서 제외하고 닫힌목록에 추가
                //Debug.Log(leastCostNode.NodePos.x + " , " + leastCostNode.NodePos.z + "노드 방문 / CostF = " + leastCostNode.CostF);
                openedList.RemoveAt(leastCostNodeIndex);
                closedList.Add(leastCostNode);

                //그 노드 기반으로 재귀
                return FindPath(leastCostNode, destPos);
            }
            else
            {
                Debug.Log("갈 수 없는 길입니다");
                return null;
            }
        }

        //시작점부터 끝지점까지의 Vector3Int 리스트를 리턴
        //리스트가 비어있으면 갈 수 없거나 현재 위치이므로 리스트가 비어있지 않은지 체크 필요
        public List<Vector3Int> GetPath(Vector3Int startPos, Vector3Int destPos, List<List<BasicTile>> tileList)
        {
            this.tileList = tileList;
            List<Vector3Int> foundPath = new List<Vector3Int>();

            openedList.Clear();
            closedList.Clear();

            PathNode startNode = new PathNode(startPos, 0, destPos, null);

            stackDepth = 0;
            closedList.Add(startNode);
            PathNode foundPathNode = FindPath(startNode, destPos);

            if (foundPathNode == null) // 길을 못찾음
            {
                return foundPath;
            }
            else
            {
                while (foundPathNode != null)
                {
                    foundPath.Insert(0, foundPathNode.NodePos);
                    foundPathNode = foundPathNode.ParentNode;
                }
            }
            return foundPath;
        }
    }
}