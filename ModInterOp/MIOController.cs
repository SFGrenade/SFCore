using System;
using System.Collections.Generic;
using System.Reflection;

namespace SFCore.ModInterOp
{
    public static class MIOController
    {
        private static Dictionary<string, InterOpItem> m_methodMap = new();

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
        public static bool AddAction(Action a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1>(Action<T1> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2>(Action<T1, T2> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3>(Action<T1, T2, T3> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<TRes>(Func<TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, TRes>(Func<T1, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, TRes>(Func<T1, T2, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, TRes>(Func<T1, T2, T3, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, TRes>(Func<T1, T2, T3, T4, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, TRes>(Func<T1, T2, T3, T4, T5, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, TRes>(Func<T1, T2, T3, T4, T5, T6, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRes> a, string n) => AddDelegate((Delegate) a, n);
        public static bool AddFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRes>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRes> a, string n) => AddDelegate((Delegate) a, n);
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
