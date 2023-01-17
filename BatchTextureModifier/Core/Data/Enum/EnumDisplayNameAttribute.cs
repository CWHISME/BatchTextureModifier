//=========================================
//作者：wangjiaying@cwhisme
//日期：2023.1.17
//描述：
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;

namespace BatchTextureModifier
{
    [AttributeUsage(AttributeTargets.Field)]
    class EnumDisplayNameAttribute : Attribute
    {

        private readonly string displayName;
        public string DisplayName { get { return displayName; } }

        public EnumDisplayNameAttribute(string name)
        {
            displayName = name;
        }
    }
}