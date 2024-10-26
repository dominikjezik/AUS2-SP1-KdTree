using System.Text;
using AUS.DataStructures.KDTree;

namespace AUS.Tester;

public record KDTreeSimpleKey : IKDTreeKeyComparable<KDTreeSimpleKey>, IKDTreeTesterKey<KDTreeSimpleKey>
{
    public int NumberOfDimension => Values.Length;
    
    public int[] Values { get; }
    
    public KDTreeSimpleKey(int[] values)
    {
        Values = values;
    }

    public KDTreeSimpleKey()
    {
    }
    
    public KDTreeSimpleKey(int numberOfDimension)
    {
        Values = new int[numberOfDimension];
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
    
    public KDTreeSimpleKey GenerateRandom(Random random)
    {
        var array = new int[NumberOfDimension];
        
        for (int i = 0; i < NumberOfDimension; i++)
        {
            //array[i] = _random.Next(1000000);
            array[i] = random.Next(1000);
        }

        return new(array);
    }
}