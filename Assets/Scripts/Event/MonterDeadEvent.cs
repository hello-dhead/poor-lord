using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class MonsterDeadEvent : IEvent
    {
        public Vector3Int DeadPos;
        public MonsterUnit DeadUnit;

        public static MonsterDeadEvent Create(MonsterUnit unit, Vector3Int deadPos)
        {
            MonsterDeadEvent e = new MonsterDeadEvent();
            e.DeadUnit = unit;
            e.DeadPos = deadPos;

            return e;
        }
    }
}