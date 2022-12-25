using Galaxon.Core.Numbers;

namespace Galaxon.Astronomy.Repository;

public class VSOP87DRecord
{
    #region Properties

    public int Id { get; set; }

    [Column(TypeName = "varchar(10)")]
    public string PlanetName { get; set; } = "";

    // Link to Planet record.
    public int AstroObjectId { get; set; }
    public AstroObject? AstroObject { get; set; }

    [Column(TypeName = "char(1)")]
    public char Variable { get; set; }

    [Column(TypeName = "tinyint")]
    public byte Exponent { get; set; }

    [Column(TypeName = "smallint")]
    public ushort Index { get; set; }

    public double Amplitude { get; set; }

    public double Phase { get; set; }

    public double Frequency { get; set; }

    #endregion Properties

    /// <summary>
    /// Parse a VSOP87 data file downloaded from the VSOP87 ftp site.
    /// <see href="ftp://ftp.imcce.fr/pub/ephem/planets/vsop87"/>
    /// As per AA2 we're using VSOP87D data files, which contain values for
    /// heliocentric dynamical ecliptic and equinox of the date.
    /// <see href="https://www.caglow.com/info/compute/vsop87"/>
    /// </summary>
    /// <param name="fileName">The name of the data file.</param>
    public static void ParseDataFile(string fileName)
    {
        using AstroDbContext db = new();

        // Get the data from the data file as an array of strings.
        string dataFilePath = $"{AstroDbContext.DataDirectory()}/VSOP87/{fileName}";
        using StreamReader sr = new(dataFilePath);
        while (sr.ReadLine() is { } line)
        {
            Console.WriteLine($"Parsing {line}");
            // Get the planet number (called "code of body" in vsop87.doc).
            string strPlanetNum = line.Substring(2, 1);
            if (!byte.TryParse(strPlanetNum, out byte planetNum))
            {
                Console.WriteLine("Could not read planet number, skipping line.");
                continue;
            }
            Planet? planet = db.Planets.FirstOrDefault(p => p.Number == planetNum);
            if (planet == null)
            {
                Console.WriteLine($"Could not find planet number {planetNum}.");
                continue;
            }
            Console.WriteLine($"Planet number {planetNum} => {planet.Name}");

            // Get the variable.
            string strVariableIndex = line.Substring(3, 1);
            if (!byte.TryParse(strVariableIndex, out byte variableIndex))
            {
                Console.WriteLine("Could not read variable index, skipping line.");
                continue;
            }
            char variable = variableIndex switch
            {
                1 => 'L',
                2 => 'B',
                3 => 'R',
                _ => ' '
            };
            Console.WriteLine($"Variable = {variable}");

            // Get the exponent of T (called "degree alpha of time variable T"
            // in vsop87.doc).
            string strExponent = line.Substring(4, 1);
            if (!byte.TryParse(strExponent, out byte exponent))
            {
                Console.WriteLine("Could not read exponent, skipping line.");
                continue;
            }
            Console.WriteLine($"Exponent = {exponent}");

            // Get the index (rank) of the term within a series (n).
            string strIndex = line.Substring(6, 5).Trim();
            if (!ushort.TryParse(strIndex, out ushort index))
            {
                Console.WriteLine("Could not read index, skipping line.");
                continue;
            }
            Console.WriteLine($"Index = {index}");

            // Get the amplitude (A).
            string strAmplitude = line.Substring(80, 18).Trim();
            if (!double.TryParse(strAmplitude, out double amplitude))
            {
                Console.WriteLine("Could not read amplitude, skipping line.");
                continue;
            }
            Console.WriteLine($"Amplitude = {amplitude}");

            // Get the phase (B).
            string strPhase = line.Substring(98, 14).Trim();
            if (!double.TryParse(strPhase, out double phase))
            {
                Console.WriteLine("Could not read phase, skipping line.");
                continue;
            }
            Console.WriteLine($"Phase = {phase}");

            // Get the frequency (C).
            string strFrequency = line.Substring(112, 20).Trim();
            if (!double.TryParse(strFrequency, out double frequency))
            {
                Console.WriteLine("Could not read frequency, skipping line.");
                continue;
            }
            Console.WriteLine($"Frequency = {frequency}");

            // Look for an existing record.
            VSOP87DRecord? record = db.VSOP87D.FirstOrDefault(record =>
                record.AstroObjectId == planet.Id && record.Variable == variable &&
                record.Exponent == exponent && record.Index == index);
            if (record == null)
            {
                // Add a new record.
                Console.WriteLine("Adding new record.");
                db.VSOP87D.Add(new VSOP87DRecord
                {
                    AstroObjectId = planet.Id,
                    Variable = variable,
                    Exponent = exponent,
                    Index = index,
                    Amplitude = amplitude,
                    Phase = phase,
                    Frequency = frequency
                });
                db.SaveChanges();
            }
            else if (!record.Amplitude.FuzzyEquals(amplitude) || !record.Phase.FuzzyEquals(phase)
                || !record.Frequency.FuzzyEquals(frequency))
            {
                // Update the record.
                Console.WriteLine("Updating record.");
                record.Amplitude = amplitude;
                record.Phase = phase;
                record.Frequency = frequency;
                db.SaveChanges();
            }
        }
    }

    public static void ParseAllDataFiles()
    {
        for (byte planetNum = 1; planetNum <= 8; planetNum++)
        {
            string name = Planet.NumberToName(planetNum);
            string abbrev = name[..3].ToLower();
            ParseDataFile($"VSOP87D.{abbrev}");
        }
    }

    public static void SetAstroObjectIds()
    {
        using AstroDbContext db = new();

        // Get the planet ids.
        Dictionary<string, int> planetIds = new();
        foreach (Planet? planet in db.Planets)
        {
            planetIds[planet.Name ?? ""] = planet?.Id ?? 0;
        }

        // Update the VSOP87D records.
        foreach (VSOP87DRecord vsop87Rec in db.VSOP87D)
        {
            vsop87Rec.AstroObjectId = planetIds[vsop87Rec.PlanetName];
        }

        db.SaveChanges();
    }
}
