﻿namespace Pitstop.Models
{
    public class UserLoginModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        public DateTime DateNow { get; set; }
    }
}
