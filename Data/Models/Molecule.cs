﻿using System.ComponentModel.DataAnnotations;

namespace Galaxon.Astronomy.Data.Models;

public class Molecule
{
    // Primary key.
    public int Id { get; set; }

    // Name.
    [MaxLength(50)]
    public string Name { get; set; } = "";

    // Chemical symbol.
    [MaxLength(20)]
    public string Symbol { get; set; } = "";

    /// <summary>
    /// Add a new molecule to the database if it doesn't already exist.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="name">The element or molecule name.</param>
    /// <param name="symbol">The element or molecule symbol.</param>
    public static void CreateOrUpdate(AstroDbContext db, string name, string symbol)
    {
        // Check if we already have this one.
        Molecule? m = db.Molecules.FirstOrDefault(m => m.Symbol == symbol);
        if (m == null)
        {
            // Create it.
            m = new Molecule
            {
                Name = name,
                Symbol = symbol
            };
            db.Molecules.Add(m);
        }
        else
        {
            // Update it.
            db.Molecules.Attach(m);
            m.Name = name;
        }
        db.SaveChanges();
    }
}
