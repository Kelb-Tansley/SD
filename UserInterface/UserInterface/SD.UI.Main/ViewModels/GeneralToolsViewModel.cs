using CommunityToolkit.Mvvm.ComponentModel;
using SD.UI.Enums;
using SD.UI.Events;

namespace SD.UI.Main.ViewModels;
public partial class GeneralToolsViewModel : ObservableObject
{
    private readonly IEventAggregator _eventAggregator;

    private readonly GeneralToolsViewChangedEvent _generalToolsViewChangedEvent;

    [ObservableProperty]
    private GeneralToolsView _selectedToolView;

    public GeneralToolsViewModel(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;

        _generalToolsViewChangedEvent = _eventAggregator.GetEvent<GeneralToolsViewChangedEvent>();

        _generalToolsViewChangedEvent.Subscribe(GeneralToolsViewChanged);
    }

    public void GeneralToolsViewChanged(GeneralToolsView generalToolsView)
    {
        SelectedToolView = generalToolsView;
    }
}