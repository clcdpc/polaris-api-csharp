namespace Clc.Polaris.Api.Models
{
    /// <summary>
    /// Item status options for PatronItemsOutGet
    /// </summary>
    public enum PatronItemsOutGetStatus
    {
        /// <summary>
        /// All items
        /// </summary>
        All,

        /// <summary>
        /// Overdue items only
        /// </summary>
        Overdue,

        /// <summary>
        /// Lost items only
        /// </summary>
        Lost
    }
}
