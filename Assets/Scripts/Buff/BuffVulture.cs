using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 적이 죽을때마다 공격력 증가
    /// </summary>
    public class BuffVulture : ContinuousBuff
    {
        public override string BuffName { get { return "Vulture"; } protected set {} }

        public override void Init(Unit bufftarget)
        {
            Target = bufftarget;
            GameManager.Instance.MessageSystem.Subscribe(typeof(MonsterDeadEvent), this);
        }

        public override Buff Copy()
        {
            return new BuffVulture();
        }

        public override void Dispose()
        {
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(MonsterDeadEvent), this);
            Target = null;
        }

        public override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(MonsterDeadEvent))
            {
                GameManager.Instance.EffectSystem.CreateEffect("HealExplosionGreen", Target.UnitPosition + new Vector3(0, 0.4f, -0.1f), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
                int hp = Target.HP;
                if (hp + (int)(hp * 0.1) < Target.MaxHP)
                {
                    Target.SetHP(hp + (int)(hp * 0.1));
                }
                else
                {
                    Target.SetHP(Target.MaxHP);
                }
            }
            return true;
        }
    }
}