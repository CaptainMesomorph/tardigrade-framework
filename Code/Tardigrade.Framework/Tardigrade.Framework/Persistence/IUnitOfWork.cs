namespace Tardigrade.Framework.Persistence
{
    /// <summary>
    /// Interface used to define a Unit of Work (such as a transaction or session).
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Begin a Unit of Work.
        /// </summary>
        void Begin();

        /// <summary>
        /// Commit a Unit of Work.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback a Unit of Work.
        /// </summary>
        void Rollback();
    }
}