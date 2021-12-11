using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace poorlord
{
    public class GoldReward : MonoBehaviour, IPointerClickHandler
    {
        #pragma warning disable CS0649

        private Text coinText;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private GameObject reward;

        [SerializeField]
        private Text rewardText;

        // 최소 골드 수급량 증가 계수
        private int minGoldSupply = 1;

        // 최대 골드 수급량 증가 계수
        private int maxGoldSupply = 5;

        // 최소 시작 골드 개수
        private int minStartGold = 1;

        // 최대 시작 골드 개수
        private int maxStartGold = 5;

        public void OnPointerClick(PointerEventData eventData)
        {
            coinText = GameManager.Instance.RewardSystem.coinText;
            int coin = Int32.Parse(coinText.text);
            if (coin > 0 && GameManager.Instance.RewardSystem.IsGacha == false)
            {
                coinText.text = (coin-1).ToString();
                StartCoroutine("GetReward");
            }
        }

        public IEnumerator GetReward()
        {
            SoundManager.Instance.PlaySfx("Gacha");
            GameManager.Instance.RewardSystem.IsGacha = true;
            animator.Play("Gacha_Gold_Pay");
            yield return new WaitForSeconds(3f);

            int randReward = UnityEngine.Random.Range(0, 2);
            SoundManager.Instance.PlaySfx("RewardGet", 0.8f);
            if (randReward == 0)
            {
                float randRewardAmount = ((float)UnityEngine.Random.Range(minGoldSupply, maxGoldSupply+1))/10;
                GameManager.Instance.BattleSystem.AddSupplyGold(randRewardAmount);
                rewardText.text = $"골드 수급량이 {randRewardAmount}만큼 상승하였습니다!";
            }
            else
            {
                int randRewardAmount = UnityEngine.Random.Range(minStartGold, maxStartGold + 1);
                GameManager.Instance.BattleSystem.AddStageStartGold(randRewardAmount);
                rewardText.text = $"시작 골드가 {randRewardAmount}만큼 상승하였습니다!";
            }
            reward.SetActive(true);
            yield return new WaitForSeconds(2f);
            reward.SetActive(false);
            GameManager.Instance.RewardSystem.IsGacha = false;
        }
    }
}