using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class TileLeaveEvent : IEvent
    {
        public Vector3Int LeaveTilePos;
        public Unit LeaveUnit;

        public static TileLeaveEvent Create(Vector3Int tilePos, Unit unit)
        {
            TileLeaveEvent e = new TileLeaveEvent();
            e.LeaveTilePos = tilePos;
            e.LeaveUnit = unit;

            return e;
        }
    }
}