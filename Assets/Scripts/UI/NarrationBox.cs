using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Commons;

namespace poorlord
{
    public class NarrationBox : MonoSingleton<NarrationBox>
    {
        #pragma warning disable CS0649

        [SerializeField]
        private GameObject narrationSprite;

        [SerializeField]
        private Text narrationText;

        private int narrationCount = 0;

        public void ShowNarration(string str, float time = 1)
        {
            StartCoroutine(NarrationCor(str, time));
        }

        private IEnumerator NarrationCor(string str, float time)
        {
            narrationCount++;
            narrationText.text = str;
            narrationSprite.gameObject.SetActive(true);
            narrationText.gameObject.SetActive(true);

            yield return new WaitForSeconds(time);

            narrationCount--;
            if (narrationCount == 0)
            {
                narrationSprite.gameObject.SetActive(false);
                narrationText.gameObject.SetActive(false);
            }
        }
    }
}