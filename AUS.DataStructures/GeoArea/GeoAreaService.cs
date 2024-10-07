using AUS.DataStructures.KDTree;

namespace AUS.DataStructures.GeoArea;

public class GeoAreaService
{
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeRealEstates = new(2);
    private readonly KDTree<GPSCoordinate, AreaObject> _kdTreeParcels = new(2);

    public List<AreaObject> FindRealEstates(GPSCoordinate coordinate)
    {
        return _kdTreeRealEstates.FindByKey(coordinate);
    }

    public List<AreaObject> FindParcels(GPSCoordinate coordinate)
    {
        return _kdTreeParcels.FindByKey(coordinate);
    }

    public List<AreaObject> FindGeoAreas(GPSCoordinate coordinateA, GPSCoordinate coordinateB)
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

    public void Insert(AreaObject areaObject)
    {
        if (areaObject.Type == AreaObjectType.RealEstate)
        {
            var parcelsA = _kdTreeParcels.FindByKey(areaObject.CoordinateA);
            var parcelsB = _kdTreeParcels.FindByKey(areaObject.CoordinateB);
            areaObject.AssociatedObjects = parcelsA;
            areaObject.AssociatedObjects.AddRange(parcelsB);
            
            _kdTreeRealEstates.Insert(areaObject.CoordinateA, areaObject);
            _kdTreeRealEstates.Insert(areaObject.CoordinateB, areaObject);
        }
        else
        {
            var realEstatesA = _kdTreeRealEstates.FindByKey(areaObject.CoordinateA);
            var realEstatesB = _kdTreeRealEstates.FindByKey(areaObject.CoordinateB);
            areaObject.AssociatedObjects = realEstatesA;
            areaObject.AssociatedObjects.AddRange(realEstatesB);
            
            _kdTreeParcels.Insert(areaObject.CoordinateA, areaObject);
            _kdTreeParcels.Insert(areaObject.CoordinateB, areaObject);
        }
    }
    
    // TODO: Edit (v pripade editu suradnic pravdepodobne odobrat a na novo pridat)

    public void Delete(GPSCoordinate coordinate)
    {
        // pozn.: nezabudnut odstranit inverzne referenciu z associovanych objektov
    }
}
