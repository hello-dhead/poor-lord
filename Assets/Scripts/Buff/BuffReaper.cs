using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 캐릭터가 공격하면 일정확률로 즉사
    /// </summary>
    public class BuffReaper : ContinuousBuff
    {
        public override string BuffName { get { return "Reaper"; } protected set {} }

        private readonly int instantDeathPer = 5;

        public override void Init(Unit bufftarget)
        {
            Target = bufftarget;
            GameManager.Instance.MessageSystem.Subscribe(typeof(DamageEvent), this);
        }

        public override Buff Copy()
        {
            return new BuffReaper();
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
                    if (UnityEngine.Random.Range(1, 101) <= instantDeathPer && damageEvent.Target.HP > 0)
                    {
                        GameManager.Instance.EffectSystem.CreateEffect("DeathSuperEvil", damageEvent.Target.transform.position + new Vector3(0, 0.1f, -0.1f), 
                            new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(-90, 0, 0)), 2);

                        GameManager.Instance.MessageSystem.Publish(DamageEvent.Create(playertarget, damageEvent.Target, damageEvent.Target.MaxHP));
                    }
                }
            }
            return true;
        }
    }
}