using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격력을 배로 증가 시키는 버프
/// </summary>
namespace poorlord
{
    public class BuffIncreasedamageTwice : ImmediatelyBuff
    {
        public override void Init(Unit target)
        {
            BuffName = "Attack Twice";
            target.SetDamage(target.Damage * 2);
        }

        public override Buff Copy()
        {
            return new BuffIncreasedamageTwice();
        }

        public override void Dispose()
        {

        }
    }
}
