using AUS.DataStructures.GeoArea;
using AUS.DataStructures.KDTree;

Console.WriteLine("Hello, World!");

var kdtree = new KDTree<double, object>(2);

kdtree.Insert([50, 70], "50, 70");
kdtree.Insert([20, 30], "20, 30");
kdtree.Insert([30, 20], "30, 20");

var testValue = kdtree.FindByKeys([20, 30]);
foreach (var value in testValue)
{
    Console.WriteLine(value);
}


var geoArea = new GeoAreaTree();
geoArea.Insert([1, 2], new AreaObject
{
    CoordinateA = new GPSCoordinate(1, 2),
    CoordinateB = new GPSCoordinate(2, 3)
});


