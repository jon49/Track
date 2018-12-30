using System;

namespace Server.Pages.Teams
{
    public interface ITeam
    {
        string TeamName { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        DateTime TimeStamp { get; set; }
    }
}
