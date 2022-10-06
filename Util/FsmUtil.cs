﻿using System;
using HutongGames.PlayMaker;
using Logger = Modding.Logger;

namespace SFCore.Utils
{
    /// <summary>
    ///     Utils specifically for PlayMakerFSMs.
    /// </summary>
    public static class FsmUtil
    {
        #region Get

        /// <summary>
        ///     Gets a state in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state</param>
        /// <returns>The found state, null if none are found.</returns>
        public static FsmState GetState(this PlayMakerFSM fsm, string stateName) => fsm.GetFsmState(stateName);
        /// <inheritdoc cref="GetState(PlayMakerFSM, string)"/>
        public static FsmState GetFsmState(this PlayMakerFSM fsm, string stateName)
        {
            var fsmStates = fsm.FsmStates;
            int fsmStatesCount = fsmStates.Length;
            int i;
            for (i = 0; i < fsmStatesCount; i++)
            {
                if (fsmStates[i].Name == stateName)
                {
                    return fsmStates[i];
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets a transtition in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the from state</param>
        /// <param name="eventName">The name of the event</param>
        /// <returns>The found transtition, null if none are found.</returns>
        public static FsmTransition GetTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetFsmTransition(stateName, eventName);
        /// <inheritdoc cref="GetTransition(PlayMakerFSM, string, string)"/>
        public static FsmTransition GetFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetFsmState(stateName).GetFsmTransition(eventName);
        /// <inheritdoc cref="GetTransition(PlayMakerFSM, string, string)"/>
        /// <param name="state">The state</param>
        /// <param name="eventName">The name of the event</param>
        public static FsmTransition GetTransition(this FsmState state, string eventName) => state.GetFsmTransition(eventName);
        /// <inheritdoc cref="GetTransition(FsmState, string)"/>
        public static FsmTransition GetFsmTransition(this FsmState state, string eventName)
        {
            var transitions = state.Transitions;
            var transitionsCount = transitions.Length;
            int i;
            for (i = 0; i < transitionsCount; i++)
            {
                if (transitions[i].EventName == eventName)
                {
                    return transitions[i];
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets an action in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state</param>
        /// <param name="index">The index of the action</param>
        /// <returns>The action.</returns>
        public static TAction GetAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction => fsm.GetFsmAction<TAction>(stateName, index);
        /// <inheritdoc cref="GetAction{TAction}(PlayMakerFSM, string, int)"/>
        public static TAction GetFsmAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction
        {
            return (TAction) fsm.GetFsmState(stateName).Actions[index];
        }

        #endregion

        #region Add

        /// <summary>
        ///     Adds a state in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state</param>
        /// <returns>The created state.</returns>
        public static FsmState AddState(this PlayMakerFSM fsm, string stateName) => fsm.AddFsmState(stateName);
        /// <inheritdoc cref="AddState(PlayMakerFSM, string)"/>
        public static FsmState AddFsmState(this PlayMakerFSM fsm, string stateName) => fsm.AddFsmState(new FsmState(fsm.Fsm) { Name = stateName });
        /// <inheritdoc cref="AddState(PlayMakerFSM, string)"/>
        /// <param name="fsm">The fsm</param>
        /// <param name="state">The state</param>
        public static FsmState AddState(this PlayMakerFSM fsm, FsmState state) => fsm.AddFsmState(state);
        /// <inheritdoc cref="AddState(PlayMakerFSM, FsmState)"/>
        public static FsmState AddFsmState(this PlayMakerFSM fsm, FsmState state)
        {
            FsmState[] origStates = fsm.FsmStates;
            FsmState[] states = new FsmState[origStates.Length + 1];
            origStates.CopyTo(states, 0);
            states[origStates.Length] = state;
            fsm.Fsm.States = states;
            return states[origStates.Length];
        }

        /// <summary>
        ///     Copies a state in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="fromState">The name of the state to copy</param>
        /// <param name="toState">The name of the new state</param>
        /// <returns>The new state.</returns>
        public static FsmState CopyState(this PlayMakerFSM fsm, string fromState, string toState) => fsm.CopyFsmState(fromState, toState);
        /// <inheritdoc cref="CopyState(PlayMakerFSM, string, string)"/>
        public static FsmState CopyFsmState(this PlayMakerFSM fsm, string fromState, string toState)
        {
            FsmState copy = new FsmState(fsm.GetFsmState(fromState))
            {
                Name = toState
            };
            FsmTransition[] transitions = copy.Transitions;
            int transitionsCount = transitions.Length;
            int i;
            for (i = 0; i < transitionsCount; i++)
            {
                // because playmaker is bad, so this has to be done extra
                transitions[i].ToFsmState = fsm.GetFsmState(transitions[i].ToState);
            }
            fsm.AddFsmState(copy);
            return copy;
        }

        /// <summary>
        ///     Adds a transition in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state from which the transition starts</param>
        /// <param name="eventName">The name of transition event</param>
        /// <param name="toState">The name of the new state</param>
        /// <returns>The event of the transition.</returns>
        public static FsmEvent AddTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.AddFsmTransition(stateName, eventName, toState);
        /// <inheritdoc cref="AddTransition(PlayMakerFSM, string, string, string)"/>
        public static FsmEvent AddFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetFsmState(stateName).AddFsmTransition(eventName, toState);
        /// <inheritdoc cref="AddTransition(PlayMakerFSM, string, string, string)"/>
        /// <param name="state">The fsm state</param>
        /// <param name="eventName">The name of transition event</param>
        /// <param name="toState">The name of the new state</param>
        public static FsmEvent AddTransition(this FsmState state, string eventName, string toState) => state.AddFsmTransition(eventName, toState);
        /// <inheritdoc cref="AddTransition(FsmState, string, string)"/>
        public static FsmEvent AddFsmTransition(this FsmState state, string eventName, string toState)
        {
            var ret = FsmEvent.GetFsmEvent(eventName);
            FsmTransition[] origTransitions = state.Transitions;
            FsmTransition[] transitions = new FsmTransition[origTransitions.Length + 1];
            origTransitions.CopyTo(transitions, 0);
            transitions[origTransitions.Length] = new FsmTransition
            {
                ToState = toState,
                ToFsmState = state.Fsm.GetState(toState),
                FsmEvent = ret
            };
            state.Transitions = transitions;
            return ret;
        }

        /// <summary>
        ///     Adds a global transition in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="globalEventName">The name of transition event</param>
        /// <param name="toState">The name of the new state</param>
        /// <returns>The event of the transition.</returns>
        public static FsmEvent AddGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState) => fsm.AddFsmGlobalTransitions(globalEventName, toState);
        /// <inheritdoc cref="AddGlobalTransition(PlayMakerFSM, string, string)"/>
        public static FsmEvent AddFsmGlobalTransitions(this PlayMakerFSM fsm, string globalEventName, string toState)
        {
            var ret = new FsmEvent(globalEventName) { IsGlobal = true };
            FsmTransition[] origTransitions = fsm.FsmGlobalTransitions;
            FsmTransition[] transitions = new FsmTransition[origTransitions.Length + 1];
            origTransitions.CopyTo(transitions, 0);
            transitions[origTransitions.Length] = new FsmTransition
            {
                ToState = toState,
                ToFsmState = fsm.GetState(toState),
                FsmEvent = ret
            };
            fsm.Fsm.GlobalTransitions = transitions;
            return ret;
        }

        /// <summary>
        ///     Adds an action in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state in which the action is added</param>
        /// <param name="action">The action</param>
        public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.AddFsmAction(stateName, action);
        /// <inheritdoc cref="AddAction(PlayMakerFSM, string, FsmStateAction)"/>
        public static void AddFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.GetFsmState(stateName).AddFsmAction(action);
        /// <inheritdoc cref="AddAction(PlayMakerFSM, string, FsmStateAction)"/>
        /// <param name="state">The fsm state</param>
        /// <param name="action">The action</param>
        public static void AddAction(this FsmState state, FsmStateAction action) => state.AddFsmAction(action);
        /// <inheritdoc cref="AddAction(FsmState, FsmStateAction)"/>
        public static void AddFsmAction(this FsmState state, FsmStateAction action)
        {
            FsmStateAction[] origActions = state.Actions;
            FsmStateAction[] actions = new FsmStateAction[origActions.Length + 1];
            origActions.CopyTo(actions, 0);
            actions[origActions.Length] = action;
            state.Actions = actions;
            action.Init(state);
        }

        /// <summary>
        ///     Adds a method in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state in which the method is added</param>
        /// <param name="method">The method that will be invoked</param>
        public static void AddMethod(this PlayMakerFSM fsm, string stateName, Action method) => fsm.GetFsmState(stateName).AddMethod(method);
        /// <inheritdoc cref="AddMethod(PlayMakerFSM, string, Action)"/>
        /// <param name="state">The fsm state</param>
        /// <param name="method">The method that will be invoked</param>
        public static void AddMethod(this FsmState state, Action method)
        {
            state.AddFsmAction(new MethodAction() { method = method });
        }

        #endregion

        #region Insert

        /// <summary>
        ///     Inserts an action in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state in which the action is added</param>
        /// <param name="action">The action</param>
        /// <param name="index">The index to place the action in</param>
        public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.InsertFsmAction(stateName, action, index);
        /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
        public static void InsertFsmAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetFsmState(stateName).InsertFsmAction(action, index);
        /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
        /// <param name="state">The fsm state</param>
        /// <param name="action">The action</param>
        /// <param name="index">The index to place the action in</param>
        public static void InsertAction(this FsmState state, FsmStateAction action, int index) => state.InsertFsmAction(action, index);
        /// <inheritdoc cref="InsertAction(FsmState, FsmStateAction, int)"/>
        public static void InsertFsmAction(this FsmState state, FsmStateAction action, int index)
        {
            FsmStateAction[] origActions = state.Actions;
            FsmStateAction[] actions = new FsmStateAction[origActions.Length + 1];
            int i;
            for (i = 0; i < index; i++)
            {
                actions[i] = origActions[i];
            }
            actions[index] = action;
            for (i = index; i < origActions.Length; i++)
            {
                actions[i + 1] = origActions[i];
            }

            state.Actions = actions;
            action.Init(state);
        }

        /// <summary>
        ///     Inserts an action in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state in which the method is added</param>
        /// <param name="method">The method that will be invoked</param>
        /// <param name="index">The index to place the action in</param>
        public static void InsertMethod(this PlayMakerFSM fsm, string stateName, Action method, int index) => fsm.GetFsmState(stateName).InsertMethod(method, index);
        /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action, int)"/>
        /// <param name="state">The fsm state</param>
        /// <param name="method">The method that will be invoked</param>
        /// <param name="index">The index to place the action in</param>
        public static void InsertMethod(this FsmState state, Action method, int index)
        {
            state.InsertFsmAction(new MethodAction() { method = method }, index);
        }

        #endregion

        #region Change

        /// <summary>
        ///     Changes a transition endpoint in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state from which the transition starts</param>
        /// <param name="eventName">The event of the transition</param>
        /// <param name="toState">The new endpoint of the transition</param>
        public static void ChangeTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.ChangeFsmTransition(stateName, eventName, toState);
        /// <inheritdoc cref="ChangeTransition(PlayMakerFSM, string, string, string)"/>
        public static void ChangeFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetFsmState(stateName).ChangeFsmTransition(eventName, toState);
        /// <inheritdoc cref="ChangeTransition(PlayMakerFSM, string, string, string)"/>
        /// <param name="state">The fsm state</param>
        /// <param name="eventName">The event of the transition</param>
        /// <param name="toState">The new endpoint of the transition</param>
        public static void ChangeTransition(this FsmState state, string eventName, string toState) => state.ChangeFsmTransition(eventName, toState);
        /// <inheritdoc cref="ChangeTransition(FsmState, string, string)"/>
        public static void ChangeFsmTransition(this FsmState state, string eventName, string toState)
        {
            var transition = state.GetFsmTransition(eventName);
            transition.ToState = toState;
            transition.ToFsmState = state.Fsm.GetState(toState);
        }

        #endregion

        #region Remove

        /// <summary>
        ///     Removes a state in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state to remove</param>
        public static void RemoveState(this PlayMakerFSM fsm, string stateName) => fsm.RemoveFsmState(stateName);
        /// <inheritdoc cref="RemoveState(PlayMakerFSM, string)"/>
        public static void RemoveFsmState(this PlayMakerFSM fsm, string stateName)
        {
            FsmState[] origStates = fsm.FsmStates;
            FsmState[] newStates = new FsmState[origStates.Length - 1];
            int i;
            int foundInt = 0;
            for (i = 0; i < newStates.Length; i++)
            {
                if (origStates[i].Name == stateName)
                {
                    foundInt = 1;
                }
                newStates[i] = origStates[i + foundInt];
            }

            fsm.Fsm.States = newStates;
        }

        /// <summary>
        ///     Removes a transition in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state from which the transition starts</param>
        /// <param name="eventName">The event of the transition</param>
        public static void RemoveTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.RemoveFsmTransition(stateName, eventName);
        /// <inheritdoc cref="RemoveTransition(PlayMakerFSM, string, string)"/>
        public static void RemoveFsmTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetState(stateName).RemoveFsmTransition(eventName);
        /// <inheritdoc cref="RemoveTransition(PlayMakerFSM, string, string)"/>
        /// <param name="state">The fsm state</param>
        /// <param name="eventName">The event of the transition</param>
        public static void RemoveTransition(this FsmState state, string eventName) => state.RemoveFsmTransition(eventName);
        /// <inheritdoc cref="RemoveTransition(FsmState, string)"/>
        public static void RemoveFsmTransition(this FsmState state, string eventName)
        {
            FsmTransition[] origTransitions = state.Transitions;
            FsmTransition[] newTransitions = new FsmTransition[origTransitions.Length - 1];
            int i;
            int foundInt = 0;
            for (i = 0; i < newTransitions.Length; i++)
            {
                if (origTransitions[i].EventName == eventName)
                {
                    foundInt = 1;
                }
                newTransitions[i] = origTransitions[i + foundInt];
            }

            state.Transitions = newTransitions;
        }

        /// <summary>
        ///     Removes an action in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="stateName">The name of the state with the action</param>
        /// <param name="index">The index of the action</param>
        public static void RemoveAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.RemoveFsmAction(stateName, index);
        /// <inheritdoc cref="RemoveAction(PlayMakerFSM, string, int)"/>
        public static void RemoveFsmAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetFsmState(stateName).RemoveFsmAction(index);
        /// <inheritdoc cref="RemoveAction(PlayMakerFSM, string, int)"/>
        /// <param name="state">The fsm state</param>
        /// <param name="index">The index of the action</param>
        public static void RemoveAction(this FsmState state, int index) => state.RemoveFsmAction(index);
        /// <inheritdoc cref="RemoveAction(FsmState, int)"/>
        public static void RemoveFsmAction(this FsmState state, int index)
        {
            FsmStateAction[] origActions = state.Actions;
            FsmStateAction[] actions = new FsmStateAction[origActions.Length - 1];
            int i;
            for (i = 0; i < index; i++)
            {
                actions[i] = origActions[i];
            }
            for (i = index; i < actions.Length; i++)
            {
                actions[i] = origActions[i + 1];
            }

            state.Actions = actions;
        }

        #endregion

        #region FSM Variables

        private static TVar[] makeNewVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
        {
            TVar[] newArray = new TVar[orig.Length + 1];
            orig.CopyTo(newArray, 0);
            newArray[orig.Length] = new TVar() { Name = name };
            return newArray;
        }
        /// <summary>
        ///     Adds a fsm variable in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="name">The name of the new variable</param>
        /// <returns>The newly created variable.</returns>
        public static FsmFloat AddFloatVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmFloatVariable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmFloat AddFsmFloatVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmFloat>(fsm.FsmVariables.FloatVariables, name);
            fsm.FsmVariables.FloatVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmInt AddIntVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmIntVariable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmInt AddFsmIntVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmInt>(fsm.FsmVariables.IntVariables, name);
            fsm.FsmVariables.IntVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmBool AddBoolVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmBoolVariable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmBool AddFsmBoolVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmBool>(fsm.FsmVariables.BoolVariables, name);
            fsm.FsmVariables.BoolVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmString AddStringVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmStringVariable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmString AddFsmStringVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmString>(fsm.FsmVariables.StringVariables, name);
            fsm.FsmVariables.StringVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector2 AddVector2Variable(this PlayMakerFSM fsm, string name) => fsm.AddFsmVector2Variable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector2 AddFsmVector2Variable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmVector2>(fsm.FsmVariables.Vector2Variables, name);
            fsm.FsmVariables.Vector2Variables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector3 AddVector3Variable(this PlayMakerFSM fsm, string name) => fsm.AddFsmVector3Variable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector3 AddFsmVector3Variable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmVector3>(fsm.FsmVariables.Vector3Variables, name);
            fsm.FsmVariables.Vector3Variables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmColor AddColorVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmColorVariable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmColor AddFsmColorVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmColor>(fsm.FsmVariables.ColorVariables, name);
            fsm.FsmVariables.ColorVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmRect AddRectVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmRectVariable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmRect AddFsmRectVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmRect>(fsm.FsmVariables.RectVariables, name);
            fsm.FsmVariables.RectVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmQuaternion AddQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmQuaternionVariable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmQuaternion AddFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmQuaternion>(fsm.FsmVariables.QuaternionVariables, name);
            fsm.FsmVariables.QuaternionVariables = tmp;
            return tmp[tmp.Length - 1];
        }
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmGameObject AddGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.AddFsmGameObjectVariable(name);
        /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
        public static FsmGameObject AddFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = makeNewVariableArray<FsmGameObject>(fsm.FsmVariables.GameObjectVariables, name);
            fsm.FsmVariables.GameObjectVariables = tmp;
            return tmp[tmp.Length - 1];
        }

        private static TVar findInVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
        {
            int count = orig.Length;
            int i;
            for (i = 0; i < count; i++)
            {
                if (orig[i].Name == name)
                {
                    return orig[i];
                }
            }
            return null;
        }
        /// <summary>
        ///     Finds a fsm variable in a PlayMakerFSM.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="name">The name of the variable</param>
        /// <returns>The variable, null if not found.</returns>
        public static FsmFloat FindFloatVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmFloatVariable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmFloat FindFsmFloatVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmFloat>(fsm.FsmVariables.FloatVariables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmInt FindIntVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmIntVariable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmInt FindFsmIntVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmInt>(fsm.FsmVariables.IntVariables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmBool FindBoolVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmBoolVariable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmBool FindFsmBoolVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmBool>(fsm.FsmVariables.BoolVariables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmString FindStringVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmStringVariable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmString FindFsmStringVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmString>(fsm.FsmVariables.StringVariables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector2 FindVector2Variable(this PlayMakerFSM fsm, string name) => fsm.FindFsmVector2Variable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector2 FindFsmVector2Variable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmVector2>(fsm.FsmVariables.Vector2Variables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector3 FindVector3Variable(this PlayMakerFSM fsm, string name) => fsm.FindFsmVector3Variable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector3 FindFsmVector3Variable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmVector3>(fsm.FsmVariables.Vector3Variables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmColor FindColorVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmColorVariable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmColor FindFsmColorVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmColor>(fsm.FsmVariables.ColorVariables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmRect FindRectVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmRectVariable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmRect FindFsmRectVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmRect>(fsm.FsmVariables.RectVariables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmQuaternion FindQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmQuaternionVariable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmQuaternion FindFsmQuaternionVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmQuaternion>(fsm.FsmVariables.QuaternionVariables, name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmGameObject FindGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.FindFsmGameObjectVariable(name);
        /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
        public static FsmGameObject FindFsmGameObjectVariable(this PlayMakerFSM fsm, string name) => findInVariableArray<FsmGameObject>(fsm.FsmVariables.GameObjectVariables, name);

        /// <summary>
        ///     Gets a fsm variable in a PlayMakerFSM. Creates a new one if none with the name are present.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="name">The name of the variable</param>
        /// <returns>The variable.</returns>
        public static FsmFloat GetFloatVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmFloatVariable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmFloat GetFsmFloatVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindFloatVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddFloatVariable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmInt GetIntVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmIntVariable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmInt GetFsmIntVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindIntVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddIntVariable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmBool GetBoolVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmBoolVariable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmBool GetFsmBoolVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindBoolVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddBoolVariable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmString GetStringVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmStringVariable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmString GetFsmStringVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindStringVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddStringVariable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector2 GetVector2Variable(this PlayMakerFSM fsm, string name) => fsm.GetFsmVector2Variable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector2 GetFsmVector2Variable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindVector2Variable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddVector2Variable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector3 GetVector3Variable(this PlayMakerFSM fsm, string name) => fsm.GetFsmVector3Variable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmVector3 GetFsmVector3Variable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindVector3Variable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddVector3Variable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmColor GetColorVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmColorVariable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmColor GetFsmColorVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindColorVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddColorVariable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmRect GetRectVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmRectVariable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmRect GetFsmRectVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindRectVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddRectVariable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmQuaternion GetQuaternionVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmQuaternionVariable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmQuaternion GetFsmQuaternionVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindQuaternionVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddQuaternionVariable(name);
        }
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmGameObject GetGameObjectVariable(this PlayMakerFSM fsm, string name) => fsm.GetFsmGameObjectVariable(name);
        /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
        public static FsmGameObject GetFsmGameObjectVariable(this PlayMakerFSM fsm, string name)
        {
            var tmp = fsm.FindGameObjectVariable(name);
            if (tmp != null)
                return tmp;
            return fsm.AddGameObjectVariable(name);
        }

        #endregion

        #region Log

        /// <summary>
        ///     Adds actions to a PlayMakerFSM so it gives a log message before and after every single normal action.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        /// <param name="additionalLogging">Flag if, additionally, every log should also log the state of all fsm variables</param>
        public static void MakeLog(this PlayMakerFSM fsm, bool additionalLogging = false)
        {
            foreach (var s in fsm.FsmStates)
            {
                for (int i = s.Actions.Length - 1; i >= 0; i--)
                {
                    string actionTypeName = fsm.GetState(s.Name).Actions[i].GetType().FullName;
                    fsm.InsertAction(s.Name, new StatusLog() { text = $"{i} - {actionTypeName}" }, i);
                    if (additionalLogging)
                    {
                        fsm.InsertMethod(s.Name, () =>
                        {
                            var fsmVars = fsm.FsmVariables;
                            var origFloatVariables = fsmVars.FloatVariables;
                            var origIntVariables = fsmVars.IntVariables;
                            var origBoolVariables = fsmVars.BoolVariables;
                            var origStringVariables = fsmVars.StringVariables;
                            var origVector2Variables = fsmVars.Vector2Variables;
                            var origVector3Variables = fsmVars.Vector3Variables;
                            var origColorVariables = fsmVars.ColorVariables;
                            var origRectVariables = fsmVars.RectVariables;
                            var origQuaternionVariables = fsmVars.QuaternionVariables;
                            var origGameObjectVariables = fsmVars.GameObjectVariables;
                            int i;
                            for (i = 0; i < origFloatVariables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "FloatVariables", $"'{origFloatVariables[i].Name}': '{origFloatVariables[i].Value}'");
                            for (i = 0; i < origIntVariables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "IntVariables", $"'{origIntVariables[i].Name}': '{origIntVariables[i].Value}'");
                            for (i = 0; i < origBoolVariables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "BoolVariables", $"'{origBoolVariables[i].Name}': '{origBoolVariables[i].Value}'");
                            for (i = 0; i < origStringVariables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "StringVariables", $"'{origStringVariables[i].Name}': '{origStringVariables[i].Value}'");
                            for (i = 0; i < origVector2Variables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "Vector2Variables", $"'{origVector2Variables[i].Name}': '({origVector2Variables[i].Value.x}, {origVector2Variables[i].Value.y})'");
                            for (i = 0; i < origVector3Variables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "Vector3Variables", $"'{origVector3Variables[i].Name}': '({origVector3Variables[i].Value.x}, {origVector3Variables[i].Value.y}, {origVector3Variables[i].Value.z})'");
                            for (i = 0; i < origColorVariables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "ColorVariables", $"'{origColorVariables[i].Name}': '{origColorVariables[i].Value}'");
                            for (i = 0; i < origRectVariables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "RectVariables", $"'{origRectVariables[i].Name}': '{origRectVariables[i].Value}'");
                            for (i = 0; i < origQuaternionVariables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "QuaternionVariables", $"'{origQuaternionVariables[i].Name}': '{origQuaternionVariables[i].Value}'");
                            for (i = 0; i < origGameObjectVariables.Length; i++)
                                Log(fsm.gameObject.name, fsm.FsmName, "GameObjectVariables", $"'{origGameObjectVariables[i].Name}': '{origGameObjectVariables[i].Value}'");
                        }, i + 1);
                    }
                }
            }
        }

        /// <summary>
        ///     Logs the fsm and its states, transitions and variables.
        /// </summary>
        /// <param name="fsm">The fsm</param>
        public static void Log(this PlayMakerFSM fsm)
        {
            Log($"FSM \"{fsm.FsmName}\" on \"{fsm.gameObject.name}\"");
            Log($"\t{fsm.FsmStates.Length} States");
            foreach (var s in fsm.FsmStates)
            {
                Log($"\t\tState \"{s.Name}\"");
                foreach (var t in s.Transitions)
                {
                    Log($"\t\t\t-> \"{t.ToState}\" via \"{t.EventName}\"");
                }
            }
            Log($"\t{fsm.FsmGlobalTransitions.Length} Global Transitions");
            foreach (var t in fsm.FsmGlobalTransitions)
            {
                Log($"\t\tGlobal Transition \"{t.EventName}\" to \"{t.ToState}\"");
            }
            Log($"\tVariables");
            var fsmVar = fsm.FsmVariables;
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

        #endregion
    }
}