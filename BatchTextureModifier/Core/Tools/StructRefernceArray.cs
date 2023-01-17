//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月17日
//描述：扩展 值类型数组，自动扩容，支持 ref，需要注意使用危险性
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;

namespace BatchTextureModifier
{
    /// <summary>
    /// 扩展 值类型数组，自动扩容，支持 ref，需要注意使用危险性
    /// </summary>
    public class StructRefernceArray<T> where T : struct
    {
        private T[] _datas;
        private int _usingIndex;
        //不足时，递增数量
        private int _increaseNum;

        public StructRefernceArray(int capacity = 1, int increaseNum = 5)
        {
            _datas = new T[capacity];
            _usingIndex = 0;
            _increaseNum = increaseNum;
        }

        public ref T this[int index]
        {
            get
            {
                if (index > _usingIndex) _usingIndex = index + 1;
                //超出上限，递增扩容
                if (_usingIndex > _datas.Length)
                {
                    T[] newDatas = new T[_usingIndex + _increaseNum];
                    Array.Copy(_datas, 0, newDatas, 0, _datas.Length);
                    _datas = newDatas;
                }
                return ref _datas[index];
            }
        }

        /// <summary>
        /// 返回当前使用过的最后一位，并将使用下标递增一位
        /// </summary>
        /// <returns></returns>
        public ref T Next()
        {
            return ref this[_usingIndex++];
        }

        /// <summary>
        /// 返回指定值的第一个下标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(T value)
        {
            return Array.IndexOf(_datas, value, 0, _usingIndex);
        }

        /// <summary>
        /// 返回指定值的第一个下标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(object? value)
        {
            return Array.IndexOf(_datas, value, 0, _usingIndex);
        }

        /// <summary>
        /// 返回匹配的的第一个下标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int FindIndex(Predicate<T> match)
        {
            return Array.FindIndex(_datas, 0, _usingIndex, match);
        }

        /// <summary>
        /// 当前使用过的数量
        /// </summary>
        public int Count { get { return _usingIndex; } }
        /// <summary>
        /// 当前总量
        /// </summary>
        public int TotalCount { get { return _datas.Length; } }

    }
}