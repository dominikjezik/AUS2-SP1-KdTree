using System.Text;
using AUS.DataStructures.KDTree;

namespace AUS.DataStructures.GeoArea;

public class GeoAreaService
{
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeRealEstates = new(2);
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeParcels = new(2);
    private readonly Random _random = new();
    
    #region FindOperations
    
    public List<AreaObjectDTO> Find(AreaObjectDTO areaObjectQuery)
    {
        bool searchByCoordinateAX = double.TryParse(areaObjectQuery.CoordinateAX, out var coordinateAX);
        bool searchByCoordinateAY = double.TryParse(areaObjectQuery.CoordinateAY, out var coordinateAY);
        
        bool searchByCoordinateBX = double.TryParse(areaObjectQuery.CoordinateBX, out var coordinateBX);
        bool searchByCoordinateBY = double.TryParse(areaObjectQuery.CoordinateBY, out var coordinateBY);
        
        var coordinateAXDirection = areaObjectQuery.CoordinateAXDirection;
        var coordinateAYDirection = areaObjectQuery.CoordinateAYDirection;
            
        var coordinateBXDirection = areaObjectQuery.CoordinateBXDirection;
        var coordinateBYDirection = areaObjectQuery.CoordinateBYDirection;
        
        if (searchByCoordinateAX && searchByCoordinateAY && searchByCoordinateBX && searchByCoordinateBY)
        {
            return Find(
                areaObjectQuery.Type,
                new GPSCoordinate(
                    coordinateAXDirection == 'W' ? -coordinateAX : coordinateAX,
                    coordinateAYDirection == 'S' ? -coordinateAY : coordinateAY
                ),
                new GPSCoordinate(
                    coordinateBXDirection == 'W' ? -coordinateBX : coordinateBX,
                    coordinateBYDirection == 'S' ? -coordinateBY : coordinateBY
                )
            );
        }
        
        if (searchByCoordinateAX && searchByCoordinateAY)
        {
            return Find(
                areaObjectQuery.Type,
                new GPSCoordinate(
                    coordinateAXDirection == 'W' ? -coordinateAX : coordinateAX,
                    coordinateAYDirection == 'S' ? -coordinateAY : coordinateAY
                )
            );
        }
        
        if (searchByCoordinateBX && searchByCoordinateBY)
        {
            return Find(
                areaObjectQuery.Type,
                new GPSCoordinate(
                    coordinateBXDirection == 'W' ? -coordinateBX : coordinateBX,
                    coordinateBYDirection == 'S' ? -coordinateBY : coordinateBY
                )
            );
        }
        
        return Get(areaObjectQuery.Type);
    }
    
    public List<AreaObjectDTO> Get(AreaObjectType areaObjectType)
    {
        List<AreaObject> result = new();
        
        if (areaObjectType == AreaObjectType.RealEstate)
        {
            _kdTreeRealEstates.ExecuteInOrder(result.AddRange);
        }
        else if (areaObjectType == AreaObjectType.Parcel)
        {
            _kdTreeParcels.ExecuteInOrder(result.AddRange);
        }
        else
        {
            _kdTreeRealEstates.ExecuteInOrder(result.AddRange);
            _kdTreeParcels.ExecuteInOrder(result.AddRange);
        }

        return result.Distinct().Select(a => a.ToDTO()).ToList();
    }

    public List<AreaObjectDTO> Find(AreaObjectType areaObjectType, GPSCoordinate coordinate)
    {
        switch (areaObjectType)
        {
            case AreaObjectType.RealEstate:
                return FindRealEstates(coordinate).ToList();
            case AreaObjectType.Parcel:
                return FindParcels(coordinate).ToList();
            default:
                return FindAreaObjects(coordinate).ToList();
        }
    }
    
    public List<AreaObjectDTO> Find(AreaObjectType areaObjectType, GPSCoordinate coordinateA, GPSCoordinate coordinateB)
    {
        switch (areaObjectType)
        {
            case AreaObjectType.RealEstate:
                return FindRealEstates(coordinateA, coordinateB).Distinct().ToList();
            case AreaObjectType.Parcel:
                return FindParcels(coordinateA, coordinateB).Distinct().ToList();
            default:
                return FindAreaObjects(coordinateA, coordinateB).Distinct().ToList();
        }
    }
    
    public List<AreaObjectDTO> FindRealEstates(GPSCoordinate coordinate)
    {
        return _kdTreeRealEstates.FindByKey(coordinate).Distinct().Select(a => a.ToDTO()).ToList();
    }

    public List<AreaObjectDTO> FindParcels(GPSCoordinate coordinate)
    {
        return _kdTreeParcels.FindByKey(coordinate).Distinct().Select(a => a.ToDTO()).ToList();
    }
    
    public List<AreaObjectDTO> FindAreaObjects(GPSCoordinate coordinate)
    {
        var result = _kdTreeRealEstates.FindByKey(coordinate);
        var tmp = _kdTreeParcels.FindByKey(coordinate);
        result.AddRange(tmp);
        return result.Distinct().Select(a => a.ToDTO()).ToList();
    }
    
    public List<AreaObjectDTO> FindRealEstates(GPSCoordinate coordinateA, GPSCoordinate coordinateB)
    {
        var result = _kdTreeRealEstates.FindByKey(coordinateA);
        var tmp = _kdTreeRealEstates.FindByKey(coordinateB);
        result.AddRange(tmp);
        return result.Distinct().Select(a => a.ToDTO()).ToList();
    }
    
    public List<AreaObjectDTO> FindParcels(GPSCoordinate coordinateA, GPSCoordinate coordinateB)
    {
        var result = _kdTreeParcels.FindByKey(coordinateA);
        var tmp = _kdTreeParcels.FindByKey(coordinateB);
        result.AddRange(tmp);
        return result.Distinct().Select(a => a.ToDTO()).ToList();
    }

    public List<AreaObjectDTO> FindAreaObjects(GPSCoordinate coordinateA, GPSCoordinate coordinateB)
    {
        var result = _kdTreeRealEstates.FindByKey(coordinateA);
        var tmp = _kdTreeRealEstates.FindByKey(coordinateB);
        result.AddRange(tmp);
        
        tmp = _kdTreeParcels.FindByKey(coordinateA);
        result.AddRange(tmp);

        tmp = _kdTreeParcels.FindByKey(coordinateB);
        result.AddRange(tmp);

        return result.Distinct().Select(a => a.ToDTO()).ToList();
    }
    
    #endregion

    #region ModifyOperations
    
    public AreaObjectDTO Insert(AreaObjectDTO areaObjectDTO)
    {
        var areaObjectToInsert = areaObjectDTO.ToAreaObject();
        
        if (areaObjectToInsert.Type == AreaObjectType.RealEstate)
        {
            var parcelsA = _kdTreeParcels.FindByKey(areaObjectToInsert.CoordinateA);
            var parcelsB = _kdTreeParcels.FindByKey(areaObjectToInsert.CoordinateB);
            areaObjectToInsert.AssociatedObjects = parcelsA;
            areaObjectToInsert.AssociatedObjects.AddRange(parcelsB);

            foreach (var parcel in areaObjectToInsert.AssociatedObjects)
            {
                parcel.AssociatedObjects.Add(areaObjectToInsert);
            }
            
            _kdTreeRealEstates.Insert(areaObjectToInsert.CoordinateA, areaObjectToInsert);
            _kdTreeRealEstates.Insert(areaObjectToInsert.CoordinateB, areaObjectToInsert);
        }
        else
        {
            var realEstatesA = _kdTreeRealEstates.FindByKey(areaObjectToInsert.CoordinateA);
            var realEstatesB = _kdTreeRealEstates.FindByKey(areaObjectToInsert.CoordinateB);
            areaObjectToInsert.AssociatedObjects = realEstatesA;
            areaObjectToInsert.AssociatedObjects.AddRange(realEstatesB);
            
            foreach (var realEstate in areaObjectToInsert.AssociatedObjects)
            {
                realEstate.AssociatedObjects.Add(areaObjectToInsert);
            }
            
            _kdTreeParcels.Insert(areaObjectToInsert.CoordinateA, areaObjectToInsert);
            _kdTreeParcels.Insert(areaObjectToInsert.CoordinateB, areaObjectToInsert);
        }
        
        return areaObjectToInsert.ToDTO();
    }
    
    public void Delete(AreaObjectDTO areaObject)
    {
        List<AreaObject> associatedObjects = new();
        
        var coordinateA = areaObject.OriginalCoordinateA;
        var coordinateB = areaObject.OriginalCoordinateB;

        if (areaObject.Type == AreaObjectType.Parcel)
        {
            var deletedAreaObject = _kdTreeParcels.Delete(coordinateA, areaObject.IsEqualTo);
            _kdTreeParcels.Delete(coordinateB, areaObject.IsEqualTo);

            associatedObjects = deletedAreaObject.AssociatedObjects;
        }
        else
        {
            var deletedAreaObject = _kdTreeRealEstates.Delete(coordinateA, areaObject.IsEqualTo);
            _kdTreeRealEstates.Delete(coordinateB, areaObject.IsEqualTo);

            associatedObjects = deletedAreaObject.AssociatedObjects;
        }

        // Mazanie inverznych referencii
        foreach (var associatedObject in associatedObjects)
        {
            var associatedObjectOfAnother = associatedObject.AssociatedObjects;

            var associatedObjectToDelete = associatedObjectOfAnother.Find(areaObject.IsEqualTo);
            
            if (associatedObjectToDelete != null)
            {
                associatedObjectOfAnother.Remove(associatedObjectToDelete);
            }
        }
    }
    
    public AreaObjectDTO Update(AreaObjectDTO areaObject)
    {
        if (!areaObject.AreCoordinatesChanged())
        {
            areaObject.UpdateDetailsOriginalAreaObject();
            return areaObject;
        }
        
        Delete(areaObject);
        return Insert(areaObject);
    }
    
    #endregion

    #region GenerateOperations
    
    public void GenerateOperations(int count, double probabilityOfOverlay, int minX, int maxX, int minY, int maxY, int numberOfDecimalPlaces, bool generateRandomDescription)
    {
        List<AreaObjectDTO> realEstates = new();
        List<AreaObjectDTO> parcels = new();
        
        for (var i = 0; i < count; i++)
        {
            var objectType = _random.NextDouble() < 0.5 ? AreaObjectType.RealEstate : AreaObjectType.Parcel;
            
            var coordinateAX = _random.Next(-minX, maxX) + _random.NextDouble();
            coordinateAX = Math.Round(coordinateAX, numberOfDecimalPlaces);
            var coordinateAXDirection = coordinateAX < 0 ? 'W' : 'E';
            
            var coordinateAY = _random.Next(-minY, maxY) + _random.NextDouble();
            coordinateAY = Math.Round(coordinateAY, numberOfDecimalPlaces);
            var coordinateAYDirection = coordinateAY < 0 ? 'S' : 'N';

            double coordinateBX = 0;
            char coordinateBXDirection = ' ';
            double coordinateBY = 0;
            char coordinateBYDirection = ' ';
            
            if (_random.NextDouble() < probabilityOfOverlay && ((objectType == AreaObjectType.RealEstate && parcels.Count > 0) || (objectType == AreaObjectType.Parcel && realEstates.Count > 0)))
            {
                var randomIndex = _random.Next(0, objectType == AreaObjectType.RealEstate ? parcels.Count : realEstates.Count);
                var randomObject = objectType == AreaObjectType.RealEstate ? parcels[randomIndex] : realEstates[randomIndex];
                
                coordinateBX = double.Parse(randomObject.CoordinateAX);
                coordinateBXDirection = randomObject.CoordinateAXDirection;
                
                coordinateBY = double.Parse(randomObject.CoordinateAY);
                coordinateBYDirection = randomObject.CoordinateAYDirection;
            }
            else
            {
                coordinateBX = _random.Next(-minX, maxX) + _random.NextDouble();
                coordinateBX = Math.Round(coordinateBX, numberOfDecimalPlaces);
                coordinateBXDirection = coordinateBX < 0 ? 'W' : 'E';
            
                coordinateBY = _random.Next(-minY, maxY) + _random.NextDouble();
                coordinateBY = Math.Round(coordinateBY, numberOfDecimalPlaces);
                coordinateBYDirection = coordinateBY < 0 ? 'S' : 'N';
            }
            
            var areaObject = new AreaObjectDTO
            {
                Type = objectType,
                Description = generateRandomDescription ? GenerateDescription() : string.Empty,
                Id = _random.Next(1, 99999).ToString(),
                CoordinateAX = Math.Abs(coordinateAX).ToString(),
                CoordinateAXDirection = coordinateAXDirection,
                CoordinateAY = Math.Abs(coordinateAY).ToString(),
                CoordinateAYDirection = coordinateAYDirection,
                CoordinateBX = Math.Abs(coordinateBX).ToString(),
                CoordinateBXDirection = coordinateBXDirection,
                CoordinateBY = Math.Abs(coordinateBY).ToString(),
                CoordinateBYDirection = coordinateBYDirection
            };
            
            Insert(areaObject);
            
            if (objectType == AreaObjectType.RealEstate)
            {
                realEstates.Add(areaObject);
            }
            else
            {
                parcels.Add(areaObject);
            }
        }
    }

    private string GenerateDescription()
    {
        // https://www.lipsum.com/
        string[] words =
        [
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua"
        ];
        
        var countOfWords = _random.Next(4, 12);
        var description = new StringBuilder();
        
        for (var i = 0; i < countOfWords; i++)
        {
            description.Append(words[_random.Next(words.Length)]);
            description.Append(" ");
        }
        
        description.Replace(' ', '.', description.Length - 1, 1);
        description[0] = char.ToUpper(description[0]);
        
        return description.ToString();
    }
    
    #endregion
    
    #region SaveOperations

    public void SaveToFolder(Uri pathToFolder)
    {
        // TODO: Potom nezabudnut aby boli spravne nastavene asociovane objekty
        
        var realEstatesPath = Path.Combine(pathToFolder.LocalPath, "realEstates.csv");
        
        SaveRealEstatesToFile(realEstatesPath);
    }

    public void SaveRealEstatesToFile(string path)
    {
        using var writer = new StreamWriter(path);
        
        _kdTreeRealEstates.ExecuteLevelOrder((key, areaObjects) =>
        {
            writer.Write($"{key.X};{key.Y}");
            
            foreach (var areaObject in areaObjects)
            {
                if (areaObject.CoordinateA == key)
                {
                    // zapis bez duplicitnej suradnice cize kluca
                    writer.Write($";{areaObject.ToReducedCSV()}");
                }
            }
            
            writer.WriteLine();
        });
    }
    
    public void SaveParcelsToFile(string path)
    {
    }

    #endregion
    
    #region LoadOperations
    
    public void LoadFromFolder(Uri pathToFolder)
    {
        _kdTreeRealEstates.Clear();
        _kdTreeParcels.Clear();
        
        var realEstatesPath = Path.Combine(pathToFolder.LocalPath, "realEstates.csv");
        
        LoadRealEstatesFromFile(realEstatesPath);
    }
    
    public void LoadRealEstatesFromFile(string path)
    {
        using var reader = new StreamReader(path);
        
        var areaObjects = new List<AreaObject>();
        
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            
            // TODO: naimplementovat reescapeovanie riadku zatedy ignorujeme
            
            var segments = line.Split(';');
            
            if (segments.Length < 2)
            {
                throw new ArgumentException("Invalid file format");
            }
            
            var coordinateA = new GPSCoordinate(double.Parse(segments[0]), double.Parse(segments[1]));
            
            // vzdy 2 + (4 * n) segmentov, kde n je pocet objektov
            if ((segments.Length - 2) % 4 != 0)
            {
                throw new ArgumentException("Invalid file format");
            }
            
            if (segments.Length == 2)
            {
                _kdTreeRealEstates.Insert(coordinateA);
                continue;
            }
            
            for (var i = 2; i < segments.Length; i += 4)
            {
                var areaObject = new AreaObject
                {
                    Type = AreaObjectType.RealEstate,
                    CoordinateA = coordinateA,
                    CoordinateB = new GPSCoordinate(double.Parse(segments[i]), double.Parse(segments[i + 1])),
                    Id = int.Parse(segments[i + 2]),
                    Description = segments[i + 3]
                };
                
                _kdTreeRealEstates.Insert(coordinateA, areaObject);
                
                areaObjects.Add(areaObject);
            }
        }
        
        // adding real estates by secondary coordinate
        foreach (var areaObject in areaObjects)
        {
            _kdTreeRealEstates.Insert(areaObject.CoordinateB, areaObject);
        }
    }
    
    #endregion
}
