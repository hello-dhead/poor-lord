﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격력을 배로 증가 시키는 버프
/// </summary>
namespace poorlord
{
    public class BuffIncreaseDamageTwice : ImmediatelyBuff
    {
        public override void Init(Unit target)
        {
            BuffName = "Damage X 2";
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
