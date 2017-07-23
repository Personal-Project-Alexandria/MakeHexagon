using System;
using System.Collections.Generic;
using UnityEngine;

internal static class Messenger
{
    public class BroadcastException : Exception
    {
        public BroadcastException(string msg) : base(msg)
        {
        }
    }

    public class ListenerException : Exception
    {
        public ListenerException(string msg) : base(msg)
        {
        }
    }

    private static MessengerHelper messengerHelper = new GameObject("MessengerHelper").AddComponent<MessengerHelper>();

    public static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    public static List<string> permanentMessages = new List<string>();

    public static void MarkAsPermanent(string eventType)
    {
        Messenger.permanentMessages.Add(eventType);
    }

    public static void Cleanup()
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, Delegate> current in Messenger.eventTable)
        {
            bool flag = false;
            foreach (string current2 in Messenger.permanentMessages)
            {
                if (current.Key == current2)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                list.Add(current.Key);
            }
        }
        foreach (string current3 in list)
        {
            Messenger.eventTable.Remove(current3);
        }
    }

    public static void PrintEventTable()
    {
        Debug.Log("\t\t\t=== MESSENGER PrintEventTable ===");
        foreach (KeyValuePair<string, Delegate> current in Messenger.eventTable)
        {
            Debug.Log(string.Concat(new object[]
            {
                "\t\t\t",
                current.Key,
                "\t\t",
                current.Value
            }));
        }
        Debug.Log("\n");
    }

    public static void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
        if (!Messenger.eventTable.ContainsKey(eventType))
        {
            Messenger.eventTable.Add(eventType, null);
        }
        Delegate @delegate = Messenger.eventTable[eventType];
        if (@delegate != null && @delegate.GetType() != listenerBeingAdded.GetType())
        {
            throw new Messenger.ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, @delegate.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    public static void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
    {
        if (!Messenger.eventTable.ContainsKey(eventType))
        {
            throw new Messenger.ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
        }
        Delegate @delegate = Messenger.eventTable[eventType];
        if (@delegate == null)
        {
            throw new Messenger.ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
        }
        if (@delegate.GetType() != listenerBeingRemoved.GetType())
        {
            throw new Messenger.ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, @delegate.GetType().Name, listenerBeingRemoved.GetType().Name));
        }
    }

    public static void OnListenerRemoved(string eventType)
    {
        if (Messenger.eventTable[eventType] == null)
        {
            Messenger.eventTable.Remove(eventType);
        }
    }

    public static void OnBroadcasting(string eventType)
    {
    }

    public static Messenger.BroadcastException CreateBroadcastSignatureException(string eventType)
    {
        return new Messenger.BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
    }

    public static void AddListener(string eventType, Callback handler)
    {
        Messenger.OnListenerAdding(eventType, handler);
        Messenger.eventTable[eventType] = (Callback)Delegate.Combine((Callback)Messenger.eventTable[eventType], handler);
    }

    public static void AddListener<T>(string eventType, Callback<T> handler)
    {
        Messenger.OnListenerAdding(eventType, handler);
        Messenger.eventTable[eventType] = (Callback<T>)Delegate.Combine((Callback<T>)Messenger.eventTable[eventType], handler);
    }

    public static void AddListener<T, U>(string eventType, Callback<T, U> handler)
    {
        Messenger.OnListenerAdding(eventType, handler);
        Messenger.eventTable[eventType] = (Callback<T, U>)Delegate.Combine((Callback<T, U>)Messenger.eventTable[eventType], handler);
    }

    public static void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        Messenger.OnListenerAdding(eventType, handler);
        Messenger.eventTable[eventType] = (Callback<T, U, V>)Delegate.Combine((Callback<T, U, V>)Messenger.eventTable[eventType], handler);
    }

    public static void RemoveListener(string eventType, Callback handler)
    {
        Messenger.OnListenerRemoving(eventType, handler);
        Messenger.eventTable[eventType] = (Callback)Delegate.Remove((Callback)Messenger.eventTable[eventType], handler);
        Messenger.OnListenerRemoved(eventType);
    }

    public static void RemoveListener<T>(string eventType, Callback<T> handler)
    {
        Messenger.OnListenerRemoving(eventType, handler);
        Messenger.eventTable[eventType] = (Callback<T>)Delegate.Remove((Callback<T>)Messenger.eventTable[eventType], handler);
        Messenger.OnListenerRemoved(eventType);
    }

    public static void RemoveListener<T, U>(string eventType, Callback<T, U> handler)
    {
        Messenger.OnListenerRemoving(eventType, handler);
        Messenger.eventTable[eventType] = (Callback<T, U>)Delegate.Remove((Callback<T, U>)Messenger.eventTable[eventType], handler);
        Messenger.OnListenerRemoved(eventType);
    }

    public static void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        Messenger.OnListenerRemoving(eventType, handler);
        Messenger.eventTable[eventType] = (Callback<T, U, V>)Delegate.Remove((Callback<T, U, V>)Messenger.eventTable[eventType], handler);
        Messenger.OnListenerRemoved(eventType);
    }

    public static void Broadcast(string eventType)
    {
        Messenger.OnBroadcasting(eventType);
        Delegate @delegate;
        if (Messenger.eventTable.TryGetValue(eventType, out @delegate))
        {
            Callback callback = @delegate as Callback;
            if (callback == null)
            {
                throw Messenger.CreateBroadcastSignatureException(eventType);
            }
            callback();
        }
    }

    public static void Broadcast<T>(string eventType, T arg1)
    {
        Messenger.OnBroadcasting(eventType);
        Delegate @delegate;
        if (Messenger.eventTable.TryGetValue(eventType, out @delegate))
        {
            Callback<T> callback = @delegate as Callback<T>;
            if (callback == null)
            {
                throw Messenger.CreateBroadcastSignatureException(eventType);
            }
            callback(arg1);
        }
    }

    public static void Broadcast<T, U>(string eventType, T arg1, U arg2)
    {
        Messenger.OnBroadcasting(eventType);
        Delegate @delegate;
        if (Messenger.eventTable.TryGetValue(eventType, out @delegate))
        {
            Callback<T, U> callback = @delegate as Callback<T, U>;
            if (callback == null)
            {
                throw Messenger.CreateBroadcastSignatureException(eventType);
            }
            callback(arg1, arg2);
        }
    }

    public static void Broadcast<T, U, V>(string eventType, T arg1, U arg2, V arg3)
    {
        Messenger.OnBroadcasting(eventType);
        Delegate @delegate;
        if (Messenger.eventTable.TryGetValue(eventType, out @delegate))
        {
            Callback<T, U, V> callback = @delegate as Callback<T, U, V>;
            if (callback == null)
            {
                throw Messenger.CreateBroadcastSignatureException(eventType);
            }
            callback(arg1, arg2, arg3);
        }
    }
}
public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);