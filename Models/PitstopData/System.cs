using System;
using System.Collections.Generic;

namespace Pitstop.Models.PitstopData;

public partial class System
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string LogoPath { get; set; } = null!;

    public string CssbackgroundColor { get; set; } = null!;

    public bool IsExternal { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; }

    public bool AppliableCampaign { get; set; }

    public int? SortOrder { get; set; }

    public virtual ICollection<SystemPermission> SystemPermissions { get; set; } = new List<SystemPermission>();

    public virtual ICollection<UserSystemMapping> UserSystemMappings { get; set; } = new List<UserSystemMapping>();
}
