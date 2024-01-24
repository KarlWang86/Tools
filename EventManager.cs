using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add listener
// EventManager.instance.AddListener(EventName.ExampleEvent, OnExampleEvent);

// trigger event
// object[] eventParams = { 42, "Hello, World!" };
// EventManager.instance.Raise(EventName.ExampleEvent, eventParams);

// deal event
// private void OnExampleEvent(EventParams eventParams)
//    {
//        object[] parameters = eventParams.Params;
//        Debug.Log($"ExampleEvent triggered with parameters: {parameters[0]}, {parameters[1]}");
//    }

// remove listener
// EventManager.instance.RemoveListener(EventName.ExampleEvent, OnExampleEvent);


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
        EventDelegate del;
        if (delegates.TryGetValue(eventName, out del)) {
            EventParams paramsObject = new EventParams(eventParams);
            del.Invoke(paramsObject);
        }

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
}
