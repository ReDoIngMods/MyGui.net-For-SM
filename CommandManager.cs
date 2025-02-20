using System.Diagnostics;

namespace MyGui.net
{
	public class CommandManager
    {
        private Stack<IEditorAction> _undoStack = new Stack<IEditorAction>();
        private Stack<IEditorAction> _redoStack = new Stack<IEditorAction>();

        public void clearUndoStack()
        {
            _undoStack.Clear();
        }

        public void clearRedoStack()
        {
            _redoStack.Clear();
        }

        public int getUndoStackCount() {
            return _undoStack.Count;
        }

        public int getRedoStackCount()
        {
            return _redoStack.Count;
        }

        public void ExecuteCommand(IEditorAction command)
        {
            if (command.Execute()) //If command succeeded
            {
                DebugConsole.WriteLine($"Executed \"{command.ToString()}\"", DebugConsole.LogLevels.Info);
                _undoStack.Push(command);
                _redoStack.Clear();
                //this.PrintStacks();
            }
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
				DebugConsole.WriteLine($"Undone \"{command.ToString()}\"", DebugConsole.LogLevels.Info);
				command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
				DebugConsole.WriteLine($"Redone \"{command.ToString()}\"", DebugConsole.LogLevels.Info);
				command.Execute();
                _undoStack.Push(command);
            }
        }

        public void PrintStacks()
        {
            Debug.WriteLine("Undo Stack Count: " + _undoStack.Count);

            Debug.WriteLine("Undo Stack:");
            foreach (var command in _undoStack)
            {
                Debug.WriteLine(command.ToString());
            }
        }
    }
}
