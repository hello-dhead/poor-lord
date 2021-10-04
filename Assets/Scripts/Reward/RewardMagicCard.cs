using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace poorlord
{
    public class RewardMagicCard : MonoBehaviour, IPointerClickHandler
    {
        #pragma warning disable CS0649

        [SerializeField]
        private Image cardFrame;

        [SerializeField]
        private Image blockSprite;

        [SerializeField]
        private Text blockText;

        [SerializeField]
        private Text coinText;

        [SerializeField]
        private GameObject reward;

        private CardValue value;

        private BlockID block;

        public void Init(BlockID block)
        {
            PlayerTile playerTile = (PlayerTile)FieldObjectManager.Instance.CreateTile(block);

            value = playerTile.GetValue();
            value = playerTile.GetValue();
            blockText.text = playerTile.GetName();

            FieldObjectManager.Instance.ReleaseTile(block, playerTile);

            this.block = block;

            coinText.text = GameManager.Instance.CardSystem.GetUnitValue(value).ToString();
            cardFrame.sprite = GameManager.Instance.CardSystem.GetCardFrame(value);
            blockSprite.sprite = GameManager.Instance.CardSystem.GetSprite(block);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GameManager.Instance.CardSystem.CreateCardData(value, block, blockText.text);
            reward.SetActive(false);
        }
    }
}