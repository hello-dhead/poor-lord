using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class BattleStageEndEvent : IEvent
    {
        public static BattleStageEndEvent Create()
        {
            BattleStageEndEvent e = new BattleStageEndEvent();
            return e;
        }
    }
}