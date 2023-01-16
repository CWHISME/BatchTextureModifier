//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��15��
//��������չ ö�٣�ʹ����Ը�����֧�� WPF ���ݰ�
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace BatchTextureModifier
{
    /// <summary>
    /// ��չ ö�٣�ʹ����Ը�����֧�� WPF ���ݰ�
    /// </summary>
    public static class EnumExtends
    {

        //private static Dictionary<Type, string[]> _enumNamesCache = new Dictionary<Type, string[]>();
        /// <summary>
        /// ö��ת�ַ�������
        /// </summary>
        private static List<CacheData> _cacheDatas = new List<CacheData>(10);
        private const string FirstName = "�Զ�";

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
            if (em == null) return null;
            return GetCacheNames(em, isAddFirstName, addFirstName).Names;
        }

        /// <summary>
        /// ��ö��ת��Ϊ����� System.ComponentModel.Description ����
        /// ���Զ����� Names �б����棬����������ַ��������ȵ��� EnumToNames(true)
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string[] EnumToDescriptions(this Enum em)
        {
            if (em == null) return null;
            return GetCacheNames(em, false).Descriptions;
        }

        /// <summary>
        /// ö��ֱ��ת�ַ������飬��ѡ�Ƿ��ڵ�һλ���һ���Զ�������
        /// �ɿ�����ʹ��
        /// </summary>
        /// <returns></returns>
        public static string[] EnumToNames<T>(this Enum? em, bool isAddFirstName = false, string? addFirstName = null)
        {
            return GetCacheNames(typeof(T), isAddFirstName, addFirstName).Names;
        }

        /// <summary>
        /// ö��ֵת�±꣬ö��Ϊ�ɿ�������Ϊ�գ��򷵻� 0
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static int EnumToIndex(this Enum? em, bool isAddFirstName = false, string? addFirstName = null)
        {
            if (em == null) return 0;
            return Array.IndexOf(GetCacheNames(em, isAddFirstName, addFirstName).Names, em.ToString());
        }

        /// <summary>
        /// �±�תö�٣�ת��ʧ���򷵻�Ĭ�ϣ�ע�⣺�ɿ�������ʹ�� IndexToEnumNullable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///<param name="isAddFirstName">�Ƿ��ڵ�һλ���һ���Զ������֣���Ϊtrue��û�д���addFirstName�����ڵ�һλ���һ��Ĭ������</param>
        /// <param name="addFirstName">�ڵ�һλ���һ���Զ������֣�ע�����ڻ�����ڣ�addFirstName ȡ���ڵ�һ�ε��õĴ���</param>
        /// <returns></returns>
        public static T IndexToEnum<T>(this int index, bool isAddFirstName = false, string? addFirstName = null) where T : struct, Enum
        {
            object o = IndexToEnumNullable<T>(index, isAddFirstName, addFirstName);
            if (o == null) return default(T);
            return (T)o;
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
        /// �ӻ�����ȡ�������б��������ڣ��򴴽�������
        /// </summary>
        private static CacheData GetCacheNames(Type enumType, bool isAddFirstName, string? addFirstName = null)
        {
            Type type = enumType;
            int index = _cacheDatas.FindIndex(x => x.EnumType == type);
            if (index > -1) return _cacheDatas[index];
            //��������
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
        /// �ӻ�����ȡ�������б��������ڣ��򴴽�������
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