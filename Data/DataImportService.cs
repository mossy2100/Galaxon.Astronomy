using System.Globalization;
using CsvHelper;
using Galaxon.Astronomy.Data.Models;
using Galaxon.Astronomy.Data.Repositories;
using Galaxon.Astronomy.Data.Enums;
using Galaxon.Core.Exceptions;
using Galaxon.Core.Numbers;
using Galaxon.Core.Strings;
using Galaxon.Core.Time;

namespace Galaxon.Astronomy.Data;

public class DataImportService(
    AstroDbContext astroDbContext,
    AstroObjectRepository astroObjectRepository,
    AstroObjectGroupRepository astroObjectGroupRepository)
{
    /// <summary>
    /// Parse the data file from the US Census Bureau.
    /// </summary>
    /// <see href="https://www.census.gov/data/software/x13as/genhol/easter-dates.html"/>
    private void ParseEasterDates1600_2099()
    {
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
                EasterDate? existingEasterDate = astroDbContext.EasterDates
                    .FirstOrDefault(ed => ed.Date.Year == year);

                // Add or update the record as needed.
                if (existingEasterDate == null)
                {
                    // Add a record.
                    Console.WriteLine($"Adding new Easter date {newEasterDate}");
                    astroDbContext.EasterDates.Add(new EasterDate { Date = newEasterDate });
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

        astroDbContext.SaveChanges();
    }

    /// <summary>
    /// Parse the data file from the Astronomical Society of South Australia.
    /// </summary>
    /// <see href="https://www.assa.org.au/edm"/>
    private void ParseEasterDates1700_2299()
    {
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
                EasterDate? existingEasterDate = astroDbContext.EasterDates
                    .FirstOrDefault(ed => ed.Date.Year == year);
                if (existingEasterDate == null)
                {
                    astroDbContext.EasterDates.Add(new EasterDate { Date = newEasterDate });
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

        astroDbContext.SaveChanges();
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

        using AstroDbContext astroDbContext = new ();

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
                        if (astroDbContext.LunarPhases != null)
                        {
                            astroDbContext.LunarPhases.Add(new LunarPhase
                            {
                                PhaseNumber = (ELunarPhase)phase,
                                UtcDateTime = phaseDateTime
                            });
                            astroDbContext.SaveChanges();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Initialize all the groups.
    /// </summary>
    public void InitializeAstroObjects()
    {
        // Stars.
        AstroObjectGroup star = astroObjectGroupRepository.CreateOrUpdate("Star");
        astroObjectGroupRepository.CreateOrUpdate("Hypergiant", star);
        astroObjectGroupRepository.CreateOrUpdate("Supergiant", star);

        AstroObjectGroup giantStar = astroObjectGroupRepository.CreateOrUpdate("Giant", star);
        astroObjectGroupRepository.CreateOrUpdate("Subgiant", giantStar);
        astroObjectGroupRepository.CreateOrUpdate("Bright giant", giantStar);
        astroObjectGroupRepository.CreateOrUpdate("Red giant", giantStar);
        astroObjectGroupRepository.CreateOrUpdate("Yellow giant", giantStar);
        astroObjectGroupRepository.CreateOrUpdate("Blue giant", giantStar);
        astroObjectGroupRepository.CreateOrUpdate("White giant", giantStar);

        AstroObjectGroup mainSequence =
            astroObjectGroupRepository.CreateOrUpdate("Main sequence", star);
        astroObjectGroupRepository.CreateOrUpdate("Red dwarf", mainSequence);
        astroObjectGroupRepository.CreateOrUpdate("Orange dwarf", mainSequence);
        astroObjectGroupRepository.CreateOrUpdate("Yellow dwarf", mainSequence);
        astroObjectGroupRepository.CreateOrUpdate("Blue main sequence star", mainSequence);
        astroObjectGroupRepository.CreateOrUpdate("White dwarf", mainSequence);

        astroObjectGroupRepository.CreateOrUpdate("Subdwarf", star);
        astroObjectGroupRepository.CreateOrUpdate("Brown dwarf", star);

        // Planets.
        AstroObjectGroup planet = astroObjectGroupRepository.CreateOrUpdate("Planet");
        astroObjectGroupRepository.CreateOrUpdate("Terrestrial planet", planet);

        AstroObjectGroup giantPlanet =
            astroObjectGroupRepository.CreateOrUpdate("Giant planet", planet);
        astroObjectGroupRepository.CreateOrUpdate("Gas giant", giantPlanet);
        astroObjectGroupRepository.CreateOrUpdate("Ice giant", giantPlanet);

        // Planetoids.
        AstroObjectGroup minorPlanet = astroObjectGroupRepository.CreateOrUpdate("Minor planet");
        astroObjectGroupRepository.CreateOrUpdate("Centaur", minorPlanet);
        astroObjectGroupRepository.CreateOrUpdate("Trojan", minorPlanet);
        astroObjectGroupRepository.CreateOrUpdate("Quasi-satellite", minorPlanet);

        AstroObjectGroup dwarfPlanet =
            astroObjectGroupRepository.CreateOrUpdate("Dwarf planet", minorPlanet);
        astroObjectGroupRepository.CreateOrUpdate("Plutoid", dwarfPlanet);

        AstroObjectGroup asteroid =
            astroObjectGroupRepository.CreateOrUpdate("Asteroid", minorPlanet);
        astroObjectGroupRepository.CreateOrUpdate("Potentially hazardous asteroid", asteroid);

        AstroObjectGroup nea =
            astroObjectGroupRepository.CreateOrUpdate("Near Earth asteroid", asteroid);
        astroObjectGroupRepository.CreateOrUpdate("Apohele asteroid", nea);
        astroObjectGroupRepository.CreateOrUpdate("Aten asteroid", nea);
        astroObjectGroupRepository.CreateOrUpdate("Apollo asteroid", nea);
        astroObjectGroupRepository.CreateOrUpdate("Amor asteroid", nea);

        AstroObjectGroup sssb =
            astroObjectGroupRepository.CreateOrUpdate("Small Solar System body");
        astroObjectGroupRepository.CreateOrUpdate("Comet", sssb);

        AstroObjectGroup tno = astroObjectGroupRepository.CreateOrUpdate("Trans-Neptunian Object");
        astroObjectGroupRepository.CreateOrUpdate("Oort cloud", tno);

        AstroObjectGroup kbo = astroObjectGroupRepository.CreateOrUpdate("Kuper Belt Object", tno);
        astroObjectGroupRepository.CreateOrUpdate("Cubewano", kbo);
        AstroObjectGroup resonentKbo =
            astroObjectGroupRepository.CreateOrUpdate("Resonant KBO", kbo);
        astroObjectGroupRepository.CreateOrUpdate("Plutino", resonentKbo);

        AstroObjectGroup sdo =
            astroObjectGroupRepository.CreateOrUpdate("Scattered-disc object", tno);
        astroObjectGroupRepository.CreateOrUpdate("Resonant SDO", sdo);

        AstroObjectGroup etno =
            astroObjectGroupRepository.CreateOrUpdate("Extreme Trans-Neptunian object", tno);
        AstroObjectGroup detached =
            astroObjectGroupRepository.CreateOrUpdate("Detached object", etno);
        astroObjectGroupRepository.CreateOrUpdate("Sednoid", detached);

        // Satellites.
        AstroObjectGroup satellite = astroObjectGroupRepository.CreateOrUpdate("Satellite");
        astroObjectGroupRepository.CreateOrUpdate("Regular satellite", satellite);
        astroObjectGroupRepository.CreateOrUpdate("Irregular satellite", satellite);
        astroObjectGroupRepository.CreateOrUpdate("Prograde satellite", satellite);
        astroObjectGroupRepository.CreateOrUpdate("Retrograde satellite", satellite);
    }

    /// <summary>
    /// Initialize the planet data from the CSV file.
    /// </summary>
    public void InitializePlanets()
    {
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

            AstroObject? planet = astroObjectRepository.Load(name, "planet");

            if (planet == null)
            {
                // Create a new planet in the database.
                Console.WriteLine($"Adding new planet {name}.");
                planet = new AstroObject(name);
                astroDbContext.AstroObjects.Add(planet);
            }
            else
            {
                // Update planet in the database.
                Console.WriteLine($"Updating planet {name}.");
                astroDbContext.AstroObjects.Attach(planet);
            }

            // Set the planet's basic parameters.

            // Set its number.
            string? num = csv.GetField(1);
            planet.Number = num == null ? null : uint.Parse(num);

            // Set its groups.
            astroObjectGroupRepository.AddToGroup(planet, "planet");
            astroObjectGroupRepository.AddToGroup(planet, $"{type} planet");

            // Set its parent.
            planet.Parent = sun;

            // Save the planet object now to ensure it has an Id before attaching composition
            // objects to it.
            astroDbContext.SaveChanges();

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
            astroDbContext.SaveChanges();

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
            astroDbContext.SaveChanges();

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
            astroDbContext.SaveChanges();

            // Atmosphere.
            planet.Atmosphere ??= new AtmosphereRecord();
            planet.Atmosphere.SurfacePressure = csv.GetField(40).ToDouble();
            planet.Atmosphere.ScaleHeight = csv.GetField(41).ToDouble() * kilo;
            planet.Atmosphere.IsSurfaceBoundedExosphere = name == "Mercury";
            astroDbContext.SaveChanges();
        }
    }

    /// <summary>
    /// Extract the seasonal marker data from the text file and copy the data
    /// into the database.
    /// </summary>
    public static void ParseSeasonalMarkerData()
    {
        using AstroDbContext astroDbContext = new ();

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
                SeasonalMarker? sm = astroDbContext.SeasonalMarkers?
                    .FirstOrDefault(sm => sm.UtcDateTime.Year == year && sm.MarkerNumber == i);

                // Add a new row or update the existing row as required.
                if (sm == null)
                {
                    // Add a new row.
                    astroDbContext.SeasonalMarkers!.Add(new SeasonalMarker
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

        astroDbContext.SaveChanges();
    }

    /// <summary>
    /// Parse a VSOP87 data file downloaded from the VSOP87 ftp site.
    /// <see href="ftp://ftp.imcce.fr/pub/ephem/planets/vsop87"/>
    /// As per AA2 we're using VSOP87D data files, which contain values for
    /// heliocentric dynamical ecliptic and equinox of the date.
    /// <see href="https://www.caglow.com/info/compute/vsop87"/>
    /// </summary>
    /// <param name="fileName">The name of the data file.</param>
    public void ParseVSOP87DataFile(string fileName)
    {
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
            AstroObject? planet = astroObjectRepository.Load(planetNum, "planet");
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
            VSOP87DRecord? record = astroDbContext.VSOP87DRecords.FirstOrDefault(record =>
                record.AstroObjectId == planet.Id && record.Variable == variable &&
                record.Exponent == exponent && record.Index == index);
            if (record == null)
            {
                // Add a new record.
                Console.WriteLine("Adding new record.");
                astroDbContext.VSOP87DRecords.Add(new VSOP87DRecord
                {
                    AstroObjectId = planet.Id,
                    Variable = variable,
                    Exponent = exponent,
                    Index = index,
                    Amplitude = amplitude,
                    Phase = phase,
                    Frequency = frequency
                });
                astroDbContext.SaveChanges();
            }
            else if (!record.Amplitude.FuzzyEquals(amplitude) || !record.Phase.FuzzyEquals(phase)
                || !record.Frequency.FuzzyEquals(frequency))
            {
                // Update the record.
                Console.WriteLine("Updating record.");
                record.Amplitude = amplitude;
                record.Phase = phase;
                record.Frequency = frequency;
                astroDbContext.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Get a planet name given a number.
    /// </summary>
    /// <param name="num">The planet number as used in VSOP87 data.</param>
    /// <returns>The planet name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If the number is not in the range 1..8.
    /// </exception>
    public static string PlanetNumberToName(byte num)
    {
        return num switch
        {
            1 => "Mercury",
            2 => "Venus",
            3 => "Earth",
            4 => "Mars",
            5 => "Jupiter",
            6 => "Saturn",
            7 => "Uranus",
            8 => "Neptune",
            _ => throw new MatchNotFoundException("Planet number must be in the range 1..8.")
        };
    }

    public void ParseAllVSOP87DataFiles()
    {
        for (byte planetNum = 1; planetNum <= 8; planetNum++)
        {
            string name = PlanetNumberToName(planetNum);
            string abbrev = name[..3].ToLower();
            ParseVSOP87DataFile($"VSOP87D.{abbrev}");
        }
    }

    public void SetAstroObjectIds()
    {
        // Get the planet ids.
        List<AstroObject> planets = astroObjectRepository.LoadAllInGroup("planet");
        Dictionary<string, int> planetIds = new ();
        foreach (AstroObject planet in planets)
        {
            if (planet.Name != null)
            {
                planetIds[planet.Name] = planet.Id;
            }
        }

        // Update the VSOP87D records.
        foreach (VSOP87DRecord vsop87Rec in astroDbContext.VSOP87DRecords)
        {
            vsop87Rec.AstroObjectId = planetIds[vsop87Rec.PlanetName];
        }

        astroDbContext.SaveChanges();
    }
}
