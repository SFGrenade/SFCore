using HutongGames.PlayMaker;
using UnityEngine;
using Logger = Modding.Logger;

namespace SFCore.Utils
{
    public class LogAction : FsmStateAction
    {
        public FsmString text;

        public override void Reset()
        {
            text = string.Empty;

            base.Reset();
        }

        public override void OnEnter()
        {
            if (text.Value != null) Log($"FSM Log: \"{text.Value}\"");
            Finish();
        }

        private new void Log(string message)
        {
            Debug.Log($"[SFCore]:[Util]:[LogAction] - {message}");
            Logger.Log($"[SFCore]:[Util]:[LogAction] - {message}");
        }
    }

    public class StatusLog : FsmStateAction
    {
        public FsmString text;

        public override void Reset()
        {
            text = string.Empty;

            base.Reset();
        }

        public override void OnEnter()
        {
            if (text.Value != null) Log($"{text.Value}");
            Finish();
        }

        private new void Log(string message)
        {
            
            Debug.Log($"[{Fsm.GameObjectName}]:[{Fsm.Name}]:[{State.Name}] - {message}");
            Logger.Log($"[{Fsm.GameObjectName}]:[{Fsm.Name}]:[{State.Name}] - {message}");
        }
    }
}