using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AUS.DataStructures.GeoArea;
using AUS.GUI.Models;

namespace AUS.GUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private AreaObject? _selectedAreaObject;
    private AreaObjectForm? _selectedAreaObjectForm;
    private ObservableCollection<AreaObject> _areaObjects;
    private ObservableCollection<AreaObject> _associatedObjects;
    
    public AreaObject? SelectedAreaObject
    {
        get => _selectedAreaObject;
        set
        {
            _selectedAreaObject = value;
            SelectedAreaObjectForm = value?.ToAreaObjectForm();
            OnPropertyChanged();
        }
    }

    public AreaObjectForm? SelectedAreaObjectForm
    {
        get => _selectedAreaObjectForm;
        set {
            _selectedAreaObjectForm = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsSelectedAreaObject));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(TitleAssociatedObjects));

            // TODO: Otazka na konzultaciu, bez Distinct() zobrazuje Duplicity (lebo 2 body) a blbne zobrazenie zoznamu
            if (SelectedAreaObject != null)
            {
                AssociatedObjects = new ObservableCollection<AreaObject>(SelectedAreaObject.AssociatedObjects.Distinct());
            }
        }
    }
    
    public bool IsSelectedAreaObject => SelectedAreaObject != null;

    public string Title => SelectedAreaObject == null ? string.Empty : "Úprava " + (SelectedAreaObject.Type == AreaObjectType.RealEstate ? "nehnuteľnosti" : "parcely");

    public string TitleAssociatedObjects => SelectedAreaObject == null ? string.Empty : (SelectedAreaObject.Type == AreaObjectType.RealEstate ? "Parcely" : "Nehnut.");
    
    public AreaObjectForm AreaObjectQuery { get; set; } = new() { Type = AreaObjectType.Unknown };

    public ObservableCollection<AreaObject> AreaObjects
    {
        get => _areaObjects;
        set {
            _areaObjects = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<AreaObject> AssociatedObjects
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

    public MainWindowViewModel()
    {
        AreaObjects = new ObservableCollection<AreaObject>();
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
        AreaObjects = new ObservableCollection<AreaObject>(AreaObjects);
        SelectedAreaObject = selected;
    }
}
