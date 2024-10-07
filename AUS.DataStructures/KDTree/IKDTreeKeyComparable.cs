namespace AUS.DataStructures.KDTree;

public interface IKDTreeKeyComparable<T>
{
    int CompareTo(T another, int dimension);
}
