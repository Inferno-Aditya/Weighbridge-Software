using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using VinayakEnterprises.Core.Models;

namespace VinayakEnterprises.App.Converters;

public class RoleToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Role role && parameter is string allowedRolesStr)
        {
            var allowedRoles = allowedRolesStr.Split(',').Select(r => r.Trim());
            if (allowedRoles.Contains(role.Name))
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
