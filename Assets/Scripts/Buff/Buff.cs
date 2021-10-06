using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public abstract class Buff
    {
        public abstract string BuffName { get; protected set; }
        public Unit Target { get; protected set; }
        public abstract void Init(Unit target);
        public abstract Buff Copy();
        public abstract void Dispose();
    }
}