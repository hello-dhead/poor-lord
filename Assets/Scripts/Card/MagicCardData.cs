using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace poorlord
{
    /// <summary>
    /// 마법카드데이터 : 마법의 정보 및 소환
    /// </summary>
    public class MagicCardData : CardData
    {
        private BlockID block;

        // 생성자
        public MagicCardData(int cost, string name, Sprite frame, Sprite image, BlockID block) : base(cost, name, frame, image)
        {
            this.block = block;
        }

        // 카드 사용
        public override bool Spend(Vector3Int pos)
        {
            PlayerTile playerTile = (PlayerTile)FieldObjectManager.Instance.CreateTile(block);

            List<Vector3Int> checkTileList = playerTile.CheckTile;
            for (int i = 0; i < checkTileList.Count; i++)
            {
                checkTileList[i] += pos;
            }

            if (TileManager.Instance.CheckBuildableTileList(checkTileList) && GameManager.Instance.BattleSystem.CheckOverlapMonsterPath(checkTileList) && GameManager.Instance.BattleSystem.SpendGold(Cost))
            {
                playerTile.Init(pos);
                for (int i = 0; i < checkTileList.Count; i++)
                {
                    TileManager.Instance.ChangeState(checkTileList[i], TileState.PlayerTile);
                }

                GameManager.Instance.BattleSystem.ChangeMonsterPath();
                return true;
            }
            FieldObjectManager.Instance.ReleaseTile(block, playerTile);
            return false;
        }

        // 카드 텍스트 스트링 반환
        public override List<String> GetCardStr()
        {
            List<String> cardString = new List<String>();
            //cardString.Add(text.text);

            return cardString;
        }
    }
}