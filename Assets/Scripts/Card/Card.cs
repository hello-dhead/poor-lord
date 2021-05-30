using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace poorlord
{
    /// <summary>
    /// 카드 역할 : 데이터를 받으면 해당 데이터에 맞게 이미지 스프라이트를 변경하기
    /// 카드 끌어당겨 사용하기, 마우스 올리면 설명 크게 뜨기, 데이터 기반으로 카드 사용하기
    /// </summary>
    public class Card : MonoBehaviour
    {
        public CardData CardData { get; private set; }

        // frameSpriteRenderer.sprite = frameSprite;
        [SerializeField]
        private SpriteRenderer frameSpriteRenderer;

        // imageSpriteRenderer.sprite = ImageSprite;
        [SerializeField]
        private SpriteRenderer imageSpriteRenderer;
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Text costText;
        [SerializeField]
        private List<Text> textList;
        [SerializeField]
        private GameObject coin;

        // 카드 받은 데이터를 기반으로 카드 생성하기
        public void SetCard(CardData cardData)
        {
            for (int i = 0; i < textList.Count; i++)
            {
                textList[i].text = "";
            }

            List<String> textStr = cardData.GetCardStr();
            for (int i = 0; i < textStr.Count; i++)
            {
                if (i < textList.Count)
                    textList[i].text = textStr[i];
            }

            CardData = cardData;
            costText.text = CardData.Cost.ToString();
            nameText.text = CardData.Name.ToString();

            frameSpriteRenderer.sprite = CardData.FrameSprite;
            imageSpriteRenderer.sprite = CardData.ImageSprite;

        }

        // 마우스 올리면 카드 정보 나오게 하기
        public void ToDetail()
        {
        }

        // 카드 사용하기
        public void Spend()
        {
        }
        
    }
}