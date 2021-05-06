using System;

namespace Common.Domain.Types
{
    public class AuditableEntity<TId, TIdentity> : EntityBase<TId, TIdentity>, IAuditable
        where TIdentity : IdentityBase<TId>
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}