using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace poorlord
{
    /// <summary>
    /// 카드 역할 : 데이터를 받으면 해당 데이터에 맞게 이미지 스프라이트를 변경하기
    /// 카드 끌어당겨 사용하기, 마우스 올리면 설명 크게 뜨기, 데이터 기반으로 카드 사용하기
    /// </summary>
    public class Card : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
    {
        #pragma warning disable CS0649

        public CardData CardData { get; private set; }
        public bool IsHold { get; private set; }

        [SerializeField]
        private Image frameSpriteRenderer;

        [SerializeField]
        private Image imageSpriteRenderer;
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
            IsHold = false;
            // 텍스트 초기화
            for (int i = 0; i < textList.Count; i++)
            {
                textList[i].text = "";
            }

            // 텍스트 넣기
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

        // 카드 사용하기
        public void Spend(int x, int z)
        {
            IsHold = false;
            if (CardData.Spend(new Vector3Int(x, 0, z)) == true)
                Dispose();
        }

        public void Dispose()
        {
            GameManager.Instance.CardSystem.RemoveCard(this);
            CardData = null;
            PoolManager.Instance.Release<Card>(GameManager.Instance.CardSystem.CardPoolKey, this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            IsHold = true;
            gameObject.transform.SetAsLastSibling();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsHold)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("Tile"))
                    {
                        Spend((int)hit.transform.position.x, (int)hit.transform.position.z);
                    }
                }
                IsHold = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(IsHold)
            {
                Vector2 currentPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out currentPos);
                
                this.transform.localPosition = new Vector3(this.transform.localPosition.x + currentPos.x, this.transform.localPosition.y + currentPos.y, 0);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                IsHold = false;
            }
        }
    }
}