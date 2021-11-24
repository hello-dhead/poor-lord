using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 캐릭터가 공격하면 체력 회복
    /// </summary>
    public class BuffVampire : ContinuousBuff
    {
        public override string BuffName { get { return "Vampire"; } protected set {} }

        public override void Init(Unit bufftarget)
        {
            Target = bufftarget;
            GameManager.Instance.MessageSystem.Subscribe(typeof(DamageEvent), this);
        }

        public override Buff Copy()
        {
            return new BuffVampire();
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
                    GameManager.Instance.EffectSystem.CreateEffect("HealExplosionGreen", Target.UnitPosition + new Vector3(0, 0.4f, -0.1f), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
                    int hp = playertarget.HP;
                    if (hp + (int)(damageEvent.Damage * 0.2) < playertarget.MaxHP)
                    {
                        playertarget.SetHP(hp + (int)(damageEvent.Damage * 0.2));
                    }
                    else
                    {
                        playertarget.SetHP(playertarget.MaxHP);
                    }
                }
            }
            return true;
        }
    }
}