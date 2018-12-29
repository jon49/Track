namespace Server.Pages.Teams
{
    public static class URL
    {
        public static string Edit(int teamId, int userId)
            => $"/teams/edit/{teamId}?userId={userId}";
        public static string EditForm(int teamId, int userId)
            => $"/teams/editform/{teamId}?userId={userId}";

        public const string ADD = "/teams/add";
        public const string NEW =  "/teams/new";
        public const string VIEW = "/teams/view";
    }
}
