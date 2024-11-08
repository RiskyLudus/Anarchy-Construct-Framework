using System;
using System.Collections.Generic;
using AnarchyConstructFramework.Core.Data;
using UnityEngine;
using UnityEngine.Events;

namespace AnarchyConstructFramework.Core.Common
{
    public class AnarchyBehaviour : MonoBehaviour
    {
        private Dictionary<UnityEventBase, Delegate> eventListeners = new Dictionary<UnityEventBase, Delegate>();

        protected void SetLink(EventLink eventLink)
        {
            eventListeners[eventLink.UnityEvent] = new UnityAction(eventLink.Action);
            eventLink.UnityEvent.AddListener((UnityAction)eventListeners[eventLink.UnityEvent]);
        }

        protected void SetLinks(params EventLink[] eventLinks)
        {
            foreach (var eventLink in eventLinks)
            {
                SetLink(eventLink);
            }
        }

        protected void SetLink<T>(EventLink<T> eventLink)
        {
            eventListeners[eventLink.UnityEvent] = new UnityAction<T>(eventLink.Action);
            eventLink.UnityEvent.AddListener((UnityAction<T>)eventListeners[eventLink.UnityEvent]);
        }

        protected void SetLinks<T>(params EventLink<T>[] eventLinks)
        {
            foreach (var eventLink in eventLinks)
            {
                SetLink(eventLink);
            }
        }

        protected void SetLink<T1, T2>(EventLink<T1, T2> eventLink)
        {
            eventListeners[eventLink.UnityEvent] = new UnityAction<T1, T2>(eventLink.Action);
            eventLink.UnityEvent.AddListener((UnityAction<T1, T2>)eventListeners[eventLink.UnityEvent]);
        }

        protected void SetLinks<T1, T2>(params EventLink<T1, T2>[] eventLinks)
        {
            foreach (var eventLink in eventLinks)
            {
                SetLink(eventLink);
            }
        }

        protected void SetLink<T1, T2, T3>(EventLink<T1, T2, T3> eventLink)
        {
            eventListeners[eventLink.UnityEvent] = new UnityAction<T1, T2, T3>(eventLink.Action);
            eventLink.UnityEvent.AddListener((UnityAction<T1, T2, T3>)eventListeners[eventLink.UnityEvent]);
        }

        protected void SetLinks<T1, T2, T3>(params EventLink<T1, T2, T3>[] eventLinks)
        {
            foreach (var eventLink in eventLinks)
            {
                SetLink(eventLink);
            }
        }

        protected void SetLink<T1, T2, T3, T4>(EventLink<T1, T2, T3, T4> eventLink)
        {
            eventListeners[eventLink.UnityEvent] = new UnityAction<T1, T2, T3, T4>(eventLink.Action);
            eventLink.UnityEvent.AddListener((UnityAction<T1, T2, T3, T4>)eventListeners[eventLink.UnityEvent]);
        }

        protected void SetLinks<T1, T2, T3, T4>(params EventLink<T1, T2, T3, T4>[] eventLinks)
        {
            foreach (var eventLink in eventLinks)
            {
                SetLink(eventLink);
            }
        }

        private void OnDisable()
        {
            foreach (var listener in eventListeners)
            {
                if (listener.Key != null && listener.Value != null)
                {
                    // Remove only the specific listener we added
                    switch (listener.Value)
                    {
                        case UnityAction unityAction:
                            (listener.Key as UnityEvent)?.RemoveListener(unityAction);
                            break;
                        case UnityAction<object> unityAction1:
                            (listener.Key as UnityEvent<object>)?.RemoveListener(unityAction1);
                            break;
                        case UnityAction<object, object> unityAction2:
                            (listener.Key as UnityEvent<object, object>)?.RemoveListener(unityAction2);
                            break;
                        case UnityAction<object, object, object> unityAction3:
                            (listener.Key as UnityEvent<object, object, object>)?.RemoveListener(unityAction3);
                            break;
                        case UnityAction<object, object, object, object> unityAction4:
                            (listener.Key as UnityEvent<object, object, object, object>)?.RemoveListener(unityAction4);
                            break;
                    }
                }
            }

            eventListeners.Clear();
        }
    }
}
