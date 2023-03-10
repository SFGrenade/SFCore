using HutongGames.PlayMaker;
using UnityEngine;
using Logger = Modding.Logger;

namespace SFCore.Utils
{
    /// <summary>
    /// FsmStateAction that logs the value of an FsmString.
    /// </summary>
    public class LogAction : FsmStateAction
    {
        /// <summary>
        /// The text to log.
        /// </summary>
        public FsmString text;

        /// <summary>
        /// Resets the action.
        /// </summary>
        public override void Reset()
        {
            text = string.Empty;

            base.Reset();
        }

        /// <summary>
        /// Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            if (text.Value != null) Log($"FSM Log: \"{text.Value}\"");
            Finish();
        }

        private new void Log(string message)
        {
            Debug.Log($"[SFCore]:[Util]:[LogAction] - {message}");
            Logger.LogDebug($"[SFCore]:[Util]:[LogAction] - {message}");
        }
    }

    /// <summary>
    /// FsmStateAction that logs the value of an FsmString and gives context of which GameObject with which Fsm in which State produces the log.
    /// </summary>
    public class StatusLog : FsmStateAction
    {
        /// <summary>
        /// Resets the action.
        /// </summary>
        public FsmString text;

        /// <summary>
        /// Resets the action.
        /// </summary>
        public override void Reset()
        {
            text = string.Empty;

            base.Reset();
        }

        /// <summary>
        /// Called when the action is being processed.
        /// </summary>
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
            Logger.LogDebug($"[{path}]:[{Fsm.Name}]:[{State.Name}] - {message}");
        }
    }
}