using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace poorlord
{
    public class DrawButton : MonoBehaviour, IPointerClickHandler
    {
        #pragma warning disable CS0649
        [SerializeField]
        private Animator animator;

        private int drawGold = 3;
        public void OnPointerClick(PointerEventData eventData)
        {
            if ( eventData.button == PointerEventData.InputButton.Left 
                && GameManager.Instance.BattleSystem.Gold >= drawGold && GameManager.Instance.CardSystem.DrawCard() )
            {
                animator.Play("DrawButtonPay");
                GameManager.Instance.BattleSystem.SpendGold(drawGold);
            }
        }
    }
}