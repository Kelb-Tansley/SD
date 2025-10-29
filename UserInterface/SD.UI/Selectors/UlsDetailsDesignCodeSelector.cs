using SD.Core.Shared.Enum;
using System.Windows;
using System.Windows.Controls;

namespace SD.UI.Selectors;

public class UlsDetailsDesignCodeSelector : DataTemplateSelector
{
    public DataTemplate? SansDetailsDataTemplate { get; set; }
    public DataTemplate? ASDetailsDataTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object value, DependencyObject container)
    {
        var parString = value?.ToString();
        var parseResults = Enum.TryParse(parString, out DesignCode matchingDesignType);
        if (!parseResults)
            matchingDesignType = DesignCode.SANS;

        return matchingDesignType switch
        {
            DesignCode.SANS => SansDetailsDataTemplate,
            DesignCode.AS => ASDetailsDataTemplate,
            _ => SansDetailsDataTemplate,
        };
    }
}