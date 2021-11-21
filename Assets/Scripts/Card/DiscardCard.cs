using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace poorlord
{
    public class DiscardCard : MonoBehaviour, IPointerClickHandler
    {
        #pragma warning disable CS0649
        [SerializeField]
        private Animator animator;

        private int discardGold = 1;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left && GameManager.Instance.BattleSystem.Gold >= discardGold)
            {
                GameManager.Instance.CardSystem.DiscardFirstCard();
                animator.Rebind();
                animator.Play("DiscardButtonPay");
                GameManager.Instance.BattleSystem.SpendGold(discardGold);
            }
        }
    }
}