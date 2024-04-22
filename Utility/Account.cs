using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public enum Privilege
    {
        Common, Admin,
    }
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Privilege Privilege { get; set; }
        public Account(string id, string name, Privilege privilege)
        {
            Id = id;
            Name = name;
            Privilege = privilege;
        }
    }
}
