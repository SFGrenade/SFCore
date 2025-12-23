using System;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;

namespace SFCore.Utils;

public static partial class FsmUtil
{

    /// <inheritdoc cref="GetState(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetState(PlayMakerFSM, string)` instead!")]
    public static FsmState GetFsmState(this PlayMakerFSM fsm, string stateName) => GetState(fsm, stateName);

    /// <inheritdoc cref="GetState(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetState(Fsm, string)` instead!")]
    public static FsmState GetFsmState(this Fsm fsm, string stateName) => GetState(fsm, stateName);

    /// <inheritdoc cref="GetTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetTransition(PlayMakerFSM, string, string)` instead!")]
    public static FsmTransition GetFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName) => GetTransition(fsm, stateName, eventName);

    /// <inheritdoc cref="GetTransition(Fsm, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetTransition(Fsm, string, string)` instead!")]
    public static FsmTransition GetFsmTransition(this Fsm fsm, string stateName, string eventName) => GetTransition(fsm, stateName, eventName);

    /// <inheritdoc cref="GetTransition(FsmState, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetTransition(FsmState, string)` instead!")]
    public static FsmTransition GetFsmTransition(this FsmState state, string eventName) => GetTransition(state, eventName);

    /// <inheritdoc cref="GetGlobalTransition(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetGlobalTransition(PlayMakerFSM, string)` instead!")]
    public static FsmTransition GetFsmGlobalTransition(this PlayMakerFSM fsm, string globalEventName) => GetGlobalTransition(fsm, globalEventName);

    /// <inheritdoc cref="GetGlobalTransition(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetGlobalTransition(Fsm, string)` instead!")]
    public static FsmTransition GetFsmGlobalTransition(this Fsm fsm, string globalEventName) => GetGlobalTransition(fsm, globalEventName);

    /// <inheritdoc cref="GetAction{TAction}(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    [Obsolete("Use method `GetAction<TAction>(PlayMakerFSM, string, int)` instead!")]
    public static TAction GetFsmAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction => GetAction<TAction>(fsm, stateName, index);

    /// <inheritdoc cref="GetAction{TAction}(Fsm, string, int)"/>
    [PublicAPI]
    [Obsolete("Use method `GetAction<TAction>(Fsm, string, int)` instead!")]
    public static TAction GetFsmAction<TAction>(this Fsm fsm, string stateName, int index) where TAction : FsmStateAction => GetAction<TAction>(fsm, stateName, index);

    /// <inheritdoc cref="GetAction{TAction}(FsmState, int)"/>
    [PublicAPI]
    [Obsolete("Use method `GetAction<TAction>(FsmState, int)` instead!")]
    public static TAction GetFsmAction<TAction>(this FsmState state, int index) where TAction : FsmStateAction => GetAction<TAction>(state, index);

    /// <inheritdoc cref="GetStateAction(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    [Obsolete("Use method `GetStateAction(PlayMakerFSM, string, int)` instead!")]
    public static FsmStateAction GetFsmStateAction(this PlayMakerFSM fsm, string stateName, int index) => GetStateAction(fsm, stateName, index);

    /// <inheritdoc cref="GetStateAction(Fsm, string, int)"/>
    [PublicAPI]
    [Obsolete("Use method `GetStateAction(Fsm, string, int)` instead!")]
    public static FsmStateAction GetFsmStateAction(this Fsm fsm, string stateName, int index) => GetStateAction(fsm, stateName, index);

    /// <inheritdoc cref="GetStateAction(FsmState, int)"/>
    [PublicAPI]
    [Obsolete("Use method `GetStateAction(FsmState, int)` instead!")]
    public static FsmStateAction GetFsmStateAction(this FsmState state, int index) => GetStateAction(state, index);

    /// <inheritdoc cref="GetActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetActionsOfType<TAction>(PlayMakerFSM, string)` instead!")]
    public static TAction[] GetFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => GetActionsOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="GetActionsOfType{TAction}(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetActionsOfType<TAction>(Fsm, string)` instead!")]
    public static TAction[] GetFsmActionsOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction => GetActionsOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="GetActionsOfType{TAction}(FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `GetActionsOfType<TAction>(FsmState)` instead!")]
    public static TAction[] GetFsmActionsOfType<TAction>(this FsmState state) where TAction : FsmStateAction => GetActionsOfType<TAction>(state);

    /// <inheritdoc cref="GetFirstActionOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetFirstActionOfType<TAction>(PlayMakerFSM, string)` instead!")]
    public static TAction GetFsmFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => GetFirstActionOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="GetFirstActionOfType{TAction}(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetFirstActionOfType<TAction>(Fsm, string)` instead!")]
    public static TAction GetFsmFirstActionOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction => GetFirstActionOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="GetFirstActionOfType{TAction}(FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `GetFirstActionOfType<TAction>(FsmState)` instead!")]
    public static TAction GetFsmFirstActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction => GetFirstActionOfType<TAction>(state);

    /// <inheritdoc cref="GetLastActionOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetLastActionOfType<TAction>(PlayMakerFSM, string)` instead!")]
    public static TAction GetFsmLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => GetLastActionOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="GetLastActionOfType{TAction}(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetLastActionOfType<TAction>(Fsm, string)` instead!")]
    public static TAction GetFsmLastActionOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction => GetLastActionOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="GetLastActionOfType{TAction}(FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `GetLastActionOfType<TAction>(FsmState)` instead!")]
    public static TAction GetFsmLastActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction => GetLastActionOfType<TAction>(state);

    /// <inheritdoc cref="AddState(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddState(PlayMakerFSM, string)` instead!")]
    public static FsmState AddFsmState(this PlayMakerFSM fsm, string stateName) => AddState(fsm, stateName);

    /// <inheritdoc cref="AddState(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddState(Fsm, string)` instead!")]
    public static FsmState AddFsmState(this Fsm fsm, string stateName) => AddState(fsm, stateName);

    /// <inheritdoc cref="AddState(PlayMakerFSM, FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `AddState(PlayMakerFSM, FsmState)` instead!")]
    public static FsmState AddFsmState(this PlayMakerFSM fsm, FsmState state) => AddState(fsm, state);

    /// <inheritdoc cref="AddState(Fsm, FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `AddState(Fsm, FsmState)` instead!")]
    public static FsmState AddFsmState(this Fsm fsm, FsmState state) => AddState(fsm, state);

    /// <inheritdoc cref="CopyState(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `CopyState(PlayMakerFSM, string, string)` instead!")]
    public static FsmState CopyFsmState(this PlayMakerFSM fsm, string fromState, string toState) => CopyState(fsm, fromState, toState);

    /// <inheritdoc cref="CopyState(Fsm, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `CopyState(Fsm, string, string)` instead!")]
    public static FsmState CopyFsmState(this Fsm fsm, string fromState, string toState) => CopyState(fsm, fromState, toState);

    /// <inheritdoc cref="AddTransition(PlayMakerFSM, string, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddTransition(PlayMakerFSM, string, string, string)` instead!")]
    public static FsmEvent AddFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => AddTransition(fsm, stateName, eventName, toState);

    /// <inheritdoc cref="AddTransition(Fsm, string, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddTransition(Fsm, string, string, string)` instead!")]
    public static FsmEvent AddFsmTransition(this Fsm fsm, string stateName, string eventName, string toState) => AddTransition(fsm, stateName, eventName, toState);

    /// <inheritdoc cref="AddTransition(FsmState, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddTransition(FsmState, string, string)` instead!")]
    public static FsmEvent AddFsmTransition(this FsmState state, string eventName, string toState) => AddTransition(state, eventName, toState);

    /// <inheritdoc cref="AddGlobalTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddGlobalTransition(PlayMakerFSM, string, string)` instead!")]
    public static FsmEvent AddFsmGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState) => AddGlobalTransition(fsm, globalEventName, toState);

    /// <inheritdoc cref="AddGlobalTransition(Fsm, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddGlobalTransition(Fsm, string, string)` instead!")]
    public static FsmEvent AddFsmGlobalTransition(this Fsm fsm, string globalEventName, string toState) => AddGlobalTransition(fsm, globalEventName, toState);

    /// <inheritdoc cref="AddAction(PlayMakerFSM, string, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `AddAction(PlayMakerFSM, string, FsmStateAction)` instead!")]
    public static void AddFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => AddAction(fsm, stateName, action);

    /// <inheritdoc cref="AddAction(Fsm, string, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `AddAction(Fsm, string, FsmStateAction)` instead!")]
    public static void AddFsmAction(this Fsm fsm, string stateName, FsmStateAction action) => AddAction(fsm, stateName, action);

    /// <inheritdoc cref="AddAction(FsmState, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `AddAction(FsmState, FsmStateAction)` instead!")]
    public static void AddFsmAction(this FsmState state, FsmStateAction action) => AddAction(state, action);

    /// <inheritdoc cref="AddActions(PlayMakerFSM, string, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `AddActions(PlayMakerFSM, string, params FsmStateAction[])` instead!")]
    public static void AddFsmActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions) => AddActions(fsm, stateName, actions);

    /// <inheritdoc cref="AddActions(Fsm, string, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `AddActions(Fsm, string, params FsmStateAction[])` instead!")]
    public static void AddFsmActions(this Fsm fsm, string stateName, params FsmStateAction[] actions) => AddActions(fsm, stateName, actions);

    /// <inheritdoc cref="AddActions(FsmState, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `AddActions(FsmState, params FsmStateAction[])` instead!")]
    public static void AddFsmActions(this FsmState state, params FsmStateAction[] actions) => AddActions(state, actions);

    /// <inheritdoc cref="AddMethod(PlayMakerFSM, string, Action)"/>
    [PublicAPI]
    [Obsolete("Use method `AddMethod(PlayMakerFSM, string, Action)` instead!")]
    public static void AddFsmMethod(this PlayMakerFSM fsm, string stateName, Action method) => AddMethod(fsm, stateName, method);

    /// <inheritdoc cref="AddMethod(Fsm, string, Action)"/>
    [PublicAPI]
    [Obsolete("Use method `AddMethod(Fsm, string, Action)` instead!")]
    public static void AddFsmMethod(this Fsm fsm, string stateName, Action method) => AddMethod(fsm, stateName, method);

    /// <inheritdoc cref="AddMethod(FsmState, Action)"/>
    [PublicAPI]
    [Obsolete("Use method `AddMethod(FsmState, Action)` instead!")]
    public static void AddFsmMethod(this FsmState state, Action method) => AddMethod(state, method);

    /// <inheritdoc cref="AddLambdaMethod(PlayMakerFSM, string, Action{Action})"/>
    [PublicAPI]
    [Obsolete("Use method `AddLambdaMethod(PlayMakerFSM, string, Action<Action>)` instead!")]
    public static void AddFsmLambdaMethod(this PlayMakerFSM fsm, string stateName, Action<Action> method) => AddLambdaMethod(fsm, stateName, method);

    /// <inheritdoc cref="AddLambdaMethod(Fsm, string, Action{Action})"/>
    [PublicAPI]
    [Obsolete("Use method `AddLambdaMethod(Fsm, string, Action<Action>)` instead!")]
    public static void AddFsmLambdaMethod(this Fsm fsm, string stateName, Action<Action> method) => AddLambdaMethod(fsm, stateName, method);

    /// <inheritdoc cref="AddLambdaMethod(FsmState, Action{Action})"/>
    [PublicAPI]
    [Obsolete("Use method `AddLambdaMethod(FsmState, Action<Action>)` instead!")]
    public static void AddFsmLambdaMethod(this FsmState state, Action<Action> method) => AddLambdaMethod(state, method);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertAction(PlayMakerFSM, string, FsmStateAction, int)` instead!")]
    public static void InsertFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => InsertAction(fsm, stateName, action, index);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, int, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertAction(PlayMakerFSM, string, int, FsmStateAction)` instead!")]
    public static void InsertFsmAction(this PlayMakerFSM fsm, string stateName, int index, FsmStateAction action) => InsertAction(fsm, stateName, index, action);

    /// <inheritdoc cref="InsertAction(Fsm, string, FsmStateAction, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertAction(Fsm, string, FsmStateAction, int)` instead!")]
    public static void InsertFsmAction(this Fsm fsm, string stateName, FsmStateAction action, int index) => InsertAction(fsm, stateName, action, index);

    /// <inheritdoc cref="InsertAction(Fsm, string, int, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertAction(Fsm, string, int, FsmStateAction)` instead!")]
    public static void InsertFsmAction(this Fsm fsm, string stateName, int index, FsmStateAction action) => InsertAction(fsm, stateName, index, action);

    /// <inheritdoc cref="InsertAction(FsmState, FsmStateAction, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertAction(FsmState, FsmStateAction, int)` instead!")]
    public static void InsertFsmAction(this FsmState state, FsmStateAction action, int index) => InsertAction(state, action, index);

    /// <inheritdoc cref="InsertAction(FsmState, int, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertAction(FsmState, int, FsmStateAction)` instead!")]
    public static void InsertFsmAction(this FsmState state, int index, FsmStateAction action) => InsertAction(state, index, action);

    /// <inheritdoc cref="InsertActions(PlayMakerFSM, string, int, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `InsertActions(PlayMakerFSM, string, int, params FsmStateAction[])` instead!")]
    public static void InsertFsmActions(this PlayMakerFSM fsm, string stateName, int index, params FsmStateAction[] actions) => InsertActions(fsm, stateName, index, actions);

    /// <inheritdoc cref="InsertActions(Fsm, string, int, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `InsertActions(Fsm, string, int, params FsmStateAction[])` instead!")]
    public static void InsertFsmActions(this Fsm fsm, string stateName, int index, params FsmStateAction[] actions) => InsertActions(fsm, stateName, index, actions);

    /// <inheritdoc cref="InsertActions(FsmState, int, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `InsertActions(FsmState, int, params FsmStateAction[])` instead!")]
    public static void InsertFsmActions(this FsmState state, int index, params FsmStateAction[] actions) => InsertActions(state, index, actions);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertMethod(PlayMakerFSM, string, Action, int)` instead!")]
    public static void InsertFsmMethod(this PlayMakerFSM fsm, string stateName, Action method, int index) => InsertMethod(fsm, stateName, method, index);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, int, Action)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertMethod(PlayMakerFSM, string, int, Action)` instead!")]
    public static void InsertFsmMethod(this PlayMakerFSM fsm, string stateName, int index, Action method) => InsertMethod(fsm, stateName, index, method);

    /// <inheritdoc cref="InsertMethod(Fsm, string, Action, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertMethod(Fsm, string, Action, int)` instead!")]
    public static void InsertFsmMethod(this Fsm fsm, string stateName, Action method, int index) => InsertMethod(fsm, stateName, method, index);

    /// <inheritdoc cref="InsertMethod(Fsm, string, int, Action)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertMethod(Fsm, string, int, Action)` instead!")]
    public static void InsertFsmMethod(this Fsm fsm, string stateName, int index, Action method) => InsertMethod(fsm, stateName, index, method);

    /// <inheritdoc cref="InsertMethod(FsmState, Action, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertMethod(FsmState, Action, int)` instead!")]
    public static void InsertFsmMethod(this FsmState state, Action method, int index) => InsertMethod(state, method, index);

    /// <inheritdoc cref="InsertMethod(FsmState, int, Action)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertMethod(FsmState, int, Action)` instead!")]
    public static void InsertFsmMethod(this FsmState state, int index, Action method) => InsertMethod(state, index, method);

    /// <inheritdoc cref="InsertLambdaMethod(PlayMakerFSM, string, Action{Action}, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertLambdaMethod(PlayMakerFSM, string, Action<Action>, int)` instead!")]
    public static void InsertFsmLambdaMethod(this PlayMakerFSM fsm, string stateName, Action<Action> method, int index) => InsertLambdaMethod(fsm, stateName, method, index);

    /// <inheritdoc cref="InsertLambdaMethod(PlayMakerFSM, string, int, Action{Action})"/>
    [PublicAPI]
    [Obsolete("Use method `InsertLambdaMethod(PlayMakerFSM, string, int, Action<Action>)` instead!")]
    public static void InsertFsmLambdaMethod(this PlayMakerFSM fsm, string stateName, int index, Action<Action> method) => InsertLambdaMethod(fsm, stateName, index, method);

    /// <inheritdoc cref="InsertLambdaMethod(Fsm, string, Action{Action}, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertLambdaMethod(Fsm, string, Action<Action>, int)` instead!")]
    public static void InsertFsmLambdaMethod(this Fsm fsm, string stateName, Action<Action> method, int index) => InsertLambdaMethod(fsm, stateName, method, index);

    /// <inheritdoc cref="InsertLambdaMethod(Fsm, string, int, Action{Action})"/>
    [PublicAPI]
    [Obsolete("Use method `InsertLambdaMethod(Fsm, string, int, Action<Action>)` instead!")]
    public static void InsertFsmLambdaMethod(this Fsm fsm, string stateName, int index, Action<Action> method) => InsertLambdaMethod(fsm, stateName, index, method);

    /// <inheritdoc cref="InsertLambdaMethod(FsmState, Action{Action}, int)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertLambdaMethod(FsmState, Action<Action>, int)` instead!")]
    public static void InsertFsmLambdaMethod(this FsmState state, Action<Action> method, int index) => InsertLambdaMethod(state, method, index);

    /// <inheritdoc cref="InsertLambdaMethod(FsmState, int, Action{Action})"/>
    [PublicAPI]
    [Obsolete("Use method `InsertLambdaMethod(FsmState, int, Action<Action>)` instead!")]
    public static void InsertFsmLambdaMethod(this FsmState state, int index, Action<Action> method) => InsertLambdaMethod(state, index, method);

    /// <inheritdoc cref="InsertMethodBefore(FsmStateAction, Action)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertMethodBefore(FsmStateAction, Action)` instead!")]
    public static void InsertFsmMethodBefore(this FsmStateAction action, Action method) => InsertMethodBefore(action, method);

    /// <inheritdoc cref="InsertMethodAfter(FsmStateAction, Action)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertMethodAfter(FsmStateAction, Action)` instead!")]
    public static void InsertFsmMethodAfter(this FsmStateAction action, Action method) => InsertMethodAfter(action, method);

    /// <inheritdoc cref="InsertActionBefore(FsmStateAction, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertActionBefore(FsmStateAction, FsmStateAction)` instead!")]
    public static void InsertFsmActionBefore(this FsmStateAction action, FsmStateAction newAction) => InsertActionBefore(action, newAction);

    /// <inheritdoc cref="InsertActionAfter(FsmStateAction, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `InsertActionAfter(FsmStateAction, FsmStateAction)` instead!")]
    public static void InsertFsmActionAfter(this FsmStateAction action, FsmStateAction newAction) => InsertActionAfter(action, newAction);

    /// <inheritdoc cref="ReplaceAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAction(PlayMakerFSM, string, FsmStateAction, int)` instead!")]
    public static void ReplaceFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => ReplaceAction(fsm, stateName, action, index);

    /// <inheritdoc cref="ReplaceAction(PlayMakerFSM, string, int, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAction(PlayMakerFSM, string, int, FsmStateAction)` instead!")]
    public static void ReplaceFsmAction(this PlayMakerFSM fsm, string stateName, int index, FsmStateAction action) => ReplaceAction(fsm, stateName, index, action);

    /// <inheritdoc cref="ReplaceAction(Fsm, string, FsmStateAction, int)"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAction(Fsm, string, FsmStateAction, int)` instead!")]
    public static void ReplaceFsmAction(this Fsm fsm, string stateName, FsmStateAction action, int index) => ReplaceAction(fsm, stateName, action, index);

    /// <inheritdoc cref="ReplaceAction(Fsm, string, int, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAction(Fsm, string, int, FsmStateAction)` instead!")]
    public static void ReplaceFsmAction(this Fsm fsm, string stateName, int index, FsmStateAction action) => ReplaceAction(fsm, stateName, index, action);

    /// <inheritdoc cref="ReplaceAction(FsmState, FsmStateAction, int)"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAction(FsmState, FsmStateAction, int)` instead!")]
    public static void ReplaceFsmAction(this FsmState state, FsmStateAction action, int index) => ReplaceAction(state, action, index);

    /// <inheritdoc cref="ReplaceAction(FsmState, int, FsmStateAction)"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAction(FsmState, int, FsmStateAction)` instead!")]
    public static void ReplaceFsmAction(this FsmState state, int index, FsmStateAction action) => ReplaceAction(state, index, action);

    /// <inheritdoc cref="ReplaceAllActions(PlayMakerFSM, string, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAllActions(PlayMakerFSM, string, params FsmStateAction[])` instead!")]
    public static void ReplaceFsmAllActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions) => ReplaceAllActions(fsm, stateName, actions);

    /// <inheritdoc cref="ReplaceAllActions(Fsm, string, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAllActions(Fsm, string, params FsmStateAction[])` instead!")]
    public static void ReplaceFsmAllActions(this Fsm fsm, string stateName, params FsmStateAction[] actions) => ReplaceAllActions(fsm, stateName, actions);

    /// <inheritdoc cref="ReplaceAllActions(FsmState, FsmStateAction[])"/>
    [PublicAPI]
    [Obsolete("Use method `ReplaceAllActions(FsmState, params FsmStateAction[])` instead!")]
    public static void ReplaceFsmAllActions(this FsmState state, params FsmStateAction[] actions) => ReplaceAllActions(state, actions);

    /// <inheritdoc cref="ChangeTransition(PlayMakerFSM, string, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `ChangeTransition(PlayMakerFSM, string, string, string)` instead!")]
    public static bool ChangeFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => ChangeTransition(fsm, stateName, eventName, toState);

    /// <inheritdoc cref="ChangeTransition(Fsm, string, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `ChangeTransition(Fsm, string, string, string)` instead!")]
    public static bool ChangeFsmTransition(this Fsm fsm, string stateName, string eventName, string toState) => ChangeTransition(fsm, stateName, eventName, toState);

    /// <inheritdoc cref="ChangeTransition(FsmState, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `ChangeTransition(FsmState, string, string)` instead!")]
    public static bool ChangeFsmTransition(this FsmState state, string eventName, string toState) => ChangeTransition(state, eventName, toState);

    /// <inheritdoc cref="ChangeGlobalTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `ChangeGlobalTransition(PlayMakerFSM, string, string)` instead!")]
    public static bool ChangeFsmGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState) => ChangeGlobalTransition(fsm, globalEventName, toState);

    /// <inheritdoc cref="ChangeGlobalTransition(Fsm, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `ChangeGlobalTransition(Fsm, string, string)` instead!")]
    public static bool ChangeFsmGlobalTransition(this Fsm fsm, string globalEventName, string toState) => ChangeGlobalTransition(fsm, globalEventName, toState);

    /// <inheritdoc cref="RemoveState(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveState(PlayMakerFSM, string)` instead!")]
    public static void RemoveFsmState(this PlayMakerFSM fsm, string stateName) => RemoveState(fsm, stateName);

    /// <inheritdoc cref="RemoveState(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveState(Fsm, string)` instead!")]
    public static void RemoveFsmState(this Fsm fsm, string stateName) => RemoveState(fsm, stateName);

    /// <inheritdoc cref="RemoveTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransition(PlayMakerFSM, string, string)` instead!")]
    public static void RemoveFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName) => RemoveTransition(fsm, stateName, eventName);

    /// <inheritdoc cref="RemoveTransition(Fsm, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransition(Fsm, string, string)` instead!")]
    public static void RemoveFsmTransition(this Fsm fsm, string stateName, string eventName) => RemoveTransition(fsm, stateName, eventName);

    /// <inheritdoc cref="RemoveTransition(FsmState, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransition(FsmState, string)` instead!")]
    public static void RemoveFsmTransition(this FsmState state, string eventName) => RemoveTransition(state, eventName);

    /// <inheritdoc cref="RemoveGlobalTransition(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveGlobalTransition(PlayMakerFSM, string)` instead!")]
    public static void RemoveFsmGlobalTransition(this PlayMakerFSM fsm, string globalEventName) => RemoveGlobalTransition(fsm, globalEventName);

    /// <inheritdoc cref="RemoveGlobalTransition(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveGlobalTransition(Fsm, string)` instead!")]
    public static void RemoveFsmGlobalTransition(this Fsm fsm, string globalEventName) => RemoveGlobalTransition(fsm, globalEventName);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransitionsTo(PlayMakerFSM, string)` instead!")]
    public static void RemoveFsmTransitionsTo(this PlayMakerFSM fsm, string toState) => RemoveTransitionsTo(fsm, toState);

    /// <inheritdoc cref="RemoveTransitionsTo(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransitionsTo(Fsm, string)` instead!")]
    public static void RemoveFsmTransitionsTo(this Fsm fsm, string toState) => RemoveTransitionsTo(fsm, toState);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransitionsTo(PlayMakerFSM, string, string)` instead!")]
    public static void RemoveFsmTransitionsTo(this PlayMakerFSM fsm, string stateName, string toState) => RemoveTransitionsTo(fsm, stateName, toState);

    /// <inheritdoc cref="RemoveTransitionsTo(Fsm, string, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransitionsTo(Fsm, string, string)` instead!")]
    public static void RemoveFsmTransitionsTo(this Fsm fsm, string stateName, string toState) => RemoveTransitionsTo(fsm, stateName, toState);

    /// <inheritdoc cref="RemoveTransitionsTo(FsmState, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransitionsTo(FsmState, string)` instead!")]
    public static void RemoveFsmTransitionsTo(this FsmState state, string toState) => RemoveTransitionsTo(state, toState);

    /// <inheritdoc cref="RemoveTransitions(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransitions(PlayMakerFSM, string)` instead!")]
    public static void RemoveFsmTransitions(this PlayMakerFSM fsm, string stateName) => RemoveTransitions(fsm, stateName);

    /// <inheritdoc cref="RemoveTransitions(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransitions(Fsm, string)` instead!")]
    public static void RemoveFsmTransitions(this Fsm fsm, string stateName) => RemoveTransitions(fsm, stateName);

    /// <inheritdoc cref="RemoveTransitions(FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveTransitions(FsmState)` instead!")]
    public static void RemoveFsmTransitions(this FsmState state) => RemoveTransitions(state);

    /// <inheritdoc cref="RemoveAction(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveAction(PlayMakerFSM, string, int)` instead!")]
    public static bool RemoveFsmAction(this PlayMakerFSM fsm, string stateName, int index) => RemoveAction(fsm, stateName, index);

    /// <inheritdoc cref="RemoveAction(Fsm, string, int)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveAction(Fsm, string, int)` instead!")]
    public static bool RemoveFsmAction(this Fsm fsm, string stateName, int index) => RemoveAction(fsm, stateName, index);

    /// <inheritdoc cref="RemoveAction(FsmState, int)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveAction(FsmState, int)` instead!")]
    public static bool RemoveFsmAction(this FsmState state, int index) => RemoveAction(state, index);

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveActionsOfType<TAction>(PlayMakerFSM)` instead!")]
    public static void RemoveFsmActionsOfType<TAction>(this PlayMakerFSM fsm) => RemoveActionsOfType<TAction>(fsm);

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(Fsm)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveActionsOfType<TAction>(Fsm)` instead!")]
    public static void RemoveFsmActionsOfType<TAction>(this Fsm fsm) => RemoveActionsOfType<TAction>(fsm);

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveActionsOfType<TAction>(PlayMakerFSM, string)` instead!")]
    public static void RemoveFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => RemoveActionsOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveActionsOfType<TAction>(Fsm, string)` instead!")]
    public static void RemoveFsmActionsOfType<TAction>(this Fsm fsm, string stateName) => RemoveActionsOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveActionsOfType<TAction>(FsmState)` instead!")]
    public static void RemoveFsmActionsOfType<TAction>(this FsmState state) => RemoveActionsOfType<TAction>(state);

    /// <inheritdoc cref="RemoveFirstActionOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveFirstActionOfType<TAction>(PlayMakerFSM, string)` instead!")]
    public static void RemoveFsmFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) => RemoveFirstActionOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="RemoveFirstActionOfType{TAction}(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveFirstActionOfType<TAction>(Fsm, string)` instead!")]
    public static void RemoveFsmFirstActionOfType<TAction>(this Fsm fsm, string stateName) => RemoveFirstActionOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="RemoveFirstActionOfType{TAction}(FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveFirstActionOfType<TAction>(FsmState)` instead!")]
    public static void RemoveFsmFirstActionOfType<TAction>(this FsmState state) => RemoveFirstActionOfType<TAction>(state);

    /// <inheritdoc cref="RemoveLastActionOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveLastActionOfType<TAction>(PlayMakerFSM, string)` instead!")]
    public static void RemoveFsmLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) => RemoveLastActionOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="RemoveLastActionOfType{TAction}(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveLastActionOfType<TAction>(Fsm, string)` instead!")]
    public static void RemoveFsmLastActionOfType<TAction>(this Fsm fsm, string stateName) => RemoveLastActionOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="RemoveLastActionOfType{TAction}(FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `RemoveLastActionOfType<TAction>(FsmState)` instead!")]
    public static void RemoveFsmLastActionOfType<TAction>(this FsmState state) => RemoveLastActionOfType<TAction>(state);

    /// <inheritdoc cref="DisableAction(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    [Obsolete("Use method `DisableAction(PlayMakerFSM, string, int)` instead!")]
    public static bool DisableFsmAction(this PlayMakerFSM fsm, string stateName, int index) => DisableAction(fsm, stateName, index);

    /// <inheritdoc cref="DisableAction(Fsm, string, int)"/>
    [PublicAPI]
    [Obsolete("Use method `DisableAction(Fsm, string, int)` instead!")]
    public static bool DisableFsmAction(this Fsm fsm, string stateName, int index) => DisableAction(fsm, stateName, index);

    /// <inheritdoc cref="DisableAction(FsmState, int)"/>
    [PublicAPI]
    [Obsolete("Use method `DisableAction(FsmState, int)` instead!")]
    public static bool DisableFsmAction(this FsmState state, int index) => DisableAction(state, index);

    /// <inheritdoc cref="DisableActions(PlayMakerFSM, string, int[])"/>
    [PublicAPI]
    [Obsolete("Use method `DisableActions(PlayMakerFSM, string, params int[])` instead!")]
    public static bool DisableFsmActions(this PlayMakerFSM fsm, string stateName, params int[] indices) => DisableActions(fsm, stateName, indices);

    /// <inheritdoc cref="DisableActions(Fsm, string, int[])"/>
    [PublicAPI]
    [Obsolete("Use method `DisableActions(Fsm, string, params int[])` instead!")]
    public static bool DisableFsmActions(this Fsm fsm, string stateName, params int[] indices) => DisableActions(fsm, stateName, indices);

    /// <inheritdoc cref="DisableActions(FsmState, int[])"/>
    [PublicAPI]
    [Obsolete("Use method `DisableActions(FsmState, params int[])` instead!")]
    public static bool DisableFsmActions(this FsmState state, params int[] indices) => DisableActions(state, indices);

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM)"/>
    [PublicAPI]
    [Obsolete("Use method `DisableActionsOfType<TAction>(PlayMakerFSM)` instead!")]
    public static void DisableFsmActionsOfType<TAction>(this PlayMakerFSM fsm) => DisableActionsOfType<TAction>(fsm);

    /// <inheritdoc cref="DisableActionsOfType{TAction}(Fsm)"/>
    [PublicAPI]
    [Obsolete("Use method `DisableActionsOfType<TAction>(Fsm)` instead!")]
    public static void DisableFsmActionsOfType<TAction>(this Fsm fsm) => DisableActionsOfType<TAction>(fsm);

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `DisableActionsOfType<TAction>(PlayMakerFSM, string)` instead!")]
    public static void DisableFsmActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => DisableActionsOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="DisableActionsOfType{TAction}(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `DisableActionsOfType<TAction>(Fsm, string)` instead!")]
    public static void DisableFsmActionsOfType<TAction>(this Fsm fsm, string stateName) => DisableActionsOfType<TAction>(fsm, stateName);

    /// <inheritdoc cref="DisableActionsOfType{TAction}(FsmState)"/>
    [PublicAPI]
    [Obsolete("Use method `DisableActionsOfType<TAction>(FsmState)` instead!")]
    public static void DisableFsmActionsOfType<TAction>(this FsmState state) => DisableActionsOfType<TAction>(state);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddFloatVariable(PlayMakerFSM, string)` instead!")]
    public static FsmFloat AddFsmFloatVariable(this PlayMakerFSM fsm, string name) => AddFloatVariable(fsm, name);

    /// <inheritdoc cref="AddFloatVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddFloatVariable(Fsm, string)` instead!")]
    public static FsmFloat AddFsmFloatVariable(this Fsm fsm, string name) => AddFloatVariable(fsm, name);

    /// <inheritdoc cref="AddIntVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddIntVariable(PlayMakerFSM, string)` instead!")]
    public static FsmInt AddFsmIntVariable(this PlayMakerFSM fsm, string name) => AddIntVariable(fsm, name);

    /// <inheritdoc cref="AddIntVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddIntVariable(Fsm, string)` instead!")]
    public static FsmInt AddFsmIntVariable(this Fsm fsm, string name) => AddIntVariable(fsm, name);

    /// <inheritdoc cref="AddBoolVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddBoolVariable(PlayMakerFSM, string)` instead!")]
    public static FsmBool AddFsmBoolVariable(this PlayMakerFSM fsm, string name) => AddBoolVariable(fsm, name);

    /// <inheritdoc cref="AddBoolVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddBoolVariable(Fsm, string)` instead!")]
    public static FsmBool AddFsmBoolVariable(this Fsm fsm, string name) => AddBoolVariable(fsm, name);

    /// <inheritdoc cref="AddStringVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddStringVariable(PlayMakerFSM, string)` instead!")]
    public static FsmString AddFsmStringVariable(this PlayMakerFSM fsm, string name) => AddStringVariable(fsm, name);

    /// <inheritdoc cref="AddStringVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddStringVariable(Fsm, string)` instead!")]
    public static FsmString AddFsmStringVariable(this Fsm fsm, string name) => AddStringVariable(fsm, name);

    /// <inheritdoc cref="AddVector2Variable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddVector2Variable(PlayMakerFSM, string)` instead!")]
    public static FsmVector2 AddFsmVector2Variable(this PlayMakerFSM fsm, string name) => AddVector2Variable(fsm, name);

    /// <inheritdoc cref="AddVector2Variable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddVector2Variable(Fsm, string)` instead!")]
    public static FsmVector2 AddFsmVector2Variable(this Fsm fsm, string name) => AddVector2Variable(fsm, name);

    /// <inheritdoc cref="AddVector3Variable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddVector3Variable(PlayMakerFSM, string)` instead!")]
    public static FsmVector3 AddFsmVector3Variable(this PlayMakerFSM fsm, string name) => AddVector3Variable(fsm, name);

    /// <inheritdoc cref="AddVector3Variable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddVector3Variable(Fsm, string)` instead!")]
    public static FsmVector3 AddFsmVector3Variable(this Fsm fsm, string name) => AddVector3Variable(fsm, name);

    /// <inheritdoc cref="AddColorVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddColorVariable(PlayMakerFSM, string)` instead!")]
    public static FsmColor AddFsmColorVariable(this PlayMakerFSM fsm, string name) => AddColorVariable(fsm, name);

    /// <inheritdoc cref="AddColorVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddColorVariable(Fsm, string)` instead!")]
    public static FsmColor AddFsmColorVariable(this Fsm fsm, string name) => AddColorVariable(fsm, name);

    /// <inheritdoc cref="AddRectVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddRectVariable(PlayMakerFSM, string)` instead!")]
    public static FsmRect AddFsmRectVariable(this PlayMakerFSM fsm, string name) => AddRectVariable(fsm, name);

    /// <inheritdoc cref="AddRectVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddRectVariable(Fsm, string)` instead!")]
    public static FsmRect AddFsmRectVariable(this Fsm fsm, string name) => AddRectVariable(fsm, name);

    /// <inheritdoc cref="AddQuaternionVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddQuaternionVariable(PlayMakerFSM, string)` instead!")]
    public static FsmQuaternion AddFsmQuaternionVariable(this PlayMakerFSM fsm, string name) => AddQuaternionVariable(fsm, name);

    /// <inheritdoc cref="AddQuaternionVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddQuaternionVariable(Fsm, string)` instead!")]
    public static FsmQuaternion AddFsmQuaternionVariable(this Fsm fsm, string name) => AddQuaternionVariable(fsm, name);

    /// <inheritdoc cref="AddGameObjectVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddGameObjectVariable(PlayMakerFSM, string)` instead!")]
    public static FsmGameObject AddFsmGameObjectVariable(this PlayMakerFSM fsm, string name) => AddGameObjectVariable(fsm, name);

    /// <inheritdoc cref="AddGameObjectVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `AddGameObjectVariable(Fsm, string)` instead!")]
    public static FsmGameObject AddFsmGameObjectVariable(this Fsm fsm, string name) => AddGameObjectVariable(fsm, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindFloatVariable(PlayMakerFSM, string)` instead!")]
    public static FsmFloat FindFsmFloatVariable(this PlayMakerFSM fsm, string name) => FindFloatVariable(fsm, name);

    /// <inheritdoc cref="FindFloatVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindFloatVariable(Fsm, string)` instead!")]
    public static FsmFloat FindFsmFloatVariable(this Fsm fsm, string name) => FindFloatVariable(fsm, name);

    /// <inheritdoc cref="FindIntVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindIntVariable(PlayMakerFSM, string)` instead!")]
    public static FsmInt FindFsmIntVariable(this PlayMakerFSM fsm, string name) => FindIntVariable(fsm, name);

    /// <inheritdoc cref="FindIntVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindIntVariable(Fsm, string)` instead!")]
    public static FsmInt FindFsmIntVariable(this Fsm fsm, string name) => FindIntVariable(fsm, name);

    /// <inheritdoc cref="FindBoolVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindBoolVariable(PlayMakerFSM, string)` instead!")]
    public static FsmBool FindFsmBoolVariable(this PlayMakerFSM fsm, string name) => FindBoolVariable(fsm, name);

    /// <inheritdoc cref="FindBoolVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindBoolVariable(Fsm, string)` instead!")]
    public static FsmBool FindFsmBoolVariable(this Fsm fsm, string name) => FindBoolVariable(fsm, name);

    /// <inheritdoc cref="FindStringVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindStringVariable(PlayMakerFSM, string)` instead!")]
    public static FsmString FindFsmStringVariable(this PlayMakerFSM fsm, string name) => FindStringVariable(fsm, name);

    /// <inheritdoc cref="FindStringVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindStringVariable(Fsm, string)` instead!")]
    public static FsmString FindFsmStringVariable(this Fsm fsm, string name) => FindStringVariable(fsm, name);

    /// <inheritdoc cref="FindVector2Variable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindVector2Variable(PlayMakerFSM, string)` instead!")]
    public static FsmVector2 FindFsmVector2Variable(this PlayMakerFSM fsm, string name) => FindVector2Variable(fsm, name);

    /// <inheritdoc cref="FindVector2Variable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindVector2Variable(Fsm, string)` instead!")]
    public static FsmVector2 FindFsmVector2Variable(this Fsm fsm, string name) => FindVector2Variable(fsm, name);

    /// <inheritdoc cref="FindVector3Variable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindVector3Variable(PlayMakerFSM, string)` instead!")]
    public static FsmVector3 FindFsmVector3Variable(this PlayMakerFSM fsm, string name) => FindVector3Variable(fsm, name);

    /// <inheritdoc cref="FindVector3Variable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindVector3Variable(Fsm, string)` instead!")]
    public static FsmVector3 FindFsmVector3Variable(this Fsm fsm, string name) => FindVector3Variable(fsm, name);

    /// <inheritdoc cref="FindColorVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindColorVariable(PlayMakerFSM, string)` instead!")]
    public static FsmColor FindFsmColorVariable(this PlayMakerFSM fsm, string name) => FindColorVariable(fsm, name);

    /// <inheritdoc cref="FindColorVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindColorVariable(Fsm, string)` instead!")]
    public static FsmColor FindFsmColorVariable(this Fsm fsm, string name) => FindColorVariable(fsm, name);

    /// <inheritdoc cref="FindRectVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindRectVariable(PlayMakerFSM, string)` instead!")]
    public static FsmRect FindFsmRectVariable(this PlayMakerFSM fsm, string name) => FindRectVariable(fsm, name);

    /// <inheritdoc cref="FindRectVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindRectVariable(Fsm, string)` instead!")]
    public static FsmRect FindFsmRectVariable(this Fsm fsm, string name) => FindRectVariable(fsm, name);

    /// <inheritdoc cref="FindQuaternionVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindQuaternionVariable(PlayMakerFSM, string)` instead!")]
    public static FsmQuaternion FindFsmQuaternionVariable(this PlayMakerFSM fsm, string name) => FindQuaternionVariable(fsm, name);

    /// <inheritdoc cref="FindQuaternionVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindQuaternionVariable(Fsm, string)` instead!")]
    public static FsmQuaternion FindFsmQuaternionVariable(this Fsm fsm, string name) => FindQuaternionVariable(fsm, name);

    /// <inheritdoc cref="FindGameObjectVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindGameObjectVariable(PlayMakerFSM, string)` instead!")]
    public static FsmGameObject FindFsmGameObjectVariable(this PlayMakerFSM fsm, string name) => FindGameObjectVariable(fsm, name);

    /// <inheritdoc cref="FindGameObjectVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `FindGameObjectVariable(Fsm, string)` instead!")]
    public static FsmGameObject FindFsmGameObjectVariable(this Fsm fsm, string name) => FindGameObjectVariable(fsm, name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetFloatVariable(PlayMakerFSM, string)` instead!")]
    public static FsmFloat GetFsmFloatVariable(this PlayMakerFSM fsm, string name) => GetFloatVariable(fsm, name);

    /// <inheritdoc cref="GetFloatVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetFloatVariable(Fsm, string)` instead!")]
    public static FsmFloat GetFsmFloatVariable(this Fsm fsm, string name) => GetFloatVariable(fsm, name);

    /// <inheritdoc cref="GetIntVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetIntVariable(PlayMakerFSM, string)` instead!")]
    public static FsmInt GetFsmIntVariable(this PlayMakerFSM fsm, string name) => GetIntVariable(fsm, name);

    /// <inheritdoc cref="GetIntVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetIntVariable(Fsm, string)` instead!")]
    public static FsmInt GetFsmIntVariable(this Fsm fsm, string name) => GetIntVariable(fsm, name);

    /// <inheritdoc cref="GetBoolVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetBoolVariable(PlayMakerFSM, string)` instead!")]
    public static FsmBool GetFsmBoolVariable(this PlayMakerFSM fsm, string name) => GetBoolVariable(fsm, name);

    /// <inheritdoc cref="GetBoolVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetBoolVariable(Fsm, string)` instead!")]
    public static FsmBool GetFsmBoolVariable(this Fsm fsm, string name) => GetBoolVariable(fsm, name);

    /// <inheritdoc cref="GetStringVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetStringVariable(PlayMakerFSM, string)` instead!")]
    public static FsmString GetFsmStringVariable(this PlayMakerFSM fsm, string name) => GetStringVariable(fsm, name);

    /// <inheritdoc cref="GetStringVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetStringVariable(Fsm, string)` instead!")]
    public static FsmString GetFsmStringVariable(this Fsm fsm, string name) => GetStringVariable(fsm, name);

    /// <inheritdoc cref="GetVector2Variable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetVector2Variable(PlayMakerFSM, string)` instead!")]
    public static FsmVector2 GetFsmVector2Variable(this PlayMakerFSM fsm, string name) => GetVector2Variable(fsm, name);

    /// <inheritdoc cref="GetVector2Variable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetVector2Variable(Fsm, string)` instead!")]
    public static FsmVector2 GetFsmVector2Variable(this Fsm fsm, string name) => GetVector2Variable(fsm, name);

    /// <inheritdoc cref="GetVector3Variable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetVector3Variable(PlayMakerFSM, string)` instead!")]
    public static FsmVector3 GetFsmVector3Variable(this PlayMakerFSM fsm, string name) => GetVector3Variable(fsm, name);

    /// <inheritdoc cref="GetVector3Variable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetVector3Variable(Fsm, string)` instead!")]
    public static FsmVector3 GetFsmVector3Variable(this Fsm fsm, string name) => GetVector3Variable(fsm, name);

    /// <inheritdoc cref="GetColorVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetColorVariable(PlayMakerFSM, string)` instead!")]
    public static FsmColor GetFsmColorVariable(this PlayMakerFSM fsm, string name) => GetColorVariable(fsm, name);

    /// <inheritdoc cref="GetColorVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetColorVariable(Fsm, string)` instead!")]
    public static FsmColor GetFsmColorVariable(this Fsm fsm, string name) => GetColorVariable(fsm, name);

    /// <inheritdoc cref="GetRectVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetRectVariable(PlayMakerFSM, string)` instead!")]
    public static FsmRect GetFsmRectVariable(this PlayMakerFSM fsm, string name) => GetRectVariable(fsm, name);

    /// <inheritdoc cref="GetRectVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetRectVariable(Fsm, string)` instead!")]
    public static FsmRect GetFsmRectVariable(this Fsm fsm, string name) => GetRectVariable(fsm, name);

    /// <inheritdoc cref="GetQuaternionVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetQuaternionVariable(PlayMakerFSM, string)` instead!")]
    public static FsmQuaternion GetFsmQuaternionVariable(this PlayMakerFSM fsm, string name) => GetQuaternionVariable(fsm, name);

    /// <inheritdoc cref="GetQuaternionVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetQuaternionVariable(Fsm, string)` instead!")]
    public static FsmQuaternion GetFsmQuaternionVariable(this Fsm fsm, string name) => GetQuaternionVariable(fsm, name);

    /// <inheritdoc cref="GetGameObjectVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetGameObjectVariable(PlayMakerFSM, string)` instead!")]
    public static FsmGameObject GetFsmGameObjectVariable(this PlayMakerFSM fsm, string name) => GetGameObjectVariable(fsm, name);

    /// <inheritdoc cref="GetGameObjectVariable(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `GetGameObjectVariable(Fsm, string)` instead!")]
    public static FsmGameObject GetFsmGameObjectVariable(this Fsm fsm, string name) => GetGameObjectVariable(fsm, name);

    /// <inheritdoc cref="MakeLog(PlayMakerFSM, bool)"/>
    [PublicAPI]
    [Obsolete("Use method `MakeLog(PlayMakerFSM, bool)` instead!")]
    public static void MakeFsmLog(this PlayMakerFSM fsm, bool additionalLogging = false) => MakeLog(fsm);

    /// <inheritdoc cref="MakeLog(Fsm, bool)"/>
    [PublicAPI]
    [Obsolete("Use method `MakeLog(Fsm, bool)` instead!")]
    public static void MakeFsmLog(this Fsm fsm, bool additionalLogging = false) => MakeLog(fsm, additionalLogging);

    /// <inheritdoc cref="Log(PlayMakerFSM)"/>
    [PublicAPI]
    [Obsolete("Use method `Log(PlayMakerFSM)` instead!")]
    public static void LogFsm(this PlayMakerFSM fsm) => Log(fsm);

    /// <inheritdoc cref="Log(Fsm, string)"/>
    [PublicAPI]
    [Obsolete("Use method `Log(Fsm, string)` instead!")]
    public static void LogFsm(this Fsm fsm, string gameObjectName = "") => Log(fsm, gameObjectName);
}