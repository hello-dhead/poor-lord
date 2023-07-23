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
            var continuousBuffTypeList = TypeUtility.GetTypesWithBaseType(typeof(ContinuousBuff));
            var immediatelyBuffTypeList = TypeUtility.GetTypesWithBaseType(typeof(ImmediatelyBuff));
            foreach (var continuousBuffType in continuousBuffTypeList)
            {
                buffList.Add((Buff)System.Activator.CreateInstance(continuousBuffType));
            }
            foreach (var immediatelyBuffType in immediatelyBuffTypeList)
            {
                buffList.Add((Buff)System.Activator.CreateInstance(immediatelyBuffType));
            }
        }

        // 랜덤한 버프 리턴
        public Buff GetRandomBuff()
        {
            return buffList[Random.Range(0, buffList.Count)].Copy();
        }
    }
}

