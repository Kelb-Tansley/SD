using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Extensions;
using SD.Core.Shared.Models;
using System.Collections.ObjectModel;

namespace SD.UI.ViewModel;
public abstract partial class LoadCasesViewModelBase(IProcessModel processModel) : ViewModelBase(processModel)
{
    [ObservableProperty]
    public bool _areAllSelected = false;

    [ObservableProperty]
    public bool _autoCalculate = false;

    [ObservableProperty]
    public bool _canCalculate = false;

    [ObservableProperty]
    public ObservableCollection<LoadCaseCombination> _loadCaseCombinations = [];

    [ObservableProperty]
    public LoadCaseCombination? _selectedLoadCaseCombination;

    [RelayCommand]
    public async Task SelectedItemChanged()
    {
        if (SelectedLoadCaseCombination == null)
            return;

        SelectedLoadCaseCombination.Include = !SelectedLoadCaseCombination.Include;

        AreAllSelected = LoadCaseCombinations.All(slcc => slcc.Include);
        
        await AutoCalculateChanged();
    }

    [RelayCommand]
    public async Task CheckAll()
    {
        foreach (var loadCaseCombination in LoadCaseCombinations)
            loadCaseCombination.Include = AreAllSelected;

        await AutoCalculateChanged();
    }

    [RelayCommand]
    public async Task AutoCalculateChanged()
    {
        if (AutoCalculate)
            await SelectedLoadCombinationsChanged();

        CanCalculate = LoadCaseCombinations.Any(slcc => slcc.Include) && !AutoCalculate ? true : false;
    }

    [RelayCommand]
    public async Task Calculate()
    {
        await SelectedLoadCombinationsChanged();
    }

    protected void UpdateLoadCombinations(IEnumerable<LoadCaseCombination> loadCaseCombinations)
    {
        var updatedLoadCombinations = new List<LoadCaseCombination>();

        foreach (var lcc in loadCaseCombinations)
        {
            //Perform a deep clone not to effect the shared fem model parameters
            var updatedLoadCombination = LoadCaseCombination.Clone(lcc);

            //Default all cloned load cases to unselected state
            updatedLoadCombination.Include = false;

            //Try to find a match with the previous load combination so that we can keep their selected state
            if (LoadCaseCombinations.Any())
            {
                var matchingLoadCombination = LoadCaseCombinations.FirstOrDefault(lc => LoadCaseCombination.AreEqual(updatedLoadCombination, lc));
                if (matchingLoadCombination != null)
                    updatedLoadCombination.Include = matchingLoadCombination.Include;
            }

            updatedLoadCombinations.Add(updatedLoadCombination);
        }

        LoadCaseCombinations.SetRange(updatedLoadCombinations);
    }
    protected abstract Task SelectedLoadCombinationsChanged();
}
