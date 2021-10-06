using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace poorlord
{
    public class RewardUnitCard : MonoBehaviour, IPointerClickHandler
    {
        #pragma warning disable CS0649

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

        [SerializeField]
        private List<Text> buffTextList;

        private List<Buff> buffList = new List<Buff>();

        private CardValue value;

        private UnitID unit;

        public void Init(CardValue value, UnitID unit)
        {
            this.value = value;
            this.unit = unit;

            cardFrame.sprite = GameManager.Instance.CardSystem.GetCardFrame(value);
            unitSprite.sprite = GameManager.Instance.CardSystem.GetSprite(unit);

            coinText.text = GameManager.Instance.CardSystem.GetUnitValue(value).ToString();
            unitText.text = UnitID.GetName(unit.GetType(), unit);

            for (int i = 0; i < buffTextList.Count; i++)
                buffTextList[i].text = "";

            AddBuff(value);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameManager.Instance.CardSystem.CreateCardData(value, unit, buffList);
            reward.SetActive(false);
        }

        private void AddBuff(CardValue value)
        {
            int buffNum = ((int)value + 1);
            buffList.Clear();
            for (int i = 0; i < buffNum; i++)
            {
                Buff buff = BuffManager.Instance.GetRandomBuff();
                buffTextList[i].text = buff.BuffName;
                buffList.Add(buff);
            }
        }
    }
}