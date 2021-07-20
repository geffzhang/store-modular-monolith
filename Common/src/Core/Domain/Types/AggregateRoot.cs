using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core.Domain.Exceptions;

namespace Common.Core.Domain.Types
{
    public abstract class AggregateRoot<TId, TIdentity> : EntityBase<TId, TIdentity>, IAggregateRoot
        where TIdentity : IdentityBase<TId>
    {
        private readonly List<IDomainEvent> _events = new();
        private bool _versionIncremented;
        public int Version { get; protected set; }
        
        // [JsonIgnore]
        public IEnumerable<IDomainEvent> Events => _events;

        protected AggregateRoot()
        {
        }

        protected AggregateRoot(TIdentity id) : base(id)
        {
        }
         
        protected void AddDomainEvent(IDomainEvent @event)
        {
            if (!_events.Any() && !_versionIncremented)
            {
                Version++;
                _versionIncremented = true;
            }

            _events.Add(@event);
        }

        protected void RemoveDomainEvent(IDomainEvent @event)
        {
            _events?.Remove(@event);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        protected void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleValidationException(rule);
        }

        protected void IncrementVersion()
        {
            if (_versionIncremented) return;

            Version++;
            _versionIncremented = true;
        }
    }

    public abstract class AggregateRoot : AggregateRoot<Guid, AggregateId>
    {
        protected AggregateRoot()
        {
        }
        protected AggregateRoot(AggregateId id) : base(id)
        {
        }
    }
}