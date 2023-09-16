using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace poorlord
{
    public class TitleButton : MonoBehaviour, IPointerClickHandler
    {
#pragma warning disable CS0649
        [SerializeField]
        private GameObject titleCanvas;

        [SerializeField]
        private Image textImage;

        private bool isAlreadyClick = false;

        private void Start()
        {
            StartCoroutine(BlinkText());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isAlreadyClick == false)
                StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            isAlreadyClick = true;
            StopCoroutine(BlinkText());
            textImage.gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            textImage.gameObject.SetActive(false);

            SoundManager.Instance.PlaySfx("TitleButton", 0.6f);
            yield return new WaitForSeconds(0.1f);
            textImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            textImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            textImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            textImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);

            GameManager.Instance.OpeningStart();
            titleCanvas.gameObject.SetActive(false);
        }

        private IEnumerator BlinkText()
        {
            while(true)
            {
                yield return new WaitForSeconds(1f);

                textImage.gameObject.SetActive(false);

                yield return new WaitForSeconds(0.4f);

                textImage.gameObject.SetActive(true);
            }
        }
    }
}