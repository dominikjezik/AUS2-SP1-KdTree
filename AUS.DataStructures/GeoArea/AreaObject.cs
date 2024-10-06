namespace AUS.DataStructures.GeoArea;

public class AreaObject
{
    public AreaObjectType Type { get; set; }

    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public GPSCoordinate CoordinateA { get; set; } = new(0, 0);

    public GPSCoordinate CoordinateB { get; set; } = new(0, 0);

    // TODO: Zoznam prekryvajucich sa objektov

    public bool ContainsCoordinate(double x, double y)
    {
        // porovnat s A a B
        throw new NotImplementedException();
    }
}
