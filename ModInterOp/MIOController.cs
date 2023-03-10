using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace SFCore.ModInterOp
{
    /// <summary>
    /// Class that handles mod interop.
    /// </summary>
    public static class MIOController
    {
        private static Dictionary<string, InterOpItem> m_methodMap = new();

        /// <summary>
        /// Adds a Delegate with a unique Identifier to the Mod Interop Enviroment.
        /// </summary>
        /// <param name="d">The Delegate that is going to be invoked.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        /// <returns>True if the Delegate was added, False if the name was already used.</returns>
        [PublicAPI]
        public static bool AddDelegate(Delegate d, string n)
        {
            if (m_methodMap.ContainsKey(n))
                return false;
            InterOpItem toAdd;
            if (!d.Method.IsStatic)
                toAdd = new InterOpItem(d, d.Target, false);
            else
                toAdd = new InterOpItem(d);
            m_methodMap.Add(n, toAdd);
            return true;
        }
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with no arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction(Action a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 1 argument and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1>(Action<T1> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 2 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2>(Action<T1, T2> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 3 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3>(Action<T1, T2, T3> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 4 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 5 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 6 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 7 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 8 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 9 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 10 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 11 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 12 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 13 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 14 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 15 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 16 arguments and no return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with no arguments but with a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<TRes>(Func<TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 1 argument and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, TRes>(Func<T1, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 2 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, TRes>(Func<T1, T2, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 3 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, TRes>(Func<T1, T2, T3, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 4 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, TRes>(Func<T1, T2, T3, T4, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 5 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, TRes>(Func<T1, T2, T3, T4, T5, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 6 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, TRes>(Func<T1, T2, T3, T4, T5, T6, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 7 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 8 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 9 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 10 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 11 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 12 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 13 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 14 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 15 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRes> a, string n) => AddDelegate((Delegate) a, n);
        /// <inheritdoc cref='AddDelegate' />
        /// <param name="a">A method with 16 arguments and a return value.</param>
        /// <param name="n">The unique name that is used to invoke the Delegate.</param>
        [PublicAPI]
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRes> a, string n) => AddDelegate((Delegate) a, n);

        private static bool CheckForMethod(InterOpItem ioi, object[] @params)
        {
            ParameterInfo[] ioiParams = ioi.method.Method.GetParameters();
            if (ioiParams.Length != @params.Length)
                return false;
            bool allParamTypesCorrect = true;
            for (int i = 0; i < ioiParams.Length; i++)
                allParamTypesCorrect &= ioiParams[i].ParameterType.FullName == @params.GetType().FullName;
            return allParamTypesCorrect;
        }

        /// <summary>
        /// Invokes a Delegate of the Mod Interop Enviroment that has no return value.
        /// </summary>
        /// <param name="name">The unique name of the Delegate.</param>
        /// <param name="params">The parameters that are given to the Delegate.</param>
        /// <returns>True if the Delegate was invoked, False if the Delegate wasn't found.</returns>
        [PublicAPI]
        public static bool CallAction(string name, object[] @params = null)
        {
            if (!m_methodMap.ContainsKey(name))
                return false;
            InterOpItem ioi = m_methodMap[name];
            if (!CheckForMethod(ioi, @params))
                return false;
            if (ioi.isStatic)
                ioi.method.DynamicInvoke(@params);
            else
                ioi.method.Method.Invoke(ioi.obj, @params);
            return true;
        }
        /// <summary>
        /// Invokes a Delegate of the Mod Interop Enviroment that has a return value.
        /// </summary>
        /// <param name="name">The unique name of the Delegate.</param>
        /// <param name="res">The return value of the Delegate.</param>
        /// <param name="params">The parameters that are given to the Delegate.</param>
        /// <returns>True if the Delegate was invoked, False if the Delegate wasn't found.</returns>
        [PublicAPI]
        public static bool CallFunc<TRes>(string name, out TRes res, object[] @params = null)
        {
            if (!m_methodMap.ContainsKey(name))
            {
                res = default(TRes);
                return false;
            }
            InterOpItem ioi = m_methodMap[name];
            if (!CheckForMethod(ioi, @params))
            {
                res = default(TRes);
                return false;
            }
            if (ioi.isStatic)
                res = (TRes) ioi.method.DynamicInvoke(@params);
            else
                res = (TRes) ioi.method.Method.Invoke(ioi.obj, @params);
            return true;
        }
    }
}
