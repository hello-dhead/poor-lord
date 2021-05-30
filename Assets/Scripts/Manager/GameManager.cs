using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.Commons;

namespace poorlord
{
    public class GameManager : MonoSingleton<GameManager>
    {
        // 매 프레임 업데이트되야하는 해시셋 아직까지는 우선순위가 필요없지만 추후 필요하게 되면 리스트로 교체
        private HashSet<IUpdatable> updateHashSet;

        // 다음 프레임이 시작될 때 추가 될 업데이트
        private List<IUpdatable> addUpdateList;

        // 다음 프레임이 시작될 때 삭제 될 업데이트
        private List<IUpdatable> removeUpdateList;

        // 이벤트 관리,처리하는 메세지 시스템
        public MessageSystem MessageSystem;

        // 배틀 관련 모든 처리를 하는 시스템
        public BattleSystem BattleSystem;

        // 카드 관련 모든 처리를 하는 시스템
        public CardSystem CardSystem;

        private int stage = 0;

        public void AddUpdate(IUpdatable updatable)
        {
            addUpdateList.Add(updatable);
        }

        public void RemoveUpdate(IUpdatable updatable)
        {
            removeUpdateList.Add(updatable);
        }

        private void Start()
        {
            Screen.SetResolution(2560, 1440, true);
            
            updateHashSet = new HashSet<IUpdatable>();
            addUpdateList = new List<IUpdatable>();
            removeUpdateList = new List<IUpdatable>();

            MessageSystem = new MessageSystem();
            BattleSystem = new BattleSystem();
            CardSystem = new CardSystem();

            // JsonUtility.FromJsonOverwrite(savedData, this);
            //TileManager.Instance.CreateTileMap((TileTheme)Random.Range(0, 3), Random.Range(10, 15), Random.Range(4, 6));
            TileManager.Instance.CreateTileMap((TileTheme)0, 10, 4);
            //TileManager.Instance.ReleaseTileMap();
            StartBattleStage();

            Warrior_Alice alice = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Warrior_Alice>("Prefabs/Warrior_Alice", "Prefabs/Warrior_Alice");
            alice.Init(new Vector3Int(3, 0, 0), new List<ImmediatelyBuff>(), new List<ContinuousBuff>(), "Prefabs/Warrior_Alice");

            PlayerUnit alice2 = alice.GetPrefabs();
            alice2.Init(new Vector3Int(3, 0, 1), new List<ImmediatelyBuff>(), new List<ContinuousBuff>(), "Prefabs/Warrior_Alice");
            //Warrior_Alice alice2 = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Warrior_Alice>("Prefabs/Warrior_Alice", "Prefabs/Warrior_Alice");
            //alice2.Init(new Vector3Int(3, 0, 1), new List<ImmediatelyBuff>(), new List<ContinuousBuff>(), "Prefabs/Warrior_Alice");
            Warrior_Alice alice3 = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Warrior_Alice>("Prefabs/Warrior_Alice", "Prefabs/Warrior_Alice");
            alice3.Init(new Vector3Int(3, 0, 2), new List<ImmediatelyBuff>(), new List<ContinuousBuff>(), "Prefabs/Warrior_Alice");
            Warrior_Alice alice4 = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Warrior_Alice>("Prefabs/Warrior_Alice", "Prefabs/Warrior_Alice");
            alice4.Init(new Vector3Int(3, 0, 3), new List<ImmediatelyBuff>(), new List<ContinuousBuff>(), "Prefabs/Warrior_Alice");
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            foreach (var updatable in addUpdateList)
            {
                if (updateHashSet.Contains(updatable))
                    continue;
                updateHashSet.Add(updatable);
            }
            addUpdateList.Clear();

            foreach (var updatable in removeUpdateList)
            {
                updateHashSet.Remove(updatable);
            }
            removeUpdateList.Clear();

            // 메세지 시스템은 무조건 다른 업데이트돌기 전에 발행 요청한 이벤트들 우선 처리
            MessageSystem.UpdateFrame(dt);

            foreach (var updatable in updateHashSet)
            {
                updatable.UpdateFrame(dt);
            }
        }

        private void StartBattleStage()
        {
            GameManager.Instance.MessageSystem.Publish(BattleStageStartEvent.Create(stage));
            stage++;
        }
    }
}