using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheWorld.Models
{
    public class WorldUser : IdentityUser // inherit from IdentityUser so the entity can easily support Identity
    {
        public DateTime FirstTrip { get; set; }


    }
}