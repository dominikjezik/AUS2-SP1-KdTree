using System.Text;
using AUS.DataStructures.KDTree;

namespace AUS.DataStructures.GeoArea;

public class GeoAreaService
{
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeRealEstates = new(2);
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeParcels = new(2);
    private readonly Random _random = new();
    
    public List<AreaObject> Get(AreaObjectType areaObjectType)
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

        // TODO: Otazka na konzultaciu, bez Distinct() zobrazuje Duplicity (lebo 2 body) a blbne zobrazenie zoznamu
        return result.Distinct().ToList();
    }
    
    public List<AreaObject> Find(AreaObjectType areaObjectType, GPSCoordinate coordinate)
    {
        switch (areaObjectType)
        {
            // TODO: Otazka na konzultaciu, bez Distinct() zobrazuje Duplicity (lebo 2 body) a blbne zobrazenie zoznamu
            
            case AreaObjectType.RealEstate:
                return FindRealEstates(coordinate).Distinct().ToList();
            case AreaObjectType.Parcel:
                return FindParcels(coordinate).Distinct().ToList();
            default:
                return FindAreaObjects(coordinate).Distinct().ToList();
        }
    }
    
    public List<AreaObject> Find(AreaObjectType areaObjectType, GPSCoordinate coordinateA, GPSCoordinate coordinateB)
    {
        switch (areaObjectType)
        {
            // TODO: Otazka na konzultaciu, bez Distinct() zobrazuje Duplicity (lebo 2 body) a blbne zobrazenie zoznamu
            
            case AreaObjectType.RealEstate:
                return FindRealEstates(coordinateA, coordinateB).Distinct().ToList();
            case AreaObjectType.Parcel:
                return FindParcels(coordinateA, coordinateB).Distinct().ToList();
            default:
                return FindAreaObjects(coordinateA, coordinateB).Distinct().ToList();
        }
    }
    
    public List<AreaObject> FindRealEstates(GPSCoordinate coordinate)
    {
        return _kdTreeRealEstates.FindByKey(coordinate);
    }

    public List<AreaObject> FindParcels(GPSCoordinate coordinate)
    {
        return _kdTreeParcels.FindByKey(coordinate);
    }
    
    public List<AreaObject> FindAreaObjects(GPSCoordinate coordinate)
    {
        var result = _kdTreeRealEstates.FindByKey(coordinate);
        var tmp = _kdTreeParcels.FindByKey(coordinate);
        result.AddRange(tmp);
        return result;
    }
    
    public List<AreaObject> FindRealEstates(GPSCoordinate coordinateA, GPSCoordinate coordinateB)
    {
        var result = _kdTreeRealEstates.FindByKey(coordinateA);
        var tmp = _kdTreeRealEstates.FindByKey(coordinateB);
        result.AddRange(tmp);
        return result;
    }
    
    public List<AreaObject> FindParcels(GPSCoordinate coordinateA, GPSCoordinate coordinateB)
    {
        var result = _kdTreeParcels.FindByKey(coordinateA);
        var tmp = _kdTreeParcels.FindByKey(coordinateB);
        result.AddRange(tmp);
        return result;
    }

    public List<AreaObject> FindAreaObjects(GPSCoordinate coordinateA, GPSCoordinate coordinateB)
    {
        var result = _kdTreeRealEstates.FindByKey(coordinateA);
        var tmp = _kdTreeRealEstates.FindByKey(coordinateB);
        result.AddRange(tmp);
        
        tmp = _kdTreeParcels.FindByKey(coordinateA);
        result.AddRange(tmp);

        tmp = _kdTreeParcels.FindByKey(coordinateB);
        result.AddRange(tmp);

        return result;
    }

    public void Insert(AreaObject areaObjectToInsert)
    {
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
    }
    
    // TODO: Edit (v pripade editu suradnic pravdepodobne odobrat a na novo pridat)

    public void Delete(AreaObject areaObject)
    {
        // pozn.: nezabudnut odstranit inverzne referenciu z associovanych objektov
    }
    
    public void ExecuteInOrder(Action<List<AreaObject>> actionToExec)
    {
        _kdTreeRealEstates.ExecuteInOrder(actionToExec);
        _kdTreeParcels.ExecuteInOrder(actionToExec);
    }

    public void GenerateOperations(int count, double probabilityOfInsert, int minX, int maxX, int minY, int maxY, int numberOfDecimalPlaces, bool generateRandomDescription)
    {
        for (var i = 0; i < count; i++)
        {
            if (_random.NextDouble() < probabilityOfInsert)
            {
                var coordinateAX = _random.Next(minX, maxX) + _random.NextDouble();
                var coordinateAY = _random.Next(minY, maxY) + _random.NextDouble();
                coordinateAX = Math.Round(coordinateAX, numberOfDecimalPlaces);
                coordinateAY = Math.Round(coordinateAY, numberOfDecimalPlaces);
                
                var coordinateBX = _random.Next(minX, maxX) + _random.NextDouble();
                var coordinateBY = _random.Next(minY, maxY) + _random.NextDouble();
                coordinateBX = Math.Round(coordinateBX, numberOfDecimalPlaces);
                coordinateBY = Math.Round(coordinateBY, numberOfDecimalPlaces);
                
                var areaObject = new AreaObject
                {
                    Type = _random.NextDouble() < 0.5 ? AreaObjectType.RealEstate : AreaObjectType.Parcel,
                    Description = generateRandomDescription ? GenerateDescription() : string.Empty,
                    Id = _random.Next(1, 99999),
                    CoordinateA = new GPSCoordinate(coordinateAX, coordinateAY),
                    CoordinateB = new GPSCoordinate(coordinateBX, coordinateBY)
                };
                
                Insert(areaObject);
            }
            else
            {
                // TODO cez prehliadku najst nahodny objekt?
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
}
