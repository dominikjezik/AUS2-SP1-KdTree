namespace AUS.DataStructures.KDTree;

internal class KDTreeNodeWithDimension<TKey, TData> where TKey : IKDTreeKeyComparable<TKey>
{
    public KDTreeNode<TKey, TData> Node { get; set; }
    public int Dimension { get; set; }
    
    public KDTreeNodeWithDimension(KDTreeNode<TKey, TData> node, int dimension)
    {
        Node = node;
        Dimension = dimension;
    }
}
