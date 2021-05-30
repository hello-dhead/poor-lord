using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    public class DamageEvent : IEvent
    {
        public int Damage { get; private set; }
        public Unit Target { get; private set; }
        public Unit Publisher { get; private set; }

        public static DamageEvent Create(Unit publisher,Unit target, int damage)
        {
            DamageEvent e = new DamageEvent();
            e.Publisher = publisher;
            e.Target = target;
            e.Damage = damage;

            return e;
        }
    }
}