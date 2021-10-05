using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지속 버프, 걸린 이후부터 이벤트를 받으면서 유지되는 버프
/// </summary>
namespace poorlord
{
    public abstract class ContinuousBuff : Buff, IEventListener
    {
        // 지속 버프의 경우 이벤트를 받아서 처리
        public abstract bool OnEvent(IEvent e);
    }
}