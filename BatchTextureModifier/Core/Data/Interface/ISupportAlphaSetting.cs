//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��15��
//������֧��͸��ͨ����ͼƬ��ʽ��������������
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

namespace BatchTextureModifier
{
    public interface ISupportAlphaSetting
    {
        //��ǰ�����Ƿ�֧��͸��������
        public bool IsAllowAlpha();
        //���ò�����͸����
        public void SetAllowAlpha(bool allow);
    }
}