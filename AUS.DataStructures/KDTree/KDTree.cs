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
        
        
        // Vrchol nie je listom => nahrad ho najmensim/najvacsim v pravom/lavom podstrome podla poslednej dimenzie
        
        // 1. FAZA - Odstranenie najdeneho uzla a rekurzivne hladanie nahradnika (min/max) az po odstranenie listu
        
        // Zoznam nodov, ktore musime tiez zmazat a opatovne pridat (LEBO DUPLICITY)
        var nodesToDelete = new List<KDTreeNodeWithDimension<TKey, TData>>();
        
        // Zoznam nodov, ktore musime nasledne opatovne pridat nanovo
        var nodesToAdd = new List<(TKey, List<TData>)>();
        

        // Proces bude bezat pokial najdeny nahradnik (min) nie je listom
        while (foundNode != null || nodesToDelete.Any())
        {
            if (foundNode == null)
            {
                // 2. FAZA - mazanie foundNodu sa dokoncilo, teraz postupne odstranujeme nody zo zoznamu nodesToDelete
                // TODO: Otazka optimalizacie - nebrat radsej prvky od konca lebo je vacsia pravdepodobnost ze su blizsie k listu?
                foundNode = nodesToDelete[0].Node;
                lastDimenstion = nodesToDelete[0].Dimension;
                
                nodesToDelete.RemoveAt(0);
                
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
            
            if (foundNode.RightNode != null)
            {
                // Pravy podstrom je k dispozici => hladame v pravom podstrome minimum podla lastDimension
                replacementNodes = FindMinimumBy(lastDimenstion, foundNode.RightNode);
                //replacementNode = FindMinimumBy(lastDimenstion, foundNode.RightNode, out lastDimenstion);
            }
            else
            {
                // Pravy podstrom nie je => hladame v lavom podstrome maximum podla lastDimension
                replacementNodes = FindMaximumBy(lastDimenstion, foundNode.LeftNode);
                //replacementNode = FindMaximumBy(lastDimenstion, foundNode.LeftNode, out lastDimenstion);
            }
            
            var selectedReplacementNode = replacementNodes[0];
            
            // Premiestnenie najdeneho nahradnika na miesto vymazaneho uzla (v tomto momente mazaneho uzla)
            foundNode.Key = selectedReplacementNode.Node.Key;
            foundNode.Data = selectedReplacementNode.Node.Data;
            
            replacementNodes[0] = new(foundNode, lastDimenstion);
            
            // Ak je v replacementNodes viac prvkov tak ich je treba zaznamenat do nodesToDelete
            // pozor: prvky sa tu uz mozu vyskytovat preto treba skontrolovat ci su uz tam a ak ano tak iba aktualizovat ich dimenziu
            foreach (var replacementNode in replacementNodes)
            {
                var foundNodeIndex = nodesToDelete.FindIndex(x => x.Node.Key.Equals(replacementNode.Node.Key));
                
                if (foundNodeIndex == -1)
                {
                    // pridame iba ak je ich viac (cize duplicita)
                    if (replacementNodes.Count > 1)
                    {
                        nodesToDelete.Add(replacementNode);
                    }
                }
                else
                {
                    //nodesToDelete[foundNodeIndex].Dimension = replacementNode.Dimension;
                    // TODO: Otázka z hľadiska optimalizácie - je táto aktualizácia vždy nutná? nestačí iba pre selektnutý node aktualizovať node a referenciu?
                    nodesToDelete[foundNodeIndex] = new(replacementNode.Node, replacementNode.Dimension);
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
    
    private List<KDTreeNodeWithDimension<TKey, TData>> FindMaximumBy(int targetDimension, KDTreeNode<TKey, TData>? actualNode)
    {
        var actualDimension = (targetDimension + 1) % _numberOfDimension;
        
        var foundMaximalNodes = new List<KDTreeNodeWithDimension<TKey, TData>>();
        
        var stack = new KDTreeDimensionalStack<TKey, TData>();
        
        while (stack.Any() || actualNode != null)
        {
            if (actualNode != null)
            {
                if (!foundMaximalNodes.Any())
                {
                    foundMaximalNodes.Add(new(actualNode, actualDimension));
                }
                else
                {
                    var compareResult = actualNode.Key.CompareTo(foundMaximalNodes[0].Node.Key, targetDimension);
                    
                    // Nove maximum?
                    if (compareResult > 0)
                    {
                        foundMaximalNodes.Clear();
                        foundMaximalNodes.Add(new(actualNode, actualDimension));
                    }
                    else if (compareResult == 0)
                    {
                        foundMaximalNodes.Add(new(actualNode, actualDimension));
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
        
        return foundMaximalNodes;
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
