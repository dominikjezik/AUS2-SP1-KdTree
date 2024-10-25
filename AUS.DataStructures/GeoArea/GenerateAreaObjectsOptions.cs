namespace AUS.DataStructures.GeoArea;

public class GenerateAreaObjectsOptions
{
    public int CountOfRealEstates { get; set; }
    
    public int CountOfParcels { get; set; }
    
    public double ProbabilityOfOverlay { get; set; }
    
    public int MinX { get; set; } = -100;
    
    public int MaxX { get; set; } = 100;
    
    public int MinY { get; set; } = -100;
    
    public int MaxY { get; set; } = 100;
    
    public int NumberOfDecimalPlaces { get; set; } = 2;
    
    public bool GenerateRandomDescription { get; set; } = true;
}
