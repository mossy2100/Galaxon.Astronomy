using Galaxon.Core.Exceptions;

namespace Galaxon.Astronomy.Repository;

public class Atmosphere
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Atmosphere() { }

    /// <summary>
    /// Add a constituent to the atmosphere.
    /// Does not save the Atmosphere record.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="symbol">The molecule symbol.</param>
    /// <param name="percentage">The percentage of it in the atmosphere.</param>
    public void AddConstituent(AstroDbContext db, string symbol, double? percentage = null)
    {
        AtmoConstituent? constituent = Constituents
            .FirstOrDefault(ac => ac.Molecule.Symbol == symbol);
        if (constituent == null)
        {
            // Get the molecule.
            // TODO maybe use Molecule.Load() here.
            Molecule? m = db.Molecules.FirstOrDefault(m => m.Symbol == symbol);
            if (m == null)
            {
                throw new DataNotFoundException($"Molecule '{symbol}' not found.");
            }

            // Create the new atmo constituent object.
            constituent = new AtmoConstituent
            {
                Molecule = m,
                Percentage = percentage
            };

            // Add it.
            Constituents.Add(constituent);
        }
        else
        {
            // Update the constituent percentage.
            constituent.Percentage = percentage;
        }
    }

    #region Properties

    // Primary key.
    public int Id { get; set; }

    // Link to owner.
    public int AstroObjectId { get; set; }

    public AstroObject? AstroObject { get; set; }

    // Surface pressure (Pa).
    public double? SurfacePressure { get; set; }

    // Scale height (km).
    public double? ScaleHeight { get; set; }

    // Atmosphere constituents.
    public List<AtmoConstituent> Constituents { get; set; } = new ();

    // Is it a surface-bounded exosphere?
    public bool? IsSurfaceBoundedExosphere { get; set; }

    #endregion Properties
}
