﻿namespace Web.Models
{
    public class AuthenticationModel
    {
        public string Application { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}