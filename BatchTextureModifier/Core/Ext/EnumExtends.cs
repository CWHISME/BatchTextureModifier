//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：扩展 枚举，使其可以更方便支持 WPF 数据绑定
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;

namespace BatchTextureModifier
{
    /// <summary>
    /// 扩展 枚举，使其可以更方便支持 WPF 数据绑定
    /// </summary>
    public static partial class EnumExtends
    {

        //private static Dictionary<Type, string[]> _enumNamesCache = new Dictionary<Type, string[]>();
        /// <summary>
        /// 枚举转字符串缓存
        /// </summary>
        //private static List<CacheData> _cacheDatas = new List<CacheData>(10);
        //private static CacheData[] _cacheDatas = new CacheData[10];
        private static StructRefernceArray<CacheData> _cacheDatas = new StructRefernceArray<CacheData>();
        //private static int _cacheUsingIndex = 0;

        //缓存不足时，递增数量
        //private const int CacheIncreaseNum = 5;
        private const string FirstName = "自动";
        public static readonly string[] NullStrings = new string[] { "Null" };

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
            if (em == null) return NullStrings;
            return GetCacheData(em, isAddFirstName, addFirstName).DisplayNames!;
        }

        /// <summary>
        /// 枚举直接转字符串数组，可选是否在第一位添加一个自定义名字
        /// 可空类型使用
        /// </summary>
        /// <returns></returns>
        public static string[] EnumToNames<T>(this Enum? em, bool isAddFirstName = false, string? addFirstName = null)
        {
            return GetCacheData(typeof(T), isAddFirstName, addFirstName).DisplayNames!;
        }

        /// <summary>
        /// 将枚举转化为添加了 System.ComponentModel.Description 描述
        /// 会自动生成 Names 列表并缓存，如需添加首字符串，请先调用 EnumToNames(true)
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string[] EnumToDescriptions(this Enum em)
        {
            if (em == null) return NullStrings;
            ref CacheData data = ref GetCacheData(em, false);
            return data.Descriptions ?? NullStrings;
            //if(data.Descriptions)
            //return GetCacheData(em, false).Descriptions!;
        }

        /// <summary>
        /// 枚举值转下标，枚举为可空类型且为空，则返回 0
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static int EnumToIndex(this Enum? em)
        {
            if (em == null) return 0;
            //return Array.IndexOf(GetCacheData(em, isAddFirstName, addFirstName).Names!, em.ToString());
            return GetCacheData(em).ToDisplayNameIndex(em);
        }

        /// <summary>
        /// 下标转枚举，转换失败则返回默认，注意：可空类型请使用 IndexToEnumNullable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///<param name="isAddFirstName">是否在第一位添加一个自定义名字，若为true且没有传入addFirstName，则在第一位添加一个默认名字</param>
        /// <param name="addFirstName">在第一位添加一个自定义名字，注：由于缓存存在，addFirstName 取决于第一次调用的传参</param>
        /// <returns></returns>
        public static T IndexToEnum<T>(this int index) where T : struct, Enum
        {
            return IndexToEnumNullable<T>(index) ?? default(T);
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
        public static T? IndexToEnumNullable<T>(this int index) where T : struct, Enum
        {
            //Type type = typeof(T);
            //CacheData data = GetCacheData(type, isAddFirstName, addFirstName);
            //object? value;
            //if (Enum.TryParse(type, data.Names![index], out value))
            //    return (T)value!;
            //return null;// new Nullable<T>();
            return GetCacheData(typeof(T)).DisplayIndexToEnumNullable<T>(index);
        }

        /// <summary>
        /// 从缓存中取出名称列表，若不存在，则创建并缓存
        /// </summary>
        private static ref CacheData GetCacheData(Type enumType, bool isAddFirstName = false, string? addFirstName = null)
        {
            //Type type = enumType;
            //查找是否存在缓存
            int index = _cacheDatas.IndexOf(enumType);//_cacheDatas.FindIndex(x => x.EnumType == type);
            if (index > -1) return ref _cacheDatas[index];
            //创建缓存
            ref CacheData data = ref _cacheDatas.Next();
            data.InitNames(enumType, isAddFirstName, addFirstName);
            return ref data;
        }

        /// <summary>
        /// 从缓存中取出名称列表，若不存在，则创建并缓存
        /// </summary>
        private static ref CacheData GetCacheData(Enum em, bool isAddFirstName = false, string? addFirstName = null)
        {
            return ref GetCacheData(em.GetType(), isAddFirstName, addFirstName);
        }
    }
}