using System.Text;
using AUS.DataStructures.KDTree;

namespace AUS.Tester;

public class KDTreeTester
{
    private readonly Random _random = new();
    private readonly int _numberOfDimension;
    private readonly KDTree<KDTreeSimpleKey, KDTreeSimpleKey> _kdTree;
    private readonly List<KDTreeSimpleKey> _helperList = [];

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
                // TODO
            }
        }
    }

    private void TestInsert()
    {
        var newKey = GenerateNewKeyForInsert();
        Console.WriteLine($"Vygenerovany novy kluc pre vlozenie {newKey}");
        
        _kdTree.Insert(newKey, newKey);
        
        Console.WriteLine($"Kluc {newKey} vlozeny");
        
        _helperList.Add(newKey);
                
        // Kontrola ci tam naozaj je vlozeny a ci ho dokazem vybrat
        if (CheckCountOfExists(newKey) == 0)
        {
            throw new Exception("Bola volana operacia insert ale find nenasiel polozku");
        }

        TestInorder();
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
            array[i] = _random.Next(1000000);
        }

        return new(array);
    }

    private int CheckCountOfExists(KDTreeSimpleKey key)
    {
        var result = _kdTree.FindByKey(key);
        var count = 0;
        
        foreach (var item in result)
        {
            if (!item.Equals(key))
            {
                throw new Exception($"Vstupny kluc bol {key} are Find vratil {item}");
            }

            count++;
        }

        return count;
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
