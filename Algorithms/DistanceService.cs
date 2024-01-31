using Galaxon.Astronomy.Data.Models;
using Galaxon.Numerics.Geometry;
using GeoCoordinatePortable;

namespace Galaxon.Astronomy.Algorithms;

public static class DistanceService
{
    /// <summary>
    /// Calculate shortest distance between two points along the surface of a
    /// oblate spheroid world using Andoyer's method.
    /// The formula is from Astronomical Algorithms 2nd ed. by Jeen Meeus, p85.
    /// Unlike the Haversine formula, which assumes a spherical body, Andoyer's
    /// method takes flattening into account.
    /// Although you can provide radiuses for other worlds, this algorithm was
    /// designed for Earth and therefore assumes:
    /// (a) the object is an oblate spheroid;
    /// (b) coordinates are given in degrees;
    /// (c) the usual coordinate system is used, i.e. latitude is in the range
    /// -90..90, and longitude is in the range -180..180
    /// Altitude is ignored.
    /// </summary>
    /// <param name="location1">The geographical coordinates of location 1.</param>
    /// <param name="location2">The geographical coordinates of location 2.</param>
    /// <param name="radiusEquat">The equatorial radius in kilometres.</param>
    /// <param name="radiusPolar">The polar radius in kilometres.</param>
    /// <returns>The distance between the two locations in kilometres.</returns>
    public static double ShortestDistanceBetween(GeoCoordinate location1,
        GeoCoordinate location2, double radiusEquat, double radiusPolar)
    {
        // Calculate the flattening.
        double f = (radiusEquat - radiusPolar) / radiusEquat;

        double F = Angle.DegToRad((location1.Latitude + location2.Latitude) / 2);
        double sin2F = Angle.Sin2(F);
        double cos2F = Angle.Cos2(F);

        double G = Angle.DegToRad((location1.Latitude - location2.Latitude) / 2);
        double sin2G = Angle.Sin2(G);
        double cos2G = Angle.Cos2(G);

        double lambda = Angle.DegToRad((location1.Longitude - location2.Longitude) / 2);
        double sin2Lambda = Angle.Sin2(lambda);
        double cos2Lambda = Angle.Cos2(lambda);

        double S = sin2G * cos2Lambda + cos2F * sin2Lambda;
        double C = cos2G * cos2Lambda + sin2F * sin2Lambda;

        double omega = Atan(Sqrt(S / C));
        double R = Sqrt(S * C) / omega;
        double D = 2 * omega * radiusEquat;
        double H1 = (3 * R - 1) / 2 / C;
        double H2 = (3 * R + 1) / 2 / S;
        return D * (1 + f * H1 * sin2F * cos2G - f * H2 * cos2F * sin2G);
    }

    public static double ShortestDistanceBetween(GeoCoordinate location1, GeoCoordinate location2,
        AstroObject astroObj)
    {
        return ShortestDistanceBetween(location1, location2, astroObj.Physical!.EquatorialRadius,
            astroObj.Physical.PolarRadius);
    }
}
