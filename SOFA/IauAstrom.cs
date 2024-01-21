using System.Runtime.InteropServices;

namespace Galaxon.Astronomy.SOFA;

[StructLayout(LayoutKind.Sequential)]
public struct IauAstrom
{
    public double pmt; // PM time interval (SSB, Julian years)

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public double[] eb; // SSB to observer (vector, au)

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public double[] eh; // Sun to observer (unit vector)

    public double em; // distance from Sun to observer (au)

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public double[] v; // barycentric observer velocity (vector, c)

    public double bm1; // sqrt(1-|v|^2): reciprocal of Lorenz factor

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
    public double[] bpn; // bias-precession-nutation matrix

    public double along; // longitude + s' + dERA(DUT) (radians)

    public double phi; // geodetic latitude (radians)

    public double xpl; // polar motion xp wrt local meridian (radians)

    public double ypl; // polar motion yp wrt local meridian (radians)

    public double sphi; // sine of geodetic latitude

    public double cphi; // cosine of geodetic latitude

    public double diurab; // magnitude of diurnal aberration vector

    public double eral; // "local" Earth rotation angle (radians)

    public double refa; // refraction constant A (radians)

    public double refb; // refraction constant B (radians)

    // Constructor to initialize arrays
    public IauAstrom()
    {
        eb = new double[3];
        eh = new double[3];
        v = new double[3];
        bpn = new double[9]; // 3x3 matrix flattened into an array
        // Initialize other fields as necessary
        pmt = em = bm1 = along = phi = xpl = ypl = sphi = cphi = diurab = eral = refa = refb = 0;
    }
}
