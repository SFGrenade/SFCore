using System;

namespace SFCore.ModInterOp
{
    internal class InterOpItem
    {
        public InterOpItem(Delegate m, object o = null, bool isStatic = true)
        {
            m_method = m;
            m_obj = o;
            m_isStatic = isStatic;
        }
        private Delegate m_method;
        private object m_obj;
        private bool m_isStatic;
        public Delegate method { get => m_method; }
        public object obj { get => m_obj; }
        public bool isStatic { get => m_isStatic; }
    }
}
