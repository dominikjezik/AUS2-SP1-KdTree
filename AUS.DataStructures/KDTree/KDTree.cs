namespace AUS.DataStructures.KDTree;

public class KDTree<TKey, TData> where TKey : IKDTreeKeyComparable<TKey>
{
    private readonly int _numberOfDimension;

    private KDTreeNode<TKey, TData>? _root;

    public KDTree(int numberOfDimension)
    {
        _numberOfDimension = numberOfDimension;
    }

    public void Clear()
    {
        _root = null;
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
        var isFound = TryFindNode(keyForSearch, out var foundNode, out _);

        if (!isFound)
        {
            return [];
        }
        
        return new List<TData>(foundNode!.Data);
    }
    
    public bool Contains(TKey keyForSearch)
    {
        return TryFindNode(keyForSearch, out _, out _);
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
    
    public void Insert(TKey keyForInsert, List<TData> data)
    {
        if (_root == null)
        {
            _root = new KDTreeNode<TKey, TData>(keyForInsert, data);
            return;
        }

        var isFound = TryFindNode(keyForInsert, out var foundNode, out var lastDimenstion);

        if (isFound)
        {
            foundNode!.Data.AddRange(data);
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

    public void Insert(TKey keyForInsert)
    {
        if (_root == null)
        {
            _root = new KDTreeNode<TKey, TData>(keyForInsert);
            return;
        }

        var isFound = TryFindNode(keyForInsert, out var foundNode, out var lastDimenstion);

        if (isFound)
        {
            return;
        }

        // Nenasiel sa cize vo foundNode je posledny uzol, kde skoncilo hladanie
        if (keyForInsert.CompareTo(foundNode!.Key, lastDimenstion) <= 0)
        {
            // Vlozenie do lava
            var newNode = new KDTreeNode<TKey, TData>(keyForInsert);
            foundNode.LeftNode = newNode;
            newNode.ParentNode = foundNode;
        }
        else
        {
            // Vlozenie do prava
            var newNode = new KDTreeNode<TKey, TData>(keyForInsert);
            foundNode.RightNode = newNode;
            newNode.ParentNode = foundNode;
        }
    }

    public TData? Delete(TKey keyForDelete, TData dataWithInternalId)
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
            // Vyhladavame na zaklade metody Equals
            var dataForDelete = foundNode.Data.Find(x => x.Equals(dataWithInternalId));
            
            if (dataForDelete != null)
            {
                foundNode.Data.Remove(dataForDelete);
            }
            else
            {
                throw new InvalidOperationException("Item with given key and data not found!");
            }

            return dataForDelete;
        }

        var dataThatWillBeDeleted = foundNode.Data.FirstOrDefault();
        
        // Node obsahuje iba jeden prvok cize ideme mazat cely node
        Delete(foundNode, lastDimenstion);

        return dataThatWillBeDeleted;
    }

    private void Delete(KDTreeNode<TKey, TData> foundNode, int lastDimenstion)
    {
        // 1. FAZA - Odstranenie najdeneho uzla a rekurzivne hladanie nahradnika (min/max) az po odstranenie listu
        
        // Zoznam nodov, ktore musime tiez zmazat a opatovne pridat (LEBO DUPLICITY)
        var nodesToDelete = new LinkedList<KDTreeNodeWithDimension<TKey, TData>>();
        
        // Zoznam nodov, ktore musime nasledne opatovne pridat nanovo
        var nodesToAdd = new List<(TKey, List<TData>)>();

        // Proces bude bezat pokial najdeny nahradnik nie je listom alebo pokial je nieco v zozname na delete
        while (foundNode != null || nodesToDelete.Any())
        {
            if (foundNode == null)
            {
                // 2. FAZA - mazanie foundNodu sa dokoncilo, teraz postupne odstranujeme nody zo zoznamu nodesToDelete
                foundNode = nodesToDelete.First!.Value.Node;
                lastDimenstion = nodesToDelete.First!.Value.Dimension;
                
                nodesToDelete.RemoveFirst();
                
                // Zaznamename node aby sme ho mohli neskor znova pridat
                nodesToAdd.Add((foundNode.Key, foundNode.Data));
            }
            
            
            // Trivialna situacia ked node je list
            if (foundNode.LeftNode == null && foundNode.RightNode == null)
            {
                if (foundNode.ParentNode == null)
                {
                    _root = null;
                    
                    // --- KONIEC 1. FAZY / 2.B FAZY ---
                    foundNode = null;
                    lastDimenstion = -1;
                    
                    continue;
                }
            
                if (foundNode.ParentNode.LeftNode == foundNode)
                {
                    foundNode.ParentNode.LeftNode = null;
                }
                else
                {
                    foundNode.ParentNode.RightNode = null;
                }
            
                foundNode.ParentNode = null;
                
                // --- KONIEC 1. FAZY / 2.B FAZY ---
                foundNode = null;
                lastDimenstion = -1;
                
                continue;
            }
            
            
            List<KDTreeNodeWithDimension<TKey, TData>> replacementNodes;

            if (foundNode.LeftNode != null)
            {
                // Lavý podstrom je k dispozicii => hladame v lavom podstrome maximum podla lastDimension
                var replacementNode = FindMaximumBy(lastDimenstion, foundNode.LeftNode);
                replacementNodes = [ replacementNode ];
            }
            else
            {
                // Lavý podstrom nie je => hladame v pravom podstrome minimum podla lastDimension
                // Chceme aj vsetky duplicity podla daneho kluca
                replacementNodes = FindMinimumBy(lastDimenstion, foundNode.RightNode);
            }
            
            var selectedReplacementNode = replacementNodes[0];
            
            // Premiestnenie najdeneho nahradnika na miesto vymazaneho uzla (v tomto momente mazaneho uzla)
            foundNode.Key = selectedReplacementNode.Node.Key;
            foundNode.Data = selectedReplacementNode.Node.Data;
            
            replacementNodes[0] = new(foundNode, lastDimenstion);
            
            
            
            // Kontrola ci je nahradnik (replacementNodes[0]) uz v nodesToDelete
            // => ak ano treba ho aktualizovat (zmena dimenzie)

            var firstReplacementNode = replacementNodes[0];
            
            var iteratedLinkedListNode = nodesToDelete.First;
               
            while (iteratedLinkedListNode != null)
            {
                if (iteratedLinkedListNode.Value.Node.Key.Equals(firstReplacementNode.Node.Key))
                {
                    iteratedLinkedListNode.Value = new(firstReplacementNode.Node, firstReplacementNode.Dimension);
                    break;
                }
                    
                iteratedLinkedListNode = iteratedLinkedListNode.Next;
            }
               
            // Ak sa nenasiel node v nodesToDelete a su duplicity tak ho pridame 
            if (iteratedLinkedListNode == null && replacementNodes.Count > 1)
            {
                nodesToDelete.AddLast(firstReplacementNode);
            }
            
            
            // V pripade duplicitnych prvokv ich pridame do nodesToDelete
            foreach (var replacementNode in replacementNodes.Skip(1))
            {
                if (!nodesToDelete.Any(x => x.Node.Key.Equals(replacementNode.Node.Key)))
                {
                    nodesToDelete.AddLast(replacementNode);
                }
            }
            
            
            
            // Ak je nahradnik listom tak ho vymazeme (uplne)
            if (selectedReplacementNode.Node.LeftNode == null && selectedReplacementNode.Node.RightNode == null)
            {
                if (selectedReplacementNode.Node.ParentNode!.LeftNode == selectedReplacementNode.Node)
                {
                    selectedReplacementNode.Node.ParentNode.LeftNode = null;
                }
                else
                {
                    selectedReplacementNode.Node.ParentNode.RightNode = null;
                }
                
                selectedReplacementNode.Node.ParentNode = null;
                
                // --- KONIEC 1. FAZY / 2.B FAZY ---
                foundNode = null;
                lastDimenstion = -1;
                
                continue;
            }
            
            // Ak replacementNode nie je listom tak pokracujeme vymazavanim nodu kde bolo najdene minimum/maximum
            foundNode = selectedReplacementNode.Node;
            lastDimenstion = selectedReplacementNode.Dimension;
        }
        
        
        // 3. FAZA - pridanie nodov z nodesToAdd
        foreach (var nodeToAdd in nodesToAdd)
        {
            Insert(nodeToAdd.Item1, nodeToAdd.Item2);
        }
    }
    
    private List<KDTreeNodeWithDimension<TKey, TData>> FindMinimumBy(int targetDimension, KDTreeNode<TKey, TData>? actualNode)
    {
        var actualDimension = (targetDimension + 1) % _numberOfDimension;
        
        var foundMinimalNodes = new List<KDTreeNodeWithDimension<TKey, TData>>();
        
        var stack = new KDTreeDimensionalStack<TKey, TData>();
        
        while (stack.Any() || actualNode != null)
        {
            if (actualNode != null)
            {
                if (!foundMinimalNodes.Any())
                {
                    foundMinimalNodes.Add(new(actualNode, actualDimension));
                }
                else
                {
                    var compareResult = actualNode.Key.CompareTo(foundMinimalNodes[0].Node.Key, targetDimension);
                    
                    // Nove minimum?
                    if (compareResult < 0)
                    {
                        foundMinimalNodes.Clear();
                        foundMinimalNodes.Add(new(actualNode, actualDimension));
                    }
                    else if (compareResult == 0)
                    {
                        foundMinimalNodes.Add(new(actualNode, actualDimension));
                    }
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
        
        return foundMinimalNodes;
    }
    
    private KDTreeNodeWithDimension<TKey, TData> FindMaximumBy(int targetDimension, KDTreeNode<TKey, TData>? actualNode)
    {
        var actualDimension = (targetDimension + 1) % _numberOfDimension;
        
        KDTreeNodeWithDimension<TKey, TData>? foundMaximalNode = null;
        
        var stack = new KDTreeDimensionalStack<TKey, TData>();
        
        while (stack.Any() || actualNode != null)
        {
            if (actualNode != null)
            {
                if (foundMaximalNode == null)
                {
                    foundMaximalNode = new(actualNode, actualDimension);
                }
                else
                {
                    var compareResult = actualNode.Key.CompareTo(foundMaximalNode.Node.Key, targetDimension);
                    
                    // Nove maximum?
                    if (compareResult > 0)
                    {
                        foundMaximalNode = new(actualNode, actualDimension);
                    }
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
    
    public void ExecuteLevelOrder(Action<TKey, List<TData>> actionToExec)
    {
        if (_root == null)
        {
            return;
        }
        
        var queue = new LinkedList<KDTreeNode<TKey, TData>>();
        
        queue.AddLast(_root);

        while (queue.Any())
        {
            var currentNode = queue.First!.Value;
            queue.RemoveFirst();
            
            actionToExec(currentNode.Key, currentNode.Data);
            
            if (currentNode.LeftNode != null)
            {
                queue.AddLast(currentNode.LeftNode);
            }
            
            if (currentNode.RightNode != null)
            {
                queue.AddLast(currentNode.RightNode);
            }
        }
    }
}
