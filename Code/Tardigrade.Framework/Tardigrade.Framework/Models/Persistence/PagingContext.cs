namespace Tardigrade.Framework.Models.Persistence
{
    /// <summary>
    /// Paging parameters.
    /// </summary>
    public class PagingContext
    {
        /// <summary>
        /// Page index (starts at 0).
        /// </summary>
        public uint PageIndex { get; set; }

        /// <summary>
        /// Number of items on the page. If 0, it is assumed no items will be returned.
        /// </summary>
        public uint PageSize { get; set; }
    }
}