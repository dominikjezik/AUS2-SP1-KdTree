namespace AUS.DataStructures.KDTree;

public class KDTree<TKey, TData> where TKey : IKDTreeKeyComparable<TKey>
{
    private readonly int _numberOfDimension;

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

    public void Delete(TKey keyForDelete, TData dataForDelete)
    {
        if (_root == null)
        {
            throw new InvalidOperationException("Item with given key not found!");
        }

        var isFound = TryFindNode(keyForDelete, out var foundNode, out var lastDimenstion);

        if (!isFound)
        {
            throw new InvalidOperationException("Item with given key not found!");
        }

        // Trivialna situacia ked mam v node viac prvkov
        if (foundNode.Data.Count > 1)
        {
            if (foundNode.Data.Contains(dataForDelete))
            {
                foundNode.Data.Remove(dataForDelete);
            }
            else
            {
                throw new InvalidOperationException("Item with given key and data not found!");
            }
            
            return;
        }
        
        // Trivialna situacia ked node je list
        if (foundNode.LeftNode == null && foundNode.RightNode == null)
        {
            if (foundNode.ParentNode == null)
            {
                _root = null;
                return;
            }
            
            if (foundNode.ParentNode.LeftNode == foundNode)
            {
                foundNode.ParentNode.LeftNode = null;
            }
            else
            {
                foundNode.ParentNode.RightNode = null;
            }
            
            return;
        }
        
        // Vrchol nie je listom => nahrad ho najmensim/najvacsim v pravom/lavom podstrome podla poslednej dimenzie

        // Proces bude bezat pokial najdeny nahradnik (min) nie je listom
        while (true)
        {
            KDTreeNode<TKey, TData> replacementNode;
            
            /*
            if (foundNode.RightNode != null)
            {
                // Pravy podstrom je k dispozici => hladame v pravom podstrome minimum podla lastDimension
                replacementNode = FindMinimumBy(lastDimenstion, foundNode.RightNode, out lastDimenstion);
            }
            else
            {
                // Pravy podstrom nie je => hladame v lavom podstrome maximum podla lastDimension
                replacementNode = FindMaximumBy(lastDimenstion, foundNode.LeftNode, out lastDimenstion);
            }
            */
            
            if (foundNode.LeftNode != null)
            {
                // Lavny podstrom je k dispozici => hladame v lavom podstrome maximum podla lastDimension
                replacementNode = FindMaximumBy(lastDimenstion, foundNode.LeftNode, out lastDimenstion);
            }
            else
            {
                // Lavny podstrom nie je => hladame v pravom podstrome minimum podla lastDimension
                replacementNode = FindMinimumBy(lastDimenstion, foundNode.RightNode, out lastDimenstion);
            }
            
            // Premiestnenie najdeneho nahradnika na miesto vymazaneho uzla (v tomto momente mazaneho uzla)
            foundNode.Key = replacementNode.Key;
            foundNode.Data = replacementNode.Data;
            
            // Ak je nahradnik listom tak ho vymazeme (uplne)
            if (replacementNode.LeftNode == null && replacementNode.RightNode == null)
            {
                if (replacementNode.ParentNode!.LeftNode == replacementNode)
                {
                    replacementNode.ParentNode.LeftNode = null;
                }
                else
                {
                    replacementNode.ParentNode.RightNode = null;
                }
                
                replacementNode.ParentNode = null;
                
                // --- KONIEC DELETU ---
                return;
            }
            
            // Ak replacementNode nie je listom tak pokracujeme vymazavanim nodu kde bolo najdene minimum/maximum
            foundNode = replacementNode;
        }
    }

    // TODO: Samostatne pretestovat
    private KDTreeNode<TKey, TData> FindMinimumBy(int targetDimension, KDTreeNode<TKey, TData>? actualNode, out int actualDimension)
    {
        var foundMinimalNode = actualNode;
        actualDimension = (targetDimension + 1) % _numberOfDimension;
        var foundMinimalNodeDimension = actualDimension;
        
        var stack = new KDTreeDimensionalStack<TKey, TData>();
        
        while (stack.Any() || actualNode != null)
        {
            if (actualNode != null)
            {
                // Pozor prvy krat sa toto vykona vzdy (porovnava so samym sebou)
                // Nove minimum?
                if (actualNode.Key.CompareTo(foundMinimalNode!.Key, targetDimension) < 0)
                {
                    foundMinimalNode = actualNode;
                    foundMinimalNodeDimension = actualDimension;
                }
                
                stack.Push(actualNode, actualDimension);
                actualNode = actualNode.LeftNode;
                actualDimension = ++actualDimension % _numberOfDimension;
            }
            else
            {
                (actualNode, actualDimension) = stack.Pop();
                
                // Ak sa nachadzam na leveli targetDimension tak koncim => do prava ISTO nemusim ist
                if (actualDimension == targetDimension)
                {
                    actualNode = null;
                }
                else
                {
                    if (actualNode.RightNode == null)
                    {
                        actualNode = null;
                    }
                    else
                    {
                        actualNode = actualNode.RightNode;
                        actualDimension = ++actualDimension % _numberOfDimension;
                    }
                }
            }
        }
        
        actualDimension = foundMinimalNodeDimension;
        return foundMinimalNode!;
    }
    
    // TODO: Urcite pretestovat - nemal som cas teraz testovat
    private KDTreeNode<TKey, TData> FindMaximumBy(int targetDimension, KDTreeNode<TKey, TData>? actualNode, out int actualDimension)
    {
        var foundMaximalNode = actualNode;
        actualDimension = (targetDimension + 1) % _numberOfDimension;
        var foundMaximalNodeDimension = actualDimension;
        
        var stack = new KDTreeDimensionalStack<TKey, TData>();
        
        while (stack.Any() || actualNode != null)
        {
            if (actualNode != null)
            {
                // Pozor prvy krat sa toto vykona vzdy (porovnava so samym sebou)
                // Nove maximum?
                if (actualNode.Key.CompareTo(foundMaximalNode!.Key, targetDimension) > 0)
                {
                    foundMaximalNode = actualNode;
                    foundMaximalNodeDimension = actualDimension;
                }
                
                stack.Push(actualNode, actualDimension);
                actualNode = actualNode.RightNode;
                actualDimension = ++actualDimension % _numberOfDimension;
            }
            else
            {
                (actualNode, actualDimension) = stack.Pop();
                
                // Ak sa nachadzam na leveli targetDimension tak koncim => do lava ISTO nemusim ist
                if (actualDimension == targetDimension)
                {
                    actualNode = null;
                }
                else
                {
                    if (actualNode.LeftNode == null)
                    {
                        actualNode = null;
                    }
                    else
                    {
                        actualNode = actualNode.LeftNode;
                        actualDimension = ++actualDimension % _numberOfDimension;
                    }
                }
            }
        }
        
        actualDimension = foundMaximalNodeDimension;
        return foundMaximalNode!;
    }
    
    public void ExecuteInOrder(Action<List<TData>> actionToExec)
    {
        ExecuteInOrder(node =>
        {
            actionToExec(node.Data);
        });
    }
    
    public void ExecuteInOrder(Action<TKey> actionToExec)
    {
        ExecuteInOrder(node =>
        {
            actionToExec(node.Key);
        });
    }

    private void ExecuteInOrder(Action<KDTreeNode<TKey, TData>> actionToExec)
    {
        if (_root == null)
        {
            return;
        }

        var stack = new List<KDTreeNode<TKey, TData>>();

        var currentNode = _root;
        
        
        while (stack.Any() || currentNode != null)
        {
            if (currentNode != null)
            {
                stack.Add(currentNode);
                currentNode = currentNode.LeftNode;
            }
            else
            {
                currentNode = stack[stack.Count - 1];
                stack.RemoveAt(stack.Count - 1);

                actionToExec(currentNode);

                currentNode = currentNode.RightNode;
            }
        }
        
    }
}
