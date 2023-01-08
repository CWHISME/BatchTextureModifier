using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BatchTextureModifier
{
    /// <summary>
    /// 支持非 Text 的多数据源绑定
    /// </summary>
    public class MultiContentConverter : IMultiValueConverter
    {
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return string.Format(parameter.ToString(), values);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
