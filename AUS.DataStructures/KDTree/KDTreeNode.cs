namespace AUS.DataStructures.KDTree;

public class KDTreeNode<TKey, TData> where TKey : IComparable
{
    public TKey[] Keys { get; }

    public List<TData> Data { get; } = new();

    //public TData Data { get; }
    //public KDTreeDataContainer<TData> Container { get; private set; }

    //public TDataContainer Container { get; set; }

    public KDTreeNode<TKey, TData>? ParentNode { get; set; }

    public KDTreeNode<TKey, TData>? LeftNode { get; set; }

    public KDTreeNode<TKey, TData>? RightNode { get; set; }

    public KDTreeNode(TKey[] keys, TData data)
    {
        Keys = keys;
        Data.Add(data);
        //Data = data;
        //Container = new();
        //Container.DataList.Add(data);
    }
}
