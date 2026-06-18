using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Interfaces;

namespace SD.UI.UltimateLimitState.ViewModels;
public partial class BeamPropertiesViewModel(IDesignModel designModel,
                                             IFemModelParameters femModelParameters,
                                             IDesignCodeAdapter femDesignAdapter) : ObservableObject
{
    private readonly IDesignCodeAdapter _femDesignAdapter = femDesignAdapter ?? throw new ArgumentNullException(nameof(femDesignAdapter));

    [ObservableProperty]
    public partial IDesignModel DesignModel { get; set; } = designModel ?? throw new ArgumentNullException(nameof(designModel));

    [ObservableProperty]
    public partial Section? SelectedBeamSection { get; set; } = null;

    [ObservableProperty]
    public partial IFemModelParameters FemModelParameters { get; set; } = femModelParameters ?? throw new ArgumentNullException(nameof(femModelParameters));

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
