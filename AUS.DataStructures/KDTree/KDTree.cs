namespace AUS.DataStructures.KDTree;

public class KDTree<TKey, TData> where TKey : IComparable
{
    private int _numberOfDimension;

    protected KDTreeNode<TKey, TData>? _root;

    public KDTree(int numberOfDimension)
    {
        _numberOfDimension = numberOfDimension;
    }

    protected bool TryFindNode(TKey[] keysForSearch, out KDTreeNode<TKey, TData>? currentNode, out int actualDimenstion)
    {
        actualDimenstion = -1;

        if (_root == null)
        {
            currentNode = null;
            return false;
        }
        else if (KeysEqual(keysForSearch, _root.Keys))
        {
            currentNode = _root;
            return true;
        }

        actualDimenstion = 0;
        currentNode = _root;

        while (true)
        {
            if (keysForSearch[actualDimenstion].CompareTo(currentNode.Keys[actualDimenstion]) <= 0)
            {
                if (currentNode.LeftNode != null)
                {
                    // Kontrola ci som ho prave nenasiel
                    if (KeysEqual(currentNode.Keys, keysForSearch))
                    {
                        return true;
                    }

                    // Presun do lava
                    currentNode = currentNode.LeftNode;
                }
                else
                {
                    return false;
                }
            }
            //else if (keysForSearch[actualDimenstion].CompareTo(currentNode.Keys[actualDimenstion]) > 0)
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

            actualDimenstion = ++actualDimenstion % _numberOfDimension;
        }
    }

    public List<TData> FindByKeys(TKey[] keysForSearch)
    {
        var isFound = TryFindNode(keysForSearch, out var foundNode, out var _);

        if (!isFound)
        {
            return [];
        }

        return foundNode!.Data;
    }

    public List<TData> FindByInterval(TKey[] keysA, TKey[] keysB)
    {
        throw new NotImplementedException();
    }

    public virtual void Insert(TKey[] keys, TData data)
    {
        if (_root == null)
        {
            _root = new KDTreeNode<TKey, TData>(keys, data);
            return;
        }

        var isFound = TryFindNode(keys, out var foundNode, out var lastDimenstion);

        if (isFound)
        {
            foundNode!.Data.Add(data);
            return;
        }

        // Nenasiel sa cize vo foundNode je posledny uzol, kde skoncilo hladanie
        if (keys[lastDimenstion].CompareTo(foundNode!.Keys[lastDimenstion]) <= 0)
        {
            // Vlozenie do lava
            var newNode = new KDTreeNode<TKey, TData>(keys, data);
            foundNode.LeftNode = newNode;
            newNode.ParentNode = foundNode.LeftNode;
        }
        else
        {
            // Vlozenie do prava
            var newNode = new KDTreeNode<TKey, TData>(keys, data);
            foundNode.RightNode = newNode;
            newNode.ParentNode = foundNode.RightNode;
        }
    }

    public void Delete(TKey[] keys)
    {
        throw new NotImplementedException();
    }


    private static bool KeysEqual(TKey[] keyA, TKey[] keyB)
    {
        for (int i = 0; i < keyA.Length; i++)
        {
            if (keyA[i].CompareTo(keyB[i]) != 0)
            {
                return false;
            }
        }

        return true;
    }
}
