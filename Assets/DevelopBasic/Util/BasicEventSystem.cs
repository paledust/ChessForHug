using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A basic C# Event System
public static class EventHandler
{
    public static event Action<Piece, PLAYER_SIDE> E_OnCapturePiece;
    public static void Call_OnCapturePiece(Piece piece, PLAYER_SIDE side){E_OnCapturePiece?.Invoke(piece, side);}
    public static event Action<Piece, PLAYER_SIDE> E_OnMovePieceOnly;
    public static void Call_OnMovePieceOnly(Piece piece, PLAYER_SIDE side){E_OnMovePieceOnly?.Invoke(piece, side);}
    public static event Action<Vector3> E_OnSelectGrid;
    public static void Call_OnSelectGrid(Vector3 pos){E_OnSelectGrid?.Invoke(pos);}
    public static event Action<GameObject> E_OnGrabPiece;
    public static void Call_OnGrabPiece(GameObject piece){E_OnGrabPiece?.Invoke(piece);}
    public static event Action<GameObject> E_OnPutDownPiece;
    public static void Call_OnPutDownPiece(GameObject piece){E_OnPutDownPiece?.Invoke(piece);}
}

//A More Strict Event System
namespace SimpleEventSystem{
    public abstract class Event{
        public delegate void Handler(Event e);
    }
    public class E_OnTestEvent:Event{
        public float value;
        public E_OnTestEvent(float data){value = data;}
    }

    public class EventManager{
        private static  EventManager instance;
        public static EventManager Instance{
            get{
                if(instance == null) instance = new EventManager();
                return instance;
            }
        }

        private Dictionary<Type, Event.Handler> RegisteredHandlers = new Dictionary<Type, Event.Handler>();
        public void Register<T>(Event.Handler handler) where T: Event{
            Type type = typeof(T);

            if(RegisteredHandlers.ContainsKey(type)){
                RegisteredHandlers[type] += handler;
            }
            else{
                RegisteredHandlers[type] = handler;
            }
        }
        public void UnRegister<T>(Event.Handler handler) where T: Event{
            Type type = typeof(T);
            Event.Handler handlers;

            if(RegisteredHandlers.TryGetValue(type, out handlers)){
                handlers -= handler;
                if(handlers == null){
                    RegisteredHandlers.Remove(type);
                }
                else{
                    RegisteredHandlers[type] = handlers;
                }
            }
        }
        public void FireEvent<T>(T e) where T:Event {
            Type type = e.GetType();
            Event.Handler handlers;

            if(RegisteredHandlers.TryGetValue(type, out handlers)){
                handlers?.Invoke(e);
            }
        }
        public void ClearList(){RegisteredHandlers.Clear();}
    }
}
