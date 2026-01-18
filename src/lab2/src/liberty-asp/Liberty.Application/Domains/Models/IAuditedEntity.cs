namespace Liberty.Application.Domains.Models
{
    public interface IAuditedEntity
    {
        /// <summary>
        /// Create Date
        /// </summary>
        DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Create By
        /// </summary>
        string? CreateBy { get; set; }

        /// <summary>
        /// Update Date
        /// </summary>
        DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Update By
        /// </summary>
        string? UpdateBy { get; set; }
    }
}
