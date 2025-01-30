namespace MyGui.net
{
	public interface IEditorAction
    {
        bool Execute(); // Redo
        bool Undo();    // Undo
    }
}
