using Galaxon.Astronomy.Data.Models;
using Galaxon.Core.Exceptions;
using Galaxon.Core.Strings;

namespace Galaxon.Astronomy.Data.Repositories;

public class AstroObjectGroupRepository(AstroDbContext astroDbContext)
{
    /// <summary>
    /// Load an AstroObjectGroup from the database.
    /// </summary>
    /// <param name="name">The group name.</param>
    /// <returns>The found object or null.</returns>
    public AstroObjectGroup? Load(string name)
    {
        return astroDbContext.AstroObjectGroups.FirstOrDefault(g => g.Name == name);
    }

    /// <summary>
    /// Create or update a group.
    /// </summary>
    /// <param name="name">The group name.</param>
    /// <param name="parent">The group's parent.</param>
    /// <returns>The updated group.</returns>
    public AstroObjectGroup CreateOrUpdate(string name, AstroObjectGroup? parent = null)
    {
        AstroObjectGroup? group = Load(name);
        if (group == null)
        {
            // The group was not found in the database. Create a new one.
            group = new AstroObjectGroup
            {
                Name = name,
                Parent = parent
            };
            astroDbContext.AstroObjectGroups.Add(group);
        }
        else
        {
            // The group was found.
            group.Parent = parent;
            astroDbContext.AstroObjectGroups.Update(group);
        }
        astroDbContext.SaveChanges();
        return group;
    }

    /// <summary>
    /// Check if the object is in a certain group.
    /// </summary>
    /// <param name="astroObject">The AstroObject to look for.</param>
    /// <param name="group">The AstroObjectGroup to look in.</param>
    /// <returns>If the object is in the specified group.</returns>
    public bool IsInGroup(AstroObject astroObject, AstroObjectGroup group)
    {
        return astroObject.Groups?.Contains(group) ?? false;
    }

    /// <summary>
    /// Check if the object is in a certain group.
    /// </summary>
    /// <param name="astroObject">The AstroObject to look for.</param>
    /// <param name="groupName">The name of the group to check (case sensitive).</param>
    /// <returns>If the object is in the specified group.</returns>
    public bool IsInGroup(AstroObject astroObject, string groupName)
    {
        return astroObject.Groups != null
            && astroObject.Groups.Any(group => groupName.EqualsIgnoreCase(group.Name));
    }

    /// <summary>
    /// Add the object to a group, if it's not already a member.
    /// </summary>
    public void AddToGroup(AstroObject astroObject, AstroObjectGroup group)
    {
        if (!IsInGroup(astroObject, group))
        {
            astroObject.Groups ??= [];
            astroObject.Groups.Add(group);
        }
    }

    /// <summary>
    /// Add the object to a group, if it's not already a member.
    /// </summary>
    /// <param name="astroObject">The AstroObject to add to the group.</param>
    /// <param name="groupName">The group name.</param>
    public void AddToGroup(AstroObject astroObject, string groupName)
    {
        AstroObjectGroup? group = Load(groupName);
        if (group == null)
        {
            throw new DataNotFoundException($"Group '{groupName}' not found.");
        }
        AddToGroup(astroObject, group);
    }
}
