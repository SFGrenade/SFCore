using System;
using HutongGames.PlayMaker;

namespace SFCore.Utils
{
    /// <summary>
    ///     FsmStateAction that invokes methods with an argument.
    /// </summary>
    public class FunctionAction<TArg> : FsmStateAction
    {
        /// <summary>
        ///     The method to invoke.
        /// </summary>
        public Action<TArg> action;
        /// <summary>
        ///     The argument.
        /// </summary>
        public TArg arg;

        /// <summary>
        ///     Resets the action.
        /// </summary>
        public override void Reset()
        {
            action = null;

            base.Reset();
        }

        /// <summary>
        ///     Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            if (action != null) action.Invoke(arg);
            Finish();
        }
    }
}