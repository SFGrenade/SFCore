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
            string path = Fsm.GameObjectName;
            Transform t = Fsm.GameObject.transform.parent;
            while (t != null)
            {
                path = $"{t.gameObject.name}/{path}";
                t = t.parent;
            }
            Debug.Log($"[{path}]:[{Fsm.Name}]:[{State.Name}] - {message}");
            Logger.Log($"[{path}]:[{Fsm.Name}]:[{State.Name}] - {message}");
        }
    }
}