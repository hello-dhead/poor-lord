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

            if (TileManager.Instance.CheckBuildableTileList(checkTileList) && GameManager.Instance.BattleSystem.CheckOverlapMonsterPath(checkTileList))
            {
                if (GameManager.Instance.BattleSystem.SpendGold(Cost) == false)
                {
                    NarrationBox.Instance.ShowNarration("골드가 부족합니다");
                    FieldObjectManager.Instance.ReleaseTile(block, playerTile);
                    return false;
                }

                playerTile.Init(pos);
                for (int i = 0; i < checkTileList.Count; i++)
                {
                    TileManager.Instance.ChangeState(checkTileList[i], TileState.PlayerTile);
                }
                SoundManager.Instance.PlaySfx("Summon", 0.5f);
                GameManager.Instance.MessageSystem.Publish(CreateBlockEvent.Create(checkTileList));
                return true;
            }
            NarrationBox.Instance.ShowNarration("설치할 수 없는 위치입니다");
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