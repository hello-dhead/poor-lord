using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격력을 배로 증가 시키는 버프
/// </summary>
namespace poorlord
{
    public class BuffIncreaseDamageTwice : ImmediatelyBuff
    {
        public override string BuffName { get { return "Damage X 2"; } protected set { } }

        public override void Init(Unit target)
        {
            target.AddDamageMultiplier(2);
        }

        public override Buff Copy()
        {
            return new BuffIncreaseDamageTwice();
        }

        public override void Dispose()
        {
        }
    }
}
