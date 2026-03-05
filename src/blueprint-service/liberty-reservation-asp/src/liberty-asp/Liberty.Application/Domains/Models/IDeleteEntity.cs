namespace Liberty.Application.Domains.Models
{
    public  interface IDeleteEntity
    {
        /// <summary>
        /// Is Deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
