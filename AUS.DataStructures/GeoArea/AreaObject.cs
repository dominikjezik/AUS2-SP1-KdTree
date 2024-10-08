namespace AUS.DataStructures.GeoArea;

public class AreaObject
{
    public AreaObjectType Type { get; set; }

    public string DisplayType => Type == AreaObjectType.RealEstate ? "Nehnuteľnosť" : "Parcela";

    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public GPSCoordinate CoordinateA { get; set; } = new(0, 0);

    public GPSCoordinate CoordinateB { get; set; } = new(0, 0);

    public List<AreaObject> AssociatedObjects = new();
    
    public override string ToString()
    {
        return $"Id: {Id}, Type: {Type}, Description: {Description}, CoordinateA: {CoordinateA}, CoordinateB: {CoordinateB}";
    }
}
