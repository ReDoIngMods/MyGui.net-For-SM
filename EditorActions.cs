using System.Drawing;

namespace MyGui.net
{
	public interface IEditorAction
	{
		bool Execute(string? reason = null); // Redo
		bool Undo(); // Undo
        string[] ToHumanReadable();
	}

	public class MoveCommand : IEditorAction
    {
		private string _reason = null;
		private MyGuiWidgetData _control;
        private Point _oldPosition;
        private Point _newPosition;

        public MoveCommand(MyGuiWidgetData control, Point newPosition)
        {
            _control = control;
            _oldPosition = control.position; // Store the initial position
            _newPosition = newPosition;
        }

        public MoveCommand(MyGuiWidgetData control, Point newPosition, Point oldPosition)
        {
            _control = control;
            _oldPosition = oldPosition; // Store the initial position
            _newPosition = newPosition;
        }

        public bool Execute(string? reason = null)
        {
            if (_oldPosition == _newPosition) { return false; }
            _reason = reason;
			//_control.Location = _newPosition; // Move to the new position
			_control.position = _newPosition;
            return true;
        }

        public bool Undo()
        {
            //_control.Location = _oldPosition; // Revert to the old position
            _control.position = _oldPosition;
            return true;
        }

        public string[] ToHumanReadable()
        {
            return [$"{(_reason != null ? $"({_reason}) " : "")}Widget Moved", $"From {_oldPosition} to {_newPosition}"];
        }

		public override string ToString()
        {
            return $"MoveCommand: {_control} from {_oldPosition} to {_newPosition}";
        }
    }

    public class ResizeCommand : IEditorAction
    {
		private string _reason = null;
		private MyGuiWidgetData _control;
        private Size _oldSize;
        private Size _newSize;

        public ResizeCommand(MyGuiWidgetData control, Size newSize)
        {
            _control = control;
            _oldSize = (Size)control.size;
            _newSize = newSize;
        }

        public ResizeCommand(MyGuiWidgetData control, Size newSize, Size oldSize)
        {
            _control = control;
            _oldSize = oldSize;
            _newSize = newSize;
        }

        public bool Execute(string? reason = null)
        {
            if (_oldSize == _newSize) { return false; }
			_reason = reason;
			//_control.Size = _newSize;
			_control.size = (Point)_newSize;
            return true;
        }

        public bool Undo() 
        { 
            //_control.size = _oldSize;
            _control.size = (Point)_oldSize;
            return true;
        }

		public string[] ToHumanReadable()
		{
			return [$"{(_reason != null ? $"({_reason}) " : "")}Widget Resized", $"From {_oldSize} to {_newSize}"];
		}

		public override string ToString()
        {
            return $"ResizeCommand: {_control} from {_oldSize} to {_newSize}";
        }
    }

    public class MoveResizeCommand : IEditorAction
    {
		private string _reason = null;
		private MyGuiWidgetData _control;
        private Point _oldPosition;
        private Point _newPosition;
        private Size _oldSize;
        private Size _newSize;

        public MoveResizeCommand(MyGuiWidgetData control, Point newPosition, Size newSize)
        {
            _control = control;
            _oldPosition = control.position; // Store the initial position
            _newPosition = newPosition;
            _oldSize = (Size)control.size;
            _newSize = newSize;
        }

        public MoveResizeCommand(MyGuiWidgetData control, Point newPosition, Size newSize, Point oldPosition, Size oldSize)
        {
            _control = control;
            _oldPosition = oldPosition; // Store the initial position
            _newPosition = newPosition;
            _oldSize = oldSize;
            _newSize = newSize;
        }

        public bool Execute(string? reason = null)
        {
            if (_oldPosition == _newPosition && _oldSize == _newSize) { return false; }
			_reason = reason;
			//_control.Location = _newPosition; // Move to the new position
			_control.position = _newPosition;
            //_control.Size = _newSize;
            _control.size = (Point)_newSize;
            return true;
        }

        public bool Undo()
        {
            //_control.Location = _oldPosition; // Revert to the old position
            _control.position = _oldPosition;
            //_control.Size = _oldSize;
            _control.size = (Point)_oldSize;
            return true;
        }

		public string[] ToHumanReadable()
		{
			return [$"{(_reason != null ? $"({_reason}) " : "")}Widget Moved & Resized", $"Moved from {_oldPosition} to {_newPosition}, resized from {_oldSize} to {_newSize}"];

        }

		public override string ToString()
        {
            return $"MoveResizeCommand: {_control} moved from {_oldPosition} to {_newPosition}, resized from {_oldSize} to {_newSize}";
        }
    }

    public class ChangePropertyCommand : IEditorAction
    {
		private string _reason = null;
		private MyGuiWidgetData _control;
        private object _oldValue;
        private object _newValue;
        private string _property;

        public ChangePropertyCommand(MyGuiWidgetData control, string property, object newValue)
        {
            _control = control;
            _property = property;
            _oldValue = Util.GetPropertyValue(control, property);
            _newValue = newValue;
        }

        public ChangePropertyCommand(MyGuiWidgetData control, string property, object newValue, object oldValue)
        {
            _control = control;
            _property = property;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public bool Execute(string? reason = null) 
        {
            if (_oldValue == _newValue) { return false; }
			_reason = reason;
			return Util.SetPropertyValue(_control, _property, _newValue);
        }
        public bool Undo() 
        { 
            return Util.SetPropertyValue(_control, _property, _oldValue);
        }

		public string[] ToHumanReadable()
		{
			return [$"{(_reason != null ? $"({_reason}) " : "")}Widget Property Changed", $"Property \"{_property}\" changed from \"{_oldValue}\" to \"{_newValue}\""];
		}

		public override string ToString()
        {
            return $"ChangePropertyCommand: {_control} changed property \"{_property}\" from \"{_oldValue}\" to \"{_newValue}\"";
        }
    }

    public class CreateControlCommand : IEditorAction
    {
		private string _reason = null;
		private MyGuiWidgetData _control;
        private MyGuiWidgetData _parent;
        private int _index;
        private List<MyGuiWidgetData> _defaultList;

        public CreateControlCommand(MyGuiWidgetData control, MyGuiWidgetData parent, List<MyGuiWidgetData> defaultList = null)
        {
            _control = control;
            _parent = parent;
            _defaultList = defaultList;
        }

        public bool Execute(string? reason = null)
        {
			_reason = reason;
			if (_parent == null)
            {
                if (_defaultList != null)
                {
                    _defaultList.Add(_control);
                    _index = _defaultList.Count - 1;
                }
                return true;
            }
            if (_control != null)
            {
                _parent.children.Add(_control);
                _index = _parent.children.Count - 1;
            }
            else
            {
                _parent.children.Add(new MyGuiWidgetData());
                _index = _parent.children.Count - 1;
            }
            return true;
        }

        public bool Undo()
        {
            if (_parent != null)
            {
                _parent.children.RemoveAt(_index);
            }
            else if (_defaultList != null)
            {
                _defaultList.RemoveAt(_index);
            }
            return true;
        }

		public string[] ToHumanReadable()
		{
			return [$"{(_reason != null ? $"({_reason}) " : "")}Widget Created", $"Created widget {_control} with parent {(_parent == null ? "None" : _parent.ToString())}"];
		}

		public override string ToString()
        {
            return $"CreateControlCommand: created widget {_control} with parent {(_parent == null ? "None" : _parent.ToString())}";
        }
    }

    public class DeleteControlCommand : IEditorAction
    {
		private string _reason = null;
		private MyGuiWidgetData _control;
        private MyGuiWidgetData _parent;
        private List<MyGuiWidgetData> _defaultList;
        private int _index; // Store the index to restore control in the correct position

        //TODO: figure out structure
        public DeleteControlCommand(MyGuiWidgetData control, List<MyGuiWidgetData> defaultList)
        {
            _control = control;
            _parent = control.Parent;
            _defaultList = defaultList;
            _index = _parent == null ? _defaultList.IndexOf(control) : _parent.children.IndexOf(control); // Store the original position
        }

        public DeleteControlCommand(MyGuiWidgetData control, MyGuiWidgetData parent, List<MyGuiWidgetData> defaultList = null)
        {
            _control = control;
            _parent = parent;
            _defaultList = defaultList;
            _index = _parent == null ? _defaultList.IndexOf(control) : _parent.children.IndexOf(control); // Store the original position
        }

        public bool Execute(string? reason = null)
        {
			_reason = reason;
			if (_parent != null)
            {
                _parent.children.RemoveAt(_index);
            }
            else if (_defaultList != null)
            {
                _defaultList.RemoveAt(_index);
            }
            return true;
        }

        public bool Undo()
        {
            if (_parent != null)
            {
                _parent.children.Insert(_index, _control);
            }
            else if (_defaultList != null)
            {
                _defaultList.Insert(_index, _control);
            }
            return true;
        }

		public string[] ToHumanReadable()
		{
			return [$"{(_reason != null ? $"({_reason}) " : "")}Widget Deleted", $"Deleted widget {_control} from within parent {(_parent == null ? "None" : _parent.ToString())} at index {_index}"];
		}

		public override string ToString()
        {
            return $"DeleteControlCommand: deleted widget {_control} from within parent {_parent} at index {_index}";
        }
    }

	public class CompoundCommand : IEditorAction
	{
        private string _reason = null;
        private List<IEditorAction> _actions;

		public CompoundCommand(List<IEditorAction> actions)
		{
            _actions = actions;
		}

		public bool Execute(string? reason = null)
		{
            _reason = reason;
			foreach (var item in _actions)
            {
                item.Execute();
            }
			return true;
		}

		public bool Undo()
		{
			for (int i = _actions.Count - 1; i >= 0; i--)
			{
				_actions[i].Undo();
			}
			return true;
		}

		public string[] ToHumanReadable()
		{
			var serialized = string.Join( "|&|", _actions.Select(a => string.Join("|-|", a.ToHumanReadable())));

			return [$"{(_reason != null ? $"({_reason}) " : "")}Multiple Actions", serialized];
		}

		public override string ToString()
		{
			var actionsStr = string.Join(", ", _actions.Select(a => a.ToString()));
			return $"CompountCommand: [{actionsStr}]";
		}
	}
}
