using Galaxon.Numerics.Geometry;
using Galaxon.Quantities;

namespace Galaxon.Astronomy.Algorithms;

public static class Solar
{
    /// <summary>
    /// Calculate apparent solar latitude and longitude for a given instant
    /// specified as a Julian Day in Terrestrial Time (this is also known as a
    /// Julian Ephemeris Day or JDE, and differs from the Julian Day by ∆T).
    /// This method uses the higher accuracy algorithm from AA2 Ch25 p166
    /// (p174 in PDF)
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns>The longitude of the Sun (Ls) in radians at the given
    /// instant.</returns>
    public static (double Lng, double Lat) CalcPosition(double jdtt)
    {
        // Get the Earth's heliocentric position.
        (double lngEarth, double latEarth, double R_m) = Terran.CalcPosition(jdtt);

        // Reverse to get the mean dynamical ecliptic and equinox of the date.
        double lngSun = Angle.NormalizeRadians(lngEarth + PI);
        double latSun = -latEarth;

        // Convert to FK5.
        // This gives the true ("geometric") longitude of the Sun referred to the
        // mean equinox of the date.
        double julCen = Terran.JulianCenturiesSinceJ2000(jdtt);
        double lambdaPrime = lngSun - Angle.DegToRad(1.397) * julCen
            - Angle.DegToRad(0.000_31) * julCen * julCen;
        lngSun -= Angle.DmsToRad(0, 0, 0.090_33);
        latSun += Angle.DmsToRad(0, 0, 0.039_16) * (Cos(lambdaPrime) - Sin(lambdaPrime));

        // To obtain the apparent longitude, nutation and aberration have to be
        // taken into account.
        double nutLng = SOFA.iauNut06a(jdtt).dpsi;
        lngSun += nutLng;

        // Calculate and add aberration.
        double julMil = julCen / 10;
        double julMil2 = julMil * julMil;
        double dLambda_as = 3548.193
            + 118.568 * Angle.SinDeg(87.5287 + 359_993.7286 * julMil)
            + 2.476 * Angle.SinDeg(85.0561 + 719_987.4571 * julMil)
            + 1.376 * Angle.SinDeg(27.8502 + 4452_671.1152 * julMil)
            + 0.119 * Angle.SinDeg(73.1375 + 450_368.8564 * julMil)
            + 0.114 * Angle.SinDeg(337.2264 + 329_644.6718 * julMil)
            + 0.086 * Angle.SinDeg(222.5400 + 659_289.3436 * julMil)
            + 0.078 * Angle.SinDeg(162.8136 + 9224_659.7915 * julMil)
            + 0.054 * Angle.SinDeg(82.5823 + 1079_981.1857 * julMil)
            + 0.052 * Angle.SinDeg(171.5189 + 225_184.4282 * julMil)
            + 0.034 * Angle.SinDeg(30.3214 + 4092_677.3866 * julMil)
            + 0.033 * Angle.SinDeg(119.8105 + 337_181.4711 * julMil)
            + 0.023 * Angle.SinDeg(247.5418 + 299_295.6151 * julMil)
            + 0.023 * Angle.SinDeg(325.1526 + 315_559.5560 * julMil)
            + 0.021 * Angle.SinDeg(155.1241 + 675_553.2846 * julMil)
            + 7.311 * julMil * Angle.SinDeg(333.4515 + 359_993.7286 * julMil)
            + 0.305 * julMil * Angle.SinDeg(330.9814 + 719_987.4571 * julMil)
            + 0.010 * julMil * Angle.SinDeg(328.5170 + 1079_981.1857 * julMil)
            + 0.309 * julMil2 * Angle.SinDeg(241.4518 + 359_993.7286 * julMil)
            + 0.021 * julMil2 * Angle.SinDeg(205.0482 + 719_987.4571 * julMil)
            + 0.004 * julMil2 * Angle.SinDeg(297.8610 + 4452_671.1152 * julMil)
            + 0.010 * julMil2 * Angle.SinDeg(154.7066 + 359_993.7286 * julMil);
        double dLambda_rad = dLambda_as * Angle.RadiansPerArcsecond;
        double R_AU = R_m / Length.MetresPerAu;
        double aberration = -0.005_775_518 * R_AU * dLambda_rad;
        lngSun += aberration;

        return (lngSun, latSun);
    }

    /// <summary>
    /// Calculate apparent solar longitude for a given instant specified as a
    /// DateTime (UT).
    /// </summary>
    /// <param name="dtut">The instant specified as a DateTime (UT).</param>
    /// <returns>The longitude and latitude of the Sun, in radians, at the given
    /// instant.</returns>
    public static (double Lng, double Lat) CalcPosition(DateTime dtut) =>
        CalcPosition(Terran.DateTimeUniversalToJulianDayTerrestrial(dtut));
}
