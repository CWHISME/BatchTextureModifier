//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�
//�������� TexturesModifyData  ��������
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System.ComponentModel;

namespace BatchTextureModifier
{
    public enum EScaleMode
    {
        [Description("NotScale�������ţ�����ı�ԭ���ֱ��ʣ����������ʽҲѡ�񱣳ֲ��䣬��ô��ûʲô��")]
        NotScale,
        [Description("Fill(�������)�����ͼƬС���趨�ֱ��ʣ�������Ŵ󣬲���֮������䣻���ͼƬ�����趨�ֱ��ʣ��������С")]
        Fill,
        [Description("DirectScale_Min(ֱ������)��[���趨�ֱ��ʲ��ᱣ��һ��]������С�߳��ﵽ�����ֱ���ֱ����С������ı�ԭ��ͼƬ������Ҳ����Ŵ�")]
        DirectScale_Min,
        [Description("DirectScale_Max(ֱ������)��[���趨�ֱ��ʲ��ᱣ��һ��]����ԭ��ͼƬ�������������·ֱ��ʵı��������ź�ֱ���һ����趨�ֱ��ʸ�С")]
        DirectScale_Max,
        [Description("StretchScale(��������)��ԭͼֱ��������µķֱ��ʣ�����ֱ�����죬�ᵼ��ͼƬ����")]
        StretchScale,
        [Description("ScaleBased(��������)����ԭͼ������ָ���ֱ���ʱ�������ֱ������䣬���ദ�����")]
        ScaleBased,
        [Description("ScaleBasedByCut(�����ü�)���Ը߶Ȼ��������Ϊ��׼�������ţ��������ֱ������䣬���ദֱ�Ӳü�")]
        ScaleBasedByCut,
        [Description("DirectCut(ֱ�Ӳü�)��ֱ�Ӳü���Ŀ��ֱ���")]
        DirectCut,
        [Description("WidthBase(���ڿ��)���Կ��Ϊ��׼�������ţ��߶Ȳ�������䣬�߶ȳ���֮����ü�")]
        WidthBase,
        [Description("HeightBase(���ڸ߶�)���Ը߶�Ϊ��׼�������ţ���Ȳ�������䣬��ȳ���֮����ü�")]
        HeightBase,
        [Description("POT���ţ��߿���������ӽ�2N�η��ķֱ��ʣ��������ֱ������䣬���㴦�����")]
        POT,
        [Description("POT�������ţ��߿���������ӽ�2N�η��ķ��ηֱ��ʣ��������ֱ������䣬���㴦�����")]
        POT_Cube,
        Max
    }

}