using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace SFCore.Utils
{
    public class MethodAction : FsmStateAction
    {
        public Action method;

        public override void Reset()
        {
            method = null;

            base.Reset();
        }

        public override void OnEnter()
        {
            if (method != null) method.Invoke();
            Finish();
        }
    }
}