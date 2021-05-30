using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 유닛카드데이터 : 각 유닛의 정보를 저장 및 소환하는 역할
    /// </summary>
    public class UnitCardData : CardData
    {
        private PlayerUnit unit;
        private List<ImmediatelyBuff> immediatelyBuff = new List<ImmediatelyBuff>();
        private List<ContinuousBuff> continuousBuff = new List<ContinuousBuff>();

        // 생성자
        public UnitCardData(int cost, string name, Sprite frame, Sprite image, List<ImmediatelyBuff> immdiBuff, List<ContinuousBuff> contiBuff, PlayerUnit Unit) : base(cost, name, frame, image)
        {
            immediatelyBuff = immdiBuff;
            continuousBuff = contiBuff;
        }

        // 유닛 소환
        public override bool Spend(Vector3Int pos)
        {
            unit = unit.GetPrefabs();
            unit.Init(pos, immediatelyBuff, continuousBuff, unit.GetKey());
            return false;
        }

        // 카드 텍스트 스트링 반환
        public override List<String> GetCardStr()
        {
            List<String> cardString = new List<String>();
            for (int i = 0; i < continuousBuff.Count; i++)
            {
                cardString.Add(continuousBuff[i].BuffName);
            }

            for (int i = 0; i < immediatelyBuff.Count; i++)
            {
                cardString.Add(immediatelyBuff[i].BuffName);
            }
            return cardString;
        }
    }
}