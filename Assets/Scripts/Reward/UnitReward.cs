using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace poorlord
{
    public class UnitReward : MonoBehaviour, IPointerClickHandler
    {
        #pragma warning disable CS0649

        [SerializeField]
        private Animator animator;

        private Text coinText;

        [SerializeField]
        private GameObject reward;

        [SerializeField]
        private List<RewardUnitCard> rewardCardList = new List<RewardUnitCard>();

        public void OnPointerClick(PointerEventData eventData)
        {
            coinText = GameManager.Instance.RewardSystem.coinText;
            int coin = Int32.Parse(coinText.text);
            if (coin > 0 && GameManager.Instance.RewardSystem.IsGacha == false)
            {
                coinText.text = (coin - 1).ToString();
                StartCoroutine("GetReward");
            }
        }

        public IEnumerator GetReward()
        {
            SoundManager.Instance.PlaySfx("Gacha");
            GameManager.Instance.RewardSystem.IsGacha = true;
            animator.Play("Gacha_Unit_Pay");
            yield return new WaitForSeconds(3f);

            SoundManager.Instance.PlaySfx("RewardGet", 0.8f);
            for (int i = 0; i < rewardCardList.Count; i++)
            {
                rewardCardList[i].Init((CardValue)UnityEngine.Random.Range(0, 3), (UnitID)UnityEngine.Random.Range(0, (int)UnitID.PlayerUnitMax));
            }

            reward.SetActive(true);
            GameManager.Instance.RewardSystem.IsGacha = false;
        }
    }
}