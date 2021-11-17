using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 카드 시스템의 역할 : 게임 매니저에 포함되어 게임 스타트 이벤트가 발행되면 업데이트를 돈다. 
    /// 덱에서 카드 드로우 연출을 하며 마우스에 따라 카드를 정렬한다.
    /// </summary>
    public class CardSystem : IUpdatable, IEventListener
    {
        // 보존용 카드 덱 리스트
        public List<CardData> CardDeckList { get; private set; }

        // 이번 스테이지 덱 리스트 (그 카드를 뽑으면 리스트에서 제거됨)
        public List<CardData> CurrentStageDeck { get; private set; }

        // 현재 핸드
        public List<Card> CurrentHand { get; private set; }

        // 카드 프리팹 풀 키
        public String CardPoolKey { get; private set; }

        // 카드 캔버스
        private Transform cardCanvas = GameObject.Find("CardCanvas").GetComponent<Transform>();

        // 카드 사이 간격
        private readonly int SORT_DISTANCE = 100;

        // 카드가 드러날 y
        private readonly int SORT_Y_POS = -150;

        // 카드가 드러날 y
        private readonly int SORT_SPEED = 4;

        // 카드 스프라이트
        private List<Sprite> cardFrameList;

        private Dictionary<UnitID, Sprite> unitSpriteDic;
        private Dictionary<BlockID, Sprite> blockSpriteDic;

        public CardSystem()
        {
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageStartEvent), this);
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageEndEvent), this);
            CardPoolKey = "Prefabs/Card";

            CardDeckList = new List<CardData>();
            CurrentStageDeck = new List<CardData>();
            CurrentHand = new List<Card>();
            cardFrameList = new List<Sprite>();
            unitSpriteDic = new Dictionary<UnitID, Sprite>();
            blockSpriteDic = new Dictionary<BlockID, Sprite>();

            CreateUnitSpriteDic();
            CreateBlockSpriteDic();

            cardFrameList.Add(Resources.Load<Sprite>("Sprites/Card/Card_1_Main"));
            cardFrameList.Add(Resources.Load<Sprite>("Sprites/Card/Card_2_Main"));
            cardFrameList.Add(Resources.Load<Sprite>("Sprites/Card/Card_3_Main"));

            // 기본 덱
            CreateBasicDeck();
        }

        // 초기화
        private void Init()
        {
            // 현재 스테이지에서 사용할 덱 카피해오기
            CopyDeck();

            DrawCard();
            DrawCard();
            DrawCard();
        }

        // 유닛 스프라이트 딕셔너리 생성
        private void CreateUnitSpriteDic()
        {
            for (int i = 0; i < (int)UnitID.PlayerUnitMax; i++)
            {
                string path = "Sprites/CardUnitSprite/" + Enum.GetName(typeof(UnitID), i);
                unitSpriteDic.Add((UnitID)i, Resources.Load<Sprite>(path));
            }
        }

        // 블럭 스프라이트 딕셔너리 생성
        private void CreateBlockSpriteDic()
        {
            for (int i = 0; i < (int)BlockID.BlockMax; i++)
            {
                string path = "Sprites/CardBlockSprite/" + Enum.GetName(typeof(BlockID), i);
                blockSpriteDic.Add((BlockID)i, Resources.Load<Sprite>(path));
            }
        }

        public void UpdateFrame(float dt)
        {
            SortCard();
        }
        
        // 게임 시작되면 현재 덱으로 게임 플레이
        public bool OnEvent(IEvent e)
        {
            Type eventType = e.GetType();
            if (eventType == typeof(BattleStageStartEvent))
            {
                BattleStageStartEvent battleStateStartEvent = e as BattleStageStartEvent;
                Init();
                GameManager.Instance.AddUpdate(this);
                return true;
            }
            else if(eventType == typeof(BattleStageEndEvent))
            {
                for (int i = 0; i < CurrentHand.Count; i++)
                    CurrentHand[i].Dispose();
                return true;
            }
            return false;
        }

        // 카드를 생성한다
        // 여기에서 유닛 카드 만들어 줄때 버프는 무조건 값 복사해서 새로 생성해야함
        private Card CreateCard(CardData card)
        {
            Card new_card = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Card>(CardPoolKey, "Prefabs/Card");
            new_card.transform.parent = cardCanvas;

            new_card.transform.localPosition = new Vector3(-((SORT_DISTANCE / 2) * (CurrentHand.Count)) + (SORT_DISTANCE* (CurrentHand.Count)), -450, 0);
            new_card.transform.localScale = new Vector3(1, 1, 1);
            new_card.SetCard(card);
            return new_card;
        }

        // 카드를 뽑는다
        public bool DrawCard()
        {
            if(CurrentStageDeck.Count == 0)
                return false;

            int randCardNum = UnityEngine.Random.Range(0, CurrentStageDeck.Count);

            // 카드 데이터를 기반으로 실제 카드로 바꿈
            CurrentHand.Add(CreateCard(CurrentStageDeck[randCardNum]));
            CurrentStageDeck.RemoveAt(randCardNum);

            return true;
        }

        // 카드를 정렬한다
        private void SortCard()
        {
            int count = CurrentHand.Count;

            // 카드 첫 x좌표의 시작 포지션
            int first_x = -((SORT_DISTANCE/2) * (count - 1));
            for (int i = 0; i < count; i++)
            {
                if(CurrentHand[i].IsHold == false)
                {
                    CurrentHand[i].transform.localPosition = Vector3.Lerp(CurrentHand[i].transform.localPosition, new Vector3(first_x, SORT_Y_POS, 0), Time.deltaTime * SORT_SPEED);
                }
                first_x += SORT_DISTANCE;
            }
        }

        // 덱을 복사한다.
        private void CopyDeck()
        {
            CurrentStageDeck.Clear();

            foreach (CardData card in CardDeckList)
            {
                CurrentStageDeck.Add(card);
            }
        }

        // 기본덱 제작
        private void CreateBasicDeck()
        {
            CardDeckList.Add(new MagicCardData(1, "1x1 Block", GetCardFrame(CardValue.Bronze), GetSprite(BlockID.PlayerTile1x1), BlockID.PlayerTile1x1));
            //CardDeckList.Add(new MagicCardData(1, "1x1 Block", GetCardFrame(CardValue.Bronze), GetSprite(BlockID.PlayerTile1x1), BlockID.PlayerTile1x1));

            CardDeckList.Add(new UnitCardData(3, "Shiori", GetCardFrame(CardValue.Bronze), GetSprite(UnitID.Shiori), new List<Buff>(), UnitID.Shiori));
            CardDeckList.Add(new UnitCardData(3, "Shiori", GetCardFrame(CardValue.Bronze), GetSprite(UnitID.Shiori), new List<Buff>(), UnitID.Shiori));
            CardDeckList.Add(new UnitCardData(3, "Alice", GetCardFrame(CardValue.Bronze), GetSprite(UnitID.Alice), new List<Buff>(), UnitID.Alice));
        }

        // 유닛ID와 밸류를 기반으로 랜덤한 카드 생성
        public void CreateCardData(CardValue value, UnitID unit, List<Buff> buff = null)
        {
            int cost = GetUnitValue(value);

            if (buff == null)
                buff = new List<Buff>();

            List <Buff> copyBuff = new List<Buff>();
            for (int i = 0; i < buff.Count; i++)
                copyBuff.Add(buff[i].Copy());

            CardDeckList.Add(new UnitCardData(cost, UnitID.GetName(unit.GetType(), unit), GetCardFrame(value), GetSprite(unit), copyBuff, unit));
        }

        // 블럭ID와 밸류를 기반으로 랜덤한 카드 생성
        public void CreateCardData(CardValue value, BlockID block, string name)
        {
            int cost = GetUnitValue(value);

            CardDeckList.Add(new MagicCardData(cost, name, GetCardFrame(value), GetSprite(block), block));
        }

        // 첫 카드를 버림
        public void DiscardFirstCard()
        {
            if(CurrentHand.Count > 0)
                CurrentHand[0].Dispose();
        }

        public void RemoveCard(Card card)
        {
            CurrentHand.Remove(card);
        }

        public int GetUnitValue(CardValue value)
        {
            return ((int)value + 1) * 3;
        }

        public Sprite GetCardFrame(CardValue value)
        {
            return cardFrameList[(int)value];
        }

        public Sprite GetSprite(UnitID unit)
        {
            return unitSpriteDic[unit];
        }

        public Sprite GetSprite(BlockID block)
        {
            return blockSpriteDic[block];
        }
    }
}