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
			_oldPosition = control.position;
			_newPosition = newPosition;
		}

		public MoveCommand(MyGuiWidgetData control, Point newPosition, Point oldPosition)
		{
			_control = control;
			_oldPosition = oldPosition;
			_newPosition = newPosition;
		}

		public bool Execute(string? reason = null)
		{
			if (_oldPosition == _newPosition) return false;
			_reason = reason;
			SetWrappedProperty("Position", _newPosition);
			return true;
		}

		public bool Undo()
		{
			SetWrappedProperty("Position", _oldPosition);
			return true;
		}

		public string[] ToHumanReadable() =>
			[$"{(_reason != null ? $"({_reason}) " : "")}Widget Moved", $"From {_oldPosition} to {_newPosition}"];

		public override string ToString() =>
			$"MoveCommand: {_control} from {_oldPosition} to {_newPosition}";

		private void SetWrappedProperty(string property, object value)
		{
			object wrapper = new MyGuiWidgetDataWidget(_control);
			var propInfo = wrapper.GetType().GetProperty(property);
			propInfo?.SetValue(wrapper, value);
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
			if (_oldSize == _newSize) return false;
			_reason = reason;
			SetWrappedProperty("Size", (Point)_newSize);
			return true;
		}

		public bool Undo()
		{
			SetWrappedProperty("Size", (Point)_oldSize);
			return true;
		}

		public string[] ToHumanReadable() =>
			[$"{(_reason != null ? $"({_reason}) " : "")}Widget Resized", $"From {_oldSize} to {_newSize}"];

		public override string ToString() =>
			$"ResizeCommand: {_control} from {_oldSize} to {_newSize}";

		private void SetWrappedProperty(string property, object value)
		{
			object wrapper = new MyGuiWidgetDataWidget(_control);
			var propInfo = wrapper.GetType().GetProperty(property);
			propInfo?.SetValue(wrapper, value);
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
			_oldPosition = control.position;
			_newPosition = newPosition;
			_oldSize = (Size)control.size;
			_newSize = newSize;
		}

		public MoveResizeCommand(MyGuiWidgetData control, Point newPosition, Size newSize, Point oldPosition, Size oldSize)
		{
			_control = control;
			_oldPosition = oldPosition;
			_newPosition = newPosition;
			_oldSize = oldSize;
			_newSize = newSize;
		}

		public bool Execute(string? reason = null)
		{
			object wrapper = new MyGuiWidgetDataWidget(_control);
			bool changed = false;
			_reason = reason;

			if (_oldPosition != _newPosition)
			{
				SetWrappedProperty(wrapper, "Position", _newPosition);
				changed = true;
			}

			if (_oldSize != _newSize)
			{
				SetWrappedProperty(wrapper, "Size", (Point)_newSize);
				changed = true;
			}

			return changed;
		}


		public bool Undo()
		{
			object wrapper = new MyGuiWidgetDataWidget(_control);
			bool changed = false;

			if (_oldPosition != _newPosition)
			{
				SetWrappedProperty(wrapper, "Position", _oldPosition);
				changed = true;
			}

			if (_oldSize != _newSize)
			{
				SetWrappedProperty(wrapper, "Size", (Point)_oldSize);
				changed = true;
			}

			return changed;
		}

		public string[] ToHumanReadable() =>
			[$"{(_reason != null ? $"({_reason}) " : "")}Widget Moved & Resized",
		$"Moved from {_oldPosition} to {_newPosition}, resized from {_oldSize} to {_newSize}"];

		public override string ToString() =>
			$"MoveResizeCommand: {_control} moved from {_oldPosition} to {_newPosition}, resized from {_oldSize} to {_newSize}";

		private void SetWrappedProperty(object wrapper, string property, object value)
		{
			var propInfo = wrapper.GetType().GetProperty(property);
			propInfo?.SetValue(wrapper, value);
		}
	}


	public class ChangePropertyCommand : IEditorAction
	{
		private string? _reason;
		private MyGuiWidgetData _control;
		private object? _oldValue;
		private object? _newValue;
		private string _property;

		public ChangePropertyCommand(MyGuiWidgetData control, string property, object newValue)
		{
			_control = control;
			_property = property;
			_newValue = newValue;

			object wrapper = WrapControl(control);
			var propInfo = wrapper.GetType().GetProperty(_property);
			_oldValue = propInfo?.GetValue(wrapper);
		}

		public ChangePropertyCommand(MyGuiWidgetData control, string property, object newValue, object oldValue)
		{
			_control = control;
			_property = property;
			_newValue = newValue;
			_oldValue = oldValue;
		}

		public bool Execute(string? reason = null)
		{
			if (Equals(_oldValue, _newValue)) return false;
			_reason = reason;

			object wrapper = WrapControl(_control);
			var propInfo = wrapper.GetType().GetProperty(_property);
			propInfo?.SetValue(wrapper, _newValue);
			return true;
		}

		public bool Undo()
		{
			object wrapper = WrapControl(_control);
			var propInfo = wrapper.GetType().GetProperty(_property);
			propInfo?.SetValue(wrapper, _oldValue);
			return true;
		}

		public string[] ToHumanReadable()
		{
			return [$"{(_reason != null ? $"({_reason}) " : "")}Widget Property Changed",
				$"Property \"{_property}\" changed from \"{_oldValue}\" to \"{_newValue}\""];
		}

		public override string ToString()
		{
			return $"ChangePropertyCommand: {_control} changed property \"{_property}\" from \"{_oldValue}\" to \"{_newValue}\"";
		}

		private static object WrapControl(MyGuiWidgetData control)
		{
			// Default to Widget wrapper if not found
			Type wrapperType = Form1.WidgetTypeToObjectType.TryGetValue(control.type, out var resolvedType)
				? resolvedType
				: typeof(MyGuiWidgetDataWidget);

			return new MyGuiWidgetDataWidget(control).ConvertTo(wrapperType);
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

	// Moves a widget between parents and/or sibling positions. Used by the Layout-tab
	// drag-drop, Move Up/Down buttons, and Alt+arrow shortcuts.
	public class ReorderCommand : IEditorAction
	{
		private string _reason;
		private readonly MyGuiWidgetData _widget;
		private readonly MyGuiWidgetData _oldParent;
		private readonly int _oldIndex;
		private readonly MyGuiWidgetData _newParent;
		private int _newIndex;
		private readonly List<MyGuiWidgetData> _rootList;

		public ReorderCommand(MyGuiWidgetData widget, MyGuiWidgetData oldParent, int oldIndex,
			MyGuiWidgetData newParent, int newIndex, List<MyGuiWidgetData> rootList)
		{
			_widget = widget;
			_oldParent = oldParent;
			_oldIndex = oldIndex;
			_newParent = newParent;
			_newIndex = newIndex;
			_rootList = rootList;
		}

		public bool Execute(string? reason = null)
		{
			_reason = reason;
			if (_oldParent == _newParent && _oldIndex == _newIndex) return false;

			RemoveFrom(_oldParent, _oldIndex);

			// If moving within the same list and the new index is past the removed item,
			// the indices have shifted by one.
			int insertAt = _newIndex;
			if (_oldParent == _newParent && _newIndex > _oldIndex) insertAt--;
			insertAt = Math.Clamp(insertAt, 0, ListFor(_newParent).Count);
			_newIndex = insertAt; // remember the actual index used so Undo is symmetric

			InsertInto(_newParent, insertAt, _widget);
			return true;
		}

		public bool Undo()
		{
			RemoveFrom(_newParent, _newIndex);
			int insertAt = _oldIndex;
			// Same kind of compensation in reverse.
			if (_oldParent == _newParent && _oldIndex > _newIndex) insertAt--;
			insertAt = Math.Clamp(insertAt, 0, ListFor(_oldParent).Count);
			InsertInto(_oldParent, insertAt, _widget);
			return true;
		}

		private IList<MyGuiWidgetData> ListFor(MyGuiWidgetData parent)
			=> parent != null ? (IList<MyGuiWidgetData>)parent.children : _rootList;

		private void RemoveFrom(MyGuiWidgetData parent, int index)
		{
			var list = ListFor(parent);
			if (index < 0 || index >= list.Count) return;
			list.RemoveAt(index);
		}

		private void InsertInto(MyGuiWidgetData parent, int index, MyGuiWidgetData widget)
		{
			var list = ListFor(parent);
			index = Math.Clamp(index, 0, list.Count);
			list.Insert(index, widget);
		}

		public string[] ToHumanReadable() =>
			[$"{(_reason != null ? $"({_reason}) " : "")}Widget Reordered",
				$"Moved \"{_widget.name ?? "[DEFAULT]"}\" from {(_oldParent?.name ?? "<root>")}[{_oldIndex}] to {(_newParent?.name ?? "<root>")}[{_newIndex}]"];

		public override string ToString() =>
			$"ReorderCommand: {_widget} from {(_oldParent?.ToString() ?? "<root>")}[{_oldIndex}] to {(_newParent?.ToString() ?? "<root>")}[{_newIndex}]";
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
			return $"CompoundCommand: [{actionsStr}]";
		}
	}
}
