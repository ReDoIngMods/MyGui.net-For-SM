using System.Diagnostics;

namespace MyGui.net
{
	public class CommandManager
    {
        private Stack<IEditorAction> _undoStack = new Stack<IEditorAction>();
        private Stack<IEditorAction> _redoStack = new Stack<IEditorAction>();

        public void ClearUndoStack()
        {
            _undoStack.Clear();
        }

        public void ClearRedoStack()
        {
            _redoStack.Clear();
        }

        public int GetUndoStackCount() {
            return _undoStack.Count;
        }

        public int GetRedoStackCount()
        {
            return _redoStack.Count;
        }

		public IEditorAction[] GetUndoStackItems()
		{
			return _undoStack.ToArray();
		}

		public IEditorAction[] GetRedoStackItems()
		{
			return _redoStack.ToArray();
		}

		public void ExecuteCommand(IEditorAction command, string reason = null)
        {
            if (command.Execute(reason)) //If command succeeded
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
