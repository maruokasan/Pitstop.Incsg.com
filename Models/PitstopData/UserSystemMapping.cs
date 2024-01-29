using System;
using System.Collections.Generic;

namespace Pitstop.Models.PitstopData;

public partial class UserSystemMapping
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string SystemId { get; set; } = null!;

    public virtual System System { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
