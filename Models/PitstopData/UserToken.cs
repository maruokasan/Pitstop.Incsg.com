using System;
using System.Collections.Generic;

namespace Pitstop.Models.PitstopData;

public partial class UserToken
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
