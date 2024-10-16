using System.Globalization;

namespace AUS.DataStructures.GeoArea;

public class AreaObject
{
    public AreaObjectType Type { get; set; }
    
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public GPSCoordinate CoordinateA { get; set; } = new(0, 0);

    public GPSCoordinate CoordinateB { get; set; } = new(0, 0);

    public List<AreaObject> AssociatedObjects = new();
    
    public override string ToString()
    {
        return $"Id: {Id}, Type: {Type}, Description: {Description}, CoordinateA: {CoordinateA}, CoordinateB: {CoordinateB}";
    }
    
    public AreaObjectDTO ToDTO(bool includeAssociatedObjects = true)
    {
        List<AreaObjectDTO> associatedObjects = new();
        
        if (includeAssociatedObjects)
        {
            associatedObjects = AssociatedObjects.Distinct().Select(areaObject => areaObject.ToDTO(false)).ToList();
        }
        
        return new AreaObjectDTO(this)
        {
            Type = Type,
            Id = Id.ToString(),
            Description = Description,
            CoordinateAX = Math.Abs(CoordinateA.X).ToString(),
            CoordinateAXDirection = CoordinateA.X < 0 ? 'W' : 'E',
            CoordinateAY = Math.Abs(CoordinateA.Y).ToString(),
            CoordinateAYDirection = CoordinateA.Y < 0 ? 'S' : 'N',
            CoordinateBX = Math.Abs(CoordinateB.X).ToString(),
            CoordinateBXDirection = CoordinateB.X < 0 ? 'W' : 'E',
            CoordinateBY = Math.Abs(CoordinateB.Y).ToString(),
            CoordinateBYDirection = CoordinateB.Y < 0 ? 'S' : 'N',
            AssociatedObjects = associatedObjects
        };
    }
}

public static class AreaObjectExtensions
{
    public static AreaObject ToAreaObject(this AreaObjectDTO areaObjectDTO, AreaObject? targetAreaObject = null)
    {
        if (!int.TryParse(areaObjectDTO.Id, out var id))
        {
            id = 0;
        }
        
        if (!double.TryParse(areaObjectDTO.CoordinateAX, out var coordinateAX))
        {
            coordinateAX = 0;
        }
        
        if (!double.TryParse(areaObjectDTO.CoordinateAY, out var coordinateAY))
        {
            coordinateAY = 0;
        }
        
        if (!double.TryParse(areaObjectDTO.CoordinateBX, out var coordinateBX))
        {
            coordinateBX = 0;
        }
        
        if (!double.TryParse(areaObjectDTO.CoordinateBY, out var coordinateBY))
        {
            coordinateBY = 0;
        }
        
        // E (East) +, W (West) - => X
        // N (North) +, S (South) - => Y
        
        if (areaObjectDTO.CoordinateAXDirection == 'W')
        {
            coordinateAX *= -1;
        }
        
        if (areaObjectDTO.CoordinateAYDirection == 'S')
        {
            coordinateAY *= -1;
        }
        
        if (areaObjectDTO.CoordinateBXDirection == 'W')
        {
            coordinateBX *= -1;
        }
        
        if (areaObjectDTO.CoordinateBYDirection == 'S')
        {
            coordinateBY *= -1;
        }

        if (targetAreaObject == null)
        {
            targetAreaObject = new AreaObject();
        }
        
        targetAreaObject.Type = areaObjectDTO.Type;
        targetAreaObject.Description = areaObjectDTO.Description;
        targetAreaObject.Id = id;
        targetAreaObject.CoordinateA = new GPSCoordinate(coordinateAX, coordinateAY);
        targetAreaObject.CoordinateB = new GPSCoordinate(coordinateBX, coordinateBY);
        
        return targetAreaObject;
    }
}
