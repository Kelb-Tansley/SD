using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Events;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;
using SD.UI.Constants;
using SD.UI.Events;
using SD.UI.Serviceability.Events;
using SD.UI.Serviceability.Models;
using SD.UI.ViewModel;

namespace SD.UI.Serviceability.ViewModels;
public partial class LoadCombinationsViewModel : LoadCasesViewModelBase
{
    private readonly IDesignCodeAdapter _femDesignAdapter;
    private readonly IFemModelDisplayService _femModelDisplayService;
    private readonly IDesignModel _designModel;
    private readonly IFemModel _femModel;
    private readonly IEventAggregator _eventAggregator;
    private readonly IFemModelParameters _femModelParameters;
    private readonly INotificationService _notificationService;
    private readonly FemLoadedEvent _femLoadedEvent;
    private readonly SelectedLoadCombinationsChangedEvent _selectedLoadCombinationsChangedEvent;

    public LoadCombinationsViewModel(IProcessModel processModel,
                                IDesignModel designModel,
                                IFemModel femModel,
                                IDesignCodeAdapter femDesignAdapter,
                                IEventAggregator eventAggregator,
                                IFemModelDisplayService femModelDisplayService,
                                IFemModelParameters femModelParameters,
                                INotificationService notificationService) : base(processModel)
    {
        _designModel = designModel;
        _femModel = femModel;
        _eventAggregator = eventAggregator;
        _femDesignAdapter = femDesignAdapter;
        _femModelDisplayService = femModelDisplayService;
        _femModelParameters = femModelParameters;
        _notificationService = notificationService;

        _femLoadedEvent = _eventAggregator.GetEvent<FemLoadedEvent>();
        _selectedLoadCombinationsChangedEvent = _eventAggregator.GetEvent<SelectedLoadCombinationsChangedEvent>();

        _femLoadedEvent.Subscribe(Loaded);
    }

    [ObservableProperty]
    public DeflectionMethodViewModel _deflectionMethodViewModel = new();

    [ObservableProperty]
    public DeflectionAxisViewModel _deflectionAxisViewModel = new();

    [ObservableProperty]
    public double _minSpanDeflectionRatio = 300;

    [RelayCommand]
    public void Loaded()
    {
        UpdateLoadCombinations(_femModelParameters.LoadCaseCombinations);
    }

    protected override async Task SelectedLoadCombinationsChanged()
    {
        try
        {
            await SetPrimaryProcess(false, true, true);
            if (LoadCaseCombinations == null)
                return;

            var selectedLoadCaseNumbers = LoadCaseCombinations.Where(sll => sll.Include)?.Select(sll => sll.Number);
            _femModelDisplayService.ReloadFemDisplayModel(FemModels.ServiceabilityModelId, _femModel.FileName, false);

            var designableBeams = _femModelParameters.Beams.Where(beam => beam.CanDesign()).ToList();
            var visibleBeams = _femModelDisplayService.GetDisplayedByGroupBeams(FemModels.ModelId, designableBeams);
            var deflectionResults = await _femDesignAdapter.GetDeflectionService(_designModel.DesignCode.ToDesignCodeEnum()).GetDeflectionResults(FemModels.ModelId, LoadCaseCombinations, visibleBeams, DeflectionAxisViewModel.Selected, DeflectionMethodViewModel.Selected);

            _selectedLoadCombinationsChangedEvent?.Publish(new CalculateEventModel()
            {
                DeflectionAxis = DeflectionAxisViewModel.Selected,
                DeflectionResults = deflectionResults,
                MinDeflectionRatio = MinSpanDeflectionRatio
            });
        }
        catch (Exception ex)
        {
            _notificationService.NotifyUserOfErrorAndCloseFile(new Notification("Error", ex.Message));
        }
        finally
        {
            await SetPrimaryProcess(true, true, true);
        }
    }
}
