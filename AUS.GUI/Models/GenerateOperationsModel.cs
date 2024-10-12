namespace AUS.GUI.Models;

public class GenerateOperationsModel
{
    public int Count { get; set; }

    public double ProbabilityOfInsert { get; set; }
    
    public int MinX { get; set; } = -100;
    
    public int MaxX { get; set; } = 100;
    
    public int MinY { get; set; } = -100;
    
    public int MaxY { get; set; } = 100;
    
    public int NumberOfDecimalPlaces { get; set; } = 2;
    
    public bool GenerateRandomDescription { get; set; } = true;
}
