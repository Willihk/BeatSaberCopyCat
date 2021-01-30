using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace BeatGame.Events
{
    public abstract class BaseGameEventListener<T, GE, UER> : MonoBehaviour
         where GE : GameEvent<T>
         where UER : UnityEvent<T>
    {
        [SerializeField]
        protected GE _GameEvent;

        [SerializeField]
        protected UER _UnityEventResponse;

        protected void OnEnable()
        {
            if (_GameEvent != null)
            {
                _GameEvent.EventListeners += TriggerResponses;
            }
        }

        protected void OnDisable()
        {
            if (_GameEvent != null)
            {
                _GameEvent.EventListeners -= TriggerResponses;
            }
        }

        [ContextMenu("Trigger Responses")]
        public void TriggerResponses(T val)
        {
            //No need to nullcheck here, since UnityEvent already does that
            _UnityEventResponse.Invoke(val);
        }
    }
}