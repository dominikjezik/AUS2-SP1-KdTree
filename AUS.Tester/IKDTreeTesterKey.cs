using AUS.DataStructures.KDTree;

namespace AUS.Tester;

public interface IKDTreeTesterKey<TKey> where TKey : IKDTreeKeyComparable<TKey>
{
    public int NumberOfDimension { get; }

    public TKey GenerateRandom(Random random);
}
