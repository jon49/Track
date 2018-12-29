using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Server.Pages.Teams
{
    public class TeamsIndexModel : BasePageModel
    {
        private static List<Team> _Teams = new List<Team>
        {
            new Team
            {
                TeamId = 1,
                UserId = 1,
                Email = "paul@123.com",
                FirstName = "Paul",
                LastName = "Paolo",
                TeamName = "Fastest Team Ever",
            },
            new Team
            {
                TeamId = 2,
                UserId = 2,
                Email = "jorge@321.com",
                FirstName = "Jorge",
                LastName = "George",
                TeamName = "Really Fast Team",
            },
        };

        public List<Team> Teams
        {
            get => _Teams;
            set
            {
                _Teams.AddRange(value);
            }
        }

        public void OnGet() { }

        public PartialViewResult OnGetView()
            => PartialView("_Teams", Teams);

        public PartialViewResult OnGetCancel()
            => Partial("_AddTeamButton");

        public PartialViewResult OnGetAdd()
            => PartialView("_AddTeam", new Team());

        public PartialViewResult OnGetEditForm(int id, int userId)
            => PartialView("_AddTeam", _Teams.Find(x => x.UserId == userId && x.TeamId == id));

        public PartialViewResult OnGetEdit(int id, int userId)
            => PartialView("_Team", _Teams.Find(x => x.TeamId == id && x.UserId == userId));

        public IActionResult OnPostEdit(int id, int userId, Team team)
            => ModelState.IsValid ? EditTeam(team, id, userId) : new NoContentResult();

        /***************** New *****************/
        public PartialViewResult OnGetNew()
        {
            var team = Teams.LastOrDefault();
            if (team is null)
            {
                return null;
            }
            return PartialView("_TeamRow", team);
        }

        public IActionResult OnPostNew(TeamPost team)
            => ModelState.IsValid ? AddTeam(team) : new NoContentResult();

        private IActionResult AddTeam(TeamPost team)
        {
            var id = Teams.Count + 1;
            _Teams.Add(Team.From(team, id, id));
            return Partial("_AddTeamButton");
        }

        private IActionResult EditTeam(Team team, int teamId, int userId)
        {
            team.TeamId = teamId;
            team.UserId = userId;
            var index = Teams.FindIndex(x => team.TeamId == x.TeamId);
            Teams.RemoveAt(index);
            Teams.Insert(index, team);
            return Partial("_AddTeamButton");
        }

    }
}