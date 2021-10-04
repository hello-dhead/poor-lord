using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class GameStartEvent : IEvent
    {
        public static GameStartEvent Create()
        {
            GameStartEvent e = new GameStartEvent();
            return e;
        }
    }
}