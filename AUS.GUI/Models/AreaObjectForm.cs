using AUS.DataStructures.GeoArea;

namespace AUS.GUI.Models;

public class AreaObjectForm
{
    public AreaObjectType Type { get; set; }
    
    public string Id { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string CoordinateAX { get; set; } = string.Empty;
    
    public string CoordinateAY { get; set; } = string.Empty;
    
    public string CoordinateBX { get; set; } = string.Empty;
    
    public string CoordinateBY { get; set; } = string.Empty;
    
    
}
