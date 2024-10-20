namespace AUS.DataStructures.GeoArea;

public class AreaObjectDTO
{
    private AreaObject? _originalAreaObject;
    
    public AreaObjectDTO(AreaObject areaObject)
    {
        _originalAreaObject = areaObject;
    }
    
    public AreaObjectDTO()
    {
    }
    
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
    
    public List<AreaObjectDTO> AssociatedObjects = new();
    
    public string DisplayType => Type == AreaObjectType.RealEstate ? "Nehnuteľnosť" : "Parcela";
    
    public string DisplayCoordinateA => $"{CoordinateAX}{CoordinateAXDirection} {CoordinateAY}{CoordinateAYDirection}";
    
    public string DisplayCoordinateB => $"{CoordinateBX}{CoordinateBXDirection} {CoordinateBY}{CoordinateBYDirection}";
    
    public bool IsEqualTo(AreaObject areaObject)
    {
        return _originalAreaObject != null && _originalAreaObject.Equals(areaObject);
    }

    public GPSCoordinate OriginalCoordinateA => new(
        _originalAreaObject?.CoordinateA.X ?? 0,
        _originalAreaObject?.CoordinateA.Y ?? 0
    );
    
    public GPSCoordinate OriginalCoordinateB => new(
        _originalAreaObject?.CoordinateB.X ?? 0,
        _originalAreaObject?.CoordinateB.Y ?? 0
    );
    
    public bool AreCoordinatesChanged()
    {
        if (!double.TryParse(CoordinateAX, out double coordinateAX))
        {
            coordinateAX = 0;
        }
        
        if (!double.TryParse(CoordinateAY, out double coordinateAY))
        {
            coordinateAY = 0;
        }
        
        if (!double.TryParse(CoordinateBX, out double coordinateBX))
        {
            coordinateBX = 0;
        }
        
        if (!double.TryParse(CoordinateBY, out double coordinateBY))
        {
            coordinateBY = 0;
        }
        
        var coordinateA = new GPSCoordinate(
            CoordinateAXDirection == 'W' ? -coordinateAX : coordinateAX,
            CoordinateAYDirection == 'S' ? -coordinateAY : coordinateAY
        );
        
        var coordinateB = new GPSCoordinate(
            CoordinateBXDirection == 'W' ? -coordinateBX : coordinateBX,
            CoordinateBYDirection == 'S' ? -coordinateBY : coordinateBY
        );
        
        return _originalAreaObject?.CoordinateA != coordinateA || _originalAreaObject?.CoordinateB != coordinateB;
    }

    public void UpdateDetailsOriginalAreaObject()
    {
        if (_originalAreaObject == null)
        {
            return;
        }

        int.TryParse(Id, out var newId);

        _originalAreaObject.Id = newId;
        _originalAreaObject.Description = Description;
    }

    public void LoadAssociatedObjects()
    {
        AssociatedObjects = _originalAreaObject?.AssociatedObjects.Distinct().Select(areaObject => areaObject.ToDTO(false)).ToList() ?? new();
    }
}
