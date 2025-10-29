using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;
using SD.UI.Constants;

namespace SD.UI.UltimateLimitState.ViewModels;
public partial class BeamPropertiesViewModel(IDesignModel designModel,
                                             IFemModelParameters femModelParameters,
                                             IDesignCodeAdapter femDesignAdapter) : ObservableObject
{
    private IDesignCodeAdapter _femDesignAdapter = femDesignAdapter ?? throw new ArgumentNullException(nameof(femDesignAdapter));

    [ObservableProperty]
    private IDesignModel _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));

    [ObservableProperty]
    private Section? selectedBeamSection = null;

    [ObservableProperty]
    private IFemModelParameters _femModelParameters = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));

    [RelayCommand]
    private void SteelGradeChanged()
    {
        if (SelectedBeamSection != null)
            _femDesignAdapter.GetBeamPropertiesService(DesignModel.DesignCode.ToDesignCodeEnum()).UpdateSectionMaterial(SelectedBeamSection);
    }

    public void SteelGradeChanged(string grade)
    {
        if (SelectedBeamSection?.Material != null && !string.IsNullOrEmpty(grade))
        {
            SelectedBeamSection.Material.SteelGrade = grade;
            _femDesignAdapter.GetBeamPropertiesService(DesignModel.DesignCode.ToDesignCodeEnum()).UpdateSectionMaterial(SelectedBeamSection);
        }
    }
}
