using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class PlayerUnitDeadEvent : IEvent
    {
        public Vector3Int DeadPos;
        public PlayerUnit DeadUnit;

        public static PlayerUnitDeadEvent Create(Vector3Int deadPos, PlayerUnit unit)
        {
            PlayerUnitDeadEvent e = new PlayerUnitDeadEvent();
            e.DeadPos = deadPos;
            e.DeadUnit = unit;

            return e;
        }
    }
}