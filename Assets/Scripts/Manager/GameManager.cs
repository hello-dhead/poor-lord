﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using Assets.Scripts.Commons;

namespace poorlord
{
    public class GameManager : MonoSingleton<GameManager>, IEventListener
    {
        /// <summary>
        /// GameManager에서 돌려주는 업데이트가 필요하거나 
        /// GameManager의 생성 이후 호출되어야 하는 클래스는 System으로 분류해서 GameManager에서 관리하도록
        /// 그 외 GameManager에 의존하지 않는 클래스는 별개의 Manager로 분류 
        /// </summary>

        // 이벤트 관리,처리하는 메세지 시스템
        public MessageSystem MessageSystem;

        // 배틀 관련 모든 처리를 하는 시스템
        public BattleSystem BattleSystem;

        // 카드 관련 모든 처리를 하는 시스템
        public CardSystem CardSystem;

        // 보상 관련 모든 처리를 하는 시스템
        public RewardSystem RewardSystem;

        // 이펙트 관련 모든 처리를 하는 시스템
        public EffectSystem EffectSystem;

        // 매 프레임 업데이트되야하는 해시셋
        private HashSet<IUpdatable> updateHashSet = new HashSet<IUpdatable>();

        // 다음 프레임이 시작될 때 추가 될 업데이트
        private List<IUpdatable> addUpdateList = new List<IUpdatable>();

        // 다음 프레임이 시작될 때 삭제 될 업데이트
        private List<IUpdatable> removeUpdateList = new List<IUpdatable>();

        [SerializeField]
        private GameObject uiCanvas;

        public int Stage { get; private set; }

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
            Stage = 0;

            MessageSystem = new MessageSystem();
            BattleSystem = new BattleSystem();
            CardSystem = new CardSystem();
            RewardSystem = new RewardSystem();
            EffectSystem = new EffectSystem();

            MessageSystem.Subscribe(typeof(BattleStageEndEvent), this);

            SoundManager.Instance.Preload("Forest");
            SoundManager.Instance.Preload("Desert");
            SoundManager.Instance.Preload("Ice");
            SoundManager.Instance.Preload("Ending");

            Opening();
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
        
        public void StartBattleStage()
        {
            string theme = "Desert";
            if (Stage == 1)
            {
                theme = "Ice";
                TileManager.Instance.CreateTileMap((TileTheme)1, 12, 3);
            }
            else if(Stage == 2)
            {
                TileManager.Instance.CreateTileMap((TileTheme)2, 9, 5);
            }
            else
            {
                int map = UnityEngine.Random.Range(0, 3);
                int x = UnityEngine.Random.Range(8, 13);
                int z = UnityEngine.Random.Range(3, 6);
                TileManager.Instance.CreateTileMap((TileTheme)map, x, z);

                switch (map)
                {
                    case 0:
                        theme = "Forest";
                        break;
                    case 1:
                        theme = "Ice";
                        break;
                    default:
                        break;
                }
            }

            SoundManager.Instance.PlayBGM(theme, 0.2f);
            Camera camera = Camera.main;
            camera.transform.position = new Vector3(4, 5.2f, -2.1f);
            ParticleSystem dust = GameManager.Instance.EffectSystem.CreateEffect(theme + "Dust", camera.transform.position + new Vector3(0, -1.5f, 2), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)));
            dust.transform.SetParent(camera.transform);

            MessageSystem.Publish(BattleStageStartEvent.Create(Stage));
            Stage++;
        }

        public void Opening()
        {
            Fade.Instance.FadeOut(0.5f);

            uiCanvas.SetActive(false);

            TileManager.Instance.CreateTileMap((TileTheme)0, 10, 4);

            SoundManager.Instance.PlayBGM("Forest", 0.2f);
            Camera camera = Camera.main;
            camera.transform.position = new Vector3(5, 5.2f, -2.1f);
            ParticleSystem dust = GameManager.Instance.EffectSystem.CreateEffect("ForestDust", camera.transform.position + new Vector3(0, -1.5f, 2), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)));
            dust.transform.SetParent(camera.transform);
        }

        public void OpeningStart()
        {
            uiCanvas.SetActive(true);
            uiCanvas = null;

            MessageSystem.Publish(BattleStageStartEvent.Create(Stage));
            Stage++;
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