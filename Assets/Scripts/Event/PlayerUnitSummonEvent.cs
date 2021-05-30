using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class PlayerUnitSummonEvent : IEvent
    {
        public Vector3Int SummonTilePos;
        public PlayerUnit SummonUnit;

        public static PlayerUnitSummonEvent Create(Vector3Int tilePos, PlayerUnit unit)
        {
            PlayerUnitSummonEvent e = new PlayerUnitSummonEvent();
            e.SummonTilePos = tilePos;
            e.SummonUnit = unit;
            return e;
        }
    }
}