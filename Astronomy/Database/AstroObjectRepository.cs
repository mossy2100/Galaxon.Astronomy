using Galaxon.Astronomy.Models;
using Galaxon.Core.Exceptions;

namespace Galaxon.Astronomy.Database;

public class AstroObjectRepository(AstroDbContext dbContext)
{
    /// <summary>
    /// Load an AstroObject from the database by specifying an object name or number, and optional
    /// group name.
    /// Examples:
    ///     Load("Earth");
    ///     Load("Ceres", "dwarf planet");
    /// If there is more than one matching result, throw an exception.
    /// </summary>
    /// <param name="searchString">The object's name (or number, as a string).</param>
    /// <param name="groupName">The name of the group to search, e.g. "planet", "asteroid",
    /// "plutoid", etc.</param>
    /// <returns>The matching AstroObject.</returns>
    /// <exception cref="ArgumentNullException">
    /// If the object name is null or whitespace.
    /// </exception>
    public AstroObject? Load(string searchString, string? groupName = null)
    {
        // Guard.
        if (string.IsNullOrWhiteSpace(searchString))
        {
            throw new ArgumentNullException(nameof(searchString), "Object name cannot be null or blank.");
        }

        // Get matching objects.
        IQueryable<AstroObject> results = from ao in dbContext.AstroObjects
            where (groupName == null || ao.IsInGroup(groupName)) && ao.IsMatch(searchString)
            select ao;

        // Check if we got multiple results.
        if (results.Count() > 1)
        {
            throw new InvalidOperationException("More than one result found.");
        }

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Add the object to a group, if it's not already a member.
    /// </summary>
    public void AddToGroup(AstroObject astroObj, AstroObjectGroup group)
    {
        if (!astroObj.IsInGroup(group))
        {
            astroObj.Groups ??= [];
            astroObj.Groups.Add(group);
        }
    }

    /// <summary>
    /// Add the object to a group, if it's not already a member.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="groupName">The group name.</param>
    public void AddToGroup(AstroObject astroObj, string groupName)
    {
        AstroObjectGroup? group = AstroObjectGroup.Load(dbContext, groupName);
        if (group == null)
        {
            throw new DataNotFoundException($"Group '{groupName}' not found.");
        }
        AddToGroup(astroObj, group);
    }
}
