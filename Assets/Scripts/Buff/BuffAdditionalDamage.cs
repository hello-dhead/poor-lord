using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격력을 배로 증가 시키는 버프
/// </summary>
namespace poorlord
{
    public class BuffAdditionalDamage : ImmediatelyBuff
    {
        public override void Init(Unit target)
        {
            BuffName = "Damage ++";
            target.AddAdditionalDamage(100);
        }

        public override Buff Copy()
        {
            return new BuffAdditionalDamage();
        }

        public override void Dispose()
        {
        }
    }
}
