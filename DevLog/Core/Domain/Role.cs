﻿using Microsoft.AspNetCore.Identity;

namespace DevLog.Core.Domain
{
    public class Role : IdentityRole<int>
    {
        public override string Name { get; set; } = string.Empty;
    }
}
