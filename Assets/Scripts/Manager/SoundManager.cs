using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Commons;

namespace poorlord
{
    /// <summary>
    /// SoundManager의 역할 : 게임 내 모든 사운드 관리
    /// </summary>
    public class SoundManager : MonoSingleton<SoundManager>
    {
        // 배경 사운드
        [SerializeField]
        private AudioSource backGroundSound;

        // 효과음 리스트
        [SerializeField]
        private List<AudioSource> sfxSourceList = new List<AudioSource>();

        // 오디오 클립 캐시
        private Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip>();

        // 오디오 클립 위치
        private readonly string CLIP_PATH = "Audios/";

        protected virtual void Start()
        {
            sfxSourceList.Clear();
            backGroundSound = null;

            for (int i = 0; i < 30; i++)
                sfxSourceList.Add(gameObject.AddComponent<AudioSource>());

            backGroundSound = gameObject.AddComponent<AudioSource>();
            backGroundSound.loop = true;
            PlayBGM("Forest", 0.2f);
        }

        public void PlaySfx(string clip, float volume = 0.5f)
        {
            // 실행되지 않는 오디오 소스를 찾아서 clip 실행
            foreach (var source in sfxSourceList)
            {
                if (source.isPlaying == false)
                {
                    source.clip = GetClip(clip);
                    source.Play();
                    source.volume = volume;
                    return;
                }
            }

            // sfx가 다 찼으면 새로 생성해서 실행
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            sfxSourceList.Add(newSource);

            newSource.clip = GetClip(clip);
            newSource.volume = volume;
            newSource.Play();
        }

        public void PlayBGM(string clip, float volume = 0.5f)
        {
            backGroundSound.clip = GetClip(clip);
            backGroundSound.volume = volume;
            backGroundSound.Play();
        }

        public void StopBGM()
        {
            backGroundSound.Stop();
        }

        // 캐시에서 클립을 얻어온다.
        private AudioClip GetClip(string key)
        {
            string path = CLIP_PATH + key;
            // 클립 딕셔너리에서 체크해서 없으면 새로 생성
            if (audioClipDic.ContainsKey(key) == false)
            {
                AudioClip clip = Resources.Load<AudioClip>(path);
                audioClipDic.Add(key, clip);
            }

            AudioClip result;
            audioClipDic.TryGetValue(key, out result);
            return result;
        }

    }
}