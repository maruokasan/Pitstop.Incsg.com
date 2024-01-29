using System;
using System.Collections.Generic;

namespace Pitstop.Models.PitstopData;


public partial class DataControl
{
    public int Id { get; set; }

    public int? Parent { get; set; }

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }

    public int? SortOrder { get; set; }
}

