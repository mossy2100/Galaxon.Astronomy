using System.Globalization;
using CsvHelper;
using Galaxon.Astronomy.Enums;
using Galaxon.Astronomy.Models;
using Galaxon.Core.Numbers;
using Galaxon.Core.Strings;
using Galaxon.Core.Time;

namespace Galaxon.Astronomy.Database;

public class ImportData(AstroDbContext astroDbContext, AstroObjectRepository astroObjectRepository)
{
    /// <summary>
    /// Parse the data file from the US Census Bureau.
    /// </summary>
    /// <see href="https://www.census.gov/data/software/x13as/genhol/easter-dates.html"/>
    private static void ParseEasterDates1600_2099()
    {
        using AstroDbContext db = new ();

        var csvFile =
            $"{AstroDbContext.DataDirectory()}/Easter/Easter Sunday Dates 1600-2099.csv";
        using StreamReader reader = new (csvFile);

        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] values = line.Split(',');
            if (values.Length != 3)
            {
                continue;
            }

            try
            {
                var year = int.Parse(values[0]);
                var month = int.Parse(values[1]);
                var day = int.Parse(values[2]);
                DateOnly newEasterDate = new (year, month, day);

                // See if we already have one for this year.
                EasterDate? existingEasterDate = db.EasterDates
                    .FirstOrDefault(ed => ed.Date.Year == year);

                // Add or update the record as needed.
                if (existingEasterDate == null)
                {
                    // Add a record.
                    Console.WriteLine($"Adding new Easter date {newEasterDate}");
                    db.EasterDates.Add(new EasterDate { Date = newEasterDate });
                }
                else if (existingEasterDate.Date != newEasterDate)
                {
                    // Update the record.
                    Console.WriteLine(
                        $"Dates for {year} are not the same! Existing = {existingEasterDate.Date}, new = {newEasterDate}");
                    existingEasterDate.Date = newEasterDate;
                }
                else
                {
                    Console.WriteLine($"Dates for {year} are the same, nothing to do.");
                }
            }
            catch
            {
                Console.WriteLine($"Invalid line in CSV: {line}");
            }
        }

        db.SaveChanges();
    }

    /// <summary>
    /// Parse the data file from the Astronomical Society of South Australia.
    /// </summary>
    /// <see href="https://www.assa.org.au/edm"/>
    private static void ParseEasterDates1700_2299()
    {
        using AstroDbContext db = new ();

        var htmlFile =
            $"{AstroDbContext.DataDirectory()}/Easter/Easter Sunday Dates 1700-2299.html";
        Regex rx = new (@"(\d{1,2})(st|nd|rd|th) (March|April) (\d{4})");
        using StreamReader reader = new (htmlFile);

        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            MatchCollection matches = rx.Matches(line);
            if (matches.Count <= 0)
            {
                continue;
            }
            foreach (Match match in matches)
            {
                var year = int.Parse(match.Groups[4].Value);
                int month = match.Groups[3].Value == "March" ? 3 : 4;
                var day = int.Parse(match.Groups[1].Value);
                DateOnly newEasterDate = new (year, month, day);
                // See if we already have one for this year.
                EasterDate? existingEasterDate = db.EasterDates
                    .FirstOrDefault(ed => ed.Date.Year == year);
                if (existingEasterDate == null)
                {
                    db.EasterDates.Add(new EasterDate { Date = newEasterDate });
                }
                else
                {
                    // Check if they are different.
                    if (existingEasterDate.Date != newEasterDate)
                    {
                        Console.WriteLine(
                            $"Dates for {year} are not the same! Existing = {existingEasterDate.Date}, new = {newEasterDate}");
                        //existingEasterDate.Date = newEasterDate;
                    }
                    else
                    {
                        Console.WriteLine($"Dates for {year} are the same, nothing to do.");
                    }
                }
            }
        }

        db.SaveChanges();
    }

    /// <summary>
    /// Initialise the data table with the current dates that had leap seconds.
    /// </summary>
    public static void ParseLeapSeconds()
    {
        using AstroDbContext db = new ();

        // Load the HTML table as a string.
        var dataFilePath =
            $"{AstroDbContext.DataDirectory()}/Leap seconds/Leap second and UT1-UTC information _ NIST.html";
        string html = File.ReadAllText(dataFilePath);

        // Get the dates.
        Regex rxDate = new (@"(\d{4})-(\d{2})-(\d{2})");
        MatchCollection matches = rxDate.Matches(html);
        List<DateOnly> dates = new ();
        foreach (Match match in matches)
        {
            var year = int.Parse(match.Groups[1].Value);
            var month = int.Parse(match.Groups[2].Value);
            var day = int.Parse(match.Groups[3].Value);
            DateOnly date = new (year, month, day);
            dates.Add(date);
        }

        // Sort, then add to database.
        if (dates.Count > 0)
        {
            foreach (DateOnly dt in dates.OrderBy(d => d.GetTotalDays()))
            {
                LeapSecond? existingRecord = db.LeapSeconds.FirstOrDefault(ls => ls.Date == dt);
                if (existingRecord == null)
                {
                    // Add a new record.
                    db.LeapSeconds.Add(new LeapSecond { Date = dt });
                    db.SaveChanges();
                }
            }
        }
    }

    /// <summary>
    /// Extract the lunar phase data from the web pages captured from
    /// AstroPixels and copy the data into the database.
    /// </summary>
    public static void ParseLunarPhaseData()
    {
        Regex rxDataLine =
            new (@"(\d{4})?\s+(([a-z]{3})\s+(\d{1,2})\s+(\d{2}):(\d{2})\s+([a-z])?){1,4}",
                RegexOptions.IgnoreCase);
        int? year = null;
        string? curYearStr = null;

        using AstroDbContext db = new ();

        for (var century = 0; century < 20; century++)
        {
            int startYear = century * 100 + 1;
            int endYear = (century + 1) * 100;
            string[] lines = File.ReadAllLines(
                $"{AstroDbContext.DataDirectory()}/Lunar phases/AstroPixels - Moon Phases_ {startYear:D4} to {endYear:D4}.html");
            foreach (string line in lines)
            {
                MatchCollection matches = rxDataLine.Matches(line);
                if (matches.Count <= 0)
                {
                    continue;
                }

                // Get the year, if present.
                string yearStr = line.Substring(1, 4);
                if (yearStr.Trim() != "")
                {
                    curYearStr = yearStr;
                    year = int.Parse(yearStr);
                }

                // The dates are meaningless without the year. This
                // shouldn't happen, but if we don't know the year, skip
                // parsing the dates.
                if (year == null)
                {
                    continue;
                }

                // Get the dates and times of the phases, if present.
                for (var phase = 0; phase < 4; phase++)
                {
                    string dateStr = line.Substring(8 + phase * 18, 13);
                    if (dateStr.Trim() == "")
                    {
                        continue;
                    }
                    bool parseOk = DateTime.TryParse($"{curYearStr} {dateStr}",
                        out DateTime phaseDateTime);
                    if (parseOk)
                    {
                        // Set the datetime to UTC.
                        phaseDateTime = DateTime.SpecifyKind(phaseDateTime, DateTimeKind.Utc);

                        // Store phase information in the database.
                        if (db.LunarPhases != null)
                        {
                            db.LunarPhases.Add(new LunarPhase
                            {
                                PhaseNumber = (ELunarPhase)phase,
                                UtcDateTime = phaseDateTime
                            });
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Initialize all the groups.
    /// </summary>
    public static void InitializeAstroObjects()
    {
        using AstroDbContext db = new ();

        // Stars.
        AstroObjectGroup star = AstroObjectGroup.CreateOrUpdate(db, "Star");
        AstroObjectGroup.CreateOrUpdate(db, "Hypergiant", star);
        AstroObjectGroup.CreateOrUpdate(db, "Supergiant", star);

        AstroObjectGroup giantStar = AstroObjectGroup.CreateOrUpdate(db, "Giant", star);
        AstroObjectGroup.CreateOrUpdate(db, "Subgiant", giantStar);
        AstroObjectGroup.CreateOrUpdate(db, "Bright giant", giantStar);
        AstroObjectGroup.CreateOrUpdate(db, "Red giant", giantStar);
        AstroObjectGroup.CreateOrUpdate(db, "Yellow giant", giantStar);
        AstroObjectGroup.CreateOrUpdate(db, "Blue giant", giantStar);
        AstroObjectGroup.CreateOrUpdate(db, "White giant", giantStar);

        AstroObjectGroup mainSequence = AstroObjectGroup.CreateOrUpdate(db, "Main sequence", star);
        AstroObjectGroup.CreateOrUpdate(db, "Red dwarf", mainSequence);
        AstroObjectGroup.CreateOrUpdate(db, "Orange dwarf", mainSequence);
        AstroObjectGroup.CreateOrUpdate(db, "Yellow dwarf", mainSequence);
        AstroObjectGroup.CreateOrUpdate(db, "Blue main sequence star", mainSequence);
        AstroObjectGroup.CreateOrUpdate(db, "White dwarf", mainSequence);

        AstroObjectGroup.CreateOrUpdate(db, "Subdwarf", star);
        AstroObjectGroup.CreateOrUpdate(db, "Brown dwarf", star);

        // Planets.
        AstroObjectGroup planet = AstroObjectGroup.CreateOrUpdate(db, "Planet");
        AstroObjectGroup.CreateOrUpdate(db, "Terrestrial planet", planet);

        AstroObjectGroup giantPlanet = AstroObjectGroup.CreateOrUpdate(db, "Giant planet", planet);
        AstroObjectGroup.CreateOrUpdate(db, "Gas giant", giantPlanet);
        AstroObjectGroup.CreateOrUpdate(db, "Ice giant", giantPlanet);

        // Planetoids.
        AstroObjectGroup minorPlanet = AstroObjectGroup.CreateOrUpdate(db, "Minor planet");
        AstroObjectGroup.CreateOrUpdate(db, "Centaur", minorPlanet);
        AstroObjectGroup.CreateOrUpdate(db, "Trojan", minorPlanet);
        AstroObjectGroup.CreateOrUpdate(db, "Quasi-satellite", minorPlanet);

        AstroObjectGroup dwarfPlanet =
            AstroObjectGroup.CreateOrUpdate(db, "Dwarf planet", minorPlanet);
        AstroObjectGroup.CreateOrUpdate(db, "Plutoid", dwarfPlanet);

        AstroObjectGroup asteroid = AstroObjectGroup.CreateOrUpdate(db, "Asteroid", minorPlanet);
        AstroObjectGroup.CreateOrUpdate(db, "Potentially hazardous asteroid", asteroid);

        AstroObjectGroup nea = AstroObjectGroup.CreateOrUpdate(db, "Near Earth asteroid", asteroid);
        AstroObjectGroup.CreateOrUpdate(db, "Apohele asteroid", nea);
        AstroObjectGroup.CreateOrUpdate(db, "Aten asteroid", nea);
        AstroObjectGroup.CreateOrUpdate(db, "Apollo asteroid", nea);
        AstroObjectGroup.CreateOrUpdate(db, "Amor asteroid", nea);

        AstroObjectGroup sssb = AstroObjectGroup.CreateOrUpdate(db, "Small Solar System body");
        AstroObjectGroup.CreateOrUpdate(db, "Comet", sssb);

        AstroObjectGroup tno = AstroObjectGroup.CreateOrUpdate(db, "Trans-Neptunian Object");
        AstroObjectGroup.CreateOrUpdate(db, "Oort cloud", tno);

        AstroObjectGroup kbo = AstroObjectGroup.CreateOrUpdate(db, "Kuper Belt Object", tno);
        AstroObjectGroup.CreateOrUpdate(db, "Cubewano", kbo);
        AstroObjectGroup resonentKbo = AstroObjectGroup.CreateOrUpdate(db, "Resonant KBO", kbo);
        AstroObjectGroup.CreateOrUpdate(db, "Plutino", resonentKbo);

        AstroObjectGroup sdo = AstroObjectGroup.CreateOrUpdate(db, "Scattered-disc object", tno);
        AstroObjectGroup.CreateOrUpdate(db, "Resonant SDO", sdo);

        AstroObjectGroup etno =
            AstroObjectGroup.CreateOrUpdate(db, "Extreme Trans-Neptunian object", tno);
        AstroObjectGroup detached = AstroObjectGroup.CreateOrUpdate(db, "Detached object", etno);
        AstroObjectGroup.CreateOrUpdate(db, "Sednoid", detached);

        // Satellites.
        AstroObjectGroup satellite = AstroObjectGroup.CreateOrUpdate(db, "Satellite");
        AstroObjectGroup.CreateOrUpdate(db, "Regular satellite", satellite);
        AstroObjectGroup.CreateOrUpdate(db, "Irregular satellite", satellite);
        AstroObjectGroup.CreateOrUpdate(db, "Prograde satellite", satellite);
        AstroObjectGroup.CreateOrUpdate(db, "Retrograde satellite", satellite);
    }

    /// <summary>
    /// Initialize the planet data from the CSV file.
    /// </summary>
    public void InitializePlanets()
    {
        using AstroDbContext db = new ();

        // Get the Sun.
        var sun = astroObjectRepository.Load("Sun");

        // Open the CSV file for parsing.
        var csvPath = $"{AstroDbContext.DataDirectory()}/Planets/Planets.csv";
        using StreamReader stream = new (csvPath);
        using CsvReader csv = new (stream, CultureInfo.InvariantCulture);
        // Skip the header row.
        csv.Read();
        csv.ReadHeader();

        // Create or update the planet records.
        while (csv.Read())
        {
            string? name = csv.GetField(0);
            string? type = csv.GetField(2);

            if (name == null)
            {
                continue;
            }

            AstroObject planet;
            List<AstroObject> planets = astroObjectRepository.LoadMany(name, "planet");

            if (planets.Count == 0)
            {
                // Create a new planet in the database.
                Console.WriteLine($"Adding new planet {name}.");
                planet = new AstroObject(name);
                db.AstroObjects.Add(planet);
            }
            else
            {
                // Update planet in the database.
                Console.WriteLine($"Updating planet {name}.");
                planet = planets.First();
                db.AstroObjects.Attach(planet);
            }

            // Set the planet's basic parameters.

            // Set its number.
            string? num = csv.GetField(1);
            planet.Number = num == null ? null : uint.Parse(num);

            // Set its groups.
            astroObjectRepository.AddToGroup(planet, "planet");
            astroObjectRepository.AddToGroup(planet, $"{type} planet");

            // Set its parent.
            planet.Parent = sun;

            // Save the planet object now to ensure it has an Id before attaching composition
            // objects to it.
            db.SaveChanges();

            // Orbital parameters.
            // TODO Fix this. The Orbit and other objects aren't loaded by
            // TODO default now when the planet is loaded, so we have to look in
            // TODO the database for the Orbit object first, before creating a new one.
            planet.Orbit ??= new OrbitalRecord();

            // Apoapsis is provided in km, convert to m.
            const double kilo = 1000;
            var apoapsis = csv.GetField(3).ToDouble();
            planet.Orbit.Apoapsis = Quantity.ConvertAmount(apoapsis, "km", "m");

            // Periapsis is provided in km, convert to m.
            var periapsis = csv.GetField(4).ToDouble();
            planet.Orbit.Periapsis = Quantity.ConvertAmount(periapsis, "km", "m");

            // Semi-major axis is provided in km, convert to m.
            planet.Orbit.SemiMajorAxis = csv.GetField(5).ToDouble() * kilo;
            planet.Orbit.Eccentricity = csv.GetField(6).ToDouble();

            // Sidereal orbit period is provided in days, convert to seconds.
            planet.Orbit.SiderealOrbitPeriod =
                csv.GetField(7).ToDouble() * XTimeSpan.SECONDS_PER_DAY;

            // Synodic orbit period is provided in days, convert to seconds.
            var synodicPeriod = csv.GetField(8).ToDouble();
            planet.Orbit.SynodicOrbitPeriod = synodicPeriod * XTimeSpan.SECONDS_PER_DAY;

            // Avg orbital speed is provided in km/s, convert to m/s.
            planet.Orbit.AvgOrbitSpeed = csv.GetField(9).ToDouble() * kilo;

            // Mean anomaly is provided in degrees, convert to radians.
            planet.Orbit.MeanAnomaly = csv.GetField(10).ToDouble() * Angle.RadiansPerDegree;

            // Inclination is provided in degrees, convert to radians.
            planet.Orbit.Inclination = csv.GetField(11).ToDouble() * Angle.RadiansPerDegree;

            // Long. of asc. node is provided in degrees, convert to radians.
            planet.Orbit.LongAscNode = csv.GetField(12).ToDouble() * Angle.RadiansPerDegree;

            // Arg. of perihelion is provided in degrees, convert to radians.
            planet.Orbit.ArgPeriapsis = csv.GetField(13).ToDouble() * Angle.RadiansPerDegree;

            // Calculate the mean motion in rad/s.
            planet.Orbit.MeanMotion = Tau / planet.Orbit.SiderealOrbitPeriod;

            // Save the orbital parameters.
            db.SaveChanges();

            // Physical parameters.
            planet.Physical ??= new PhysicalRecord();
            double? equatRadius = csv.GetField(15).ToDouble() * kilo;
            double? polarRadius = csv.GetField(16).ToDouble() * kilo;
            planet.Physical.SetSpheroidalShape(equatRadius ?? 0, polarRadius ?? 0);
            planet.Physical.MeanRadius = csv.GetField(14).ToDouble() * kilo;
            planet.Physical.Flattening = csv.GetField(17).ToDouble();
            planet.Physical.SurfaceArea = csv.GetField(18).ToDouble() * kilo * kilo;
            planet.Physical.Volume = csv.GetField(19).ToDouble() * Pow(kilo, 3);
            planet.Physical.Mass = csv.GetField(20).ToDouble();
            planet.Physical.Density =
                Density.GramsPerCm3ToKgPerM3(csv.GetField(21).ToDouble() ?? 0);
            planet.Physical.SurfaceGrav = csv.GetField(22).ToDouble();
            planet.Physical.MomentOfInertiaFactor = csv.GetField(23).ToDouble();
            planet.Physical.EscapeVelocity = csv.GetField(24).ToDouble() * kilo;
            planet.Physical.StdGravParam = csv.GetField(25).ToDouble();
            planet.Physical.GeometricAlbedo = csv.GetField(26).ToDouble();
            planet.Physical.SolarIrradiance = csv.GetField(27).ToDouble();
            planet.Physical.HasGlobalMagField = csv.GetField(28) == "Y";
            planet.Physical.HasRingSystem = csv.GetField(29) == "Y";
            planet.Physical.MinSurfaceTemp = csv.GetField(36).ToDouble();
            planet.Physical.MeanSurfaceTemp = csv.GetField(37).ToDouble();
            planet.Physical.MaxSurfaceTemp = csv.GetField(38).ToDouble();
            db.SaveChanges();

            // Rotational parameters.
            planet.Rotation ??= new RotationalRecord();
            planet.Rotation.SynodicRotationPeriod =
                csv.GetField(30).ToDouble() * XTimeSpan.SECONDS_PER_DAY;
            planet.Rotation.SiderealRotationPeriod =
                csv.GetField(31).ToDouble() * XTimeSpan.SECONDS_PER_DAY;
            planet.Rotation.EquatRotationVelocity = csv.GetField(32).ToDouble();
            planet.Rotation.Obliquity = csv.GetField(33).ToDouble() * Angle.RadiansPerDegree;
            planet.Rotation.NorthPoleRightAscension =
                csv.GetField(34).ToDouble() * Angle.RadiansPerDegree;
            planet.Rotation.NorthPoleDeclination =
                csv.GetField(35).ToDouble() * Angle.RadiansPerDegree;
            db.SaveChanges();

            // Atmosphere.
            planet.Atmosphere ??= new AtmosphereRecord();
            planet.Atmosphere.SurfacePressure = csv.GetField(40).ToDouble();
            planet.Atmosphere.ScaleHeight = csv.GetField(41).ToDouble() * kilo;
            planet.Atmosphere.IsSurfaceBoundedExosphere = name == "Mercury";
            db.SaveChanges();
        }
    }

    /// <summary>
    /// Extract the seasonal marker data from the text file and copy the data
    /// into the database.
    /// </summary>
    public static void ParseSeasonalMarkerData()
    {
        using AstroDbContext db = new ();

        // Lookup table to help with the parsing.
        Dictionary<string, int> MonthAbbrevs = new ()
        {
            { "Mar", 3 },
            { "Jun", 6 },
            { "Sep", 9 },
            { "Dec", 12 }
        };

        // Get the data from the data file as an array of strings.
        var dataFilePath =
            $"{AstroDbContext.DataDirectory()}/Seasonal markers/SeasonalMarkers2001-2100.txt";
        string[] lines = File.ReadAllLines(dataFilePath);

        // Convert the lines into our internal data structure.
        Regex rx = new ("\\s+");
        foreach (string line in lines)
        {
            string[] words = rx.Split(line);

            if (words.Length <= 1 || words[0] == "Year")
            {
                continue;
            }

            // Extract the dates from the row.
            var year = int.Parse(words[1]);
            for (var i = 0; i < 4; i++)
            {
                int j = i * 3;
                string monthAbbrev = words[j + 2];
                int month = MonthAbbrevs[monthAbbrev];
                var dayOfMonth = int.Parse(words[j + 3]);
                string[] time = words[j + 4].Split(":");
                var hour = int.Parse(time[0]);
                var minute = int.Parse(time[1]);

                // Construct the new DateTime object.
                DateTime seasonalMarkerDateTime = new (year, month, dayOfMonth, hour, minute, 0,
                    DateTimeKind.Utc);

                // Check if there is already an entry in the database table
                // for this seasonal marker.
                SeasonalMarker? sm = db.SeasonalMarkers?
                    .FirstOrDefault(sm => sm.UtcDateTime.Year == year && sm.MarkerNumber == i);

                // Add a new row or update the existing row as required.
                if (sm == null)
                {
                    // Add a new row.
                    db.SeasonalMarkers!.Add(new SeasonalMarker
                    {
                        MarkerNumber = i,
                        UtcDateTime = seasonalMarkerDateTime
                    });
                }
                else
                {
                    // Update the row.
                    sm.UtcDateTime = seasonalMarkerDateTime;
                }
            }
        }

        db.SaveChanges();
    }

    /// <summary>
    /// Parse a VSOP87 data file downloaded from the VSOP87 ftp site.
    /// <see href="ftp://ftp.imcce.fr/pub/ephem/planets/vsop87"/>
    /// As per AA2 we're using VSOP87D data files, which contain values for
    /// heliocentric dynamical ecliptic and equinox of the date.
    /// <see href="https://www.caglow.com/info/compute/vsop87"/>
    /// </summary>
    /// <param name="fileName">The name of the data file.</param>
    public static void ParseVSOP87DataFile(string fileName)
    {
        using AstroDbContext db = new ();

        // Get the data from the data file as an array of strings.
        var dataFilePath = $"{AstroDbContext.DataDirectory()}/VSOP87/{fileName}";
        using StreamReader sr = new (dataFilePath);
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
            VSOP87DRecord? record = db.VSOP87DRecords.FirstOrDefault(record =>
                record.AstroObjectId == planet.Id && record.Variable == variable &&
                record.Exponent == exponent && record.Index == index);
            if (record == null)
            {
                // Add a new record.
                Console.WriteLine("Adding new record.");
                db.VSOP87DRecords.Add(new VSOP87DRecord
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

    public static void ParseAllVSOP87DataFiles()
    {
        for (byte planetNum = 1; planetNum <= 8; planetNum++)
        {
            string name = Planet.NumberToName(planetNum);
            string abbrev = name[..3].ToLower();
            ParseVSOP87DataFile($"VSOP87D.{abbrev}");
        }
    }

    public static void SetAstroObjectIds()
    {
        using AstroDbContext db = new ();

        // Get the planet ids.
        Dictionary<string, int> planetIds = new ();
        foreach (Planet? planet in db.Planets)
        {
            planetIds[planet.Name ?? ""] = planet?.Id ?? 0;
        }

        // Update the VSOP87D records.
        foreach (VSOP87DRecord vsop87Rec in db.VSOP87DRecords)
        {
            vsop87Rec.AstroObjectId = planetIds[vsop87Rec.PlanetName];
        }

        db.SaveChanges();
    }
}
