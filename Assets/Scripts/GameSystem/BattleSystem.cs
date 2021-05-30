using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace poorlord
{
    /// <summary>
    /// BattleSystem의 역할 : 배틀 페이즈가 시작되면 배틀 페이즈진행에 관련된 모든 처리를 맡아서 한다.
    /// </summary>
    public class BattleSystem : IUpdatable, IEventListener
    {
        // 몬스터 데이터
        private StageGroupData stageGroupData = JsonUtility.FromJson<StageGroupData>(File.ReadAllText("Assets/Json/StageGroupData.json"));

        // 현재 스테이지 몬스터 데이터
        private List<MonsterData> currentStageMonsterData;

        // 현재 스테이지 total 몬스터 카운트
        private int currentStageMonsterCount;

        // 죽은 몬스터 카운트
        private int deadMonsterCount;

        // 현재 소환되는 몬스터의 인덱스
        private int summonMonsterIndex;

        // 소환 딜레이
        private float currentSummonDelay = 0;

        // 골드 딜레이
        private float currentGoldDelay = 0;

        // 골드
        private int gold = 0;

        // 초당 골드 수급량
        private float supplyGold = 1;

        // 초기 골드량
        private int stageStartGold = 0;

        // 몬스터가 성으로 가는 경로
        private List<List<Vector3Int>> monsterPathList = new List<List<Vector3Int>>();

        // TODO: 나중에 스테이지 별로 소환 시간 다르게 해야하면 데이터 안에 넣는 리팩토링 필요
        private readonly float SUMMON_DELAY = 5;

        public BattleSystem()
        {
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageStartEvent), this);
        }

        // 스테이지 시작 전 초기화
        private void Init(int stage)
        {
            // 현재 스테이지의 몬스터 데이터를 들고오고 몬스터의 수를 저장
            currentStageMonsterData = stageGroupData.StageDataGroup[stage].MonsterGroupList;
            currentStageMonsterCount = currentStageMonsterData.Count;
            deadMonsterCount = 0;
            summonMonsterIndex = 0;

            // 딜레이 초기화
            currentSummonDelay = 0;
            currentGoldDelay = 0;

            // 골드 초기량으로 고정
            gold = stageStartGold;

            GameManager.Instance.MessageSystem.Subscribe(typeof(MonsterDeadEvent), this);

            for (int i = 0; i < 4; i++)
                monsterPathList.Add(TileManager.Instance.GetPathFromMonsterCastle());
        }

        // 소환 딜레이 지나면 몬스터 소환
        public void UpdateFrame(float dt)
        {
            currentSummonDelay += dt;
            currentGoldDelay += dt * supplyGold;

            if(currentGoldDelay > 1)
            {
                currentGoldDelay = 0;
                gold++;
            }

            if (currentSummonDelay >= SUMMON_DELAY)
            {
                currentSummonDelay = 0;
                if(summonMonsterIndex < currentStageMonsterCount)
                {
                    MonsterData monster = currentStageMonsterData[summonMonsterIndex];
                    string type = monster.MonsterType;
                    string path = monster.MonsterPrefabPath;
                    List<Vector3Int> monsterPath = monsterPathList[UnityEngine.Random.Range(0, monsterPathList.Count)];

                    switch (type)
                    {
                        case "Slime":
                            Slime slime = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Slime>(path, path);
                            slime.Init(monster.MonsterHP, monster.MonsterDamage, monsterPath, path);
                            break;
                        default:
                            break;
                    }
                    summonMonsterIndex++;
                }
            }
        }

        // 배틀 시작 이벤트가 오면 init하고 자기 자신 업데이트에 넣기
        // 몬스터가 죽는 이벤트가 오면 카운트 증가시키고 죽은 몬스터가 현재 스테이지의 몬스터 수가 되면 배틀 종료 이벤트 발행
        public bool OnEvent(IEvent e)
        {
            Type eventType = e.GetType();
            if (eventType == typeof(BattleStageStartEvent))
            {
                BattleStageStartEvent battleStateStartEvent = e as BattleStageStartEvent;
                Init(battleStateStartEvent.Stage);
                GameManager.Instance.AddUpdate(this);
                return true;
            }
            else if (eventType == typeof(MonsterDeadEvent))
            {
                deadMonsterCount++;
                if (deadMonsterCount >= currentStageMonsterCount)
                {
                    EndBattleStage();
                }
                return true;
            }
            return false;
        }

        // 배틀 종료
        private void EndBattleStage()
        {
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(MonsterDeadEvent), this);
            GameManager.Instance.MessageSystem.Publish(BattleStageEndEvent.Create());
        }

        // 초기 골드 수급량 증가
        public void AddSupplyGold(float amount)
        {
            supplyGold += amount;
        }

        // 초기 골드량 증가
        public void AddStageStartGold(int amount)
        {
            stageStartGold += amount;
        }

        // 골드 사용 비용이 더 높으면 return false 살 수 있으면 돈을 차감하고 return true
        public bool SpendGold(int amount)
        {
            if(amount > gold)
                return false;

            gold -= amount;
            return true;
        }
    }
}