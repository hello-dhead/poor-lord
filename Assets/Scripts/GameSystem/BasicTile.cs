using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 타일의 상태
    /// </summary>
    public enum TileState
    {
        // 아무것도 없는 상태 = 길
        None,
        // 다리, 타일을 세울수는 없지만 지나갈수는 상태
        Bridge,
        // 몬스터 성, 주인공 성
        Castle,
        // 플레이어가 세운 벽이 있는 상태
        PlayerTile,
        // 장애물이 있는 상태 = 나무 or 벽
        Obstacle,
        // 물 블럭 상태
        Water
    }

    /// <summary>
    /// 바닥에 깔리는 기본 타일 위에 올라와있는 캐릭터의 정보를 가지고 있다.
    /// </summary>
    public class BasicTile : MonoBehaviour, IEventListener
    {
        [SerializeField]
        private TileState state = TileState.None;

        private Transform tileTransform;

        // 해당 타일에 속해있는 몬스터 유닛의 정보 
        private List<MonsterUnit> containMonsterUnitList = new List<MonsterUnit>();

        [SerializeField]
        // 해당 타일에 속해있는 플레이어 유닛의 정보
        private PlayerUnit containPlayerUnit;

        public void Init(TileState state, Vector3 position, Material materialTop, Material materialSide, bool isSubscribe = true)
        {
            if (tileTransform == null)
            {
                tileTransform = this.gameObject.transform;
            }
            this.state = state;
            tileTransform.position = position;
            containMonsterUnitList.Clear();
            containPlayerUnit = null;

            // 스프라이트 교체
            tileTransform.GetChild(0).GetComponent<MeshRenderer>().material = materialTop;
            tileTransform.GetChild(1).GetComponent<MeshRenderer>().material = materialSide;
            tileTransform.GetChild(2).GetComponent<MeshRenderer>().material = materialSide;
            tileTransform.GetChild(3).GetComponent<MeshRenderer>().material = materialSide;

            if(isSubscribe)
            {
                GameManager.Instance.MessageSystem.Subscribe(typeof(TileEnterEvent), this);
                GameManager.Instance.MessageSystem.Subscribe(typeof(TileLeaveEvent), this);
                GameManager.Instance.MessageSystem.Subscribe(typeof(MonsterDeadEvent), this);
                GameManager.Instance.MessageSystem.Subscribe(typeof(PlayerUnitSummonEvent), this);
                GameManager.Instance.MessageSystem.Subscribe(typeof(PlayerUnitDeadEvent), this);
            }
        }

        public void Dispose()
        {
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(TileEnterEvent), this);
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(TileLeaveEvent), this);
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(MonsterDeadEvent), this);
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(PlayerUnitSummonEvent), this);
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(PlayerUnitDeadEvent), this);
        }

        public bool OnEvent(IEvent e)
        {
            Type eventType = e.GetType();
            // 자기 타일에 몬스터가 들어오거나 나갈 때 리스트에서 제거
            if (eventType == typeof(TileEnterEvent))
            {
                TileEnterEvent enterEvent = e as TileEnterEvent;
                if (enterEvent.EnterTilePos.x == tileTransform.position.x && enterEvent.EnterTilePos.z == tileTransform.position.z)
                {
                    containMonsterUnitList.Add((MonsterUnit)enterEvent.EnterUnit);

                    //Debug.Log(tileTransform.position + " 몬스터 IN " + containMonsterUnitList.Count);
                }
                return true;
            }
            else if (eventType == typeof(TileLeaveEvent))
            {
                TileLeaveEvent leaveEvent = e as TileLeaveEvent;
                if (leaveEvent.LeaveTilePos.x == tileTransform.position.x && leaveEvent.LeaveTilePos.z == tileTransform.position.z)
                {
                    containMonsterUnitList.Remove((MonsterUnit)leaveEvent.LeaveUnit);

                    //Debug.Log(tileTransform.position + " 몬스터 OUT " + containMonsterUnitList.Count);
                }
                return true;
            }
            else if (eventType == typeof(MonsterDeadEvent))
            {
                MonsterDeadEvent deadEvent = e as MonsterDeadEvent;
                if (deadEvent.DeadPos.x == tileTransform.position.x && deadEvent.DeadPos.z == tileTransform.position.z)
                {
                    containMonsterUnitList.Remove(deadEvent.DeadUnit);
                    //Debug.Log(tileTransform.position + " 몬스터 Dead " + containMonsterUnitList.Count);
                }            
                return true;
            }
            else if (eventType == typeof(PlayerUnitSummonEvent))
            {
                PlayerUnitSummonEvent summonEvent = e as PlayerUnitSummonEvent;
                if (summonEvent.SummonTilePos.x == tileTransform.position.x && summonEvent.SummonTilePos.z == tileTransform.position.z)
                {
                    containPlayerUnit = summonEvent.SummonUnit;
                    //Debug.Log(tileTransform.position + " 유닛 Summon " + containPlayerUnit);
                }
                return true;
            }
            else if (eventType == typeof(PlayerUnitDeadEvent))
            {
                PlayerUnitDeadEvent deadEvent = e as PlayerUnitDeadEvent;
                if (deadEvent.DeadPos.x == tileTransform.position.x && deadEvent.DeadPos.z == tileTransform.position.z)
                {
                    containPlayerUnit = null;
                    //Debug.Log(tileTransform.position + " 유닛 Dead " + containPlayerUnit);
                }
                return true;
            }
            return false;
        }

        public void SetState(TileState state)
        {
            this.state = state;
        }

        public TileState GetState()
        {
            return state;
        }

        public bool CheckBuildable()
        {
            if ((state == TileState.None) && containPlayerUnit == null && containMonsterUnitList.Count == 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckBuildableUnit()
        {
            if ((state == TileState.None || state == TileState.Bridge) && containPlayerUnit == null && containMonsterUnitList.Count == 0)
            {
                return true;
            }
            return false;
        }

        public bool CheckRoadTile()
        {
            if (state == TileState.None || state == TileState.Castle || state == TileState.Bridge)
            {
                return true;
            }
            return false;
        }

        public PlayerUnit GetContainPlayerUnit()
        {
            return containPlayerUnit;
        }

        public List<MonsterUnit> GetContainMonsterUnitList()
        {
            return containMonsterUnitList;
        }
    }
}