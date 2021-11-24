using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 몬스터가 자기가 있는 타일에 들어 올 때마다 체력을 10퍼센트 회복 maxHP를 넘을 수는 없음
    /// </summary>
    public class BuffGateKeeper : ContinuousBuff
    {
        public override string BuffName { get { return "GateKeeper"; } protected set { } }

        public override void Init(Unit bufftarget)
        {
            Target = bufftarget;
            GameManager.Instance.MessageSystem.Subscribe(typeof(TileEnterEvent), this);
        }

        public override Buff Copy()
        {
            return new BuffGateKeeper();
        }

        public override void Dispose()
        {
            GameManager.Instance.MessageSystem.Unsubscribe(typeof(TileEnterEvent), this);
            Target = null;
        }

        public override bool OnEvent(IEvent e)
        {
            if (e.GetType() == typeof(TileEnterEvent))
            {
                TileEnterEvent tileEnter = e as TileEnterEvent;

                PlayerUnit playertarget = Target as PlayerUnit;
                if (TileManager.Instance.GetContainPlayerUnit(tileEnter.EnterTilePos.x, tileEnter.EnterTilePos.z) == playertarget)
                {
                    GameManager.Instance.EffectSystem.CreateEffect("HealExplosionGreen", Target.UnitPosition + new Vector3(0, 0.4f, -0.1f), new Vector3(0.5f, 0.5f, 0.5f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
                    int hp = playertarget.HP;
                    if (hp + (int)(hp * 0.1) < playertarget.MaxHP)
                    {
                        playertarget.SetHP(hp + (int)(hp * 0.1));
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