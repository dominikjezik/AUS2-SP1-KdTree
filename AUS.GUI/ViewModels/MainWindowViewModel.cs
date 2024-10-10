using System.Collections.Generic;
using System.Collections.ObjectModel;
using AUS.DataStructures.GeoArea;
using AUS.GUI.Models;

namespace AUS.GUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private AreaObjectForm _selectedAreaObjectForm = new();
    public AreaObject? SelectedAreaObject { get; set; }

    public AreaObjectForm SelectedAreaObjectForm
    {
        get => _selectedAreaObjectForm;
        set {
            _selectedAreaObjectForm = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<AreaObject> AreaObjects { get; }

    public MainWindowViewModel()
    {
        var areaObjects = new List<AreaObject> 
        {
            new AreaObject()
            {
                Type = AreaObjectType.Parcel,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam nec purus nec nunc.",
                Id = 1,
                CoordinateA = new(1, 2),
                CoordinateB = new(3, 4)
            },
            new AreaObject()
            {
                Type = AreaObjectType.RealEstate,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam nec purus nec nunc.",
                Id = 2,
                CoordinateA = new(5, 6),
                CoordinateB = new(7, 8)
            },
            new AreaObject()
            {
                Type = AreaObjectType.RealEstate,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam nec purus nec nunc.",
                Id = 3,
                CoordinateA = new(9, 10),
                CoordinateB = new(11, 12)
            },
            new AreaObject()
            {
                Type = AreaObjectType.Parcel,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam nec purus nec nunc.",
                Id = 4,
                CoordinateA = new(13, 14),
                CoordinateB = new(15, 16)
            },
            new AreaObject()
            {
                Type = AreaObjectType.Parcel,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam nec purus nec nunc.",
                Id = 5,
                CoordinateA = new(17, 18),
                CoordinateB = new(19, 20)
            },
            new AreaObject()
            {
                Type = AreaObjectType.Parcel,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam nec purus nec nunc.",
                Id = 6,
                CoordinateA = new(21, 22),
                CoordinateB = new(23, 24)
            },
            new AreaObject()
            {
                Type = AreaObjectType.RealEstate,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam nec purus nec nunc.",
                Id = 7,
                CoordinateA = new(25, 26),
                CoordinateB = new(27, 28)
            }
        };
        AreaObjects = new ObservableCollection<AreaObject>(areaObjects);
    }
}
