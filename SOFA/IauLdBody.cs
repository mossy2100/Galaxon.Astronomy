using System.Runtime.InteropServices;

namespace Galaxon.Astronomy.SOFA;

[StructLayout(LayoutKind.Sequential)]
public struct IauLdBody
{
    public double bm; // mass of the body (solar masses)

    public double dl; // deflection limiter (radians^2/2)

    // Since this is a 2x3 matrix, we flatten it into a 1D array of length 6.
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public double[] pv; // barycentric PV of the body (au, au/day)

    // Constructor to initialize fields.
    public IauLdBody()
    {
        bm = dl = 0;
        pv = new double[6];
    }
}
