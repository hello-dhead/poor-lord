using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 캐릭터가 공격하면 공격력 증가
    /// </summary>
    public class BuffWarmonger : ContinuousBuff
    {
        public override string BuffName { get { return "Warmonger"; } protected set {} }

        public override void Init(Unit bufftarget)
        {
            Target = bufftarget;
            GameManager.Instance.MessageSystem.Subscribe(typeof(DamageEvent), this);
        }

        public override Buff Copy()
        {
            return new BuffWarmonger();
        }

        public override void Dispose()
        {
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(DamageEvent), this);
            Target = null;
        }

        public override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(DamageEvent))
            {
                DamageEvent damageEvent = e as DamageEvent;

                PlayerUnit playertarget = Target as PlayerUnit;
                if (damageEvent.Publisher == playertarget)
                {
                    GameManager.Instance.EffectSystem.CreateEffect("BeamUpRed", Target.UnitPosition + new Vector3(0, 0.2f, -0.2f), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
                    Target.AddAdditionalDamage(2);
                }
            }
            return true;
        }
    }
}