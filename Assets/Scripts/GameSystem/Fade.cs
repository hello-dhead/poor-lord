using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Commons;

namespace poorlord
{
    // 페이드 인, 아웃
    public class Fade : MonoSingleton<Fade>
    {
        #pragma warning disable CS0649
        [SerializeField]
        private Image fadeImage;

        public void FadeIn(float time)
        {
            StartCoroutine(FadeInCoroutine(time));
        }
        public void FadeOut(float time)
        {
            StartCoroutine(FadeOutCoroutine(time));
        }

        IEnumerator FadeInCoroutine(float time)
        {
            float start = 0;
            float end = 1f;
            float fadeTime = 0;

            Color fadecolor = fadeImage.color;

            while (fadecolor.a < end)
            {
                fadeTime += Time.deltaTime / time;
                fadecolor.a = Mathf.Lerp(start, end, fadeTime);
                fadeImage.color = fadecolor;
                yield return null;
            }
        }

        IEnumerator FadeOutCoroutine(float time)
        {
            float start = 1f;
            float end = 0;
            float fadeTime = 0;

            Color fadecolor = fadeImage.color;

            while (fadecolor.a > end)
            {
                fadeTime += Time.deltaTime / time;
                fadecolor.a = Mathf.Lerp(start, end, fadeTime);
                fadeImage.color = fadecolor;
                yield return null;
            }
        }
    }
}