//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��15��
//��������չ ö�٣�ʹ����Ը�����֧�� WPF ���ݰ�
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;

namespace BatchTextureModifier
{
    /// <summary>
    /// ��չ ö�٣�ʹ����Ը�����֧�� WPF ���ݰ�
    /// </summary>
    public static partial class EnumExtends
    {

        //private static Dictionary<Type, string[]> _enumNamesCache = new Dictionary<Type, string[]>();
        /// <summary>
        /// ö��ת�ַ�������
        /// </summary>
        //private static List<CacheData> _cacheDatas = new List<CacheData>(10);
        //private static CacheData[] _cacheDatas = new CacheData[10];
        private static StructRefernceArray<CacheData> _cacheDatas = new StructRefernceArray<CacheData>();
        //private static int _cacheUsingIndex = 0;

        //���治��ʱ����������
        //private const int CacheIncreaseNum = 5;
        private const string FirstName = "�Զ�";
        public static readonly string[] NullStrings = new string[] { "Null" };

        /// <summary>
        /// ö��ֱ��ת�ַ������飬��ѡ�Ƿ��ڵ�һλ���һ���Զ�������
        /// ��֮ǰȡ��ֵ������ֱ�ӷ��ػ��棬�ɿ�������ʹ����һ�����Ͱ汾����
        /// </summary>
        /// <param name="em"></param>
        ///<param name="isAddFirstName">�Ƿ��ڵ�һλ���һ���Զ������֣���Ϊtrue��û�д���addFirstName�����ڵ�һλ���һ��Ĭ������</param>
        /// <param name="addFirstName">�ڵ�һλ���һ���Զ������֣�ע�����ڻ�����ڣ�addFirstName ȡ���ڵ�һ�ε��õĴ���</param>
        /// <returns></returns>
        public static string[] EnumToNames(this Enum em, bool isAddFirstName = false, string? addFirstName = null)
        {
            if (em == null) return NullStrings;
            return GetCacheData(em, isAddFirstName, addFirstName).DisplayNames!;
        }

        /// <summary>
        /// ö��ֱ��ת�ַ������飬��ѡ�Ƿ��ڵ�һλ���һ���Զ�������
        /// �ɿ�����ʹ��
        /// </summary>
        /// <returns></returns>
        public static string[] EnumToNames<T>(this Enum? em, bool isAddFirstName = false, string? addFirstName = null)
        {
            return GetCacheData(typeof(T), isAddFirstName, addFirstName).DisplayNames!;
        }

        /// <summary>
        /// ��ö��ת��Ϊ����� System.ComponentModel.Description ����
        /// ���Զ����� Names �б����棬����������ַ��������ȵ��� EnumToNames(true)
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
        /// ö��ֵת�±꣬ö��Ϊ�ɿ�������Ϊ�գ��򷵻� 0
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
        /// �±�תö�٣�ת��ʧ���򷵻�Ĭ�ϣ�ע�⣺�ɿ�������ʹ�� IndexToEnumNullable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///<param name="isAddFirstName">�Ƿ��ڵ�һλ���һ���Զ������֣���Ϊtrue��û�д���addFirstName�����ڵ�һλ���һ��Ĭ������</param>
        /// <param name="addFirstName">�ڵ�һλ���һ���Զ������֣�ע�����ڻ�����ڣ�addFirstName ȡ���ڵ�һ�ε��õĴ���</param>
        /// <returns></returns>
        public static T IndexToEnum<T>(this int index) where T : struct, Enum
        {
            return IndexToEnumNullable<T>(index) ?? default(T);
        }

        /// <summary>
        /// �±�תö�٣��ɿ�����ö�ٿ��ã�ת��ʧ�ܷ��ؿ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="em"></param>
        /// <param name="index"></param>
        ///<param name="isAddFirstName">�Ƿ��ڵ�һλ���һ���Զ������֣���Ϊtrue��û�д���addFirstName�����ڵ�һλ���һ��Ĭ������</param>
        /// <param name="addFirstName">�ڵ�һλ���һ���Զ������֣�ע�����ڻ�����ڣ�addFirstName ȡ���ڵ�һ�ε��õĴ���</param>
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
        /// �ӻ�����ȡ�������б��������ڣ��򴴽�������
        /// </summary>
        private static ref CacheData GetCacheData(Type enumType, bool isAddFirstName = false, string? addFirstName = null)
        {
            //Type type = enumType;
            //�����Ƿ���ڻ���
            int index = _cacheDatas.IndexOf(enumType);//_cacheDatas.FindIndex(x => x.EnumType == type);
            if (index > -1) return ref _cacheDatas[index];
            //��������
            ref CacheData data = ref _cacheDatas.Next();
            data.InitNames(enumType, isAddFirstName, addFirstName);
            return ref data;
        }

        /// <summary>
        /// �ӻ�����ȡ�������б��������ڣ��򴴽�������
        /// </summary>
        private static ref CacheData GetCacheData(Enum em, bool isAddFirstName = false, string? addFirstName = null)
        {
            return ref GetCacheData(em.GetType(), isAddFirstName, addFirstName);
        }
    }
}