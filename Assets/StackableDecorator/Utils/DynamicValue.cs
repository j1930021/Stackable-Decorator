#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;

namespace StackableDecorator
{
    public class DynamicValue<T>
    {
        public enum DynamicType { None, Fail, Static, SerializedProperty, Getter }

        private DynamicType m_Type;
        private T m_Static;
        private T m_Default;
        private SerializedProperty m_SerializedProperty;
        private string m_RelativePath;

        private Type m_DynamicType = typeof(T);
        private Type m_MethodDeclaring = null;
        private Type m_ParameterType = null;
        private Func<object, object> m_FieldGetter = null;
        private Func<T> m_StaticGetter = null;
        private Func<object, T> m_InstanceGetter = null;
        private Func<object, T> m_StaticInputGetter = null;
        private Func<object, object, T> m_InstanceInputGetter = null;

        private List<SerializedProperty> m_PropertyList = new List<SerializedProperty>();
        private List<object> m_ObjectList = new List<object>();
        private List<FieldInfo> m_FieldInfoList = new List<FieldInfo>();
        private List<Func<object, object>> m_ObjectGetterList = new List<Func<object, object>>();

        public DynamicValue()
        {
            m_Type = DynamicType.None;
            m_Default = default(T);
        }

        public DynamicValue(T input)
        {
            m_Static = input;
            m_Type = DynamicType.Static;
            m_Default = default(T);
        }

        public DynamicValue(string input, SerializedProperty property)
        {
            Update(property);
            ProcessInput(input);
            m_Default = default(T);
        }

        public DynamicValue(string input, SerializedProperty property, Type type)
        {
            Update(property);
            m_DynamicType = type;
            ProcessInput(input);
            m_Default = default(T);
        }

        public DynamicValue(string input, SerializedProperty property, T def)
        {
            Update(property);
            ProcessInput(input);
            m_Default = def;
        }

        public void UpdateAndCheckInitial(string input, SerializedProperty property)
        {
            Update(property);
            if (m_Type != DynamicType.None) return;
            ProcessInput(input);
            m_Default = default(T);
        }

        public void UpdateAndCheckInitial(string input, SerializedProperty property, Type type)
        {
            Update(property);
            if (m_Type != DynamicType.None) return;
            m_DynamicType = type;
            ProcessInput(input);
            m_Default = default(T);
        }

        public void UpdateAndCheckInitial(string input, SerializedProperty property, T def)
        {
            Update(property);
            if (m_Type != DynamicType.None) return;
            ProcessInput(input);
            m_Default = def;
        }

        public void Update(SerializedProperty property)
        {
            m_SerializedProperty = property;
        }

        public bool IsStatic()
        {
            return m_Type != DynamicType.SerializedProperty && m_Type != DynamicType.Getter;
        }

        public T GetValue()
        {
            switch (m_Type)
            {
                case DynamicType.Static:
                    return m_Static;
                case DynamicType.SerializedProperty:
                    return GetSerializedPropertyValue();
                case DynamicType.Getter:
                    return GetGetterValue();
            }
            return m_Default;
        }

        public T[] GetValues()
        {
            switch (m_Type)
            {
                case DynamicType.Static:
                    return new T[] { m_Static };
                case DynamicType.SerializedProperty:
                    return GetSerializedPropertyValues();
                case DynamicType.Getter:
                    return GetGetterValues();
            }
            return new T[] { m_Default };
        }

        private void ProcessInput(string input)
        {
            if (input == null)
                m_Type = DynamicType.Fail;
            else if (input.StartsWith("$"))
                ProcessSerializedProperty(input);
            else if (input.StartsWith("#"))
                ProcessSReflection(input);
            else if (typeof(T) == typeof(string))
            {
                m_Static = (T)(object)input;
                m_Type = DynamicType.Static;
            }
            else
                m_Type = DynamicType.Fail;
        }

        private void ProcessSerializedProperty(string input)
        {
            var prop = m_SerializedProperty;
            input = input.TrimStart('$');
            if (input != string.Empty)
            {
                m_SerializedProperty.GetProperties(input.Yield(), m_PropertyList);
                prop = m_PropertyList[0];
            }
            if (prop == null)
            {
                m_Type = DynamicType.Fail;
                return;
            }
            m_RelativePath = input;
            m_Type = DynamicType.SerializedProperty;
        }

        private SerializedProperty GetSerializedProperty(SerializedProperty property)
        {
            var prop = property;
            if (m_RelativePath != string.Empty)
            {
                property.GetProperties(m_RelativePath.Yield(), m_PropertyList);
                prop = m_PropertyList[0];
            }
            return prop;
        }

        private void ProcessSReflection(string input)
        {
            input = input.TrimStart('#');
            var flag = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            bool ok1 = m_SerializedProperty.GetFieldInfos(m_FieldInfoList);
            bool ok2 = m_SerializedProperty.GetObjectGetters(m_ObjectGetterList);
            if (!ok1 || !ok2)
            {
                m_Type = DynamicType.Fail;
                return;
            }

            var selfType = m_FieldInfoList[m_FieldInfoList.Count - 1].FieldType;
            if (selfType.IsArrayOrList())
                selfType = selfType.GetArrayOrListElementType();

            var type = m_SerializedProperty.serializedObject.targetObject.GetType();
            var field = type.GetField(input, flag);
            if (m_FieldInfoList.Count > 1)
                field = m_FieldInfoList[m_FieldInfoList.Count - 2].FieldType.GetField(input, flag);
            if (field != null && field.FieldType == m_DynamicType)
            {
                m_FieldGetter = field.MakeGetter();
                m_Type = DynamicType.Getter;
                return;
            }
            var prop = type.GetProperty(input, flag);
            if (m_FieldInfoList.Count > 1)
                prop = m_FieldInfoList[m_FieldInfoList.Count - 2].FieldType.GetProperty(input, flag);
            if (prop != null && prop.PropertyType == m_DynamicType)
            {
                m_StaticGetter = prop.GetGetMethod(true).MakeStaticFunc<Func<T>>();
                m_Type = DynamicType.Getter;
                return;
            }
            var method = type.GetMethod(input, flag);
            if (method != null && method.ReturnType == m_DynamicType)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    m_StaticGetter = method.MakeStaticFunc<Func<T>>();
                    m_MethodDeclaring = method.DeclaringType;
                    m_Type = DynamicType.Getter;
                    return;
                }
                var type2 = parameters[0].ParameterType;
                if (parameters.Length == 1 && (type2.Equals(selfType) || type2.Equals(typeof(SerializedProperty))))
                {
                    m_StaticInputGetter = method.MakeStaticFuncGenericInput<Func<object, T>>();
                    m_MethodDeclaring = method.DeclaringType;
                    m_ParameterType = type2;
                    m_Type = DynamicType.Getter;
                    return;
                }
            }
            foreach (var field2 in m_FieldInfoList.AsEnumerable().Reverse())
            {
                var type2 = field2.FieldType;
                if (type2.IsArrayOrList())
                    type2 = type2.GetArrayOrListElementType();
                method = type2.GetMethod(input, flag);
                if (method != null && method.ReturnType == m_DynamicType)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        m_StaticGetter = method.MakeStaticFunc<Func<T>>();
                        m_MethodDeclaring = method.DeclaringType;
                        m_Type = DynamicType.Getter;
                        return;
                    }
                    type2 = parameters[0].ParameterType;
                    if (parameters.Length == 1 && (type2.Equals(selfType) || type2.Equals(typeof(SerializedProperty))))
                    {
                        m_StaticInputGetter = method.MakeStaticFuncGenericInput<Func<object, T>>();
                        m_MethodDeclaring = method.DeclaringType;
                        m_ParameterType = type2;
                        m_Type = DynamicType.Getter;
                        return;
                    }
                }
            }

            flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            field = type.GetField(input, flag);
            if (m_FieldInfoList.Count > 1)
                field = m_FieldInfoList[m_FieldInfoList.Count - 2].FieldType.GetField(input, flag);
            if (field != null && field.FieldType == m_DynamicType)
            {
                m_FieldGetter = field.MakeGetter();
                m_MethodDeclaring = field.DeclaringType;
                m_Type = DynamicType.Getter;
                return;
            }
            prop = type.GetProperty(input, flag);
            if (m_FieldInfoList.Count > 1)
                prop = m_FieldInfoList[m_FieldInfoList.Count - 2].FieldType.GetProperty(input, flag);
            if (prop != null && prop.PropertyType == m_DynamicType)
            {
                m_InstanceGetter = prop.GetGetMethod(true).MakeFuncGenericThis<Func<object, T>>();
                m_MethodDeclaring = prop.DeclaringType;
                m_Type = DynamicType.Getter;
                return;
            }
            method = type.GetMethod(input, flag);
            if (method != null && method.ReturnType == m_DynamicType)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    m_InstanceGetter = method.MakeFuncGenericThis<Func<object, T>>();
                    m_MethodDeclaring = method.DeclaringType;
                    m_Type = DynamicType.Getter;
                    return;
                }
                var type2 = parameters[0].ParameterType;
                if (parameters.Length == 1 && (type2.Equals(selfType) || type2.Equals(typeof(SerializedProperty))))
                {
                    m_InstanceInputGetter = method.MakeFuncGenericInput<Func<object, object, T>>();
                    m_MethodDeclaring = method.DeclaringType;
                    m_ParameterType = type2;
                    m_Type = DynamicType.Getter;
                    return;
                }
            }
            foreach (var field2 in m_FieldInfoList.AsEnumerable().Reverse())
            {
                var type2 = field2.FieldType;
                if (type2.IsArrayOrList())
                    type2 = type2.GetArrayOrListElementType();
                method = type2.GetMethod(input, flag);
                if (method != null && method.ReturnType == m_DynamicType)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        m_InstanceGetter = method.MakeFuncGenericThis<Func<object, T>>();
                        m_MethodDeclaring = method.DeclaringType;
                        m_Type = DynamicType.Getter;
                        return;
                    }
                    type2 = parameters[0].ParameterType;
                    if (parameters.Length == 1 && (type2.Equals(selfType) || type2.Equals(typeof(SerializedProperty))))
                    {
                        m_InstanceInputGetter = method.MakeFuncGenericInput<Func<object, object, T>>();
                        m_MethodDeclaring = method.DeclaringType;
                        m_ParameterType = type2;
                        m_Type = DynamicType.Getter;
                        return;
                    }
                }
            }

            m_Type = DynamicType.Fail;
        }

        private int GetArrayIndex()
        {
            int index = -1;
            if (m_SerializedProperty.propertyPath.EndsWith("]"))
            {
                var path = m_SerializedProperty.propertyPath.TrimEnd(']');
                path = path.Substring(path.LastIndexOf("data[") + 5);
                int.TryParse(path, out index);
            }
            return index;
        }

        private T GetGetterValue()
        {
            return GetGetterValue(m_SerializedProperty.serializedObject.targetObject);
        }

        private T GetGetterValue(UnityEngine.Object obj)
        {
            object self = null, declaring = null;
            if (m_SerializedProperty.GetObjectsFromGetter(m_ObjectGetterList, m_ObjectList))
                self = m_ObjectList[m_ObjectList.Count - 1];
            declaring = m_ObjectList.FirstOrDefault(o => o != null && o.GetType().Equals(m_MethodDeclaring));

            if (m_FieldGetter != null)
                return (T)m_FieldGetter(declaring);
            if (m_StaticGetter != null)
                return m_StaticGetter();
            if (declaring != null && m_InstanceGetter != null)
                return m_InstanceGetter(declaring);

            if (m_ParameterType != null && m_ParameterType.Equals(typeof(SerializedProperty)))
                self = m_SerializedProperty;
            if (self != null && m_StaticInputGetter != null)
                return m_StaticInputGetter(self);
            if (self != null && declaring != null && m_InstanceInputGetter != null)
                return m_InstanceInputGetter(declaring, self);

            return m_Default;
        }

        private T[] GetGetterValues()
        {
            var e = m_SerializedProperty.serializedObject.targetObjects.Select(obj => GetGetterValue(obj));
            return e.ToArray();
        }

        private T GetSerializedPropertyValue()
        {
            return GetSerializedPropertyValue(GetSerializedProperty(m_SerializedProperty));
        }

        private T[] GetSerializedPropertyValues()
        {
            var prop = GetSerializedProperty(m_SerializedProperty);
            if (!prop.hasMultipleDifferentValues)
                return new T[] { GetSerializedPropertyValue(prop) };
            var e = m_SerializedProperty.serializedObject.targetObjects.Select(obj => GetSerializedPropertyValue(obj));
            return e.ToArray();
        }

        private T GetSerializedPropertyValue(UnityEngine.Object obj)
        {
            var so = new SerializedObject(obj);
            var prop = so.FindProperty(m_SerializedProperty.propertyPath);
            return GetSerializedPropertyValue(GetSerializedProperty(prop));
        }

        private static Func<bool, bool> FromBool = (input) => input;
        private static Func<int, int> FromInt = (input) => input;
        private static Func<long, long> FromLong = (input) => input;
        private static Func<float, float> FromFloat = (input) => input;
        private static Func<double, double> FromDouble = (input) => input;
        private static Func<string, string> FromString = (input) => input;
        private static Func<char, char> FromChar = (input) => input;
        private static Func<UnityEngine.Object, UnityEngine.Object> FromObject = (input) => input;

        private static Func<bool, T> BoolConvertor;
        private static Func<int, T> IntConvertor;
        private static Func<long, T> LongConvertor;
        private static Func<float, T> FloatConvertor;
        private static Func<double, T> DoubleConvertor;
        private static Func<string, T> StringConvertor;
        private static Func<char, T> CharConvertor;
        private static Func<UnityEngine.Object, T> ObjectConvertor;

        private void SetupConvertor()
        {
            var type = m_DynamicType;
            if (type == typeof(bool))
                if (BoolConvertor == null) BoolConvertor = (Func<bool, T>)(object)FromBool;
            if (type == typeof(int))
                if (IntConvertor == null) IntConvertor = (Func<int, T>)(object)FromInt;
            if (type == typeof(long))
                if (LongConvertor == null) LongConvertor = (Func<long, T>)(object)FromLong;
            if (type == typeof(float))
                if (FloatConvertor == null) FloatConvertor = (Func<float, T>)(object)FromFloat;
            if (type == typeof(double))
                if (DoubleConvertor == null) DoubleConvertor = (Func<double, T>)(object)FromDouble;
            if (type == typeof(string))
                if (StringConvertor == null) StringConvertor = (Func<string, T>)(object)FromString;
            if (type == typeof(char))
                if (CharConvertor == null) CharConvertor = (Func<char, T>)(object)FromChar;
            if (type == typeof(UnityEngine.Object))
                if (ObjectConvertor == null) ObjectConvertor = (Func<UnityEngine.Object, T>)(object)FromObject;
        }

        private T GetSerializedPropertyValue(SerializedProperty property)
        {
            SetupConvertor();
            var type = m_DynamicType;
            if (type == typeof(string))
                return StringConvertor(property.AsString());
            if (type == typeof(bool) && property.propertyType == SerializedPropertyType.Boolean)
                return BoolConvertor(property.boolValue);
            if (type == typeof(int) && property.propertyType == SerializedPropertyType.Integer)
                return IntConvertor(property.intValue);
            if (type == typeof(long) && property.propertyType == SerializedPropertyType.Integer)
                return LongConvertor(property.longValue);
            if (type == typeof(float) && property.propertyType == SerializedPropertyType.Float)
                return FloatConvertor(property.floatValue);
            if (type == typeof(double) && property.propertyType == SerializedPropertyType.Float)
                return DoubleConvertor(property.doubleValue);
            if (type == typeof(string) && property.propertyType == SerializedPropertyType.String)
                return StringConvertor(property.stringValue);
            if (type == typeof(char) && property.propertyType == SerializedPropertyType.Character)
                return CharConvertor(property.stringValue.Length == 0 ? ' ' : property.stringValue[0]);
            if (type == typeof(UnityEngine.Object) && property.propertyType == SerializedPropertyType.ObjectReference)
                return ObjectConvertor(property.objectReferenceValue);
            return m_Default;
        }
    }
}
#endif
