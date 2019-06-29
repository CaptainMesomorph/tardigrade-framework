namespace Tardigrade.Framework.Persistence
{
    /// <summary>
    /// Base repository interface of batch operations on objects of a particular type. Batch operations process a
    /// collection of objects individually, and success or failure applies to each individual object in the collection.
    /// </summary>
    /// <typeparam name="T">Object type associated with the repository operations.</typeparam>
    /// <typeparam name="PK">Unique identifier type for the object type.</typeparam>
    public interface IBatchRepository<T, PK>
    {
    }
}