using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    public abstract class PlayerTile : MonoBehaviour, IEventListener
    {
        public CardValue Value { get; protected set; }
        public string Name { get; protected set; }

        // 체크에 필요한 타일
        public abstract List<Vector3Int> CheckTile { get; }

        public abstract bool OnEvent(IEvent e);
        public abstract void Dispose();
        public abstract void Init(Vector3Int pos);

        public abstract CardValue GetValue();
        public abstract string GetName();
    }
}