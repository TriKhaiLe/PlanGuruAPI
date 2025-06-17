namespace PlanGuruAPI.DTOs.WikiDTOs
{
    internal class UndoResponse
    {
        public string? Msg { get; set; }
        public bool CanUndo { get; set; }
        public string? Content { get; set; }
        public int ContributorCount { get; set; }

        public UndoResponse(string msg, bool canUndo, string? content, int contributorCount)
        {
            Msg = msg;
            CanUndo = canUndo;
            Content = content;
            ContributorCount = contributorCount;
        }

        public UndoResponse()
        {
        }

        public override bool Equals(object? obj)
        {
            return obj is UndoResponse other &&
                   Msg == other.Msg &&
                   CanUndo == other.CanUndo &&
                   Content == other.Content &&
                   ContributorCount == other.ContributorCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Msg, CanUndo, Content, ContributorCount);
        }
    }
}
