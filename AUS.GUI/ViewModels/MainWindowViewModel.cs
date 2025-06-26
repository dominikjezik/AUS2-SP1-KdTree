using System.Collections.Generic;
using System.Collections.ObjectModel;
using AUS.DataStructures.GeoArea;

namespace AUS.GUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private AreaObjectDTO? _selectedAreaObjectUpdated;
    private ObservableCollection<AreaObjectDTO> _areaObjects;
    private ObservableCollection<AreaObjectDTO> _associatedObjects;
    
    public AreaObjectDTO? SelectedAreaObject
    {
        get => _selectedAreaObjectUpdated;
        set
        {
            OriginalSelectedAreaObject = value;
            
            if (OriginalSelectedAreaObject != null)
            {
                _selectedAreaObjectUpdated = new AreaObjectDTO(OriginalSelectedAreaObject);
                AssociatedObjects = new ObservableCollection<AreaObjectDTO>(OriginalSelectedAreaObject.AssociatedObjects);
            }
            else
            {
                _selectedAreaObjectUpdated = null;
            }
            
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsSelectedAreaObject));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(TitleAssociatedObjects));
        }
    }
    
    public AreaObjectDTO? OriginalSelectedAreaObject { get; set; }
    
    public bool IsSelectedAreaObject => SelectedAreaObject != null;

    public string Title => SelectedAreaObject == null ? string.Empty : "Edit " + (SelectedAreaObject.Type == AreaObjectType.RealEstate ? "real estate" : "parcel");

    public string TitleAssociatedObjects => SelectedAreaObject == null ? string.Empty : (SelectedAreaObject.Type == AreaObjectType.RealEstate ? "Parcels" : "REs");
    
    public AreaObjectDTO AreaObjectQuery { get; set; } = new() { Type = AreaObjectType.Unknown };

    public ObservableCollection<AreaObjectDTO> AreaObjects
    {
        get => _areaObjects;
        set {
            _areaObjects = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<AreaObjectDTO> AssociatedObjects
    {
        get => _associatedObjects;
        set {
            _associatedObjects = value;
            OnPropertyChanged();
        }
    }
    
    public Dictionary<AreaObjectType, string> ObjectTypesFilter { get; set; } = new()
    {
        { AreaObjectType.Unknown, "Any" },
        { AreaObjectType.Parcel, "Parcel" },
        { AreaObjectType.RealEstate, "Real Estate" }
    };

    public Dictionary<AreaObjectType, string> ObjectTypes { get; set; } = new()
    {
        { AreaObjectType.Parcel, "Parcel" },
        { AreaObjectType.RealEstate, "Real Estate" }
    };
    
    public List<char> CoordinateXDirection { get; set; } = ['E', 'W'];
    
    public List<char> CoordinateYDirection { get; set; } = ['N', 'S'];

    public MainWindowViewModel()
    {
        AreaObjects = new ObservableCollection<AreaObjectDTO>();
    }
}
