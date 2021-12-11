using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Assets.Scripts.Commons;

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

        // 스테이지에 존재하는 몬스터의 리스트
        private List<MonsterUnit> stageMonsterList = new List<MonsterUnit>();

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
        public int Gold { get; private set; }

        // 초당 골드 수급량
        private float supplyGold = 0.5f;

        // 초기 골드량
        private int stageStartGold = 9;

        // 몬스터가 성으로 가는 경로
        private List<List<Vector3Int>> monsterPathList = new List<List<Vector3Int>>();
        
        // 소환 간격
        private float summonDelay = 1;

        // 골드 텍스트 스프라이트
        private Text goldText = GameObject.Find("GoldText").GetComponent<Text>();
        // 스테이지 텍스트 스프라이트
        private Text stageText = GameObject.Find("StageText").GetComponent<Text>();

        // 클리어 UI
        readonly private GameObject clearUI = GameObject.Find("Clear");

        readonly private GameObject endingUI = GameObject.Find("Ending");

        readonly private GameObject canvasUI = GameObject.Find("UICanvas");

        readonly private GameObject cardCanvas = GameObject.Find("CardCanvas");

        public BattleSystem()
        {
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageStartEvent), this);
        }

        // 스테이지 시작 전 초기화
        private void Init(int stage)
        {
            stageText.text = (stage +1).ToString();
            // 현재 스테이지의 몬스터 데이터를 들고오고 몬스터의 수를 저장
            currentStageMonsterData = stageGroupData.StageDataGroup[stage].MonsterGroupList;
            currentStageMonsterCount = currentStageMonsterData.Count;
            deadMonsterCount = 0;
            summonMonsterIndex = 0;

            // 딜레이 초기화
            currentSummonDelay = 0;
            currentGoldDelay = 0;

            // 골드 초기량으로 고정
            Gold = stageStartGold;
            goldText.text = Gold.ToString();

            SetSummonDelay();

            GameManager.Instance.MessageSystem.Subscribe(typeof(MonsterDeadEvent), this);

            monsterPathList.Clear();
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
                Gold++;
                goldText.text = Gold.ToString();
            }

            if (currentSummonDelay >= summonDelay)
            {
                currentSummonDelay = 0;
                if(summonMonsterIndex < currentStageMonsterCount)
                {
                    GameManager.Instance.EffectSystem.CreateEffect("BeamUpPurple", TileManager.Instance.GetMonsterCastlePos(), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);

                    MonsterData monsterInfo = currentStageMonsterData[summonMonsterIndex];

                    string type = monsterInfo.MonsterType;
                    List<Vector3Int> monsterPath = monsterPathList[UnityEngine.Random.Range(0, monsterPathList.Count)];
                    MonsterUnit monster = (MonsterUnit)FieldObjectManager.Instance.CreateUnit(monsterInfo.MonsterType);
                    monster.Init(monsterInfo.MonsterHP, monsterInfo.MonsterDamage, monsterPath);
                    stageMonsterList.Add(monster);

                    summonMonsterIndex++;
                    SetSummonDelay();
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
                MonsterDeadEvent monsterDeadEvent = e as MonsterDeadEvent;
                stageMonsterList.Remove(monsterDeadEvent.DeadUnit);
                deadMonsterCount++;
                if (deadMonsterCount >= currentStageMonsterCount)
                {
                    if (GameManager.Instance.Stage + 1 >= stageGroupData.StageDataGroup.Count)
                    {
                        CoroutineHandler.Start_Coroutine(Ending());
                    }
                    else
                    {
                        CoroutineHandler.Start_Coroutine(EndBattleStage());
                    }
                }
                return true;
            }
            return false;
        }

        // 배틀 종료
        private IEnumerator EndBattleStage()
        {
            CoroutineHandler.Start_Coroutine(ShowClear());
            yield return new WaitForSeconds(3f);
            Fade.Instance.FadeIn(0.5f);
            GameManager.Instance.EffectSystem.RemoveAllEffect();
            yield return new WaitForSeconds(0.5f);

            GameManager.Instance.RemoveUpdate(this);
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(MonsterDeadEvent), this);
            GameManager.Instance.MessageSystem.Publish(BattleStageEndEvent.Create());
        }

        // 클리어 연출
        private IEnumerator ShowClear()
        {
            while(clearUI.transform.localPosition.x < 0)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                clearUI.transform.localPosition += new Vector3(Time.deltaTime * 4000, 0, 0);
            }
            clearUI.transform.localPosition = new Vector3(0, 0, 0);
            SoundManager.Instance.PlayBGM("GameClear", 0.2f);
            CoroutineHandler.Start_Coroutine(ShowConfetti());

            yield return new WaitForSeconds(2.2f);

            while (clearUI.transform.localPosition.x < 1000)
            {
                clearUI.transform.localPosition += new Vector3(Time.deltaTime * 4000, 0, 0);
                yield return new WaitForSeconds(Time.deltaTime);
            }

            clearUI.transform.localPosition = new Vector3(-1000, 0, 0);
        }

        // 엔딩 연출
        private IEnumerator Ending()
        {
            SoundManager.Instance.PlayBGM("Ending");
            SoundManager.Instance.PlaySfx("Firework", 0.1f);

            canvasUI.SetActive(false);
            cardCanvas.SetActive(false);

            endingUI.transform.GetChild(0).gameObject.SetActive(true);
            CoroutineHandler.Start_Coroutine(ShowConfetti());

            float startScale = 0.1f;
            float endScale = 1;
            float time = 0;
            float endTime = 0.15f;
            while (time < endTime)
            {
                float progress = Mathf.Lerp(startScale, endScale, time/endTime);
                endingUI.transform.localScale = new Vector3(progress, progress, progress);

                time += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            yield return new WaitForSeconds(0.9f);

            while (true)
            {
                CoroutineHandler.Start_Coroutine(ShowConfetti());
                yield return new WaitForSeconds(1.35f);
            }
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
            if(amount > Gold)
                return false;

            Gold -= amount;
            goldText.text = Gold.ToString();
            return true;
        }

        // 블럭을 놓았을 때 성까지 진입이 불가능한 몬스터가 있는지 체크
        public bool CheckOverlapMonsterPath(List<Vector3Int> blockPathList)
        {
            // 몬스터성에서 부터 계산
            if (TileManager.Instance.CheckOverlapPath(TileManager.Instance.GetMonsterCastlePos(), blockPathList) == false)
                return false;

            for (int i = 0; i < stageMonsterList.Count; i++)
            {
                if (TileManager.Instance.CheckOverlapPath(stageMonsterList[i].GetPathList()[0], blockPathList) == false)
                    return false;
            }
            return true;
        }

        // 몬스터와 default Path를 새로 설정
        public void ChangeMonsterPath()
        {
            monsterPathList.Clear();
            for (int i = 0; i < 4; i++)
                monsterPathList.Add(TileManager.Instance.GetPathFromMonsterCastle());

            for (int i = 0; i < stageMonsterList.Count; i++)
            {
                List<Vector3Int> path = TileManager.Instance.GetPathFromPos(stageMonsterList[i].UnitPosition);
                path.RemoveAt(0);
                stageMonsterList[i].ChangePath(path);
            }
        }

        // 다음 몬스터의 소환 딜레이 설정
        private void SetSummonDelay()
        {
            if (summonMonsterIndex < currentStageMonsterCount)
            {
                MonsterData monsterInfo = currentStageMonsterData[summonMonsterIndex];
                summonDelay = monsterInfo.SummonTerm;
            }
        }

        // 클리어나 엔딩에 쓰이는 빵빠레 연출
        private IEnumerator ShowConfetti()
        {
            Camera camera = Camera.main;

            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(-1f, -2f, 2), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            yield return new WaitForSeconds(0.15f);
            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(1f, -2f, 1), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            yield return new WaitForSeconds(0.15f);
            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(1.5f, -2f, 3), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            yield return new WaitForSeconds(0.15f);
            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(-2f, -2f, 0.5f), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            yield return new WaitForSeconds(0.15f);
            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(2f, -2f, 0.5f), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            yield return new WaitForSeconds(0.15f);
            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(-1.5f, -2f, 3), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            yield return new WaitForSeconds(0.15f);
            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(0.5f, -2f, 2f), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            yield return new WaitForSeconds(0.15f);
            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(-1f, -2f, 1.5f), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
            yield return new WaitForSeconds(0.15f);
            GameManager.Instance.EffectSystem.CreateEffect("ConfettiExplosion", camera.transform.position + new Vector3(2f, -2f, 0.5f), new Vector3(0.4f, 0.4f, 0.4f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);
        }
    }
}