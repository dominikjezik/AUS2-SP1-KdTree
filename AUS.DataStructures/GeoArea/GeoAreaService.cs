using System.Text;
using AUS.DataStructures.KDTree;

namespace AUS.DataStructures.GeoArea;

public class GeoAreaService
{
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeRealEstates = new(2);
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeParcels = new(2);
    private readonly Random _random = new();
    
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

        // TODO: Otazka na konzultaciu, bez Distinct() zobrazuje Duplicity (lebo 2 body) a blbne zobrazenie zoznamu
        return result.Distinct().Select(a => a.ToDTO()).ToList();
    }

    public List<AreaObjectDTO> Find(AreaObjectType areaObjectType, GPSCoordinate coordinate)
    {
        switch (areaObjectType)
        {
            // TODO: Otazka na konzultaciu, bez Distinct() zobrazuje Duplicity (lebo 2 body) a blbne zobrazenie zoznamu
            
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
            // TODO: Otazka na konzultaciu, bez Distinct() zobrazuje Duplicity (lebo 2 body) a blbne zobrazenie zoznamu
            
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
    
    public AreaObjectDTO Update(AreaObjectDTO areaObject)
    {
        throw new NotImplementedException();
        
        // TODO: Edit (v pripade editu suradnic pravdepodobne odobrat a na novo pridat)
        
        /*
        bool correctCoordinateAX = double.TryParse(form.CoordinateAX, out var coordinateAX); 
        bool correctCoordinateAY = double.TryParse(form.CoordinateAY, out var coordinateAY);

        bool correctCoordinateBX = double.TryParse(form.CoordinateBX, out var coordinateBX);
        bool correctCoordinateBY = double.TryParse(form.CoordinateBY, out var coordinateBY);
        
        bool correctId = int.TryParse(form.Id, out var id);
        
        if (!correctCoordinateAX || !correctCoordinateAY || !correctCoordinateBX || !correctCoordinateBY)
        {
            return;
        }
        
        // Kontrola ci sa nezmenili suradnice
        // TODO: presunutie do Service
        if (originalAreaObject.CoordinateA == new GPSCoordinate(coordinateAX, coordinateAY) && 
            originalAreaObject.CoordinateB == new GPSCoordinate(coordinateBX, coordinateBY))
        {
            originalAreaObject.Id = id;
            originalAreaObject.Description = form.Description;
            viewModel.RefreshAreaObjects();
        }
        else
        {
            // TODO: Zmenili sa aj suradnice, treba (obe?) suradnice odstranit a na novo pridat
            throw new NotImplementedException();
        }
         */

        return areaObject;
    }

    public void Delete(AreaObjectDTO areaObject)
    {
        // pozn.: nezabudnut odstranit inverzne referenciu z associovanych objektov
    }

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
}
