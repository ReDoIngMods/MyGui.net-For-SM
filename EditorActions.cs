using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MyGui.net
{
    public class MoveCommand : IEditorAction
    {
        private Control _control;
        private Point _oldPosition;
        private Point _newPosition;

        public MoveCommand(Control control, Point newPosition)
        {
            _control = control;
            _oldPosition = control.Location; // Store the initial position
            _newPosition = newPosition;
        }

        public MoveCommand(Control control, Point newPosition, Point oldPosition)
        {
            _control = control;
            _oldPosition = oldPosition; // Store the initial position
            _newPosition = newPosition;
        }

        public bool Execute()
        {
            if (_oldPosition == _newPosition) { return false; }
            _control.Location = _newPosition; // Move to the new position
            ((MyGuiWidgetData)_control.Tag).position = _newPosition;
            return true;
        }

        public bool Undo()
        {
            _control.Location = _oldPosition; // Revert to the old position
            ((MyGuiWidgetData)_control.Tag).position = _oldPosition;
            return true;
        }

        public override string ToString()
        {
            return $"MoveCommand: {_control.Name} from {_oldPosition} to {_newPosition}";
        }
    }

    public class ResizeCommand : IEditorAction
    {
        private Control _control;
        private Size _oldSize;
        private Size _newSize;

        public ResizeCommand(Control control, Size newSize)
        {
            _control = control;
            _oldSize = control.Size;
            _newSize = newSize;
        }

        public ResizeCommand(Control control, Size newSize, Size oldSize)
        {
            _control = control;
            _oldSize = oldSize;
            _newSize = newSize;
        }

        public bool Execute()
        {
            if (_oldSize == _newSize) { return false; }
            _control.Size = _newSize;
            ((MyGuiWidgetData)_control.Tag).size = (Point)_newSize;
            return true;
        }
        public bool Undo() 
        { 
            _control.Size = _oldSize;
            ((MyGuiWidgetData)_control.Tag).size = (Point)_oldSize;
            return true;
        }

        public override string ToString()
        {
            return $"ResizeCommand: {_control.Name} from {_oldSize} to {_newSize}";
        }
    }

    public class ChangePropertyCommand : IEditorAction
    {
        private Control _control;
        private object _oldValue;
        private object _newValue;
        private string _property;

        public ChangePropertyCommand(Control control, string property, object newValue)
        {
            _control = control;
            _property = property;
            _oldValue = Util.GetPropertyValue(((MyGuiWidgetData)control.Tag), property);
            _newValue = newValue;
        }

        public ChangePropertyCommand(Control control, string property, object newValue, object oldValue)
        {
            _control = control;
            _property = property;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public bool Execute() 
        {
            if (_oldValue == _newValue) { return false; }
            return Util.SetPropertyValue((MyGuiWidgetData)_control.Tag, _property, _newValue);
        }
        public bool Undo() 
        { 
            return Util.SetPropertyValue((MyGuiWidgetData)_control.Tag, _property, _oldValue);
        }

        public override string ToString()
        {
            return $"ChangePropertyCommand: {_control.Name} changed property {_property} from {_oldValue} to {_newValue}";
        }
    }

    public class CreateControlCommand : IEditorAction
    {
        private Control _control;
        private Control _parent;

        public CreateControlCommand(Control control, Control parent)
        {
            _control = control;
            _parent = parent;
        }

        public bool Execute()
        {
            _parent.Controls.Add(_control); // Add the control to the parent container
            if (_control.Tag != null && (MyGuiWidgetData)_control.Tag != null)
            {
                ((MyGuiWidgetData)_parent.Tag).children.Add((MyGuiWidgetData)_control.Tag);
            }
            else
            {
                ((MyGuiWidgetData)_parent.Tag).children.Add(new MyGuiWidgetData());
            }
            return true;
        }

        public bool Undo()
        {
            int childIndex = _parent.Controls.GetChildIndex(_control);
            _parent.Controls.Remove(_control); // Remove the control from the parent container
            if (_parent.Tag != null && (MyGuiWidgetData)_parent.Tag != null)
            {
                ((MyGuiWidgetData)_control.Tag).children.RemoveAt(childIndex);
            }
            return true;
        }

        public override string ToString()
        {
            return $"CreateControlCommand: created control {_control} with parent {_parent}";
        }
    }

    public class DeleteControlCommand : IEditorAction
    {
        private Control _control;
        private Control _parent;
        private int _index; // Store the index to restore control in the correct position

        public DeleteControlCommand(Control control, Control parent)
        {
            _control = control;
            _parent = parent;
            _index = _parent.Controls.IndexOf(control); // Store the original position
        }

        public bool Execute()
        {
            _parent.Controls.Remove(_control); // Remove the control from its parent
            if (_control.Tag != null && (MyGuiWidgetData)_control.Tag != null)
            {
                ((MyGuiWidgetData)_control.Tag).children.RemoveAt(_index);
            }
            return true;
        }

        public bool Undo()
        {
            _parent.Controls.Add(_control);  // Re-add the control to its parent
            _parent.Controls.SetChildIndex(_control, _index); // Restore to original position
            if (_parent.Tag != null && (MyGuiWidgetData)_parent.Tag != null)
            {
                ((MyGuiWidgetData)_parent.Tag).children.Insert(_index, (MyGuiWidgetData)_control.Tag);
            }
            return true;
        }

        public override string ToString()
        {
            return $"DeleteControlCommand: deleted control {_control} from parent {_parent} at index {_index}";
        }
    }
}
