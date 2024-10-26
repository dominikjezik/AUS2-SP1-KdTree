using AUS.Tester;

Console.WriteLine("--- TESTER ---");

var random = new Random();

/*
var tester = new KDTreeTester<KDTreeCompositeDataTypesKey>();
tester.TestRandomDataSet(10000, 0.8);
*/


for (double j = 0.1; j <= 1; j += 0.1)
{
    var tester = new KDTreeTester<KDTreeCompositeDataTypesKey>();
    tester.TestRandomDataSet(10000, j);
}


/*
var template = new KDTreeSimpleKey(2);

var tester = new KDTreeTester<KDTreeSimpleKey>(template);
tester.TestRandomDataSet(10000, 0.5);
*/

/*
for (int i = 2; i <= 10; i++)
{
    for (double j = 0.1; j <= 1; j += 0.1)
    {
        var template = new KDTreeSimpleKey(i);
        var tester = new KDTreeTester<KDTreeSimpleKey>(template);
        tester.TestRandomDataSet(10000, j);
    }
}
*/
