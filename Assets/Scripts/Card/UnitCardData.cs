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
        private UnitID unit;
        private List<Buff> buffList = new List<Buff>();

        // 생성자
        public UnitCardData(int cost, string name, Sprite frame, Sprite image, List<Buff> buff, UnitID unit) : base(cost, name, frame, image)
        {
            this.unit = unit;
            buffList = buff;
        }

        // 유닛 소환
        public override bool Spend(Vector3Int pos)
        {
            if (TileManager.Instance.CheckBuildableUnit(pos.x, pos.z))
            {
                if(GameManager.Instance.BattleSystem.SpendGold(Cost) == false)
                {
                    NarrationBox.Instance.ShowNarration("골드가 부족합니다");
                    return false;
                }

                SoundManager.Instance.PlaySfx("Summon", 0.5f);
                GameManager.Instance.EffectSystem.CreateEffect("NovaBlue", pos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
                PlayerUnit platerUnit = (PlayerUnit)FieldObjectManager.Instance.CreateUnit(unit);
                platerUnit.Init(pos, buffList);
                return true;
            }
            NarrationBox.Instance.ShowNarration("설치할 수 없는 위치입니다");
            return false;
        }

        // 카드 텍스트 스트링 반환
        public override List<String> GetCardStr()
        {
            List<String> cardString = new List<String>();
            for (int i = 0; i < buffList.Count; i++)
            {
                cardString.Add(buffList[i].BuffName);
            }

            return cardString;
        }
    }
}