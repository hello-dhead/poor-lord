using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace poorlord
{
    public class RewardCard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image cardFrame;

        [SerializeField]
        private Image unitSprite;

        [SerializeField]
        private Text unitText;

        [SerializeField]
        private Text coinText;

        [SerializeField]
        private GameObject reward;

        private CardValue value;

        private UnitID unit;

        public void init(CardValue value, UnitID unit)
        {
            this.value = value;
            this.unit = unit;

            cardFrame.sprite = GameManager.Instance.CardSystem.GetCardFrame(value);
            unitSprite.sprite = GameManager.Instance.CardSystem.GetUnitSprite(unit);

            coinText.text = GameManager.Instance.CardSystem.GetUnitValue(value).ToString();
            unitText.text = UnitID.GetName(unit.GetType(), unit);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameManager.Instance.CardSystem.CreateCardData(value, unit);
            reward.SetActive(false);
        }
    }
}