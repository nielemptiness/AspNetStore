using System;
using Store.Core.Contracts.Enums;
using Store.Core.Contracts.Interfaces.Models;

namespace Store.Core.Contracts.Domain
{
    public class Role : IIdentity, IEntity, IAuditable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RoleType RoleType { get; set; }
        public bool IsActive { get; set; }
        public string[] Actions { get; set; }
        public DateTime Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? Edited { get; set; }
        public Guid? EditedBy { get; set; }
    }
}