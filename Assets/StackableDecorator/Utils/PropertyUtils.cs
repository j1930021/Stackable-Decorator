#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace StackableDecorator
{
    public static class PropertyUtils
    {
        public static SerializedProperty[] GetProperties(this SerializedProperty property, IEnumerable<string> propertyPaths)
        {
            var result = propertyPaths.Select(path =>
            {
                string propertyPath;
                if (path.StartsWith("."))
                    propertyPath = property.propertyPath + path;
                else
                {
                    int index = property.propertyPath.LastIndexOf('.');
                    propertyPath = index == -1 ? path : property.propertyPath.Substring(0, index + 1) + path;
                }
                var prop = property.serializedObject.FindProperty(propertyPath);
                return prop;
            });
            return result.ToArray();
        }

        public static void GetProperties(this SerializedProperty property, IEnumerable<string> propertyPaths, List<SerializedProperty> list)
        {
            list.Clear();
            foreach (var path in propertyPaths)
            {
                string propertyPath;
                if (path.StartsWith("."))
                    propertyPath = property.propertyPath + path;
                else
                {
                    int index = property.propertyPath.LastIndexOf('.');
                    propertyPath = index == -1 ? path : property.propertyPath.Substring(0, index + 1) + path;
                }
                var prop = property.serializedObject.FindProperty(propertyPath);
                list.Add(prop);
            }
        }

        public static SerializedProperty[] GetProperties(this SerializedProperty property, int count)
        {
            var prop = property.Copy();
            var result = new List<SerializedProperty>();

            while (prop.NextVisible(false))
            {
                count--;
                if (count < 0) break;
                result.Add(prop.Copy());
            }
            return result.ToArray();
        }

        public static void GetProperties(this SerializedProperty property, int count, List<SerializedProperty> list)
        {
            list.Clear();
            var prop = property.Copy();
            while (prop.NextVisible(false))
            {
                count--;
                if (count < 0) break;
                list.Add(prop.Copy());
            }
        }

        public static SerializedProperty[] GetChildrenProperties(this SerializedProperty property, IEnumerable<string> excludePropertyPaths)
        {
            var prop = property.Copy();
            var depth = prop.depth;
            var result = new List<SerializedProperty>();

            bool enter = true;
            while (prop.NextVisible(enter) && prop.depth > depth)
            {
                int index = prop.propertyPath.LastIndexOf('.');
                if (index == -1 || !excludePropertyPaths.Contains(prop.propertyPath.Substring(index)))
                    result.Add(prop.Copy());
                enter = false;
            }
            return result.ToArray();
        }

        public static void GetChildrenProperties(this SerializedProperty property, IEnumerable<string> excludePropertyPaths, List<SerializedProperty> list)
        {
            list.Clear();
            var prop = property.Copy();
            var depth = prop.depth;

            bool enter = true;
            while (prop.NextVisible(enter) && prop.depth > depth)
            {
                int index = prop.propertyPath.LastIndexOf('.');
                if (index == -1 || !excludePropertyPaths.Contains(prop.propertyPath.Substring(index)))
                    list.Add(prop.Copy());
                enter = false;
            }
        }

        public static bool GetFieldInfos(this SerializedProperty property, List<FieldInfo> list)
        {
            list.Clear();
            var type = property.serializedObject.targetObject.GetType();
            var paths = property.propertyPath.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
                string text = paths[i];
                if (i < paths.Length - 1 && text == "Array" && paths[i + 1].StartsWith("data["))
                {
                    if (type.IsArrayOrList())
                        type = type.GetArrayOrListElementType();
                    i++;
                }
                else
                {
                    FieldInfo field = null;
                    while (field == null && type != null)
                    {
                        field = type.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        type = type.BaseType;
                    }
                    if (field == null)
                        return false;
                    list.Add(field);
                    type = field.FieldType;
                }
            }
            return true;
        }

        public static bool GetObjects(this SerializedProperty property, List<object> list)
        {
            list.Clear();
            object obj = property.serializedObject.targetObject;
            list.Add(obj);

            var type = property.serializedObject.targetObject.GetType();
            var paths = property.propertyPath.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
                string text = paths[i];
                if (i < paths.Length - 1 && text == "Array" && paths[i + 1].StartsWith("data["))
                {
                    i++;
                    var num = paths[i].TrimEnd(']').Substring(5);
                    int index;
                    if (!int.TryParse(num, out index))
                        return false;
                    if (type.IsArrayOrList())
                    {
                        type = type.GetArrayOrListElementType();
                        obj = obj.GetArrayOrListElement(index);
                        if (obj == null)
                            return false;
                        list.Add(obj);
                    }
                }
                else
                {
                    FieldInfo field = null;
                    while (field == null && type != null)
                    {
                        field = type.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        type = type.BaseType;
                    }
                    if (field == null)
                        return false;
                    obj = field.GetValue(obj);
                    if (obj == null)
                        return false;
                    list.Add(obj);
                    type = field.FieldType;
                }
            }
            return true;
        }

        public static bool GetObjectGetters(this SerializedProperty property, List<Func<object, object>> list)
        {
            list.Clear();
            var type = property.serializedObject.targetObject.GetType();
            var paths = property.propertyPath.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
                string text = paths[i];
                if (i < paths.Length - 1 && text == "Array" && paths[i + 1].StartsWith("data["))
                {
                    if (type.IsArrayOrList())
                        type = type.GetArrayOrListElementType();
                    i++;
                }
                else
                {
                    FieldInfo field = null;
                    while (field == null && type != null)
                    {
                        field = type.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        type = type.BaseType;
                    }
                    if (field == null)
                        return false;
                    list.Add(field.MakeGetter());
                    type = field.FieldType;
                }
            }
            return true;
        }

        public static bool GetObjectsFromGetter(this SerializedProperty property, List<Func<object, object>> getters, List<object> list)
        {
            list.Clear();
            object obj = property.serializedObject.targetObject;
            list.Add(obj);

            int getter = 0;
            var paths = property.propertyPath.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
                string text = paths[i];
                if (i < paths.Length - 1 && text == "Array" && paths[i + 1].StartsWith("data["))
                {
                    i++;
                    var num = paths[i].TrimEnd(']').Substring(5);
                    int index;
                    if (!int.TryParse(num, out index))
                        return false;
                    if (obj == null)
                        return false;
                    if (obj.GetType().IsArrayOrList())
                    {
                        obj = obj.GetArrayOrListElement(index);
                        if (obj == null)
                            return false;
                        list.Add(obj);
                    }
                }
                else
                {
                    if (getter < getters.Count)
                    {
                        obj = getters[getter].Invoke(obj);
                        if (obj == null && getter < getters.Count - 1)
                            return false;
                        list.Add(obj);
                    }
                    getter++;
                }
            }
            return true;
        }

        public static bool IsArrayOrList(this Type listType)
        {
            return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>));
        }

        public static Type GetArrayOrListElementType(this Type listType)
        {
            Type result;
            if (listType.IsArray)
                result = listType.GetElementType();
            else if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
                result = listType.GetGenericArguments()[0];
            else
                result = null;
            return result;
        }

        public static object GetArrayOrListElement(this object obj, int index)
        {
            var type = obj.GetType();
            if (type.IsArray)
            {
                var array = (Array)obj;
                if (array.Length <= index)
                    return null;
                return array.GetValue(index);
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = (IList)obj;
                if (list.Count <= index)
                    return null;
                return list[index];
            }
            return null;
        }

        public static string AsString(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return property.floatValue.ToString();
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue.name;
                case SerializedPropertyType.Enum:
                    return property.enumDisplayNames[property.enumValueIndex];
                case SerializedPropertyType.Vector2:
                    return property.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return property.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return property.vector4Value.ToString();
                case SerializedPropertyType.Rect:
                    return property.rectValue.ToString();
                case SerializedPropertyType.Character:
                    return property.stringValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue.ToString();
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue.ToString();
            }
            return string.Empty;
        }

        public static object GetValueAsObject(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.Character:
                    return property.stringValue[0];
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;
            }
            return null;
        }

        public static void SetObjectToValue(this SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = (int)value;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = (UnityEngine.Object)value;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.Character:
                    property.stringValue = (string)value;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = (AnimationCurve)value;
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (Quaternion)value;
                    break;
            }
        }
    }
}
#endif