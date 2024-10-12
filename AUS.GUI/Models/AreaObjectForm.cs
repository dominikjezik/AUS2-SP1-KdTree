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
    
    public AreaObject ToAreaObject()
    {
        if (!int.TryParse(Id, out var id))
        {
            id = 0;
        }
        
        if (!double.TryParse(CoordinateAX, out var coordinateAX))
        {
            coordinateAX = 0;
        }
        
        if (!double.TryParse(CoordinateAY, out var coordinateAY))
        {
            coordinateAY = 0;
        }
        
        if (!double.TryParse(CoordinateBX, out var coordinateBX))
        {
            coordinateBX = 0;
        }
        
        if (!double.TryParse(CoordinateBY, out var coordinateBY))
        {
            coordinateBY = 0;
        }
        
        return new AreaObject
        {
            Type = Type,
            Description = Description,
            Id = id,
            CoordinateA = new GPSCoordinate(coordinateAX, coordinateAY),
            CoordinateB = new GPSCoordinate(coordinateBX, coordinateBY)
        };
    }
}

public static class AreaObjectFormExtensions
{
    public static AreaObjectForm ToAreaObjectForm(this AreaObject areaObject)
    {
        return new AreaObjectForm
        {
            Type = areaObject.Type,
            Description = areaObject.Description,
            Id = areaObject.Id.ToString(),
            CoordinateAX = areaObject.CoordinateA.X.ToString(),
            CoordinateAY = areaObject.CoordinateA.Y.ToString(),
            CoordinateBX = areaObject.CoordinateB.X.ToString(),
            CoordinateBY = areaObject.CoordinateB.Y.ToString()
        };
    }
}
