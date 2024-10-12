using AUS.DataStructures.GeoArea;
using AUS.DataStructures.KDTree;

Console.WriteLine("Hello, World!");

var kdtree = new KDTree<GPSCoordinate, AreaObject>(2);

var area1 = new AreaObject
{
    CoordinateA = new(10, 10),
    CoordinateB = new(40, 20),
    Id = 1,
    Description = "Lorem ipsum dolor sit amet",
    Type = AreaObjectType.Parcel
};

var area2 = new AreaObject
{
    CoordinateA = new(0, 0),
    CoordinateB = new(40, 20),
    Id = 1,
    Description = "Lorem ipsum dolor sit amet",
    Type = AreaObjectType.RealEstate
};

kdtree.Insert(new(10, 10), area1);
kdtree.Insert(new(40, 20), area1);
kdtree.Insert(new(0, 0), area2);
kdtree.Insert(new(40, 20), area2);

var testValue = kdtree.FindByKey(new(40, 20));
foreach (var value in testValue)
{
    Console.WriteLine(value);
}

kdtree.ExecuteInOrder((GPSCoordinate key) =>
{
    Console.WriteLine(key);
});
