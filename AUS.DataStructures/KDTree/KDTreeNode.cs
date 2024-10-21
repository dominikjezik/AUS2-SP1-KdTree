namespace AUS.DataStructures.KDTree;

public class KDTreeNode<TKey, TData> where TKey : IKDTreeKeyComparable<TKey>
{
    public TKey Key { get; set; }

    public List<TData> Data { get; set; } = new();

    public KDTreeNode<TKey, TData>? ParentNode { get; set; }

    public KDTreeNode<TKey, TData>? LeftNode { get; set; }

    public KDTreeNode<TKey, TData>? RightNode { get; set; }

    public KDTreeNode(TKey key, TData data)
    {
        Key = key;
        Data.Add(data);
    }
    
    public KDTreeNode(TKey key, List<TData> data)
    {
        Key = key;
        Data = data;
    }
    
    public KDTreeNode(TKey key)
    {
        Key = key;
    }
}
