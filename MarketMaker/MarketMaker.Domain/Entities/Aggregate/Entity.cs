using MarketMaker.Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MarketMaker.Domain.Entities.Aggregate
{
    public abstract class Entity
    {
        int? _requestedHashCode;
        long _Id;
        [Key]
        public long Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }
        public Guid GUID { get; protected set; }
        public DateTime CreatedDate { get; protected set; }
        public long CreatedBy { get; protected set; }
        public DateTime? UpdatedDate { get; protected set; }
        [DefaultValue(0)]
        public long UpdatedBy { get; protected set; }
        //public int Status { get; protected set; }
        public Status Status { get; protected set; }

        /// <summary>
        /// Implementation of Create. Required parameters.
        /// </summary>
        public void SetCreate(long createdBy, DateTime createdDate)
        {
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }

        /// <summary>
        /// Implementation of Update. Required parameters.
        /// </summary>
        public void SetUpdate(long updatedBy, DateTime? updatedDate)
        {
            UpdatedBy = updatedBy;
            UpdatedDate = updatedDate;
        }

        /// <summary>
        /// Implementation of Update. In case if we supposed to update the Status
        /// </summary>
        public void SetStatusUpdate(Status status)
        {
            Status = status;
        }
        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return this.Id == default(Int32);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
