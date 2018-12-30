using System.ComponentModel.DataAnnotations;

namespace Server.Pages.Teams
{
    public class Team : TeamPost
    {
        [Required, Range(1, int.MaxValue)]
        public int UserId { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int TeamId { get; set; }

        public static Team From(TeamPost post, int teamId, int userId)
            => new Team
            {
                Email = post.Email,
                FirstName = post.FirstName,
                LastName = post.LastName,
                TeamId = teamId,
                TeamName = post.TeamName,
                TimeStamp = post.TimeStamp,
                UserId = userId,
            };
    }
}
