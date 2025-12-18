using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Scripts.Cloud.Schemas
{
    public class PlayerProfile
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public PlayerProfile() { }
    }
}
