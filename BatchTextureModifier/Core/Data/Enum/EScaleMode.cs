//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�
//�������� TexturesModifyData  ��������
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BatchTextureModifier
{
    public enum EScaleMode
    {
        [Display(Name = "������", Description = "�����ţ�����ı�ԭ���ֱ��ʣ����������ʽҲѡ�񱣳ֲ��䣬��ô��ûʲô��")]
        NotScale,
        [Display(Name = "��������", Description = "��������II����ԭͼ������ָ���ֱ���ʱ�������ֱ������䣬���ദ�����")]
        ScaleBased,
        [Display(Name = "ֱ�Ӳü�", Description = "ֱ�Ӳü���ֱ�Ӳü���Ŀ��ֱ���")]
        DirectCut,
        [Display(Name = "���ڿ��", Description = "���ڿ�ȣ��Կ��Ϊ��׼�������ţ��߶Ȳ�������䣬�߶ȳ���֮����ü�")]
        WidthBase,
        [Display(Name = "���ڸ߶�", Description = "���ڸ߶ȣ��Ը߶�Ϊ��׼�������ţ���Ȳ�������䣬��ȳ���֮����ü�")]
        HeightBase,
        [Display(Name = "��������II", Description = "�������ţ����ͼƬС���趨�ֱ��ʣ�������Ŵ󣬲���֮������䣻���ͼƬ�����趨�ֱ��ʣ��������С")]
        Pad,
        [Display(Name = "�����ü�", Description = "�����ü����Ը߶Ȼ��������Ϊ��׼�������ţ��������ֱ������䣬���ദֱ�Ӳü�")]
        ScaleBasedByCut,
        [Display(Name = "ֱ������(����)", Description = "ֱ������(����)��[���趨�ֱ��ʲ��ᱣ��һ��]������С�߳��ﵽ�����ֱ���ֱ����С������ı�ԭ��ͼƬ������Ҳ����Ŵ�")]
        DirectScale_Min,
        [Display(Name = "ֱ������(����)", Description = "ֱ������(����)��[���趨�ֱ��ʲ��ᱣ��һ��]����ԭ��ͼƬ�������������·ֱ��ʵı��������ź�ֱ���һ����趨�ֱ��ʸ�С")]
        DirectScale_Max,
        [Display(Name = "��������", Description = "�������ţ�ԭͼֱ��������µķֱ��ʣ�����ֱ�����죬�ᵼ��ͼƬ����")]
        StretchScale,
        [Display(Name = "POT����", Description = "POT���ţ��߿���������ӽ�2N�η��ķֱ��ʣ��������ֱ������䣬���㴦�����")]
        POT,
        [Display(Name = "POT��������", Description = "POT�������ţ��߿���������ӽ�2N�η��ķ��ηֱ��ʣ��������ֱ������䣬���㴦�����")]
        POT_Cube
    }

}