//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月17日
//描述：扩展 枚举
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BatchTextureModifier
{
    public static partial class EnumExtends
    {
        private struct CacheData
        {
            /// <summary>
            /// 枚举名称
            /// </summary>
            public string[]? Names = null;
            /// <summary>
            /// 用于显示的名称列表
            /// </summary>
            public string[]? DisplayNames = null;
            /// <summary>
            /// 描述，与名称一一对应
            /// </summary>
            public string[]? Descriptions = null;

            private bool IsAddFirstName = false;
            private Type? EnumType = null;

            public CacheData() { }

            /// <summary>
            /// 初始化名称列表
            /// </summary>
            /// <param name="isAddFirstName"></param>
            /// <param name="addFirstName"></param>
            public void InitNames(Type enumType, bool isAddFirstName, string? addFirstName = null)
            {
                EnumType = enumType;
                IsAddFirstName = isAddFirstName;
                Names = Enum.GetNames(EnumType);

                int displayNameIndex = 0;
                if (isAddFirstName)
                {
                    DisplayNames = new string[Names.Length + 1];
                    DisplayNames[0] = string.IsNullOrEmpty(addFirstName) ? FirstName : addFirstName;
                    displayNameIndex = 1;
                    //Names.CopyTo(DisplayNames, 1);
                }
                else DisplayNames = new string[Names.Length];

                Descriptions = new string[Names.Length];
                for (int i = 0; i < Names.Length; i++)
                {
                    DisplayAttribute? dn = GetAttr<DisplayAttribute>(EnumType.GetField(Names[i]));
                    DisplayNames[i + displayNameIndex] = dn?.Name ?? Names[i];
                    Descriptions[i] = dn?.Description ?? Names[i];
                }

            }

            /// <summary>
            /// 初始化描述列表，必须先初始化过名称列表后，才能正确初始化
            /// </summary>
            /// <returns></returns>
            //public string[]? InitDescrptions()
            //{
            //    if (EnumType == null || Names == null) return null;
            //    Descriptions = new string[Names.Length];
            //    for (int i = 0; i < Descriptions.Length; i++)
            //    {
            //        if (IsAddFirstName && i == 0) continue;
            //        DescriptionAttribute? dss = GetAttr<DescriptionAttribute>(EnumType.GetField(Names[i]));
            //        Descriptions[i] = dss?.Description ?? string.Empty;
            //    }
            //    return Descriptions;
            //}


            public int ToDisplayNameIndex(Enum em)
            {
                int index = Array.IndexOf(Names!, em!.ToString());
                if (IsAddFirstName) return index + 1;
                return index;
            }

            public T DisplayIndexToEnum<T>(int index) where T : struct, Enum
            {
                return DisplayIndexToEnumNullable<T>(index) ?? default(T);
            }

            public T? DisplayIndexToEnumNullable<T>(int index) where T : struct, Enum
            {
                //添加了额外首字段，且选中它，为可空类型
                if (index == 0 && IsAddFirstName) return null;
                object? value;
                if (Enum.TryParse(EnumType!, Names![IsAddFirstName ? index - 1 : index], out value))
                    return (T)value!;
                return null;// new Nullable<T>();
            }

            public override bool Equals([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] object? obj)
            {
                return EnumType == obj as Type;
            }

            public override int GetHashCode()
            {
                return EnumType?.GetHashCode() ?? 0;
            }

            private T? GetAttr<T>(FieldInfo? fieldInfo) where T : Attribute
            {
                if (fieldInfo == null) return null;
                return fieldInfo.GetCustomAttribute<T>();
            }
        }
    }
}