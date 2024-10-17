using System.Collections.Generic;
using System.Collections.ObjectModel;
using AUS.DataStructures.GeoArea;

namespace AUS.GUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private AreaObjectDTO? _selectedAreaObject;
    private ObservableCollection<AreaObjectDTO> _areaObjects;
    private ObservableCollection<AreaObjectDTO> _associatedObjects;
    
    public AreaObjectDTO? SelectedAreaObject
    {
        get => _selectedAreaObject;
        set
        {
            _selectedAreaObject = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsSelectedAreaObject));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(TitleAssociatedObjects));
            
            if (SelectedAreaObject != null)
            {
                AssociatedObjects = new ObservableCollection<AreaObjectDTO>(SelectedAreaObject.AssociatedObjects);
            }
        }
    }
    
    public bool IsSelectedAreaObject => SelectedAreaObject != null;

    public string Title => SelectedAreaObject == null ? string.Empty : "Úprava " + (SelectedAreaObject.Type == AreaObjectType.RealEstate ? "nehnuteľnosti" : "parcely");

    public string TitleAssociatedObjects => SelectedAreaObject == null ? string.Empty : (SelectedAreaObject.Type == AreaObjectType.RealEstate ? "Parcely" : "Nehnut.");
    
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
        { AreaObjectType.Unknown, "Ľubovoľný" },
        { AreaObjectType.Parcel, "Parcela" },
        { AreaObjectType.RealEstate, "Nehnuteľnosť" }
    };

    public Dictionary<AreaObjectType, string> ObjectTypes { get; set; } = new()
    {
        { AreaObjectType.Parcel, "Parcela" },
        { AreaObjectType.RealEstate, "Nehnuteľnosť" }
    };
    
    public List<char> CoordinateXDirection { get; set; } = new() { 'E', 'W' };
    
    public List<char> CoordinateYDirection { get; set; } = new() { 'N', 'S' };

    public MainWindowViewModel()
    {
        AreaObjects = new ObservableCollection<AreaObjectDTO>();
    }
    
    public void RefreshSelectedAreaObject()
    {
        // situacia ked mame selectnuty objekt a pridame opacny,
        // ktory s nim koliduje, aby sa hned zobrazil v zozname asociovanych objektov
        SelectedAreaObject = SelectedAreaObject;
    }
    
    public void RefreshAreaObjects()
    {
        var selected = SelectedAreaObject;
        AreaObjects = new ObservableCollection<AreaObjectDTO>(AreaObjects);
        SelectedAreaObject = selected;
    }
}
