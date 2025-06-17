using Application.Wikies.Commands;

namespace Application.Wikies
{
    public class WikiCommandManager
    {
        private IWikiCommand? _command;
        private readonly Stack<IWikiCommand> _commandHistory = new();

        public void SetCommand(IWikiCommand command)
        {
            _command = command;
        }

        public async Task ExecuteCommandAsync()
        {
            if (_command != null)
            {
                await _command.ExecuteAsync();
                _commandHistory.Push(_command);
            }
        }

        public async Task<UndoResult> UndoLastCommandAsync()
        {
            if (_commandHistory.Count > 0)
            {
                var lastCommand = _commandHistory.Pop();
                await lastCommand.UndoAsync();
                return new UndoResult
                {
                    Success = true,
                    Message = lastCommand.Description != null
                        ? $"Đã undo thao tác: {lastCommand.Description}"
                        : "Đã undo thao tác cuối cùng."
                };
            }
            return new UndoResult
            {
                Success = false,
                Message = "Không còn thao tác nào để undo."
            };
        }

        public bool CanUndo => _commandHistory.Count > 0;

        public void ClearHistory()
        {
            _commandHistory.Clear();
        }

        public int UndoableCount => _commandHistory.Count;
    }
}
