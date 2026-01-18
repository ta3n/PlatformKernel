using System.ComponentModel.DataAnnotations;

namespace Liberty.Application.Domains.Models
{
    public abstract class BaseEntity<T>: IDeleteEntity, IEnableEntity
    {
        public virtual required T Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedDate { get;  set; } = DateTime.Now;
        public DateTime? UpdatedDate { get;  set; }
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
