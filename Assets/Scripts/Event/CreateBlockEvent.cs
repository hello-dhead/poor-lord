using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class CreateBlockEvent : IEvent
    {
        public List<Vector3Int> CheckTileList;

        public static CreateBlockEvent Create(List<Vector3Int> checkTileListt)
        {
            CreateBlockEvent e = new CreateBlockEvent();
            e.CheckTileList = checkTileListt;

            return e;
        }
    }
}