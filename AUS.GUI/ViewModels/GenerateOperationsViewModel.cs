namespace AUS.GUI.ViewModels;

public class GenerateOperationsViewModel : ViewModelBase
{
    private int _countOfOperations = 1000;
    private double _probabilityOfInsert = 0.5;
    
    public int CountOfOperations
    {
        get => _countOfOperations;
        set
        {
            _countOfOperations = value;
            OnPropertyChanged();
        }
    }
    
    public double ProbabilityOfInsert
    {
        get => _probabilityOfInsert;
        set
        {
            _probabilityOfInsert = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ProbabilityOfDelete));
        }
    }

    public double ProbabilityOfDelete
    {
        get => 1 - ProbabilityOfInsert;
        set
        {
            ProbabilityOfInsert = 1 - value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ProbabilityOfInsert));
        }
    }

    public int MinX { get; set; } = -100;
    
    public int MaxX { get; set; } = 100;

    public int MinY { get; set; } = -100;
    
    public int MaxY { get; set; } = 100;

    public int NumberOfDecimalPlaces { get; set; } = 2;
    
    public bool GenerateRandomDescription { get; set; } = true;
}
