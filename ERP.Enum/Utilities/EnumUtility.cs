using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ERP.Enum.Utilities
{
    internal class EnumUtility
    {
        public static IEnumerable<TEnum> GetEnumList<TEnum>()
        {
            Type typeFromHandle = typeof(TEnum);
            FieldInfo[] fields = typeFromHandle.GetFields();
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo = array[i];
                if (fieldInfo.FieldType.IsEnum)
                {
                    yield return (TEnum)System.Enum.Parse(typeFromHandle, fieldInfo.Name);
                }
            }
            array = null;
            yield break;
        }

        public static IDictionary<int, TEnum> GetEnumDict<TEnum>()
        {
            Dictionary<int, TEnum> dictionary = new Dictionary<int, TEnum>();
            Type typeFromHandle = typeof(TEnum);
            FieldInfo[] fields = typeFromHandle.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                if (fieldInfo.FieldType.IsEnum)
                {
                    TEnum value = (TEnum)(System.Enum.Parse(typeFromHandle, fieldInfo.Name));
                    int key = (int)fieldInfo.GetValue(typeFromHandle);
                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }

        public static TArrtibute GetAttribute<TEnum, TArrtibute>(TEnum e) where TArrtibute : System.Attribute
        {
            object[] customAttributes = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(TArrtibute), false);
            if (customAttributes.Length != 0)
            {
                return customAttributes[0] as TArrtibute;
            }
            return default(TArrtibute);
        }

        public static IDictionary<int, TArrtibute> GetEnumDictWithKeyInt<TEnum, TArrtibute>() where TArrtibute : System.Attribute
        {
            Type typeFromHandle = typeof(TEnum);
            Dictionary<int, TArrtibute> dictionary = new Dictionary<int, TArrtibute>();
            FieldInfo[] fields = typeFromHandle.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                if (fieldInfo.FieldType.IsEnum)
                {
                    int key = (int)fieldInfo.GetValue(typeFromHandle);
                    object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(TArrtibute), false);
                    if (customAttributes.Length != 0)
                    {
                        TArrtibute value = customAttributes[0] as TArrtibute;
                        dictionary.Add(key, value);
                    }
                }
            }
            return dictionary;
        }

        public static IDictionary<TEnum, TArrtibute> GetEnumDict<TEnum, TArrtibute>() where TArrtibute : System.Attribute
        {
            Type typeFromHandle = typeof(TEnum);
            Dictionary<TEnum, TArrtibute> dictionary = new Dictionary<TEnum, TArrtibute>();
            FieldInfo[] fields = typeFromHandle.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                if (fieldInfo.FieldType.IsEnum)
                {
                    TEnum key = (TEnum)((object)fieldInfo.GetValue(typeFromHandle));
                    object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(TArrtibute), false);
                    if (customAttributes.Length != 0)
                    {
                        TArrtibute value = customAttributes[0] as TArrtibute;
                        dictionary.Add(key, value);
                    }
                }
            }
            return dictionary;
        }

        public static IDictionary<int, TArrtibute> GetEnumDict<TEnum, TArrtibute>(params int[] rankEnumKeys) where TArrtibute : System.Attribute
        {
            if (rankEnumKeys == null)
            {
                throw new ArgumentNullException("rankEnumKeys");
            }
            List<int> list = rankEnumKeys.ToList<int>();
            Type typeFromHandle = typeof(TEnum);
            Dictionary<int, TArrtibute> dictionary = new Dictionary<int, TArrtibute>();
            FieldInfo[] fields = typeFromHandle.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                if (fieldInfo.FieldType.IsEnum)
                {
                    int num = (int)fieldInfo.GetValue(typeFromHandle);
                    if (list.Contains(num))
                    {
                        object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(TArrtibute), false);
                        if (customAttributes.Length != 0)
                        {
                            TArrtibute value = customAttributes[0] as TArrtibute;
                            dictionary.Add(num, value);
                        }
                    }
                }
            }
            return dictionary;
        }

        public static IDictionary<TEnum, TArrtibute> GetEnumDict<TEnum, TArrtibute>(params TEnum[] rankEnumKeys) where TArrtibute : System.Attribute
        {
            if (rankEnumKeys == null)
            {
                throw new ArgumentNullException("rankEnumKeys");
            }
            List<TEnum> list = rankEnumKeys.ToList<TEnum>();
            Type typeFromHandle = typeof(TEnum);
            Dictionary<TEnum, TArrtibute> dictionary = new Dictionary<TEnum, TArrtibute>();
            FieldInfo[] fields = typeFromHandle.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fieldInfo = fields[i];
                if (fieldInfo.FieldType.IsEnum)
                {
                    TEnum tEnum = (TEnum)((object)fieldInfo.GetValue(typeFromHandle));
                    if (list.Contains(tEnum))
                    {
                        object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(TArrtibute), false);
                        if (customAttributes.Length != 0)
                        {
                            TArrtibute value = customAttributes[0] as TArrtibute;
                            dictionary.Add(tEnum, value);
                        }
                    }
                }
            }
            return dictionary;
        }
    }
}
