namespace Inventory.Logger
{
    [Serializable]
    public class BugLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public string Description => $"{Message} - {DateCreated}";
    }
}
