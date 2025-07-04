﻿namespace AUS.DataStructures.GeoArea;

public class AreaObjectDTO
{
    public Guid? InternalId { get; init; }
    
    #region Form Properties
    
    public AreaObjectType Type { get; set; }
    
    public string Id { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    // E (East) +, W (West) - => X
    // N (North) +, S (South) - => Y
    
    public string CoordinateAX { get; set; } = string.Empty;
    public char CoordinateAXDirection { get; set; } = 'E';
    
    public string CoordinateAY { get; set; } = string.Empty;
    public char CoordinateAYDirection { get; set; } = 'N';
    
    public string CoordinateBX { get; set; } = string.Empty;
    public char CoordinateBXDirection { get; set; } = 'E';
    
    public string CoordinateBY { get; set; } = string.Empty;
    public char CoordinateBYDirection { get; set; } = 'N';
    
    #endregion
    
    public List<AreaObjectDTO> AssociatedObjects { get; set; } = new();
    
    public string DisplayType => Type == AreaObjectType.RealEstate ? "Real Estate" : "Parcel";
    
    public string DisplayCoordinateA => $"{CoordinateAX}{CoordinateAXDirection} {CoordinateAY}{CoordinateAYDirection}";
    
    public string DisplayCoordinateB => $"{CoordinateBX}{CoordinateBXDirection} {CoordinateBY}{CoordinateBYDirection}";
    
    public GPSCoordinate CoordinateA
    {
        get
        {
            double coordinateAX;
            double coordinateAY;
            
            if (!double.TryParse(CoordinateAX, out coordinateAX))
            {
                coordinateAX = 0;
            }
            
            if (!double.TryParse(CoordinateAY, out coordinateAY))
            {
                coordinateAY = 0;
            }
            
            return new(
                CoordinateAXDirection == 'W' ? -coordinateAX : coordinateAX,
                CoordinateAYDirection == 'S' ? -coordinateAY : coordinateAY
            );
        }
    }

    public GPSCoordinate CoordinateB
    {
        get
        {
            double coordinateBX;
            double coordinateBY;
            
            if (!double.TryParse(CoordinateBX, out coordinateBX))
            {
                coordinateBX = 0;
            }
            
            if (!double.TryParse(CoordinateBY, out coordinateBY))
            {
                coordinateBY = 0;
            }
            
            return new(
                CoordinateBXDirection == 'W' ? -coordinateBX : coordinateBX,
                CoordinateBYDirection == 'S' ? -coordinateBY : coordinateBY
            );
        }
    }
    
    public AreaObjectDTO()
    {
    }

    public AreaObjectDTO(AreaObjectDTO another)
    {
        InternalId = another.InternalId;
        Type = another.Type;
        Id = another.Id;
        Description = another.Description;
        CoordinateAX = another.CoordinateAX;
        CoordinateAXDirection = another.CoordinateAXDirection;
        CoordinateAY = another.CoordinateAY;
        CoordinateAYDirection = another.CoordinateAYDirection;
        CoordinateBX = another.CoordinateBX;
        CoordinateBXDirection = another.CoordinateBXDirection;
        CoordinateBY = another.CoordinateBY;
        CoordinateBYDirection = another.CoordinateBYDirection;
        AssociatedObjects = new(another.AssociatedObjects);
    }

    public bool AreCoordinatesChanged(AreaObjectDTO another)
    {
        return CoordinateA != another.CoordinateA || CoordinateB != another.CoordinateB;
    }
    
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
        
        // E (East) +, W (West) - => X
        // N (North) +, S (South) - => Y
        
        return new AreaObject
        {
            Type = Type,
            Description = Description,
            Id = id,
            CoordinateA = new GPSCoordinate(
                CoordinateAXDirection == 'W' ? -coordinateAX : coordinateAX,
                CoordinateAYDirection == 'S' ? -coordinateAY : coordinateAY
            ),
            CoordinateB = new GPSCoordinate(
                CoordinateBXDirection == 'W' ? -coordinateBX : coordinateBX,
                CoordinateBYDirection == 'S' ? -coordinateBY : coordinateBY
            )
        };
    }
}
