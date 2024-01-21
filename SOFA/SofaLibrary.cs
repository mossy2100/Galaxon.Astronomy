using System.Runtime.InteropServices;

namespace Galaxon.Astronomy.SOFA;

public class SofaLibrary
{
    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauA2af(int ndp, double angle, out char sign, out int[] idmsf);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauA2tf(int ndp, double angle, out char sign, out int[] ihmsf);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAb(double[] pnat, double[] v, double s, double bm1,
        out double[] ppr);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAe2hd(double az, double el, double phi, out double ha,
        out double dec);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern int iauAf2a(char s, int ideg, int iamin, double asec, out double rad);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern double iauAnp(double a);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern double iauAnpm(double a);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauApcg(double date1, double date2, double[][] ebpv, double[] ehp,
        out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauApcg13(double date1, double date2, out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauApci(double date1, double date2, double[][] ebpv,
        double[] ehp, double x, double y, double s, out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauApci13(double date1, double date2, out IauAstrom astrom,
        out double eo);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauApco(double date1, double date2, double[][] ebpv, double[] ehp,
        double x, double y, double s, double theta, double elong, double phi, double hm, double xp,
        double yp, double sp, double refa, double refb, out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern int iauApco13(double utc1, double utc2, double dut1, double elong,
        double phi, double hm, double xp, double yp, double phpa, double tc, double rh, double wl,
        out IauAstrom astrom, out double eo);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauApcs(double date1, double date2, double[][] pv, double[][] ebpv,
        double[] ehp, out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauApcs13(double date1, double date2, double[][] pv,
        out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAper(double theta, out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAper13(double ut11, double ut12, out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauApio(double sp, double theta, double elong, double phi, double hm,
        double xp, double yp, double refa, double refb, out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern int iauApio13(double utc1, double utc2, double dut1, double elong,
        double phi, double hm, double xp, double yp, double phpa, double tc, double rh, double wl,
        out IauAstrom astrom);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAtcc13(double rc, double dc, double pr, double pd, double px,
        double rv, double date1, double date2, out double ra, out double da);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAtccq(double rc, double dc, double pr, double pd, double px,
        double rv, in IauAstrom astrom, out double ra, out double da);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAtci13(double rc, double dc, double pr, double pd, double px,
        double rv, double date1, double date2, out double ri, out double di, out double eo);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAtciq(double rc, double dc, double pr, double pd, double px,
        double rv, in IauAstrom astrom, out double ri, out double di);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAtciqn(double rc, double dc, double pr, double pd, double px,
        double rv, in IauAstrom astrom, int n, in IauLdBody[] b, out double ri, out double di);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauAtciqz(double rc, double dc, in IauAstrom astrom, out double ri,
        out double di);

    //----------------------------------------------------------------------------------------------

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern void iauNut06a(double date1, double date2, out double dpsi,
        out double deps);

    [DllImport("libsofa.dylib", CallingConvention = CallingConvention.Cdecl)]
    public static extern double iauFad03(double t);
}
