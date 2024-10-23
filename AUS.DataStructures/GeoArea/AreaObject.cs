namespace AUS.DataStructures.GeoArea;

public class AreaObject
{
    public Guid InternalId { get; private set; }
    
    public AreaObjectType Type { get; set; }
    
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public GPSCoordinate CoordinateA { get; set; } = new(0, 0);

    public GPSCoordinate CoordinateB { get; set; } = new(0, 0);

    public List<AreaObject> AssociatedObjects { get; private set; } = new();

    public AreaObject(Guid? internalId = null)
    {
        InternalId = internalId ?? Guid.NewGuid();
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, Type: {Type}, Description: {Description}, CoordinateA: {CoordinateA}, CoordinateB: {CoordinateB}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }
        
        AreaObject areaObject = (AreaObject)obj;
        return this.InternalId == areaObject.InternalId;
    }
    
    public AreaObjectDTO ToDTO(bool includeAssociatedObjects = true)
    {
        List<AreaObjectDTO> associatedObjects = new();
        
        if (includeAssociatedObjects)
        {
            associatedObjects = AssociatedObjects.Distinct().Select(areaObject => areaObject.ToDTO(false)).ToList();
        }
        
        return new AreaObjectDTO
        {
            InternalId = InternalId,
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
    
    public string ToReducedCSV()
    {
        // TODO: Otestovat
        
        // Escapeovaci mechanizmus na nahradenie pripadnych ; a \ v popise objektu
        var escapedDescription = Description.Replace(@"\", @"\\").Replace(";", @"\;");
        return $"{CoordinateB.X};{CoordinateB.Y};{Id};{escapedDescription}";
    }
}

public static class AreaObjectExtensions
{
    public static AreaObject ToAreaObject(this AreaObjectDTO areaObjectDTO)
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
        
        return new AreaObject
        {
            Type = areaObjectDTO.Type,
            Description = areaObjectDTO.Description,
            Id = id,
            CoordinateA = new GPSCoordinate(
                areaObjectDTO.CoordinateAXDirection == 'W' ? -coordinateAX : coordinateAX,
                areaObjectDTO.CoordinateAYDirection == 'S' ? -coordinateAY : coordinateAY
            ),
            CoordinateB = new GPSCoordinate(
                areaObjectDTO.CoordinateBXDirection == 'W' ? -coordinateBX : coordinateBX,
                areaObjectDTO.CoordinateBYDirection == 'S' ? -coordinateBY : coordinateBY
            )
        };
    }
}
