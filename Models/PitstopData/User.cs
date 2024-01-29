using System;
using System.Collections.Generic;

namespace Pitstop.Models.PitstopData;

public partial class User
{
    public string Id { get; set; } = null!;

    public int? EmployeeId { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public string? CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public string? AccessTypeId { get; set; }

    public bool? IsUserActive { get; set; }
    
    public DateTime? LastAccessDate { get; set; }
    public string? FirstName { get; set; }
    
    public virtual ICollection<UserSystemMapping> UserSystemMappings { get; set; } = new List<UserSystemMapping>();

    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public ICollection<UserRoles> UserRoles { get; set; }
}
