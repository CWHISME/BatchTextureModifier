//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：扩展 枚举，使其可以更方便支持 WPF 数据绑定
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace BatchTextureModifier
{
    /// <summary>
    /// 扩展 枚举，使其可以更方便支持 WPF 数据绑定
    /// </summary>
    public static class EnumExtends
    {

        //private static Dictionary<Type, string[]> _enumNamesCache = new Dictionary<Type, string[]>();
        /// <summary>
        /// 枚举转字符串缓存
        /// </summary>
        private static List<CacheData> _cacheDatas = new List<CacheData>(10);
        private const string FirstName = "自动";

        /// <summary>
        /// 枚举直接转字符串数组，可选是否在第一位添加一个自定义名字
        /// 若之前取过值，将会直接返回缓存，可空类型请使用另一个泛型版本方法
        /// </summary>
        /// <param name="em"></param>
        ///<param name="isAddFirstName">是否在第一位添加一个自定义名字，若为true且没有传入addFirstName，则在第一位添加一个默认名字</param>
        /// <param name="addFirstName">在第一位添加一个自定义名字，注：由于缓存存在，addFirstName 取决于第一次调用的传参</param>
        /// <returns></returns>
        public static string[] EnumToNames(this Enum em, bool isAddFirstName = false, string? addFirstName = null)
        {
            if (em == null) return null;
            return GetCacheNames(em, isAddFirstName, addFirstName).Names;
        }

        /// <summary>
        /// 将枚举转化为添加了 System.ComponentModel.Description 描述
        /// 会自动生成 Names 列表并缓存，如需添加首字符串，请先调用 EnumToNames(true)
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string[] EnumToDescriptions(this Enum em)
        {
            if (em == null) return null;
            return GetCacheNames(em, false).Descriptions;
        }

        /// <summary>
        /// 枚举直接转字符串数组，可选是否在第一位添加一个自定义名字
        /// 可空类型使用
        /// </summary>
        /// <returns></returns>
        public static string[] EnumToNames<T>(this Enum? em, bool isAddFirstName = false, string? addFirstName = null)
        {
            return GetCacheNames(typeof(T), isAddFirstName, addFirstName).Names;
        }

        /// <summary>
        /// 枚举值转下标，枚举为可空类型且为空，则返回 0
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static int EnumToIndex(this Enum? em, bool isAddFirstName = false, string? addFirstName = null)
        {
            if (em == null) return 0;
            return Array.IndexOf(GetCacheNames(em, isAddFirstName, addFirstName).Names, em.ToString());
        }

        /// <summary>
        /// 下标转枚举，转换失败则返回默认，注意：可空类型请使用 IndexToEnumNullable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///<param name="isAddFirstName">是否在第一位添加一个自定义名字，若为true且没有传入addFirstName，则在第一位添加一个默认名字</param>
        /// <param name="addFirstName">在第一位添加一个自定义名字，注：由于缓存存在，addFirstName 取决于第一次调用的传参</param>
        /// <returns></returns>
        public static T IndexToEnum<T>(this int index, bool isAddFirstName = false, string? addFirstName = null) where T : struct, Enum
        {
            object o = IndexToEnumNullable<T>(index, isAddFirstName, addFirstName);
            if (o == null) return default(T);
            return (T)o;
        }

        /// <summary>
        /// 下标转枚举，可空类型枚举可用，转换失败返回空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="em"></param>
        /// <param name="index"></param>
        ///<param name="isAddFirstName">是否在第一位添加一个自定义名字，若为true且没有传入addFirstName，则在第一位添加一个默认名字</param>
        /// <param name="addFirstName">在第一位添加一个自定义名字，注：由于缓存存在，addFirstName 取决于第一次调用的传参</param>
        /// <returns></returns>
        public static T? IndexToEnumNullable<T>(this int index, bool isAddFirstName = false, string? addFirstName = null) where T : struct, Enum
        {
            Type type = typeof(T);
            CacheData data = GetCacheNames(type, isAddFirstName, addFirstName);
            object value;
            if (Enum.TryParse(type, data.Names[index], out value))
                return (T)value;
            return null;// new Nullable<T>();
        }

        /// <summary>
        /// 从缓存中取出名称列表，若不存在，则创建并缓存
        /// </summary>
        private static CacheData GetCacheNames(Type enumType, bool isAddFirstName, string? addFirstName = null)
        {
            Type type = enumType;
            int index = _cacheDatas.FindIndex(x => x.EnumType == type);
            if (index > -1) return _cacheDatas[index];
            //创建缓存
            //string[] names = Enum.GetNames(type);
            //if (isAddFirstName)
            //{
            //    string[] addNames = new string[names.Length + 1];
            //    addNames[0] = string.IsNullOrEmpty(addFirstName) ? FirstName : addFirstName;
            //    names.CopyTo(addNames, 1);
            //    names = addNames;
            //}
            CacheData data = new CacheData(type);
            data.InitNames(isAddFirstName, addFirstName);
            data.InitDescrptions();
            _cacheDatas.Add(data);
            return data;
        }

        /// <summary>
        /// 从缓存中取出名称列表，若不存在，则创建并缓存
        /// </summary>
        private static CacheData GetCacheNames(Enum em, bool isAddFirstName, string? addFirstName = null)
        {
            return GetCacheNames(em.GetType(), isAddFirstName, addFirstName);
        }

        private struct CacheData
        {
            public bool IsAddFirstName = false;
            public Type EnumType;
            public string[]? Names = null;
            public string[]? Descriptions = null;

            public CacheData(Type type)
            {
                EnumType = type;
            }

            public void InitNames(bool isAddFirstName, string? addFirstName = null)
            {
                IsAddFirstName = isAddFirstName;
                Names = Enum.GetNames(EnumType);

                if (isAddFirstName)
                {
                    string[] addNames = new string[Names.Length + 1];
                    addNames[0] = string.IsNullOrEmpty(addFirstName) ? FirstName : addFirstName;
                    Names.CopyTo(addNames, 1);
                    Names = addNames;
                }
            }

            public void InitDescrptions()
            {
                if (Names == null) InitNames(false);
                Descriptions = new string[Names.Length];
                for (int i = 0; i < Descriptions.Length; i++)
                {
                    if (IsAddFirstName && i == 0) continue;
                    DescriptionAttribute? dss = GetAttr<DescriptionAttribute>(EnumType.GetField(Names[i]));
                    if (dss == null)
                    {
                        Descriptions[i] = string.Empty;
                        continue;
                    }
                    Descriptions[i] = dss.Description;
                }
            }

            private T? GetAttr<T>(FieldInfo? fieldInfo) where T : Attribute
            {
                if (fieldInfo == null) return null;
                return fieldInfo.GetCustomAttribute<T>();
            }
        }
    }
}