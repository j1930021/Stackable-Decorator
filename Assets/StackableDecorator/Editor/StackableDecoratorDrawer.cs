using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StackableDecorator
{
    [CustomPropertyDrawer(typeof(StackableFieldAttribute), true)]
    public class StackableDecoratorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var drawrer = (StackableFieldAttribute)attribute;
            drawrer.Setup(property, fieldInfo);

            bool includeChildren = true, visible = true;
            foreach (var attr in drawrer.decorators.AsEnumerable().Reverse())
            {
                attr.SetSerializedProperty(property);
                attr.SetFieldInfo(fieldInfo);
                attr.visible = visible;
                if (!attr.BeforeGetHeight(ref property, ref label, ref includeChildren))
                    visible = false;
            }

            float height = 0;
            if (visible)
                height = drawrer.GetPropertyHeight(property, label, includeChildren);

            foreach (var attr in drawrer.decorators)
            {
                height = attr.GetHeight(property, label, height);
                attr.AfterGetHeight(property, label, includeChildren);
            }

            if (height == 0)
                height = -EditorGUIUtility.standardVerticalSpacing;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.height <= 0) return;

            var drawrer = (StackableFieldAttribute)attribute;
            drawrer.Setup(property, fieldInfo);

            bool includeChildren = true, visible = true;
            foreach (var attr in drawrer.decorators.AsEnumerable().Reverse())
            {
                attr.SetSerializedProperty(property);
                attr.SetFieldInfo(fieldInfo);
                attr.visible = visible;
                visible = attr.BeforeGUI(ref position, ref property, ref label, ref includeChildren, visible);
            }

            if (visible)
                drawrer.OnGUI(position, property, label, includeChildren);

            foreach (var attr in drawrer.decorators)
                attr.AfterGUI(position, property, label);
        }

#if UNITY_2018_1_OR_NEWER && UNITY_EDITOR
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            var drawrer = (StackableDrawerAttribute)attribute;
            drawrer.Setup(property, fieldInfo);
            return !drawrer.modifiers.OfType<NoCacheInspectorGUI>().Any();
        }
#endif
    }
}