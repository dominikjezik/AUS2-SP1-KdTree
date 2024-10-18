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


/*
kdtree.Insert(new(70, 50), area1);
kdtree.Insert(new(-30, 10), area1);
kdtree.Insert(new(80, -70), area1);
kdtree.Insert(new(-100, -60), area1);
kdtree.Insert(new(-40, -70), area1);
kdtree.Insert(new(-10, 40), area1);
kdtree.Insert(new(40, 70), area1);
kdtree.Insert(new(50, 20), area1);
*/


kdtree.Insert(new(23, 35), area1);
kdtree.Insert(new(20, 33), area1);
kdtree.Insert(new(25, 36), area1);
kdtree.Insert(new(16, 31), area1);
kdtree.Insert(new(14, 39), area1);
kdtree.Insert(new(28, 34), area1);
kdtree.Insert(new(24, 40), area1);
kdtree.Insert(new(13, 32), area1);
kdtree.Insert(new(12, 41), area1);
kdtree.Insert(new(17, 42), area1);
kdtree.Insert(new(26, 35), area1);
kdtree.Insert(new(30, 33), area1);
kdtree.Insert(new(29, 46), area1);
kdtree.Insert(new(27, 43), area1);

//kdtree.Insert(new(18,40), area1);

kdtree.Insert(new(13, 42), area1);
//kdtree.Insert(new(13.5, 43), area1);

//kdtree.Insert(new(18,43), area1);
//kdtree.Insert(new(19,44), area1);

/*
var testValue = kdtree.FindByKey(new(40, 20));
foreach (var value in testValue)
{
    Console.WriteLine(value);
}

kdtree.ExecuteInOrder((GPSCoordinate key) =>
{
    Console.WriteLine(key);
});
*/

kdtree.Delete(new(20, 33), area1);


