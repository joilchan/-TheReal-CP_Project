using System;
using System.Collections.Generic;

namespace BSAPI.Models;

public partial class Cart
{
    public int UserId { get; set; }

    public int ProductVariantId { get; set; }

    public int Quantity { get; set; }

    public virtual ProductVariant ProductVariant { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
