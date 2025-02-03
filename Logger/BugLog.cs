namespace Inventory.Logger

{
    [Serializable]
    public class BugLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime DateCreated { get; set; }
        public string Description => $"{Message} - {DateCreated}";
    }
}
