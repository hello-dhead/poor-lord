using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class PlayerTile1x4 : PlayerTile
    {
        private readonly CardValue TILE_VALUE = CardValue.Bronze;
        private readonly string TILE_NAME = "1x4 BLOCK";

        public override List<Vector3Int> CheckTile
        {
            get
            {
                List<Vector3Int> checkTile = new List<Vector3Int>();
                checkTile.Add(new Vector3Int(0, 0, 0));
                checkTile.Add(new Vector3Int(0, 0, 1));
                checkTile.Add(new Vector3Int(0, 0, 2));
                checkTile.Add(new Vector3Int(0, 0, 3));

                return checkTile;
            }
        }

        public sealed override void Init(Vector3Int pos)
        {
            this.gameObject.transform.position = pos;

            Value = TILE_VALUE;
            Name = TILE_NAME;

            GameManager.Instance.MessageSystem.Subscribe(typeof(BattleStageEndEvent), this);
        }

        public sealed override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(BattleStageEndEvent))
            {
                Dispose();
            }
            return false;
        }

        public sealed override void Dispose()
        {
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(BattleStageEndEvent), this);
            FieldObjectManager.Instance.ReleaseTile(BlockID.PlayerTile1x4, this);
        }

        public sealed override CardValue GetValue()
        {
            return TILE_VALUE;
        }

        public sealed override string GetName()
        {
            return TILE_NAME;
        }
    }
}