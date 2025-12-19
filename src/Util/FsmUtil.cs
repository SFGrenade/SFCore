using System;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using Logger = Modding.Logger;
using UGameObject = UnityEngine.GameObject;

namespace SFCore.Utils;

/// <summary>
/// Utils specifically for PlayMakerFSMs.
/// </summary>
public static partial class FsmUtil
{
    #region Get a FSM

    /// <summary>
    ///     Locates a PlayMakerFSM by name.
    /// </summary>
    /// <param name="go">The GameObject to search on</param>
    /// <param name="fsmName">The name of the FSM</param>
    /// <returns>The found FSM, null if not found</returns>
    [PublicAPI]
    public static PlayMakerFSM GetFsm(this UGameObject go, string fsmName)
    {
        foreach (PlayMakerFSM fsm in go.GetComponents<PlayMakerFSM>())
        {
            if (fsm.FsmName == fsmName)
            {
                return fsm;
            }
        }
        return null;
    }

    /// <summary>
    ///     Locates a PlayMakerFSM by name and preprocesses it.
    /// </summary>
    /// <param name="go">The GameObject to search on</param>
    /// <param name="fsmName">The name of the FSM</param>
    /// <returns>The found FSM, null if not found</returns>
    [PublicAPI]
    public static PlayMakerFSM GetFsmPreprocessed(this UGameObject go, string fsmName)
    {
        foreach (PlayMakerFSM fsm in go.GetComponents<PlayMakerFSM>())
        {
            if (fsm.FsmName == fsmName)
            {
                fsm.Preprocess();
                return fsm;
            }
        }
        return null;
    }

    #endregion

    #region Get

    private static TVal GetItemFromArray<TVal>(TVal[] origArray, Func<TVal, bool> isItemCheck) where TVal : class
    {
        foreach (TVal item in origArray)
        {
            if (isItemCheck(item))
            {
                return item;
            }
        }
        return null;
    }

    private static TVal[] GetItemsFromArray<TVal>(TVal[] origArray, Func<TVal, bool> isItemCheck) where TVal : class
    {
        int foundItems = 0;
        foreach (TVal item in origArray)
        {
            if (isItemCheck(item))
            {
                foundItems++;
            }
        }
        if (foundItems == origArray.Length)
        {
            return origArray;
        }
        if (foundItems == 0)
        {
            return [];
        }
        TVal[] retArray = new TVal[foundItems];
        int foundProgress = 0;
        foreach (TVal item in origArray)
        {
            if (isItemCheck(item))
            {
                retArray[foundProgress] = item;
                foundProgress++;
            }
        }
        return retArray;
    }

    /// <summary>
    ///     Gets a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>The found state, null if none are found</returns>
    [PublicAPI]
    public static FsmState GetState(this PlayMakerFSM fsm, string stateName) => GetItemFromArray(fsm.FsmStates, x => x.Name == stateName);

    /// <inheritdoc cref="GetState(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmState GetState(this Fsm fsm, string stateName) => GetItemFromArray(fsm.States, x => x.Name == stateName);

    /// <summary>
    ///     Gets a transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the from state</param>
    /// <param name="eventName">The name of the event</param>
    /// <returns>The found transition, null if none are found</returns>
    [PublicAPI]
    public static FsmTransition GetTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetState(stateName)!.GetTransition(eventName);

    /// <inheritdoc cref="GetTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static FsmTransition GetTransition(this Fsm fsm, string stateName, string eventName) => fsm.GetState(stateName)!.GetTransition(eventName);

    /// <inheritdoc cref="GetTransition(PlayMakerFSM, string, string)"/>
    /// <param name="state">The state</param>
    /// <param name="eventName">The name of the event</param>
    [PublicAPI]
    public static FsmTransition GetTransition(this FsmState state, string eventName) => GetItemFromArray(state.Transitions, x => x.EventName == eventName);

    /// <summary>
    ///     Gets a global transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="globalEventName">The name of the event</param>
    /// <returns>The found global transition, null if none are found</returns>
    [PublicAPI]
    public static FsmTransition GetGlobalTransition(this PlayMakerFSM fsm, string globalEventName) => fsm.Fsm.GetGlobalTransition(globalEventName);

    /// <inheritdoc cref="GetGlobalTransition(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmTransition GetGlobalTransition(this Fsm fsm, string globalEventName) => GetItemFromArray(fsm.GlobalTransitions, x => x.EventName == globalEventName);

    /// <summary>
    ///     Gets an action in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of the action that is wanted</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <param name="index">The index of the action</param>
    /// <returns>The action, null if it can't be found</returns>
    [PublicAPI]
    public static TAction GetAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetAction<TAction>(index);

    /// <inheritdoc cref="GetAction{TAction}(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    public static TAction GetAction<TAction>(this Fsm fsm, string stateName, int index) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetAction<TAction>(index);

    /// <inheritdoc cref="GetAction{TAction}(PlayMakerFSM, string, int)"/>
    /// <param name="state">The state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static TAction GetAction<TAction>(this FsmState state, int index) where TAction : FsmStateAction => state.Actions[index] as TAction;

    /// <inheritdoc cref="GetAction{TAction}(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    public static FsmStateAction GetStateAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetState(stateName)!.GetStateAction(index);

    /// <inheritdoc cref="GetAction{TAction}(Fsm, string, int)"/>
    [PublicAPI]
    public static FsmStateAction GetStateAction(this Fsm fsm, string stateName, int index) => fsm.GetState(stateName)!.GetStateAction(index);

    /// <inheritdoc cref="GetAction{TAction}(FsmState, int)"/>
    [PublicAPI]
    public static FsmStateAction GetStateAction(this FsmState state, int index) => state.Actions[index];

    /// <summary>
    ///     Gets an action in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of the action that is wanted</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>An array of actions</returns>
    [PublicAPI]
    public static TAction[] GetActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetActionsOfType<TAction>();

    /// <inheritdoc cref="GetActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static TAction[] GetActionsOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetActionsOfType<TAction>();

    /// <inheritdoc cref="GetActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The state</param>
    [PublicAPI]
    public static TAction[] GetActionsOfType<TAction>(this FsmState state) where TAction : FsmStateAction => Array.ConvertAll(GetItemsFromArray<FsmStateAction>(state.Actions, x => x is TAction), x => (TAction)x);

    /// <summary>
    ///     Gets first action of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to get the actions from</param>
    [PublicAPI]
    public static TAction GetFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetFirstActionOfType<TAction>();

    /// <inheritdoc cref="GetFirstActionOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static TAction GetFirstActionOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetFirstActionOfType<TAction>();

    /// <inheritdoc cref="GetFirstActionOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static TAction GetFirstActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction
    {
        int firstActionIndex = -1;
        for (int i = 0; i < state.Actions.Length; i++)
        {
            if (state.Actions[i] is TAction)
            {
                firstActionIndex = i;
                break;
            }
        }

        if (firstActionIndex == -1)
            return null;
        return state.GetAction<TAction>(firstActionIndex);
    }

    /// <summary>
    ///     Gets last action of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to get the actions from</param>
    [PublicAPI]
    public static TAction GetLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetLastActionOfType<TAction>();

    /// <inheritdoc cref="GetLastActionOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static TAction GetLastActionOfType<TAction>(this Fsm fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetLastActionOfType<TAction>();

    /// <inheritdoc cref="GetLastActionOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static TAction GetLastActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction
    {
        int lastActionIndex = -1;
        for (int i = state.Actions.Length - 1; i >= 0; i--)
        {
            if (state.Actions[i] is TAction)
            {
                lastActionIndex = i;
                break;
            }
        }

        if (lastActionIndex == -1)
            return null;
        return state.GetAction<TAction>(lastActionIndex);
    }

    #endregion Get

    #region Add

    private static TVal[] AddItemToArray<TVal>(TVal[] origArray, TVal value)
    {
        TVal[] newArray = new TVal[origArray.Length + 1];
        origArray.CopyTo(newArray, 0);
        newArray[origArray.Length] = value;
        return newArray;
    }

    /// <summary>
    ///     Adds a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>The created state</returns>
    [PublicAPI]
    public static FsmState AddState(this PlayMakerFSM fsm, string stateName) => fsm.Fsm.AddState(new FsmState(fsm.Fsm) { Name = stateName });

    /// <inheritdoc cref="AddState(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmState AddState(this Fsm fsm, string stateName) => fsm.AddState(new FsmState(fsm) { Name = stateName });

    /// <inheritdoc cref="AddState(PlayMakerFSM, string)"/>
    /// <param name="fsm">The fsm</param>
    /// <param name="state">The state</param>
    [PublicAPI]
    public static FsmState AddState(this PlayMakerFSM fsm, FsmState state) => fsm.Fsm.AddState(state);

    /// <inheritdoc cref="AddState(PlayMakerFSM, FsmState)"/>
    [PublicAPI]
    public static FsmState AddState(this Fsm fsm, FsmState state)
    {
        FsmState[] origStates = fsm.States;
        FsmState[] states = AddItemToArray(origStates, state);
        fsm.States = states;
        // TODO: CHECK: is this necessary? afaik it saves data for each state, which might be needed for copying/applying state changes for other things
        fsm.SaveActions();
        return states[origStates.Length];
    }

    /// <summary>
    ///     Copies a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="fromState">The name of the state to copy</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The new state</returns>
    [PublicAPI]
    public static FsmState CopyState(this PlayMakerFSM fsm, string fromState, string toState) => fsm.Fsm.CopyState(fromState, toState);

    /// <inheritdoc cref="CopyState(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static FsmState CopyState(this Fsm fsm, string fromState, string toState)
    {
        FsmState from = fsm.GetState(fromState);
        // save the actions before we create a new state from this, as the copy constructor will create the new actions from the saved action data from the state we put in, and that is only updated if we call .SaveActions()
        from.SaveActions();
        FsmState copy = new FsmState(from)
        {
            Name = toState
        };
        foreach (FsmTransition transition in copy.Transitions)
        {
            // This is because playmaker is bad, it has to be done extra
            transition.ToFsmState = fsm.GetState(transition.ToState);
        }
        fsm.AddState(copy);
        return copy;
    }

    /// <summary>
    ///     Adds a transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The name of transition event</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The event of the transition</returns>
    [PublicAPI]
    public static FsmEvent AddTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetState(stateName)!.AddTransition(eventName, toState);

    /// <inheritdoc cref="AddTransition(PlayMakerFSM, string, string, string)"/>
    [PublicAPI]
    public static FsmEvent AddTransition(this Fsm fsm, string stateName, string eventName, string toState) => fsm.GetState(stateName)!.AddTransition(eventName, toState);

    /// <inheritdoc cref="AddTransition(PlayMakerFSM, string, string, string)"/>
    /// <param name="state">The state from which the transition starts</param>
    /// <param name="eventName">The name of transition event</param>
    /// <param name="toState">The name of the new state</param>
    [PublicAPI]
    public static FsmEvent AddTransition(this FsmState state, string eventName, string toState)
    {
        var ret = FsmEvent.GetFsmEvent(eventName);
        FsmTransition[] transitions = AddItemToArray(state.Transitions, new FsmTransition
        {
            ToState = toState,
            ToFsmState = state.Fsm.GetState(toState),
            FsmEvent = ret
        });
        state.Transitions = transitions;
        return ret;
    }

    /// <summary>
    ///     Adds a global transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="globalEventName">The name of transition event</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The event of the transition</returns>
    [PublicAPI]
    public static FsmEvent AddGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState) => fsm.Fsm.AddGlobalTransition(globalEventName, toState);

    /// <inheritdoc cref="AddGlobalTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static FsmEvent AddGlobalTransition(this Fsm fsm, string globalEventName, string toState)
    {
        var ret = new FsmEvent(globalEventName) { IsGlobal = true };
        FsmTransition[] transitions = AddItemToArray(fsm.GlobalTransitions, new FsmTransition
        {
            ToState = toState,
            ToFsmState = fsm.GetState(toState),
            FsmEvent = ret
        });
        fsm.GlobalTransitions = transitions;
        return ret;
    }

    /// <summary>
    ///     Adds an action in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is added</param>
    /// <param name="action">The action</param>
    [PublicAPI]
    public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.GetState(stateName)!.AddAction(action);

    /// <inheritdoc cref="AddAction(PlayMakerFSM, string, FsmStateAction)"/>
    [PublicAPI]
    public static void AddAction(this Fsm fsm, string stateName, FsmStateAction action) => fsm.GetState(stateName)!.AddAction(action);

    /// <inheritdoc cref="AddAction(PlayMakerFSM, string, FsmStateAction)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="action">The action</param>
    [PublicAPI]
    public static void AddAction(this FsmState state, FsmStateAction action)
    {
        FsmStateAction[] actions = AddItemToArray(state.Actions, action);
        state.Actions = actions;
        action.Init(state);
    }

    /// <summary>
    ///     Adds a list of actions in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is added</param>
    /// <param name="actions">The actions</param>
    [PublicAPI]
    public static void AddActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions) => fsm.GetState(stateName)!.AddActions(actions);

    /// <inheritdoc cref="AddActions(PlayMakerFSM, string, FsmStateAction[])"/>
    [PublicAPI]
    public static void AddActions(this Fsm fsm, string stateName, params FsmStateAction[] actions) => fsm.GetState(stateName)!.AddActions(actions);

    /// <inheritdoc cref="AddActions(PlayMakerFSM, string, FsmStateAction[])"/>
    /// <param name="state">The fsm state</param>
    /// <param name="actions">The actions</param>
    [PublicAPI]
    public static void AddActions(this FsmState state, params FsmStateAction[] actions)
    {
        foreach (FsmStateAction action in actions)
        {
            state.AddAction(action);
        }
    }

    /// <summary>
    ///     Adds a method in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked with the action as the parameter</param>
    [PublicAPI]
    public static void AddMethod(this PlayMakerFSM fsm, string stateName, Action<FsmStateAction> method) => fsm.GetState(stateName)!.AddMethod(method);

    /// <inheritdoc cref="AddMethod(PlayMakerFSM, string, Action{FsmStateAction})"/>
    [PublicAPI]
    public static void AddMethod(this Fsm fsm, string stateName, Action<FsmStateAction> method) => fsm.GetState(stateName)!.AddMethod(method);

    /// <inheritdoc cref="AddMethod(PlayMakerFSM, string, Action{FsmStateAction})"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked with the action as the parameter</param>
    [PublicAPI]
    public static void AddMethod(this FsmState state, Action<FsmStateAction> method)
    {
        FunctionAction<FsmStateAction> action = new FunctionAction<FsmStateAction> { action = method };
        action.arg = action;
        state.AddAction(action);
    }

    /// <summary>
    ///     Adds a method with a parameter in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked with the finish action as the parameter</param>
    [PublicAPI]
    public static void AddLambdaMethod(this PlayMakerFSM fsm, string stateName, Action<Action> method) => fsm.GetState(stateName)!.AddLambdaMethod(method);

    /// <inheritdoc cref="AddLambdaMethod(PlayMakerFSM, string, Action{Action})"/>
    [PublicAPI]
    public static void AddLambdaMethod(this Fsm fsm, string stateName, Action<Action> method) => fsm.GetState(stateName)!.AddLambdaMethod(method);

    /// <inheritdoc cref="AddLambdaMethod(PlayMakerFSM, string, Action{Action})"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked with the finish action as the parameter</param>
    [PublicAPI]
    public static void AddLambdaMethod(this FsmState state, Action<Action> method)
    {
        FunctionAction<Action> action = new FunctionAction<Action> { action = method };
        action.arg = action.Finish;
        state.AddAction(action);
    }

    #endregion Add

    #region Insert

    private static TVal[] InsertItemIntoArray<TVal>(TVal[] origArray, TVal value, int index)
    {
        int origArrayCount = origArray.Length;
        if (index < 0 || index > (origArrayCount + 1))
        {
            throw new ArgumentOutOfRangeException($"Index {index} was out of range for array with length {origArrayCount}!");
        }
        TVal[] actions = new TVal[origArrayCount + 1];
        int i;
        for (i = 0; i < index; i++)
        {
            actions[i] = origArray[i];
        }
        actions[index] = value;
        for (i = index; i < origArrayCount; i++)
        {
            actions[i + 1] = origArray[i];
        }
        return actions;
    }

    /// <summary>
    ///     Inserts an action in a PlayMakerFSM.  
    ///     Trying to insert an action out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is added</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetState(stateName)!.InsertAction(index, action);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    public static void InsertAction(this PlayMakerFSM fsm, string stateName, int index, FsmStateAction action) => fsm.GetState(stateName)!.InsertAction(index, action);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    public static void InsertAction(this Fsm fsm, string stateName, FsmStateAction action, int index) => fsm.GetState(stateName)!.InsertAction(index, action);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    public static void InsertAction(this Fsm fsm, string stateName, int index, FsmStateAction action) => fsm.GetState(stateName)!.InsertAction(index, action);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertAction(this FsmState state, FsmStateAction action, int index) => state.InsertAction(index, action);

    /// <inheritdoc cref="InsertAction(FsmState, FsmStateAction, int)"/>
    [PublicAPI]
    public static void InsertAction(this FsmState state, int index, FsmStateAction action)
    {
        FsmStateAction[] actions = InsertItemIntoArray(state.Actions, action, index);
        state.Actions = actions;
        action.Init(state);
    }

    /// <summary>
    ///     Inserts a set of actions in a PlayMakerFSM.  
    ///     Trying to insert an action out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the actions are added</param>
    /// <param name="actions">The actions</param>
    /// <param name="index">The index to place the actions in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertActions(this PlayMakerFSM fsm, string stateName, int index, params FsmStateAction[] actions) => fsm.GetState(stateName)!.InsertActions(index, actions);

    /// <inheritdoc cref="InsertActions(PlayMakerFSM, string, int, FsmStateAction[])"/>
    [PublicAPI]
    public static void InsertActions(this Fsm fsm, string stateName, int index, params FsmStateAction[] actions) => fsm.GetState(stateName)!.InsertActions(index, actions);

    /// <inheritdoc cref="InsertActions(PlayMakerFSM, string, int, FsmStateAction[])"/>
    /// <param name="state">The fsm state</param>
    /// <param name="actions">The actions</param>
    /// <param name="index">The index to place the actions in</param>
    [PublicAPI]
    public static void InsertActions(this FsmState state, int index, params FsmStateAction[] actions)
    {
        foreach (FsmStateAction action in actions)
        {
            state.InsertAction(action, index);
            index++;  // preserves order
        }
    }

    /// <summary>
    ///     Inserts a method in a PlayMakerFSM.
    ///     Trying to insert a method out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertMethod(this PlayMakerFSM fsm, string stateName, Action<FsmStateAction> method, int index) => fsm.GetState(stateName)!.InsertMethod(index, method);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action{FsmStateAction}, int)"/>
    [PublicAPI]
    public static void InsertMethod(this PlayMakerFSM fsm, string stateName, int index, Action<FsmStateAction> method) => fsm.GetState(stateName)!.InsertMethod(index, method);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action{FsmStateAction}, int)"/>
    [PublicAPI]
    public static void InsertMethod(this Fsm fsm, string stateName, Action<FsmStateAction> method, int index) => fsm.GetState(stateName)!.InsertMethod(index, method);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action{FsmStateAction}, int)"/>
    [PublicAPI]
    public static void InsertMethod(this Fsm fsm, string stateName, int index, Action<FsmStateAction> method) => fsm.GetState(stateName)!.InsertMethod(index, method);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action{FsmStateAction}, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertMethod(this FsmState state, Action<FsmStateAction> method, int index) => state.InsertMethod(index, method);

    /// <inheritdoc cref="InsertMethod(FsmState, Action{FsmStateAction}, int)"/>
    [PublicAPI]
    public static void InsertMethod(this FsmState state, int index, Action<FsmStateAction> method)
    {
        FunctionAction<FsmStateAction> action = new FunctionAction<FsmStateAction> { action = method };
        action.arg = action;
        state.InsertAction(action, index);
    }

    /// <summary>
    ///     Inserts a method with a parameter in a PlayMakerFSM.
    ///     Trying to insert a method out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertLambdaMethod(this PlayMakerFSM fsm, string stateName, Action<Action> method, int index) => fsm.GetState(stateName)!.InsertLambdaMethod(index, method);

    /// <inheritdoc cref="InsertLambdaMethod(PlayMakerFSM, string, Action{Action}, int)"/>
    [PublicAPI]
    public static void InsertLambdaMethod(this PlayMakerFSM fsm, string stateName, int index, Action<Action> method) => fsm.GetState(stateName)!.InsertLambdaMethod(index, method);

    /// <inheritdoc cref="InsertLambdaMethod(PlayMakerFSM, string, Action{Action}, int)"/>
    [PublicAPI]
    public static void InsertLambdaMethod(this Fsm fsm, string stateName, Action<Action> method, int index) => fsm.GetState(stateName)!.InsertLambdaMethod(index, method);

    /// <inheritdoc cref="InsertLambdaMethod(PlayMakerFSM, string, Action{Action}, int)"/>
    [PublicAPI]
    public static void InsertLambdaMethod(this Fsm fsm, string stateName, int index, Action<Action> method) => fsm.GetState(stateName)!.InsertLambdaMethod(index, method);

    /// <inheritdoc cref="InsertLambdaMethod(PlayMakerFSM, string, Action{Action}, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertLambdaMethod(this FsmState state, Action<Action> method, int index) => state.InsertLambdaMethod(index, method);

    /// <inheritdoc cref="InsertLambdaMethod(FsmState, Action{Action}, int)"/>
    [PublicAPI]
    public static void InsertLambdaMethod(this FsmState state, int index, Action<Action> method)
    {
        FunctionAction<Action> action = new FunctionAction<Action> { action = method };
        action.arg = action.Finish;
        state.InsertAction(action, index);
    }

    /// <summary>
    /// Insert a method to run before the specified FsmStateAction.
    /// </summary>
    /// <param name="action">The action to insert before.</param>
    /// <param name="method">The method to execute.
    /// The argument will be the FsmStateAction which is being added.</param>
    [PublicAPI]
    public static void InsertMethodBefore(this FsmStateAction action, Action<FsmStateAction> method)
    {
        FsmState state = action.State;
        int idx = Array.IndexOf(state.Actions, action);
        state.InsertMethod(idx, method);
    }

    /// <summary>
    /// Insert a method to run after the specified FsmStateAction.
    /// </summary>
    /// <param name="action">The action to insert after.</param>
    /// <param name="method">The method to execute.
    /// The argument will be the FsmStateAction which is being added.</param>
    [PublicAPI]
    public static void InsertMethodAfter(this FsmStateAction action, Action<FsmStateAction> method)
    {
        FsmState state = action.State;
        int idx = Array.IndexOf(state.Actions, action);
        state.InsertMethod(idx + 1, method);
    }

    /// <summary>
    /// Insert an action to run before the specified FsmStateAction.
    /// </summary>
    /// <param name="action">The action to insert before.</param>
    /// <param name="newAction">The action to add.</param>
    [PublicAPI]
    public static void InsertActionBefore(this FsmStateAction action, FsmStateAction newAction)
    {
        FsmState state = action.State;
        int idx = Array.IndexOf(state.Actions, action);
        state.InsertAction(idx, newAction);
    }

    /// <summary>
    /// Insert an action to run after the specified FsmStateAction.
    /// </summary>
    /// <param name="action">The action to insert after.</param>
    /// <param name="newAction">The action to add.</param>
    [PublicAPI]
    public static void InsertActionAfter(this FsmStateAction action, FsmStateAction newAction)
    {
        FsmState state = action.State;
        int idx = Array.IndexOf(state.Actions, action);
        state.InsertAction(idx + 1, newAction);
    }

    #endregion Insert

    #region Replace

    /// <summary>
    ///     Replaces an action in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is replaced</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetState(stateName)!.ReplaceAction(index, action);

    /// <inheritdoc cref="ReplaceAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, int index, FsmStateAction action) => fsm.GetState(stateName)!.ReplaceAction(index, action);

    /// <inheritdoc cref="ReplaceAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    public static void ReplaceAction(this Fsm fsm, string stateName, FsmStateAction action, int index) => fsm.GetState(stateName)!.ReplaceAction(index, action);

    /// <inheritdoc cref="ReplaceAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    [PublicAPI]
    public static void ReplaceAction(this Fsm fsm, string stateName, int index, FsmStateAction action) => fsm.GetState(stateName)!.ReplaceAction(index, action);

    /// <inheritdoc cref="ReplaceAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    /// <param name="state">The state in which the action is replaced</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static void ReplaceAction(this FsmState state, FsmStateAction action, int index) => state.ReplaceAction(index, action);

    /// <inheritdoc cref="ReplaceAction(FsmState, FsmStateAction, int)"/>
    [PublicAPI]
    public static void ReplaceAction(this FsmState state, int index, FsmStateAction action)
    {
        state.Actions[index] = action;
        action.Init(state);
    }

    /// <summary>
    ///     Replaces all actions in a PlayMakerFSM state.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the actions are to be replaced</param>
    /// <param name="actions">The new actions of the state</param>
    [PublicAPI]
    public static void ReplaceAllActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions) => fsm.GetState(stateName)!.ReplaceAllActions(actions);

    /// <inheritdoc cref="ReplaceAllActions(PlayMakerFSM, string, FsmStateAction[])"/>
    [PublicAPI]
    public static void ReplaceAllActions(this Fsm fsm, string stateName, params FsmStateAction[] actions) => fsm.GetState(stateName)!.ReplaceAllActions(actions);

    /// <inheritdoc cref="ReplaceAllActions(PlayMakerFSM, string, FsmStateAction[])"/>
    /// <param name="state">The fsm state</param>
    /// <param name="actions">The action</param>
    [PublicAPI]
    public static void ReplaceAllActions(this FsmState state, params FsmStateAction[] actions)
    {
        state.Actions = actions;
        foreach (FsmStateAction action in actions)
        {
            action.Init(state);
        }
    }

    #endregion Replace

    #region Change

    /// <summary>
    ///     Changes a transition endpoint in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The event of the transition</param>
    /// <param name="toState">The new endpoint of the transition</param>
    /// <returns>bool that indicates whether the change was successful</returns>
    [PublicAPI]
    public static bool ChangeTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetState(stateName)!.ChangeTransition(eventName, toState);

    /// <inheritdoc cref="ChangeTransition(PlayMakerFSM, string, string, string)"/>
    [PublicAPI]
    public static bool ChangeTransition(this Fsm fsm, string stateName, string eventName, string toState) => fsm.GetState(stateName)!.ChangeTransition(eventName, toState);

    /// <inheritdoc cref="ChangeTransition(PlayMakerFSM, string, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="eventName">The event of the transition</param>
    /// <param name="toState">The new endpoint of the transition</param>
    [PublicAPI]
    public static bool ChangeTransition(this FsmState state, string eventName, string toState)
    {
        var transition = state.GetTransition(eventName);
        if (transition == null)
        {
            return false;
        }
        transition.ToState = toState;
        transition.ToFsmState = state.Fsm.GetState(toState);
        return true;
    }

    /// <summary>
    ///     Changes a global transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="globalEventName">The name of transition event</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>bool that indicates whether the change was successful</returns>
    [PublicAPI]
    public static bool ChangeGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState) => fsm.Fsm.ChangeGlobalTransition(globalEventName, toState);

    /// <inheritdoc cref="ChangeGlobalTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static bool ChangeGlobalTransition(this Fsm fsm, string globalEventName, string toState)
    {
        var transition = fsm.GetGlobalTransition(globalEventName);
        if (transition == null)
        {
            return false;
        }
        transition.ToState = toState;
        transition.ToFsmState = fsm.GetState(toState);
        return true;
    }

    #endregion Change

    #region Remove

    private static TVal[] RemoveItemsFromArray<TVal>(TVal[] origArray, Func<TVal, bool> shouldBeRemovedCallback)
    {
        int amountOfRemoved = 0;
        foreach (TVal tmpValue in origArray)
        {
            if (shouldBeRemovedCallback(tmpValue))
            {
                amountOfRemoved++;
            }
        }
        if (amountOfRemoved == 0)
        {
            return origArray;
        }
        TVal[] newArray = new TVal[origArray.Length - amountOfRemoved];
        for (int i = origArray.Length - 1; i >= 0; i--)
        {
            TVal tmpValue = origArray[i];
            if (shouldBeRemovedCallback(tmpValue))
            {
                amountOfRemoved--;
                continue;
            }
            newArray[i - amountOfRemoved] = tmpValue;
        }
        return newArray;
    }

    /// <summary>
    ///     Removes a state in a PlayMakerFSM.  
    ///     Trying to remove a state that doesn't exist will result in the states not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove</param>
    [PublicAPI]
    public static void RemoveState(this PlayMakerFSM fsm, string stateName) => fsm.Fsm.RemoveState(stateName);

    /// <inheritdoc cref="RemoveState(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveState(this Fsm fsm, string stateName) => fsm.States = RemoveItemsFromArray(fsm.States, x => x.Name == stateName);

    /// <summary>
    ///     Removes a transition in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetState(stateName)!.RemoveTransition(eventName);

    /// <inheritdoc cref="RemoveTransition(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static void RemoveTransition(this Fsm fsm, string stateName, string eventName) => fsm.GetState(stateName)!.RemoveTransition(eventName);

    /// <inheritdoc cref="RemoveTransition(PlayMakerFSM, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="eventName">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransition(this FsmState state, string eventName) => state.Transitions = RemoveItemsFromArray(state.Transitions, x => x.EventName == eventName);

    /// <summary>
    ///     Removes a global transition in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="globalEventName">The event of the transition</param>
    [PublicAPI]
    public static void RemoveGlobalTransition(this PlayMakerFSM fsm, string globalEventName) => fsm.Fsm.RemoveGlobalTransition(globalEventName);

    /// <inheritdoc cref="RemoveGlobalTransition(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveGlobalTransition(this Fsm fsm, string globalEventName)
    {
        fsm.GlobalTransitions = RemoveItemsFromArray(fsm.GlobalTransitions, x => x.EventName == globalEventName);
    }

    /// <summary>
    ///     Removes all transitions to a specified transition in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="toState">The target of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this PlayMakerFSM fsm, string toState) => fsm.Fsm.RemoveTransitionsTo(toState);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveTransitionsTo(this Fsm fsm, string toState)
    {
        foreach (FsmState state in fsm.States)
        {
            state.RemoveTransitionsTo(toState);
        }
    }

    /// <summary>
    ///     Removes all transitions from a state to another specified state in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="toState">The target of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this PlayMakerFSM fsm, string stateName, string toState) => fsm.GetState(stateName)!.RemoveTransitionsTo(toState);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string, string)"/>
    [PublicAPI]
    public static void RemoveTransitionsTo(this Fsm fsm, string stateName, string toState) => fsm.GetState(stateName)!.RemoveTransitionsTo(toState);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="toState">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this FsmState state, string toState) => state.Transitions = RemoveItemsFromArray(state.Transitions, x => x.ToState == toState);

    /// <summary>
    ///     Removes all transitions from a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    [PublicAPI]
    public static void RemoveTransitions(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.RemoveTransitions();

    /// <inheritdoc cref="RemoveTransitions(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveTransitions(this Fsm fsm, string stateName) => fsm.GetState(stateName)!.RemoveTransitions();

    /// <inheritdoc cref="RemoveTransitions(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveTransitions(this FsmState state) => state.Transitions = [];

    /// <summary>
    ///     Removes an action in a PlayMakerFSM.  
    ///     Trying to remove an action that doesn't exist will result in the actions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state with the action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool RemoveAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetState(stateName)!.RemoveAction(index);

    /// <inheritdoc cref="RemoveAction(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    public static bool RemoveAction(this Fsm fsm, string stateName, int index) => fsm.GetState(stateName)!.RemoveAction(index);

    /// <inheritdoc cref="RemoveAction(PlayMakerFSM, string, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool RemoveAction(this FsmState state, int index)
    {
        FsmStateAction[] origActions = state.Actions;
        if (index < 0 || index >= origActions.Length)
        {
            return false;
        }
        FsmStateAction[] newActions = new FsmStateAction[origActions.Length - 1];
        int newActionsCount = newActions.Length;
        int i;
        for (i = 0; i < index; i++)
        {
            newActions[i] = origActions[i];
        }
        for (i = index; i < newActionsCount; i++)
        {
            newActions[i] = origActions[i + 1];
        }

        state.Actions = newActions;
        return true;
    }

    /// <summary>
    ///     Removes all actions of a given type in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this PlayMakerFSM fsm) => fsm.Fsm.RemoveActionsOfType<TAction>();

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM)"/>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this Fsm fsm)
    {
        foreach (FsmState state in fsm.States)
        {
            state.RemoveActionsOfType<TAction>();
        }
    }

    /// <summary>
    ///     Removes all actions of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove the actions from</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.RemoveActionsOfType<TAction>();

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this Fsm fsm, string stateName) => fsm.GetState(stateName)!.RemoveActionsOfType<TAction>();

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this FsmState state) => state.Actions = RemoveItemsFromArray(state.Actions, x => x is TAction);

    /// <summary>
    ///     Removes first action of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove the actions from</param>
    [PublicAPI]
    public static void RemoveFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.RemoveFirstActionOfType<TAction>();

    /// <inheritdoc cref="RemoveFirstActionOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveFirstActionOfType<TAction>(this Fsm fsm, string stateName) => fsm.GetState(stateName)!.RemoveFirstActionOfType<TAction>();

    /// <inheritdoc cref="RemoveFirstActionOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveFirstActionOfType<TAction>(this FsmState state)
    {
        int firstActionIndex = -1;
        for (int i = 0; i < state.Actions.Length; i++)
        {
            if (state.Actions[i] is TAction)
            {
                firstActionIndex = i;
                break;
            }
        }

        if (firstActionIndex == -1)
            return;
        state.RemoveAction(firstActionIndex);
    }

    /// <summary>
    ///     Removes last action of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove the actions from</param>
    [PublicAPI]
    public static void RemoveLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.RemoveLastActionOfType<TAction>();

    /// <inheritdoc cref="RemoveLastActionOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void RemoveLastActionOfType<TAction>(this Fsm fsm, string stateName) => fsm.GetState(stateName)!.RemoveLastActionOfType<TAction>();

    /// <inheritdoc cref="RemoveLastActionOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveLastActionOfType<TAction>(this FsmState state)
    {
        int lastActionIndex = -1;
        for (int i = state.Actions.Length - 1; i >= 0; i--)
        {
            if (state.Actions[i] is TAction)
            {
                lastActionIndex = i;
                break;
            }
        }

        if (lastActionIndex == -1)
            return;
        state.RemoveAction(lastActionIndex);
    }

    #endregion Remove

    #region Disable

    /// <summary>
    ///     Disables an action in a PlayMakerFSM.  
    ///     Trying to disable an action that doesn't exist will result in the actions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state with the action</param>
    /// <param name="index">The index of the action</param>
    /// <returns>bool that indicates whether the disabling was successful</returns>
    [PublicAPI]
    public static bool DisableAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetState(stateName)!.DisableAction(index);

    /// <inheritdoc cref="DisableAction(PlayMakerFSM, string, int)"/>
    [PublicAPI]
    public static bool DisableAction(this Fsm fsm, string stateName, int index) => fsm.GetState(stateName)!.DisableAction(index);

    /// <inheritdoc cref="DisableAction(PlayMakerFSM, string, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool DisableAction(this FsmState state, int index)
    {
        if (index < 0 || index >= state.Actions.Length)
        {
            return false;
        }
        state.Actions[index].Enabled = false;
        return true;
    }

    /// <summary>
    ///     Disables a set of actions in a PlayMakerFSM.  
    ///     Trying to disable an action that doesn't exist will result in the actions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state with the action</param>
    /// <param name="indices">The indices of the action</param>
    /// <returns>bool that indicates whether all the disablings were successful</returns>
    [PublicAPI]
    public static bool DisableActions(this PlayMakerFSM fsm, string stateName, params int[] indices) => fsm.GetState(stateName)!.DisableActions(indices);

    /// <inheritdoc cref="DisableActions(PlayMakerFSM, string, int[])"/>
    [PublicAPI]
    public static bool DisableActions(this Fsm fsm, string stateName, params int[] indices) => fsm.GetState(stateName)!.DisableActions(indices);

    /// <inheritdoc cref="DisableActions(PlayMakerFSM, string, int[])"/>
    /// <param name="state">The fsm state</param>
    /// <param name="indices">The indices of the action</param>
    [PublicAPI]
    public static bool DisableActions(this FsmState state, params int[] indices)
    {
        bool ret = true;
        foreach (int index in indices)
        {
            ret = ret && state.DisableAction(index);
        }
        return ret;
    }

    /// <summary>
    ///     Disables all actions of a given type in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of actions to disable</typeparam>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this PlayMakerFSM fsm) => fsm.Fsm.DisableActionsOfType<TAction>();

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM)"/>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this Fsm fsm)
    {
        foreach (FsmState state in fsm.States)
        {
            state.DisableActionsOfType<TAction>();
        }
    }

    /// <summary>
    ///     Disables all actions of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to disable</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to disable the actions from</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.DisableActionsOfType<TAction>();

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this Fsm fsm, string stateName) => fsm.GetState(stateName)!.DisableActionsOfType<TAction>();

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this FsmState state)
    {
        foreach (FsmStateAction action in state.Actions)
        {
            if (action is TAction)
            {
                action.Enabled = false;
            }
        }
    }

    #endregion Disable

    #region FSM Variables

    private static TVar[] MakeNewVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
    {
        TVar[] newArray = new TVar[orig.Length + 1];
        orig.CopyTo(newArray, 0);
        newArray[orig.Length] = new TVar { Name = name };
        return newArray;
    }

    /// <summary>
    ///     Adds a fsm variable in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the new variable</param>
    /// <returns>The newly created variable</returns>
    [PublicAPI]
    public static FsmFloat AddFloatVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddFloatVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmFloat AddFloatVariable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.FloatVariables, name);
        fsm.Variables.FloatVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt AddIntVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddIntVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt AddIntVariable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.IntVariables, name);
        fsm.Variables.IntVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool AddBoolVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddBoolVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool AddBoolVariable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.BoolVariables, name);
        fsm.Variables.BoolVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString AddStringVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddStringVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString AddStringVariable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.StringVariables, name);
        fsm.Variables.StringVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 AddVector2Variable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddVector2Variable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 AddVector2Variable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.Vector2Variables, name);
        fsm.Variables.Vector2Variables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 AddVector3Variable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddVector3Variable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 AddVector3Variable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.Vector3Variables, name);
        fsm.Variables.Vector3Variables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor AddColorVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddColorVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor AddColorVariable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.ColorVariables, name);
        fsm.Variables.ColorVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect AddRectVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddRectVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect AddRectVariable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.RectVariables, name);
        fsm.Variables.RectVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion AddQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddQuaternionVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion AddQuaternionVariable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.QuaternionVariables, name);
        fsm.Variables.QuaternionVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject AddGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.AddGameObjectVariable(name);

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject AddGameObjectVariable(this Fsm fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.Variables.GameObjectVariables, name);
        fsm.Variables.GameObjectVariables = tmp;
        return tmp[tmp.Length - 1];
    }

    private static TVar FindInVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
    {
        foreach (TVar item in orig)
        {
            if (item.Name == name)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    ///     Finds a fsm variable in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the variable</param>
    /// <returns>The variable, null if not found</returns>
    [PublicAPI]
    public static FsmFloat FindFloatVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindFloatVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmFloat FindFloatVariable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.FloatVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt FindIntVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindIntVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt FindIntVariable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.IntVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool FindBoolVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindBoolVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool FindBoolVariable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.BoolVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString FindStringVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindStringVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString FindStringVariable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.StringVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 FindVector2Variable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindVector2Variable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 FindVector2Variable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.Vector2Variables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 FindVector3Variable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindVector3Variable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 FindVector3Variable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.Vector3Variables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor FindColorVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindColorVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor FindColorVariable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.ColorVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect FindRectVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindRectVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect FindRectVariable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.RectVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion FindQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindQuaternionVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion FindQuaternionVariable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.QuaternionVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject FindGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.FindGameObjectVariable(name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject FindGameObjectVariable(this Fsm fsm, string name) => FindInVariableArray(fsm.Variables.GameObjectVariables, name);

    /// <summary>
    ///     Gets a fsm variable in a PlayMakerFSM. Creates a new one if none with the name are present.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the variable</param>
    /// <returns>The variable</returns>
    [PublicAPI]
    public static FsmFloat GetFloatVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetFloatVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmFloat GetFloatVariable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindFloatVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFloatVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt GetIntVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetIntVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt GetIntVariable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindIntVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddIntVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool GetBoolVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetBoolVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool GetBoolVariable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindBoolVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddBoolVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString GetStringVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetStringVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString GetStringVariable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindStringVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddStringVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 GetVector2Variable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetVector2Variable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 GetVector2Variable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindVector2Variable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddVector2Variable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 GetVector3Variable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetVector3Variable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 GetVector3Variable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindVector3Variable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddVector3Variable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor GetColorVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetColorVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor GetColorVariable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindColorVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddColorVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect GetRectVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetRectVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect GetRectVariable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindRectVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddRectVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion GetQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetQuaternionVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion GetQuaternionVariable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindQuaternionVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddQuaternionVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject GetGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.Fsm.GetGameObjectVariable(name);

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject GetGameObjectVariable(this Fsm fsm, string name)
    {
        var tmp = fsm.FindGameObjectVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddGameObjectVariable(name);
    }

    #endregion FSM Variables

    #region Log

    /// <summary>
    ///     Adds actions to a PlayMakerFSM so it gives a log message before and after every single normal action.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="additionalLogging">Flag if, additionally, every log should also log the state of all fsm variables</param>
    [PublicAPI]
    public static void MakeLog(this PlayMakerFSM fsm, bool additionalLogging = false) => fsm.Fsm.MakeLog(additionalLogging);

    /// <inheritdoc cref="MakeLog(PlayMakerFSM, bool)"/>
    [PublicAPI]
    public static void MakeLog(this Fsm fsm, bool additionalLogging = false)
    {
        foreach (var s in fsm.States)
        {
            for (int i = s.Actions.Length - 1; i >= 0; i--)
            {
                fsm.InsertAction(s.Name, new LogAction { text = $"{i}" }, i);
                if (additionalLogging)
                {
                    fsm.InsertMethod(s.Name, (_) =>
                    {
                        var fsmVars = fsm.Variables;
                        foreach (var variable in fsmVars.FloatVariables)
                            InternalLogger.Log($"[{fsm.Name}]:[FloatVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.IntVariables)
                            InternalLogger.Log($"[{fsm.Name}]:[IntVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.BoolVariables)
                            InternalLogger.Log($"[{fsm.Name}]:[BoolVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.StringVariables)
                            InternalLogger.Log($"[{fsm.Name}]:[StringVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.Vector2Variables)
                            InternalLogger.Log($"[{fsm.Name}]:[Vector2Variables] - '{variable.Name}': '({variable.Value.x}, {variable.Value.y})'");
                        foreach (var variable in fsmVars.Vector3Variables)
                            InternalLogger.Log($"[{fsm.Name}]:[Vector3Variables] - '{variable.Name}': '({variable.Value.x}, {variable.Value.y}, {variable.Value.z})'");
                        foreach (var variable in fsmVars.ColorVariables)
                            InternalLogger.Log($"[{fsm.Name}]:[ColorVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.RectVariables)
                            InternalLogger.Log($"[{fsm.Name}]:[RectVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.QuaternionVariables)
                            InternalLogger.Log($"[{fsm.Name}]:[QuaternionVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.GameObjectVariables)
                            InternalLogger.Log($"[{fsm.Name}]:[GameObjectVariables] - '{variable.Name}': '{variable.Value}'");
                    }, i + 1);
                }
            }
        }
    }

    /// <summary>
    ///     Logs the fsm and its states and transitions.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void Log(this PlayMakerFSM fsm) => fsm.Fsm.Log(fsm.gameObject.name);

    /// <inheritdoc cref="Log(PlayMakerFSM)"/>
    [PublicAPI]
    public static void Log(this Fsm fsm, string gameObjectName = "")
    {
        Log($"FSM \"{fsm.Name}\" on \"{gameObjectName}\"");
        Log($"\t{fsm.States.Length} States");
        foreach (var s in fsm.States)
        {
            Log($"\t\tState \"{s.Name}\"");
            foreach (var t in s.Transitions)
            {
                Log($"\t\t\t-> \"{t.ToState}\" via \"{t.EventName}\"");
            }
        }
        Log($"\t{fsm.GlobalTransitions.Length} Global Transitions");
        foreach (var t in fsm.GlobalTransitions)
        {
            Log($"\t\tGlobal Transition \"{t.EventName}\" to \"{t.ToState}\"");
        }
        Log($"\tVariables");
        var fsmVar = fsm.Variables;
        foreach (var t in fsmVar.ArrayVariables)
        {
            Log($"\t\tArray \"{t.Name}\": \"{String.Join(", ", t.objectReferences as object[])}\"");
        }
        foreach (var t in fsmVar.BoolVariables)
        {
            Log($"\t\tBool \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.ColorVariables)
        {
            Log($"\t\tColor \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.EnumVariables)
        {
            Log($"\t\tEnum \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.FloatVariables)
        {
            Log($"\t\tFloat \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.GameObjectVariables)
        {
            Log($"\t\tGameObject \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.IntVariables)
        {
            Log($"\t\tInt \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.MaterialVariables)
        {
            Log($"\t\tMaterial \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.ObjectVariables)
        {
            Log($"\t\tObject \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.QuaternionVariables)
        {
            Log($"\t\tQuaternion \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.RectVariables)
        {
            Log($"\t\tRect \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.StringVariables)
        {
            Log($"\t\tString \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.TextureVariables)
        {
            Log($"\t\tTexture \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.Vector2Variables)
        {
            Log($"\t\tVector2 \"{t.Name}\": {t.Value}");
        }
        foreach (var t in fsmVar.Vector3Variables)
        {
            Log($"\t\tVector3 \"{t.Name}\": {t.Value}");
        }
    }

    private static void Log(string goName, string fsmName, string part, string msg)
    {
        Logger.LogDebug($"[{goName}]:[{fsmName}]:[{part}] - {msg}");
        UnityEngine.Debug.Log($"[{goName}]:[{fsmName}]:[{part}] - {msg}");
    }
    private static void Log(string msg)
    {
        Logger.LogDebug($"[SFCore]:[Util]:[FsmUtil]:{msg}");
        UnityEngine.Debug.Log($"[SFCore]:[Util]:[FsmUtil]:{msg}");
    }
    private static void Log(object msg)
    {
        Log($"{msg}");
    }

    #endregion Log
}