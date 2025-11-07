namespace PubSea.Framework.Entities;

/// <summary>
/// A marker to find auditable objects.
/// </summary>
public interface IAuditable
{
    const string CREATED_AT = "CreatedAt";

    const string CREATED_BY = "CreatedBy";

    const string UPDATED_AT = "UpdatedAt";

    const string UPDATED_BY = "UpdatedBy";
}