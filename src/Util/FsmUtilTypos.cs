using System;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;

namespace SFCore.Utils;

public static partial class FsmUtil
{

    /// <inheritdoc cref="AddGlobalTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddGlobalTransition(PlayMakerFSM, string, string)` instead!")]
    public static FsmEvent AddFsmGlobalTransitions(this PlayMakerFSM fsm, string globalEventName, string toState) => AddGlobalTransition(fsm, globalEventName, toState);

}