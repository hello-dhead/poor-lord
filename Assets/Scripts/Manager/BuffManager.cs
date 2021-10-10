using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Commons;

namespace poorlord
{
    /// <summary>
    /// BuffManager 역할 : 버프 리스트를 가지고 있다가 리턴해줌
    /// </summary>
    public class BuffManager : Singleton<BuffManager>
    {
        // 버프 리스트
        static List<Buff> buffList = new List<Buff>();

        static BuffManager()
        {
            buffList = new List<Buff>();
            buffList.Add(new BuffAdditionalDamage());
            buffList.Add(new BuffIncreaseDamageTwice());
            buffList.Add(new BuffIncreasedHPTwice());
            buffList.Add(new BuffGateKeeper());
            buffList.Add(new BuffBerserker());
        }

        // 랜덤한 버프 리턴
        public Buff GetRandomBuff()
        {
            return buffList[UnityEngine.Random.Range(0, buffList.Count)].Copy();
        }
    }
}

