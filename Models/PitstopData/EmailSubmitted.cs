using System;
using System.Collections.Generic;

namespace Pitstop.Models.PitstopData;

public partial class EmailSubmitted
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? EmailSubject { get; set; }

    public bool? IsValid { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? EmailSubmitTypeId { get; set; }
}
