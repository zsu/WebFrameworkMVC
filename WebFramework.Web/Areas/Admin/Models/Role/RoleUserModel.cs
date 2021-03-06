﻿using System;
using System.Collections.Generic;

namespace Web.Areas.Admin.Models
{
    public class RoleUserModel
    {
        public RoleUserModel()
        {
            Users = new List<User>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<User> Users { get; set; }
        public class User
        {
            public Guid Id { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    } 
}