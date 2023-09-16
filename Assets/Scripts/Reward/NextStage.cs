using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace poorlord
{
    public class NextStage : MonoBehaviour, IPointerClickHandler
    {
        #pragma warning disable CS0649
        [SerializeField]
        private GameObject rewardCanvas;

        [SerializeField]
        private GameObject uiCanvas;

        [SerializeField]
        private GameObject cardCanvas;


        private bool isAlreadyClick = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            if(isAlreadyClick == false)
                StartCoroutine(SetNextStage());
        }

        private IEnumerator SetNextStage()
        {
            isAlreadyClick = true;
            Fade.Instance.FadeIn(1);
            yield return new WaitForSeconds(1f);

            GameManager.Instance.StartBattleStage();

            yield return new WaitForSeconds(1f);

            rewardCanvas.gameObject.SetActive(false);
            uiCanvas.gameObject.SetActive(true);
            cardCanvas.gameObject.SetActive(true);

            isAlreadyClick = false;
            Fade.Instance.FadeOut(0.5f);
        }
    }
}