using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace poorlord
{
    /// <summary>
    /// 몬스터가 자기가 있는 타일에 들어 올 때마다 체력을 10퍼센트 회복 maxHP를 넘을 수는 없음
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
                Debug.Log(TileManager.Instance.GetContainPlayerUnit(tileEnter.EnterTilePos.x, tileEnter.EnterTilePos.z));
                if (TileManager.Instance.GetContainPlayerUnit(tileEnter.EnterTilePos.x, tileEnter.EnterTilePos.z) == playertarget)
                {
                    Target.AddAdditionalDamage(10);
                }
            }
            return true;
        }
    }
}