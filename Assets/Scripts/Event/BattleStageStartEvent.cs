using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class BattleStageStartEvent : IEvent
    {
        public int Stage;

        public static BattleStageStartEvent Create(int stage)
        {
            BattleStageStartEvent e = new BattleStageStartEvent();
            e.Stage = stage;

            return e;
        }
    }
}