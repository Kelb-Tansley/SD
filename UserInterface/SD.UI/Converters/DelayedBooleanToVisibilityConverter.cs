using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SD.UI.Converters;
public sealed class DelayedBooleanToVisibilityConverter : DelayedBooleanConverter<Visibility>
{
    public DelayedBooleanToVisibilityConverter() :
        base(Visibility.Visible, Visibility.Collapsed)
    { }
}
public class DelayedBooleanConverter<T> : IValueConverter
{
    public DelayedBooleanConverter(T trueValue, T falseValue)
    {
        TrueValue = trueValue;
        FalseValue = falseValue;
    }

    public T TrueValue { get; set; }
    public T FalseValue { get; set; }

    public virtual object? Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is not bool)
            return FalseValue;

        var parser = int.TryParse(parameter?.ToString(), out int delay);
        if (!parser)
            delay = 0;

        var boolean = (bool)value;
        var visibility = TrueValue;
        Task.Run(async () =>
        {
            await Task.Delay(delay);
            visibility = boolean ? TrueValue : FalseValue;
        }).GetAwaiter().GetResult();

        return visibility;
    }
    public virtual object? ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        => value is T tValue && EqualityComparer<T>.Default.Equals(tValue, TrueValue);
}