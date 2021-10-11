using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Assets.Scripts.Commons;

namespace poorlord
{
    public class GameManager : MonoSingleton<GameManager>, IEventListener
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

        // 보상 관련 모든 처리를 하는 시스템
        public RewardSystem RewardSystem;

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
            Fade.Instance.FadeOut(0.5f);

            updateHashSet = new HashSet<IUpdatable>();
            addUpdateList = new List<IUpdatable>();
            removeUpdateList = new List<IUpdatable>();

            MessageSystem = new MessageSystem();
            BattleSystem = new BattleSystem();
            CardSystem = new CardSystem();
            RewardSystem = new RewardSystem();

            MessageSystem.Subscribe(typeof(BattleStageEndEvent), this);

            StartBattleStage();
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

            for (int i = 0; i < removeUpdateList.Count; i++)
            {
                updateHashSet.Remove(removeUpdateList[i]);
            }

            removeUpdateList.Clear();

            // 메세지 시스템은 무조건 다른 업데이트돌기 전에 발행 요청한 이벤트들 우선 처리
            MessageSystem.UpdateFrame(dt);

            foreach (var updatable in updateHashSet)
            {
                updatable.UpdateFrame(dt);
            }
        }

        // TODO: 사막, 얼음맵 출현하도록 수정 필요
        public void StartBattleStage()
        {
            TileManager.Instance.CreateTileMap((TileTheme)0, 10, 4);

            SoundManager.Instance.PlayBGM("Forest", 0.2f);
            Camera camera = Camera.main;
            camera.transform.position = new Vector3(4, 5.2f, -2.1f);
            ParticleSystem dust = EffectManager.Instance.CreateEffect("ForestDust", camera.transform.position + new Vector3(0, -1.5f, 2), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)));
            dust.transform.SetParent(camera.transform);

            MessageSystem.Publish(BattleStageStartEvent.Create(stage));
            stage++;
        }

        public bool OnEvent(IEvent e)
        {
            Type eventType = e.GetType();
            if (eventType == typeof(BattleStageEndEvent))
            {
                TileManager.Instance.ReleaseTileMap();
                return true;
            }
            return false;
        }
    }
}