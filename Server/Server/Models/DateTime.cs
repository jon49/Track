namespace Server.Models
{
    public struct DateTime
    {
        private readonly System.DateTime dateTime;

        private DateTime(System.DateTime dateTime)
        {
            this.dateTime = dateTime;
        }

        public static DateTime UtcNow
            => new DateTime(System.DateTime.UtcNow);
    }
}
