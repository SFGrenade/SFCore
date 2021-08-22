using System;
using HutongGames.PlayMaker;

namespace SFCore.Utils
{
    public class ActionAction<TArg> : FsmStateAction
    {
        public Action<TArg> action;
        public TArg arg;

        public override void Reset()
        {
            action = null;

            base.Reset();
        }

        public override void OnEnter()
        {
            if (action != null) action.Invoke(arg);
            Finish();
        }
    }
}