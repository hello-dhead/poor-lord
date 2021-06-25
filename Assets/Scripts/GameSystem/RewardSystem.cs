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
    /// RewardSystem의 역할 : 
    /// </summary>
    public class RewardSystem :IUpdatable, IEventListener
    {
        private Canvas rewardCanvas = GameObject.Find("RewardCanvas").GetComponent<Canvas>();
        private Canvas uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
        private Canvas cardCanvas = GameObject.Find("CardCanvas").GetComponent<Canvas>();
        public Text coinText = GameObject.Find("CoinText").GetComponent<Text>();

        public RewardSystem()
        {
            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageEndEvent), this);
            rewardCanvas.gameObject.SetActive(false);
        }

        private void Init()
        {
            rewardCanvas.gameObject.SetActive(true);
            uiCanvas.gameObject.SetActive(false);
            cardCanvas.gameObject.SetActive(false);

            // 나중에 바꿀 수 있음
            coinText.text = "3";
        }

        IEnumerator SetReward()
        {
            Init();

            yield return new WaitForSeconds(0.5f);
            Fade.Instance.FadeOut(1);
        }

        public void UpdateFrame(float dt)
        {
        }
        
        public bool OnEvent(IEvent e)
        {
            Type eventType = e.GetType();
            if (eventType == typeof(BattleStageEndEvent))
            {
                CoroutineHandler.Start_Coroutine(SetReward());
                return true;
            }
            return false;
        }
        
    }
}