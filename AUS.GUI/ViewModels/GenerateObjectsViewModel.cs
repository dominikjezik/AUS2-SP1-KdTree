namespace AUS.GUI.ViewModels;

public class GenerateObjectsViewModel : ViewModelBase
{
    private int _countOfParcels = 1000;
    private int _countOfRealEstates = 1000;
    private double _probabilityOfOverlay = 0.5;
    
    public int CountOfParcels
    {
        get => _countOfParcels;
        set
        {
            _countOfParcels = value;
            OnPropertyChanged();
        }
    }
    
    public int CountOfRealEstates
    {
        get => _countOfRealEstates;
        set
        {
            _countOfRealEstates = value;
            OnPropertyChanged();
        }
    }
    
    public double ProbabilityOfOverlay
    {
        get => _probabilityOfOverlay;
        set
        {
            _probabilityOfOverlay = value;
            OnPropertyChanged();
        }
    }

    public int MinX { get; set; } = 100;
    
    public int MaxX { get; set; } = 100;

    public int MinY { get; set; } = 100;
    
    public int MaxY { get; set; } = 100;

    public int NumberOfDecimalPlaces { get; set; } = 2;
    
    public bool GenerateRandomDescription { get; set; } = true;
}
