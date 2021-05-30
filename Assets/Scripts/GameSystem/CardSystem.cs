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
        public List<CardData> CurrentHand { get; private set; }

        public CardSystem()
        {
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageStartEvent), this);

            CardDeckList = new List<CardData>();
            CurrentStageDeck = new List<CardData>();
            CurrentHand = new List<CardData>();

            // 기본 덱
            //CardDeckList.Add(new CardData());
            //CardDeckList.Add(new CardData());
            //CardDeckList.Add(new CardData());
            //CardDeckList.Add(new CardData());
            //CardDeckList.Add(new CardData());
            //CardDeckList.Add(new CardData());

            Init();
        }

        // 초기화
        private void Init()
        {
            // 현재 스테이지에서 사용할 덱 카피해오기
            CopyDeck();
        }
        
        public void UpdateFrame(float dt)
        {
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
            return false;
        }

        // 카드를 생성한다
        // 여기에서 유닛 카드 만들어 줄때 버프는 무조건 값 복사해서 새로 생성해야함
        private void CreateCard(CardData card)
        {
            Card new_card = PoolManager.Instance.GetOrCreateObjectPoolFromPath<Card>("Prefabs/Card", "Prefabs/Card");

        }

        // 카드를 뽑는다
        private void DrawCard()
        {
            int randCardNum = UnityEngine.Random.Range(0, CurrentStageDeck.Count);
            CurrentHand.Add(CurrentStageDeck[randCardNum]);
            CurrentStageDeck.Remove(CurrentStageDeck[randCardNum]);

            // 카드 데이터를 기반으로 실제 카드로 바꿈
            //CreateCard(CurrentHand[CurrentHand.Count - 1]);
        }

        // 카드를 정렬한다
        private void SortCard()
        {

        }

        // 덱을 복사한다.
        private void CopyDeck()
        {
            CurrentStageDeck.Clear();

            foreach (CardData card in CardDeckList)
            {
            }
        }
    }
}