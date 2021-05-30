using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 카드데이터 : 각 유닛이나 마법 or 빌드카드의 정보를 저장하고 사용하는 역할
    /// </summary>
    public abstract class CardData
    {
        public int Cost { get; private set; }
        public string Name { get; private set; }
        public Sprite FrameSprite { get; private set; }
        public Sprite ImageSprite { get; private set; }

        public abstract List<String> GetCardStr();
        public abstract bool Spend(Vector3Int pos);

        // 생성자
        public CardData(int cost, string name, Sprite frame, Sprite image)
        {
            Cost = cost;
            Name = name;
            FrameSprite = frame;
            ImageSprite = image;
        }
    }
}