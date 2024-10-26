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
        // Odstranenie separatora CSV z popisu objekt
        var reducedDescription = Description.Replace(";", string.Empty);
        return $"{CoordinateB.X};{CoordinateB.Y};{Id};{reducedDescription}";
    }

    public static List<AreaObject> FromReducedCSV(string[] segments, AreaObjectType type)
    {
        var coordinateA = new GPSCoordinate(double.Parse(segments[0]), double.Parse(segments[1]));

        var areaObjects = new List<AreaObject>();
        
        for (var i = 2; i < segments.Length; i += 4)
        {
            var areaObject = new AreaObject
            {
                Type = type,
                CoordinateA = coordinateA,
                CoordinateB = new GPSCoordinate(double.Parse(segments[i]), double.Parse(segments[i + 1])),
                Id = int.Parse(segments[i + 2]),
                Description = segments[i + 3]
            };
            
            areaObjects.Add(areaObject);
        }
        
        return areaObjects;
    }
}
