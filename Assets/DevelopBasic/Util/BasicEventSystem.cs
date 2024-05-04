using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A basic C# Event System
public static class EventHandler
{
#region Game Basic
    public static event Action E_BeforeUnloadScene;
    public static void Call_BeforeUnloadScene()=>E_BeforeUnloadScene?.Invoke();
    public static event Action E_AfterLoadScene;
    public static void Call_AfterLoadScene()=>E_AfterLoadScene?.Invoke();
#endregion

#region UI Event
    public static event Action<string, float, Transform> E_UI_ShowData;
    public static event Action<int, float, Transform> E_UI_ShowNum;
    public static void Call_UI_ShowData(string data, float height, Transform root)=>E_UI_ShowData?.Invoke(data, height, root);
    public static void Call_UI_ShowData(int data, float height, Transform root)=>E_UI_ShowNum?.Invoke(data, height, root);
    public static event Action<Transform> E_UI_HideData;
    public static void Call_UI_HideData(Transform root)=>E_UI_HideData?.Invoke(root);
    public static event Action<Transform> E_UI_CleanDisplayer;
    public static void Call_UI_CleanDisplayer(Transform root)=>E_UI_CleanDisplayer?.Invoke(root);
    public static event Action<string> E_UI_ShowDescrip;
    public static void Call_UI_ShowDescrip(string content)=>E_UI_ShowDescrip?.Invoke(content);

    public static event Action<float> E_UI_StepYear;
    public static void Call_UI_StepYear(float step)=>E_UI_StepYear?.Invoke(step);
#endregion

#region Chess Event
    public static event Action<Piece, PLAYER_SIDE> E_OnCapturePiece;
    public static void Call_OnCapturePiece(Piece piece, PLAYER_SIDE side){E_OnCapturePiece?.Invoke(piece, side);}
    public static event Action<Piece, PLAYER_SIDE> E_OnMovePieceOnly;
    public static void Call_OnMovePieceOnly(Piece piece, PLAYER_SIDE side){E_OnMovePieceOnly?.Invoke(piece, side);}
    public static event Action<GameObject> E_OnGrabPiece;
    public static void Call_OnGrabPiece(GameObject piece){E_OnGrabPiece?.Invoke(piece);}
    public static event Action<GameObject, GameObject, Vector2Int> E_OnPiecesHug;
    public static void Call_OnPiecesHug(GameObject huggerPiece, GameObject huggeePiece, Vector2Int gridPoint){E_OnPiecesHug?.Invoke(huggerPiece, huggeePiece, gridPoint);}
    public static event Action E_OnBackToChessGame;
    public static void Call_OnBackToChessGame(){E_OnBackToChessGame?.Invoke();}
#endregion
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
