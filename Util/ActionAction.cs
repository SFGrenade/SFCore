using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace SFCore.Utils
{
    public class ActionAction<Targ> : FsmStateAction
    {
        public Action<Targ> action;
        public Targ arg;

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