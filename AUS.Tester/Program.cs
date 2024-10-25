using AUS.Tester;

Console.WriteLine("--- TESTER ---");

// TODO: Uprava testera podla pokynov

/*
var tester = new KDTreeTester(2);
tester.TestRandomDataSet(10000, 0.7);
*/

for (int i = 2; i <= 100; i++)
{
    for (double j = 0.1; j <= 1; j += 0.1)
    {
        var tester = new KDTreeTester(i);
        tester.TestRandomDataSet(10000, j);
    }
}

