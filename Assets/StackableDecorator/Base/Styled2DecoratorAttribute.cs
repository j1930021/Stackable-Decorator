using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackableDecorator
{
    public abstract class Styled2DecoratorAttribute : StyledDecoratorAttribute
    {
        public string style2 = null;
#if UNITY_EDITOR
        protected GUIStyle m_Style2 { get { return GetStyle2(); } }

        protected abstract string m_defaultStyle2 { get; }

        private GUIStyle m_Style2Internal = null;

        private GUIStyle GetStyle2()
        {
            if (m_Style2Internal == null)
                m_Style2Internal = style2 == null ? m_defaultStyle2 : style2;
            return m_Style2Internal;
        }
#endif
    }
}