namespace AUS.DataStructures.KDTree;

public class KDTree<TKey, TData> where TKey : IKDTreeKeyComparable<TKey>
{
    private int _numberOfDimension;

    private KDTreeNode<TKey, TData>? _root;

    public KDTree(int numberOfDimension)
    {
        _numberOfDimension = numberOfDimension;
    }

    private bool TryFindNode(TKey keysForSearch, out KDTreeNode<TKey, TData>? currentNode, out int actualDimension)
    {
        actualDimension = -1;

        if (_root == null)
        {
            currentNode = null;
            return false;
        }
        
        actualDimension = 0;
        
        currentNode = _root;

        while (true)
        {
            // Kontrola ci som ho prave nenasiel
            if (currentNode.Key.Equals(keysForSearch))
            {
                return true;
            }
            
            if (keysForSearch.CompareTo(currentNode.Key, actualDimension) <= 0)
            {
                if (currentNode.LeftNode != null)
                {
                    // Presun do lava
                    currentNode = currentNode.LeftNode;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (currentNode.RightNode != null)
                {
                    // Presun do prava
                    currentNode = currentNode.RightNode;
                }
                else
                {
                    return false;
                }
            }

            actualDimension = ++actualDimension % _numberOfDimension;
        }
    }

    public List<TData> FindByKey(TKey keyForSearch)
    {
        var isFound = TryFindNode(keyForSearch, out var foundNode, out var _);

        if (!isFound)
        {
            return [];
        }
        
        return new List<TData>(foundNode!.Data);
    }

    public void Insert(TKey keyForInsert, TData data)
    {
        if (_root == null)
        {
            _root = new KDTreeNode<TKey, TData>(keyForInsert, data);
            return;
        }

        var isFound = TryFindNode(keyForInsert, out var foundNode, out var lastDimenstion);

        if (isFound)
        {
            foundNode!.Data.Add(data);
            return;
        }

        // Nenasiel sa cize vo foundNode je posledny uzol, kde skoncilo hladanie
        if (keyForInsert.CompareTo(foundNode!.Key, lastDimenstion) <= 0)
        {
            // Vlozenie do lava
            var newNode = new KDTreeNode<TKey, TData>(keyForInsert, data);
            foundNode.LeftNode = newNode;
            newNode.ParentNode = foundNode;
        }
        else
        {
            // Vlozenie do prava
            var newNode = new KDTreeNode<TKey, TData>(keyForInsert, data);
            foundNode.RightNode = newNode;
            newNode.ParentNode = foundNode;
        }
    }

    public void Delete(TKey key)
    {
        throw new NotImplementedException();
    }

    public void ExecuteInOrder(Action<TKey> actionToExec)
    {
        ExecuteInOrder(actionToExec, _root);
    }

    private void ExecuteInOrder(Action<TKey> actionToExec, KDTreeNode<TKey, TData>? node)
    {
        if (node != null)
        {
            ExecuteInOrder(actionToExec, node.LeftNode);
            actionToExec(node.Key);
            ExecuteInOrder(actionToExec, node.RightNode);
        }
    }
    
    public void ExecuteInOrder(Action<List<TData>> actionToExec)
    {
        ExecuteInOrder(actionToExec, _root);
    }
    
    private void ExecuteInOrder(Action<List<TData>> actionToExec, KDTreeNode<TKey, TData>? node)
    {
        if (node != null)
        {
            ExecuteInOrder(actionToExec, node.LeftNode);
            actionToExec(node.Data);
            ExecuteInOrder(actionToExec, node.RightNode);
        }
    }
}
