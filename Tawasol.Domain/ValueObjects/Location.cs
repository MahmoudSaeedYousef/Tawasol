using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.ValueObjects;

public class Location
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    // Required for EF Core
    private Location() { }

    public Location(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new DomainException("Latitude must be between -90 and 90.");
        if (longitude < -180 || longitude > 180)
            throw new DomainException("Longitude must be between -180 and 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    public double CalculateDistanceInMeters(Location other)
    {
        var d1 = Latitude * (Math.PI / 180.0);
        var num1 = Longitude * (Math.PI / 180.0);
        var d2 = other.Latitude * (Math.PI / 180.0);
        var num2 = other.Longitude * (Math.PI / 180.0) - num1;
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                 Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
    }
}
