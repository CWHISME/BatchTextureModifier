//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��17��
//��������չ ֵ�������飬�Զ����ݣ�֧�� ref����Ҫע��ʹ��Σ����
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;

namespace BatchTextureModifier
{
    /// <summary>
    /// ��չ ֵ�������飬�Զ����ݣ�֧�� ref����Ҫע��ʹ��Σ����
    /// </summary>
    public class StructRefernceArray<T> where T : struct
    {
        private T[] _datas;
        private int _usingIndex;
        //����ʱ����������
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
                //�������ޣ���������
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
        /// ���ص�ǰʹ�ù������һλ������ʹ���±����һλ
        /// </summary>
        /// <returns></returns>
        public ref T Next()
        {
            return ref this[_usingIndex++];
        }

        /// <summary>
        /// ����ָ��ֵ�ĵ�һ���±�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(T value)
        {
            return Array.IndexOf(_datas, value, 0, _usingIndex);
        }

        /// <summary>
        /// ����ָ��ֵ�ĵ�һ���±�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(object? value)
        {
            return Array.IndexOf(_datas, value, 0, _usingIndex);
        }

        /// <summary>
        /// ����ƥ��ĵĵ�һ���±�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int FindIndex(Predicate<T> match)
        {
            return Array.FindIndex(_datas, 0, _usingIndex, match);
        }

        /// <summary>
        /// ��ǰʹ�ù�������
        /// </summary>
        public int Count { get { return _usingIndex; } }
        /// <summary>
        /// ��ǰ����
        /// </summary>
        public int TotalCount { get { return _datas.Length; } }

    }
}