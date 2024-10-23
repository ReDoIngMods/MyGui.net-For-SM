using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGui.net
{
    public interface IEditorAction
    {
        bool Execute(); // Redo
        bool Undo();    // Undo
    }
}
