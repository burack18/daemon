using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonTest
{
    public struct UserStructure
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public UserStructure(string email)
        {
            Email = email;
            Name = string.Empty;
        }
        public UserStructure(string email,string name)
        {
            Name = name;
            Email = email;
        }

    }
}
