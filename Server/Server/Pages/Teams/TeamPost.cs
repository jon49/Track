using System;
using System.ComponentModel.DataAnnotations;

namespace Server.Pages.Teams
{
    public class TeamPost : ITeam
    {
        [Required, Display(Name = "Team Name")]
        public string TeamName { get; set; } = "";
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; } = "";
        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; } = "";
        [Required,EmailAddress]
        public string Email { get; set; } = "";
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
