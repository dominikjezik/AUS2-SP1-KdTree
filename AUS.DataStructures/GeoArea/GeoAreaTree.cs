using AUS.DataStructures.KDTree;

namespace AUS.DataStructures.GeoArea;

public class GeoAreaTree : KDTree<double, AreaObject>
{
    public GeoAreaTree() : base(2) { }

    public List<AreaObject> FindByCoordinates(double x, double y)
    {
        var result = new List<AreaObject>();

        if (_root == null)
        {
            return result;
        }

        // Pouzitie FindByKeys by neslo lebo to je priamo bodove vyhlavanie (musela by sa trafit suradnica A)

        TryFindNode([x, y], out var foundNode, out var _);

        // v cykle pre kazdy uzol smerom k rootu sa skontroluje či sa zadana suradnica nenachadza v areaobjekte a takisto v asociovaných areaobjektoch

        while (foundNode != null)
        {
            // Kontrola objektov vložených v danom uzle (cyklus ak su duplicitne kluce)
            foreach (var area in foundNode.Data)
            {
                if (area.ContainsCoordinate(x, y))
                {
                    result.Add(area);
                }
            }

            // Este nutna kontrola v asociovaných objektov v danom uzle (znova cez cyklus)
            // TODO
        }

        return result;
    }

    public void Insert(AreaObject areaObject)
    {
        double[] keys = [areaObject.CoordinateA.X, areaObject.CoordinateA.Y];

        if (_root == null)
        {
            _root = new KDTreeNode<double, AreaObject>(keys, areaObject);
            return;
        }

        var isFound = TryFindNode(keys, out var foundNode, out var lastDimenstion);

        if (isFound)
        {
            foundNode!.Data.Add(areaObject);

            AfterInsertBacktrack(foundNode);
            return;
        }

        var newNode = new KDTreeNode<double, AreaObject>(keys, areaObject);

        // Nenasiel sa cize vo foundNode je posledny uzol, kde skoncilo hladanie
        if (keys[lastDimenstion].CompareTo(foundNode!.Keys[lastDimenstion]) <= 0)
        {
            // Vlozenie do lava
            foundNode.LeftNode = newNode;
            newNode.ParentNode = foundNode.LeftNode;
        }
        else
        {
            // Vlozenie do prava
            foundNode.RightNode = newNode;
            newNode.ParentNode = foundNode.RightNode;
        }

        AfterInsertBacktrack(newNode);
    }

    private void AfterInsertBacktrack(KDTreeNode<double, AreaObject> insertedNode)
    {
        // Po vlozeny areaObjektu
        // Začne sa od aktuálne voženého uzla a postupne sa prechádza smerom ku koreňu
        // V každom uzle sa kontroluje či vložený AreaObject nekoliduje s objektami vloženými v prechádzanom uzle
        // - Ak koliduje poznačí sa do kolidovanej aj vloženej AreaObject (je tam zoznam prekryvajucich sa objektov - zadanie)
        // Súčasne sa kontroluje či CoordinateB spĺňa podmienky aktuálne prechádzaného uzlu
        // - Ak nespĺňa požiadavky je nutné prejsť cely podstrom opačnej vetvy
        // - Otázne je či vykonávať okamžite (myslím si že nie) alebo si to stačí poznačiť do premennej, ktorá bude prípadne pri ďalšom prechádzaní ku koreňu
        // - Keď sa dostaneme až ku koreňu tak spustiť prehľadávanie podstromu pre uložený uzol v danej premennej

        // Pri prechádzaní smerom na hor je potrebné zaznačovať vkladaný objekt nie len do AreaObjectu s ktorým sa prekrýva
        // ale tiež do nodu poznačiť do asociovaných objektov (pravdepodobne tiež nie hneď ale v premennej čo najvyššie v strome a zápis vykonanť až po tom ako sa dostaneme do root-u)

        /*
         * V podstate pri behu prechádzania:
         * - kontrola ci sa nová area neprekrýva s prechádzanou (tu je celý zoznam asociovaných objektov tak s každým) ak áno vložiť do zoznamu prekrývajúcich sa objektov
         * - ak CoordinateB nespĺňa inicializuje sa premenná z NULL na daný uzol a pokračuje sa (otázka z vrchu či spustiť hneď prehladavanie nad pod stromom skôr nie)
         * - po prejdení až do root-u a jeho spraocvanie kontrola premennej => ak je NULL koniec
         * - inak:
         *   1. zaznač asociované objekty do daného uzla (podobná situácia ako Quad strom na prednáške s priamkou)
         *   2. prejsť celý podstrom a skontrolovať všetky objekty či sa neprekrývajú s vloženým objektom ak áno do daného sa zaznačí prekrývanie
         */
    }
}
