namespace Application.Wikies.Commands
{
    public interface IWikiCommand
    {
        Task ExecuteAsync();
        Task UndoAsync();
        string? Description { get; }
    }
}