using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 주변에 아군이 소환되면 체력 증가
    /// </summary>
    public class BuffGuardian : ContinuousBuff
    {
        public override string BuffName { get { return "Guardian"; } protected set {} }

        public override void Init(Unit bufftarget)
        {
            Target = bufftarget;
            GameManager.Instance.MessageSystem.Subscribe(typeof(PlayerUnitSummonEvent), this);
        }

        public override Buff Copy()
        {
            return new BuffGuardian();
        }

        public override void Dispose()
        {
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(PlayerUnitSummonEvent), this);
            Target = null;
        }

        public override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(PlayerUnitSummonEvent))
            {
                PlayerUnitSummonEvent summonEvent = e as PlayerUnitSummonEvent;
                PlayerUnit playertarget = Target as PlayerUnit;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        if (summonEvent.SummonTilePos == Target.UnitPosition + new Vector3(i, 0, j))
                        {
                            GameManager.Instance.EffectSystem.CreateEffect("BeamUpGreen", Target.UnitPosition + new Vector3(0, 0.2f, -0.2f), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
                            int hp = playertarget.MaxHP;
                            playertarget.SetMaxHP(hp + (int)(hp * 0.05));
                            playertarget.SetHP(playertarget.HP + (int)(hp * 0.05));
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}