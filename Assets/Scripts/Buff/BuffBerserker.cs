using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 몬스터가 자기가 있는 타일에 들어 올 때마다 공격력이 10 증가
    /// </summary>
    public class BuffBerserker : ContinuousBuff
    {
        public override string BuffName { get { return "Berserker"; } protected set { } }

        public override void Init(Unit bufftarget)
        {
            Target = bufftarget;
            GameManager.Instance.MessageSystem.Subscribe(typeof(TileEnterEvent), this);
        }

        public override Buff Copy()
        {
            return new BuffBerserker();
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
                    GameManager.Instance.EffectSystem.CreateEffect("BeamUpRed", Target.UnitPosition + new Vector3(0, 0.2f, -0.2f), 
                        new Vector3(0.3f, 0.3f, 0.3f), Quaternion.Euler(new Vector3(-90, 0, 0)), 1);
                    Target.AddAdditionalDamage(10);
                }
            }
            return true;
        }
    }
}