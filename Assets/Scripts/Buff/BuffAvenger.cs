using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 아군이 죽으면 공격력 증가
    /// </summary>
    public class BuffAvenger : ContinuousBuff
    {
        public override string BuffName { get { return "Avenger"; } protected set { } }

        public override void Init(Unit bufftarget)
        {
            Target = bufftarget;
            GameManager.Instance.MessageSystem.Subscribe(typeof(PlayerUnitDeadEvent), this);
        }

        public override Buff Copy()
        {
            return new BuffAvenger();
        }

        public override void Dispose()
        {
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(PlayerUnitDeadEvent), this);
            Target = null;
        }

        public override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(PlayerUnitDeadEvent))
            {
                PlayerUnitDeadEvent deadEvent = e as PlayerUnitDeadEvent;

                PlayerUnit playertarget = Target as PlayerUnit;
                if (deadEvent.DeadUnit != playertarget)
                {
                    GameManager.Instance.EffectSystem.CreateEffect("BeamUpRed", Target.UnitPosition + new Vector3(0, 0.2f, -0.2f), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
                    Target.AddAdditionalDamage(10);
                }
            }
            return true;
        }
    }
}