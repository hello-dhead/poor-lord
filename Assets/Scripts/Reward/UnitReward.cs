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
        [SerializeField]
        private Animator animator;

        private Text coinText;

        [SerializeField]
        private GameObject reward;

        [SerializeField]
        private List<RewardCard> rewardCardList = new List<RewardCard>();

        private bool isGacha = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            coinText = GameManager.Instance.RewardSystem.coinText;
            int coin = Int32.Parse(coinText.text);
            if (coin > 0)
            {
                coinText.text = (coin - 1).ToString();
                StartCoroutine("GetReward");
            }
        }

        public IEnumerator GetReward()
        {
            isGacha = true;
            animator.Play("Gacha_Unit_Pay");
            yield return new WaitForSeconds(3f);

            for (int i = 0; i < rewardCardList.Count; i++)
            {
                rewardCardList[i].init((CardValue)UnityEngine.Random.RandomRange(0, 3), (UnitID)UnityEngine.Random.Range(0, (int)UnitID.PlayerUnitMax));
            }

            reward.SetActive(true);
            isGacha = false;
        }
    }
}