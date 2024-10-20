using System.Text;
using AUS.DataStructures.KDTree;

namespace AUS.Tester;

public class KDTreeTester
{
    private readonly Random _random = new();
    private readonly int _numberOfDimension;
    private readonly KDTree<KDTreeSimpleKey, object> _kdTree;
    private readonly List<(KDTreeSimpleKey, object)> _helperList = [];

    public KDTreeTester(int numberOfDimension)
    {
        _numberOfDimension = numberOfDimension;
        _kdTree = new(numberOfDimension);
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
        var newKey = GenerateNewKeyForInsert();
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

        TestInorder();
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

        TestInorder();
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

    private void TestInorder()
    {
        KDTreeSimpleKey? lastKey = null;
        
        _kdTree.ExecuteInOrder(key =>
        {
            if (lastKey == null)
            {
                lastKey = key;
            }
            else
            {
                var foundOkKeyPart = false;
            
                for (int i = 0; i < _numberOfDimension; i++)
                {
                    // Ak je aspon jedna zlozka aktualneho kluca >=, ako minuleho kluca
                    // prejdi na dalsi, malo by to byt OK??
                    // TODO: otazka ci je toto spravny predpoklad
                    if (key.Values[i] >= lastKey.Values[i])
                    {
                        foundOkKeyPart = true;
                        break;
                    }
                }

                if (foundOkKeyPart)
                {
                    lastKey = key;
                }
                else
                {
                    throw new Exception("Nespravne inorder poradie.");
                }
            }
        });
    }

    private KDTreeSimpleKey GenerateNewKeyForInsert()
    {
        var array = new int[_numberOfDimension];
        
        for (int i = 0; i < _numberOfDimension; i++)
        {
            //array[i] = _random.Next(1000000);
            array[i] = _random.Next(100);
        }

        return new(array);
    }
}

public record KDTreeSimpleKey : IKDTreeKeyComparable<KDTreeSimpleKey>
{
    public int[] Values { get; }
    
    public KDTreeSimpleKey(int[] values)
    {
        Values = values;
    }

    public virtual bool Equals(KDTreeSimpleKey? other)
    {
        if (other == null || Values.Length != other.Values.Length)
        {
            return false;
        }

        for (int i = 0; i < Values.Length; i++)
        {
            if (Values[i] != other.Values[i])
            {
                return false;
            }
        }

        return true;
    }

    public int CompareTo(KDTreeSimpleKey another, int dimension)
    {
        return Values[dimension].CompareTo(another.Values[dimension]);
    }

    public override string ToString()
    {
        var result = new StringBuilder("[");
        
        for (int i = 0; i < Values.Length; i++)
        {
            result.Append($"{Values[i]} ");
        }

        result.Append("]");

        return result.ToString();
    }
}
