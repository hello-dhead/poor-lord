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

        private List<RequestData> subscribeList = new List<RequestData>();

        private List<RequestData> unsubscribeList = new List<RequestData>();

        // 리스너들을 모아둔 딕셔너리
        private Dictionary<Type, List<IEventListener>> listenerDictionary = new Dictionary<Type, List<IEventListener>>();

        // 이벤트 발행
        public void Publish(IEvent e)
        {
            publishEventList.Add(e);
        }

        // 이벤트 구독 다음 프레임 시작할때 구독이 된다.
        public void Subscribe(Type key, IEventListener listener)
        {
            subscribeList.Add(new RequestData(key, listener));
        }

        // 프레임 시작 시 구독 처리
        private void SubscribeOnFrame(Type key, IEventListener listener)
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
            unsubscribeList.Add(new RequestData(key, listener));
        }

        // 프레임 시작 시 구독 처리
        private void UnsubscribeOnFrame(Type key, IEventListener listener)
        {
            List<IEventListener> listenerList;
            if (listenerDictionary.TryGetValue(key, out listenerList) == true)
                listenerList.Remove(listener);
        }

        // 매프레임마다 전 프레임때 도착한 리스트를 발행한다.
        public void UpdateFrame(float dt)
        {
            for (int i = 0; i < subscribeList.Count; i++)
            {
                SubscribeOnFrame(subscribeList[i].key, subscribeList[i].listener);
            }
            subscribeList.Clear();

            for (int i = 0; i < unsubscribeList.Count; i++)
            {
                UnsubscribeOnFrame(unsubscribeList[i].key, unsubscribeList[i].listener);
            }
            unsubscribeList.Clear();

            for (int i = 0; i < publishEventList.Count; i++)
            {
                List<IEventListener> listenerList;
                if (listenerDictionary.TryGetValue(publishEventList[i].GetType(), out listenerList) == false)
                    continue;

                for (int j = 0; j < listenerList.Count; j++)
                {
                    listenerList[j].OnEvent(publishEventList[i]);
                }
            }
            publishEventList.Clear();
        }
    }

    public class RequestData
    {
        public Type key;
        public IEventListener listener;

        public RequestData(Type key, IEventListener listener)
        {
            this.key = key;
            this.listener = listener;
        }
    }
}
