using System.Text;
using AUS.DataStructures.KDTree;

namespace AUS.Tester;

public record KDTreeCompositeDataTypesKey : IKDTreeKeyComparable<KDTreeCompositeDataTypesKey>, IKDTreeTesterKey<KDTreeCompositeDataTypesKey>
{
    private const double Epsilon = 0.00000001;
    
    public int NumberOfDimension => 4;

    public double A { get; private set; }
    
    public string B { get; private set; }
    
    public int C { get; private set; }
    
    public double D { get; private set; }

    public KDTreeCompositeDataTypesKey()
    {
    }
    
    public KDTreeCompositeDataTypesKey(double a, string b, int c, double d)
    {
        A = a;
        B = b;
        C = c;
        D = d;
    }
    
    public virtual bool Equals(KDTreeCompositeDataTypesKey? other)
    {
        if (other == null)
        {
            return false;
        }
        
        var aDifference = A - other.A;
        
        if (aDifference > Epsilon || aDifference < -Epsilon)
        {
            return false;
        }
        
        if (B != other.B)
        {
            return false;
        }
        
        if (C != other.C)
        {
            return false;
        }
        
        var dDifference = D - other.D;
        
        if (dDifference > Epsilon || dDifference < -Epsilon)
        {
            return false;
        }
        
        return true;
    }

    public int CompareTo(KDTreeCompositeDataTypesKey another, int dimension)
    {
        if (dimension == 0)
        {
            var aDifference = A - another.A;
            
            if (aDifference > Epsilon)
            {
                return 1;
            }
            
            if (aDifference < -Epsilon)
            {
                return -1;
            }
            
            return B.CompareTo(another.B);
        }
        
        if (dimension == 1)
        {
            return C.CompareTo(another.C);
        }
        
        if (dimension == 2)
        {
            var dDifference = D - another.D;
            
            if (dDifference > Epsilon)
            {
                return 1;
            }
            
            if (dDifference < -Epsilon)
            {
                return -1;
            }
            
            return 0;
        }
        
        // 3-tia dimenzia
        var bComparison = B.CompareTo(another.B);
        
        if (bComparison != 0)
        {
            return bComparison;
        }
        
        return C.CompareTo(another.C);
    }

    public override string ToString()
    {
        return $"[{A}; {B}; {C}; {D}]";
    }
    
    public KDTreeCompositeDataTypesKey GenerateRandom(Random random)
    {
        var a = random.NextDouble();
        
        var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
        
        var numberOfCharacters = random.Next(5, 20);
        
        var bStringBuilder = new StringBuilder();
        
        for (int i = 0; i < numberOfCharacters; i++)
        {
            var index = random.Next(0, characters.Length);
            bStringBuilder.Append(characters[index]);
        }
        
        var b = bStringBuilder.ToString();
        var c = random.Next();
        var d = random.NextDouble();
        
        return new KDTreeCompositeDataTypesKey(a, b, c, d);
    }
}
