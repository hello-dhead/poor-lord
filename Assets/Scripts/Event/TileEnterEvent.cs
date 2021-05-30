using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class TileEnterEvent : IEvent
    {
        public Vector3Int EnterTilePos;
        public Unit EnterUnit;

        public static TileEnterEvent Create(Vector3Int tilePos, Unit unit)
        {
            TileEnterEvent e = new TileEnterEvent();
            e.EnterTilePos = tilePos;
            e.EnterUnit = unit;

            return e;
        }
    }
}