using System.Collections.Generic;
using Common.Core.Domain.Types;

namespace OnlineStore.Modules.Identity.Domain.Aggregates.Users
{
    public sealed class PermissionScope : ValueObject
    {
        public PermissionScope()
        {
            Type = GetType().Name;
        }

        /// <summary>
        /// Scope type name
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Display label for particular scope value used only for  representation 
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Represent string scope value
        /// </summary>
        public string Scope { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Type;
            yield return Scope;
        }

        public void Patch(PermissionScope target)
        {
            target.Label = Label;
            target.Scope = Scope;
        }
    }
}