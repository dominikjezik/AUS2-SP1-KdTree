using AUS.DataStructures.KDTree;

namespace AUS.Tester;

public class KDTreeTester<TKey> where TKey : IKDTreeKeyComparable<TKey>, IKDTreeTesterKey<TKey>, new()
{
    private readonly Random _random = new();
    private readonly KDTree<TKey, object> _kdTree;
    private readonly List<(TKey, object)> _helperList = [];
    private readonly int _numberOfDimension;
    
    private readonly Func<TKey> _generateFunction;

    public KDTreeTester(TKey templateKey)
    {
        _numberOfDimension = templateKey.NumberOfDimension;
        _kdTree = new(_numberOfDimension);
        _generateFunction = () => templateKey.GenerateRandom(_random);
    }
    
    public KDTreeTester()
    {
        var dummyTemplateKey = new TKey();
        
        _kdTree = new(dummyTemplateKey.NumberOfDimension);
        _generateFunction = () => dummyTemplateKey.GenerateRandom(_random);
    }
    
    public void TestRandomDataSet(int numberOfOperations, double probInsert)
    {
        for (int i = 0; i < numberOfOperations; i++)
        {
            var prob = _random.NextDouble();

            if (prob <= probInsert)
            {
                TestInsert();
            }
            else
            {
                TestDelete();
            }

            TestFindEveryItem();
        }
    }

    private void TestInsert()
    {
        var newKey = _generateFunction();
        var newData = new object();
        Console.WriteLine($"Vygenerovany novy kluc pre vlozenie {newKey}");
        
        _kdTree.Insert(newKey, newData);
        _helperList.Add((newKey, newData));
        
        Console.WriteLine($"Kluc {newKey} vlozeny");
                
        // Kontrola ci tam naozaj je vlozeny a ci ho dokazem vybrat
        var insertedData = _kdTree.FindByKey(newKey);
        
        if (!insertedData.Contains(newData))
        {
            throw new Exception("Bola volana operacia insert ale find nenasiel polozku");
        }
    }
    
    private void TestDelete()
    {
        if (_helperList.Count == 0)
        {
            Console.WriteLine("Pokus o vymazanie ale strom je momentalne prazdny");
            return;
        }
        
        var index = _random.Next(_helperList.Count);
        var (key, data) = _helperList[index];
        Console.WriteLine($"Vybrany kluc pre vymazanie {key}");

        _kdTree.Delete(key, data);
        
        Console.WriteLine($"Kluc {key} vymazany");
        
        _helperList.RemoveAt(index);
        
        // Kontrola ci tam naozaj nie je vlozeny ked som ho vymazal
        var insertedData = _kdTree.FindByKey(key);
        
        if (insertedData.Contains(data))
        {
            throw new Exception("Bola volana operacia delete ale find nasiel polozku");
        }
    }

    private void TestFindEveryItem()
    {
        foreach (var (key, data) in _helperList)
        {
            var insertedData = _kdTree.FindByKey(key);
        
            if (!insertedData.Contains(data))
            {
                throw new Exception($"Bola volana operacia insert nad klucom {key} ale find nenasiel polozku");
            }
        }
    }
}
