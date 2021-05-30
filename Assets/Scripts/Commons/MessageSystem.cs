using System;
using System.Collections.Generic;
using UnityEngine;

namespace poorlord
{
    /// <summary>
    /// 이벤트와 같은 메세지를 주고 받기 위한 중개자
    /// </summary>

    public class MessageSystem : IUpdatable
    {
        // 이벤트 리스트 다음 프레임이 시작되기 전에 모든 이벤트를 보내고 clear한다.
        private List<IEvent> publishEventList = new List<IEvent>();

        // 리스너들을 모아둔 딕셔너리
        private Dictionary<Type, List<IEventListener>> listenerDictionary = new Dictionary<Type, List<IEventListener>>();

        // 이벤트 발행
        public void Publish(IEvent e)
        {
            publishEventList.Add(e);
        }

        // 이벤트 구독
        public void Subscribe(Type key, IEventListener listener)
        {
            if (listenerDictionary.ContainsKey(key) == false)
            {
                List<IEventListener> newEventListener = new List<IEventListener>();
                listenerDictionary.Add(key, newEventListener);
            }
            List<IEventListener> listenerList;
            listenerDictionary.TryGetValue(key, out listenerList);
            listenerList.Add(listener);
        }

        // 이벤트 구독 해지
        public void Unsubscribe(Type key, IEventListener listener)
        {
            List<IEventListener> listenerList;
            if (listenerDictionary.TryGetValue(key, out listenerList) == true)
                listenerList.Remove(listener);
        }

        // 매프레임마다 전 프레임때 도착한 리스트를 발행한다.
        public void UpdateFrame(float dt)
        {
            foreach (var publishEvent in publishEventList)
            {
                List<IEventListener> listenerList;
                if (listenerDictionary.TryGetValue(publishEvent.GetType(), out listenerList) == false)
                    continue;

                foreach (var listener in listenerList)
                    listener.OnEvent(publishEvent);
            }
            publishEventList.Clear();
        }
    }
}
