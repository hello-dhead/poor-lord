using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격력을 배로 증가 시키는 버프
/// </summary>
namespace poorlord
{
    public class BuffIncreasedHPTwice : ImmediatelyBuff
    {
        public override string BuffName { get { return "HP X 2"; } protected set { } }

        public override void Init(Unit target)
        {
            target.SetHP(target.HP + target.MaxHP);
            target.SetMaxHP(target.MaxHP * 2);
        }

        public override Buff Copy()
        {
            return new BuffIncreasedHPTwice();
        }

        public override void Dispose()
        {
        }
    }
}
