using AUS.DataStructures.KDTree;

namespace AUS.DataStructures.GeoArea;

public record GPSCoordinate(double X, double Y) : IKDTreeKeyComparable<GPSCoordinate>
{
    private const double Epsilon = 0.00000001;
    
    public int CompareTo(GPSCoordinate another, int dimension)
    {
        if (dimension == 0)
        {
            var difference = X - another.X;
            
            if (difference > Epsilon)
            {
                return 1;
            }
            
            if (difference < -Epsilon)
            {
                return -1;
            }
            
            return 0;
        }
        
        if (dimension == 1)
        {
            var difference = Y - another.Y;
            
            if (difference > Epsilon)
            {
                return 1;
            }
            
            if (difference < -Epsilon)
            {
                return -1;
            }
            
            return 0;
        }

        throw new ArgumentException("Allowed dimensions are only 0 and 1");
    }

    public override string ToString()
    {
        return $"[{X}; {Y}]";
    }
}
