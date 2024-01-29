using System;
using System.Collections.Generic;

namespace Pitstop.Models.PitstopData;

public partial class SystemPermission
{
    public int Id { get; set; }

    public string SystemId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual System System { get; set; } = null!;
}
