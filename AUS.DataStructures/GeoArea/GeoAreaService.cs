using System.Text;
using AUS.DataStructures.KDTree;

namespace AUS.DataStructures.GeoArea;

public class GeoAreaService
{
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeRealEstates = new(2);
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeParcels = new(2);
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeAreaObjects = new(2);
    
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
            _kdTreeAreaObjects.ExecuteInOrder(result.AddRange);
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

    public AreaObjectDTO? Find(AreaObjectType areaObjectType, GPSCoordinate coordinate, Guid internalId)
    {
        switch (areaObjectType)
        {
            case AreaObjectType.RealEstate:
                return FindRealEstates(coordinate).FirstOrDefault(a => a.InternalId == internalId);
            case AreaObjectType.Parcel:
                return FindParcels(coordinate).FirstOrDefault(a => a.InternalId == internalId);
            default:
                return FindAreaObjects(coordinate).FirstOrDefault(a => a.InternalId == internalId);
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
        return _kdTreeAreaObjects.FindByKey(coordinate)
                .Distinct().Select(a => a.ToDTO()).ToList();
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
        var result = _kdTreeAreaObjects.FindByKey(coordinateA);
        var tmp = _kdTreeAreaObjects.FindByKey(coordinateB);
        result.AddRange(tmp);
        return result.Distinct().Select(a => a.ToDTO()).ToList();
    }
    
    #endregion

    #region ModifyOperations
    
    public AreaObjectDTO Insert(AreaObjectDTO areaObjectDTO)
    {
        var areaObjectToInsert = areaObjectDTO.ToAreaObject();
        
        Insert(areaObjectToInsert.CoordinateA, areaObjectToInsert);
            
        if (areaObjectToInsert.CoordinateA != areaObjectToInsert.CoordinateB)
        {
            Insert(areaObjectToInsert.CoordinateB, areaObjectToInsert);
        }
        
        return areaObjectToInsert.ToDTO();
    }

    private void Insert(GPSCoordinate coordinate, AreaObject areaObjectToInsert)
    {
        if (areaObjectToInsert.Type == AreaObjectType.RealEstate)
        {
            var parcels = _kdTreeParcels.FindByKey(coordinate);
            areaObjectToInsert.AssociatedObjects.AddRange(parcels);

            foreach (var parcel in areaObjectToInsert.AssociatedObjects)
            {
                parcel.AssociatedObjects.Add(areaObjectToInsert);
            }
            
            _kdTreeRealEstates.Insert(coordinate, areaObjectToInsert);
        }
        else
        {
            var realEstates = _kdTreeRealEstates.FindByKey(coordinate);
            areaObjectToInsert.AssociatedObjects.AddRange(realEstates);
            
            foreach (var realEstate in areaObjectToInsert.AssociatedObjects)
            {
                realEstate.AssociatedObjects.Add(areaObjectToInsert);
            }
            
            _kdTreeParcels.Insert(coordinate, areaObjectToInsert);
        }
        
        _kdTreeAreaObjects.Insert(coordinate, areaObjectToInsert);
    }
    
    public void Delete(AreaObjectDTO areaObject)
    {
        var areaObjectWithInternalId = new AreaObject(areaObject.InternalId);
        
        List<AreaObject> associatedObjects = new();
        
        var coordinateA = areaObject.CoordinateA;
        var coordinateB = areaObject.CoordinateB;

        if (areaObject.Type == AreaObjectType.Parcel)
        {
            var deletedAreaObject = _kdTreeParcels.Delete(coordinateA, areaObjectWithInternalId);
            
            if (coordinateA != coordinateB)
            {
                _kdTreeParcels.Delete(coordinateB, areaObjectWithInternalId);
            }

            associatedObjects = deletedAreaObject.AssociatedObjects;
        }
        else
        {
            var deletedAreaObject = _kdTreeRealEstates.Delete(coordinateA, areaObjectWithInternalId);
            
            if (coordinateA != coordinateB)
            {
                _kdTreeRealEstates.Delete(coordinateB, areaObjectWithInternalId);
            }

            associatedObjects = deletedAreaObject.AssociatedObjects;
        }

        // Mazanie inverznych referencii
        foreach (var associatedObject in associatedObjects)
        {
            var associatedObjectOfAnother = associatedObject.AssociatedObjects;

            var associatedObjectToDelete = associatedObjectOfAnother.Find(a => a.Equals(areaObjectWithInternalId));
            
            if (associatedObjectToDelete != null)
            {
                associatedObjectOfAnother.Remove(associatedObjectToDelete);
            }
        }
        
        // Zmazanie zo spolocneho stromu
        _kdTreeAreaObjects.Delete(coordinateA, areaObjectWithInternalId);
        
        if (coordinateA != coordinateB)
        {
            _kdTreeAreaObjects.Delete(coordinateB, areaObjectWithInternalId);
        }
    }
    
    public AreaObjectDTO Update(AreaObjectDTO originalAreaObject, AreaObjectDTO updatedAreaObject)
    {
        if (!updatedAreaObject.AreCoordinatesChanged(originalAreaObject))
        {
            AreaObject foundAreaObject;
            
            if (originalAreaObject.Type == AreaObjectType.RealEstate)
            {
                foundAreaObject = _kdTreeRealEstates
                    .FindByKey(originalAreaObject.CoordinateA)
                    .First(a => a.InternalId == originalAreaObject.InternalId);
            }
            else
            {
                foundAreaObject = _kdTreeParcels
                    .FindByKey(originalAreaObject.CoordinateA)
                    .First(a => a.InternalId == originalAreaObject.InternalId);
            }
            
            int.TryParse(updatedAreaObject.Id, out var newId);

            foundAreaObject.Id = newId;
            foundAreaObject.Description = updatedAreaObject.Description;

            return foundAreaObject.ToDTO();
        }
        
        Delete(originalAreaObject);
        return Insert(updatedAreaObject);
    }
    
    #endregion

    #region GenerateAreaObjects
    
    public void GenerateAreaObjects(GenerateAreaObjectsOptions options)
    {
        List<AreaObjectDTO> realEstates = new();
        List<AreaObjectDTO> parcels = new();
        
        for (var i = 0; i < options.CountOfParcels + options.CountOfRealEstates; i++)
        {
            var objectType = i < options.CountOfParcels ? AreaObjectType.Parcel : AreaObjectType.RealEstate;
            
            var coordinateAX = _random.Next(-options.MinX, options.MaxX) + _random.NextDouble();
            coordinateAX = Math.Round(coordinateAX, options.NumberOfDecimalPlaces);
            var coordinateAXDirection = coordinateAX < 0 ? 'W' : 'E';
            
            var coordinateAY = _random.Next(-options.MinY, options.MaxY) + _random.NextDouble();
            coordinateAY = Math.Round(coordinateAY, options.NumberOfDecimalPlaces);
            var coordinateAYDirection = coordinateAY < 0 ? 'S' : 'N';

            double coordinateBX;
            char coordinateBXDirection;
            double coordinateBY;
            char coordinateBYDirection;
            
            if (_random.NextDouble() < options.ProbabilityOfOverlay && ((objectType == AreaObjectType.RealEstate && parcels.Count > 0) || (objectType == AreaObjectType.Parcel && realEstates.Count > 0)))
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
                coordinateBX = _random.Next(-options.MinX, options.MaxX) + _random.NextDouble();
                coordinateBX = Math.Round(coordinateBX, options.NumberOfDecimalPlaces);
                coordinateBXDirection = coordinateBX < 0 ? 'W' : 'E';
            
                coordinateBY = _random.Next(-options.MinY, options.MaxY) + _random.NextDouble();
                coordinateBY = Math.Round(coordinateBY, options.NumberOfDecimalPlaces);
                coordinateBYDirection = coordinateBY < 0 ? 'S' : 'N';
            }
            
            var areaObject = new AreaObjectDTO
            {
                Type = objectType,
                Description = options.GenerateRandomDescription ? GenerateDescription() : string.Empty,
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
        var realEstatesPath = Path.Combine(pathToFolder.LocalPath, "realEstates.csv");
        var parcelsPath = Path.Combine(pathToFolder.LocalPath, "parcels.csv");
        
        SaveRealEstatesToFile(realEstatesPath);
        SaveParcelsToFile(parcelsPath);
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
        using var writer = new StreamWriter(path);
        
        _kdTreeParcels.ExecuteLevelOrder((key, areaObjects) =>
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

    #endregion
    
    #region LoadOperations
    
    public void LoadFromFolder(Uri pathToFolder)
    {
        _kdTreeRealEstates.Clear();
        _kdTreeParcels.Clear();
        _kdTreeAreaObjects.Clear();
        
        var realEstatesPath = Path.Combine(pathToFolder.LocalPath, "realEstates.csv");
        var parcelsPath = Path.Combine(pathToFolder.LocalPath, "parcels.csv");
        
        LoadRealEstatesFromFile(realEstatesPath);
        LoadParcelsFromFile(parcelsPath);
    }
    
    private void LoadRealEstatesFromFile(string path)
    {
        var areaObjects = new List<AreaObject>();
        
        LoadFile(path, segments =>
        {
            // vzdy 2 + (4 * n) segmentov, kde n je pocet objektov
            if ((segments.Length - 2) % 4 != 0)
            {
                throw new ArgumentException("Invalid file format");
            }

            if (segments.Length == 2)
            {
                var coordinateA = new GPSCoordinate(double.Parse(segments[0]), double.Parse(segments[1]));
                _kdTreeRealEstates.Insert(coordinateA);
            }
            else
            {
                var areaObjectsFromSegments = AreaObject.FromReducedCSV(segments);
            
                foreach (var areaObject in areaObjectsFromSegments)
                {
                    Insert(areaObject.CoordinateA, areaObject);
                }
                
                areaObjects.AddRange(areaObjectsFromSegments);
            }
        });
        
        // Pridanie objektov podla sekundarnej suradnice
        foreach (var areaObject in areaObjects)
        {
            Insert(areaObject.CoordinateB, areaObject);
        }
    }

    public void LoadParcelsFromFile(string path)
    {
        var areaObjects = new List<AreaObject>();
        
        LoadFile(path, segments =>
        {
            // vzdy 2 + (4 * n) segmentov, kde n je pocet objektov
            if ((segments.Length - 2) % 4 != 0)
            {
                throw new ArgumentException("Invalid file format");
            }

            if (segments.Length == 2)
            {
                var coordinateA = new GPSCoordinate(double.Parse(segments[0]), double.Parse(segments[1]));
                _kdTreeParcels.Insert(coordinateA);
            }
            else
            {
                var areaObjectsFromSegments = AreaObject.FromReducedCSV(segments);
            
                foreach (var areaObject in areaObjectsFromSegments)
                {
                    Insert(areaObject.CoordinateA, areaObject);
                }
                
                areaObjects.AddRange(areaObjectsFromSegments);
            }
        });
        
        // Pridanie objektov podla sekundarnej suradnice
        foreach (var areaObject in areaObjects)
        {
            Insert(areaObject.CoordinateB, areaObject);
        }
    }
    
    private void LoadFile(string path, Action<string[]> actionForLine)
    {
        using var reader = new StreamReader(path);
        
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            
            var segments = line.Split(';');
            
            if (segments.Length < 2)
            {
                throw new ArgumentException("Invalid file format");
            }
            
            actionForLine(segments);
        }
    }
    
    #endregion
}
