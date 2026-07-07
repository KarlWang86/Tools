using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple event bus.
///
/// Usage example:
/// 1) Add listener
///    EventManager.instance.AddListener(EventName.GameDataInit, OnGameDataInit);
/// 2) Raise event
///    EventManager.instance.Raise(EventName.GameDataInit, "ok", 1);
/// 3) Handle payload
///    private void OnGameDataInit(EventParams e) {
///        object[] args = e.Params;
///    }
/// 4) Remove listener
///    EventManager.instance.RemoveListener(EventName.GameDataInit, OnGameDataInit);
/// </summary>
public class EventManager {
    static EventManager _instance = null;
    public static EventManager instance {
        get {
            if (_instance == null) {
                _instance = new EventManager();
            }

            return _instance;
        }
    }

    public delegate void EventDelegate(EventParams eventParams);

    private Dictionary<EventName, EventDelegate> delegates = new Dictionary<EventName, EventDelegate>();

    public void AddListener(EventName eventName, EventDelegate del) {
        if (delegates.ContainsKey(eventName)) {
            delegates[eventName] += del;
        } else {
            delegates[eventName] = del;
        }
    }

    public void RemoveListener(EventName eventName, EventDelegate del) {
        if (delegates.ContainsKey(eventName)) {
            delegates[eventName] -= del;

            if (delegates[eventName] == null) {
                delegates.Remove(eventName);
            }
        }
    }

    public void Raise(EventName eventName, params object[] eventParams) {
        //invoke all listeners
        foreach (var pair in delegates) {
            if (pair.Key.Equals(eventName)) {
                pair.Value.Invoke(new EventParams(eventParams));
            }
        }
    }
}

public abstract class BaseEvent { }

public class EventParams {
    public object[] Params { get; private set; }

    public EventParams(params object[] parameters) {
        Params = parameters;
    }
}

public enum EventName {
    ChangeRail,
    GameDataInit,
}
