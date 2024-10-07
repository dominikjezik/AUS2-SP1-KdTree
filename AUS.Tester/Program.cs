using AUS.Tester;

Console.WriteLine("--- TESTER ---");

var tester = new KDTreeTester(2);
tester.TestRandomDataSet(10000, 1);
