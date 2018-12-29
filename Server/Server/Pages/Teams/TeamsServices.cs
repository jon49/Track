using System.Collections.Generic;

namespace Server.Pages.Teams
{
    public interface ITeamServices
    {
        IEnumerable<Team> GetAll(int userId);
        //Result Get(int userId, int teamId);
        //Result Save()
    }

    public class TeamsServices : ITeamServices
    {
        public IEnumerable<Team> GetAll(int userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
