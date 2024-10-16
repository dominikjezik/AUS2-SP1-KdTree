namespace AUS.GUI.ViewModels;

public class GenerateOperationsViewModel : ViewModelBase
{
    private int _countOfOperations = 1000;
    private double _probabilityOfOverlay = 0.5;
    
    public int CountOfOperations
    {
        get => _countOfOperations;
        set
        {
            _countOfOperations = value;
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
