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
        private static List<Buff> buffList = new List<Buff>();

        static BuffManager()
        {
            buffList = new List<Buff>();
            buffList.Add(new BuffAdditionalDamage());
            buffList.Add(new BuffIncreaseDamageTwice());
            buffList.Add(new BuffIncreasedHPTwice());
            buffList.Add(new BuffGateKeeper());
            buffList.Add(new BuffBerserker());
            buffList.Add(new BuffAvenger());
            buffList.Add(new BuffGuardian());
            buffList.Add(new BuffVanguard());
            buffList.Add(new BuffVampire());
            buffList.Add(new BuffVulture());
            buffList.Add(new BuffWarmonger());
            buffList.Add(new BuffReaper());
        }

        // 랜덤한 버프 리턴
        public Buff GetRandomBuff()
        {
            return buffList[UnityEngine.Random.Range(0, buffList.Count)].Copy();
        }
    }
}

