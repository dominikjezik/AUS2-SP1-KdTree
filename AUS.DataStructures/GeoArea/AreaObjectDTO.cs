namespace AUS.DataStructures.GeoArea;

public class AreaObjectDTO
{
    // TODO: Mozem pouzit referenciu na AreaObject (je private)
    private AreaObject? _originalAreaObject;
    
    public AreaObjectDTO(AreaObject areaObject)
    {
        _originalAreaObject = areaObject;
    }
    
    public AreaObjectDTO()
    {
    }
    
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
    
    public List<AreaObjectDTO> AssociatedObjects = new();
    
    public string DisplayType => Type == AreaObjectType.RealEstate ? "Nehnuteľnosť" : "Parcela";
    
    public string DisplayCoordinateA => $"{CoordinateAX}{CoordinateAXDirection} {CoordinateAY}{CoordinateAYDirection}";
    
    public string DisplayCoordinateB => $"{CoordinateBX}{CoordinateBXDirection} {CoordinateBY}{CoordinateBYDirection}";
    
    // TODO: Mozme pouzit pri delete
    public bool IsEqualTo(AreaObject areaObject)
    {
        return _originalAreaObject != null && _originalAreaObject.Equals(areaObject);
    }

    public void LoadAssociatedObjects()
    {
        AssociatedObjects = _originalAreaObject?.AssociatedObjects.Distinct().Select(areaObject => areaObject.ToDTO(false)).ToList() ?? new();
    }
}
