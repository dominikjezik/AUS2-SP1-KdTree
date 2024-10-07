using AUS.DataStructures.KDTree;

namespace AUS.DataStructures.GeoArea;

public record GPSCoordinate(double X, double Y) : IKDTreeKeyComparable<GPSCoordinate>
{
    public int CompareTo(GPSCoordinate another, int dimension)
    {
        if (dimension == 0)
        {
            if (X < another.X)
            {
                return -1;
            }

            if (X > another.X)
            {
                return 1;
            }

            return 0;
        }
        
        if (dimension == 1)
        {
            if (Y < another.Y)
            {
                return -1;
            }

            if (Y > another.Y)
            {
                return 1;
            }

            return 0;
        }

        throw new ArgumentException("Allowed dimensions are only 0 and 1");
    }
}
