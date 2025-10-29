using SD.UI.Enums;
using System.Windows;
using System.Windows.Controls;

namespace SD.UI.Selectors;

public class GeneralToolsViewSelector : DataTemplateSelector
{
    public DataTemplate? WindLoadingDataTemplate { get; set; }
    public DataTemplate? BucklingAnalysisDataTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object value, DependencyObject container)
    {
        var parString = value?.ToString();
        var parseResults = Enum.TryParse(parString, out GeneralToolsView matchingView);
        if (!parseResults)
            matchingView = GeneralToolsView.WindLoading;

        return matchingView switch
        {
            GeneralToolsView.WindLoading => WindLoadingDataTemplate,
            GeneralToolsView.BucklingAnalysis => BucklingAnalysisDataTemplate,
            _ => WindLoadingDataTemplate,
        };
    }
}