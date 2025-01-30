using Cyotek.Windows.Forms;
using System.Diagnostics;

namespace MyGui.net
{
	public partial class FormTextEditor : Form
	{

		string _tempFilePath;
		FileSystemWatcher _watcher;
		ColorPickerDialog _picker;

		public FormTextEditor()
		{
			InitializeComponent();
			_picker = Util.NewFixedColorPickerDialog();
		}

		private void openExternallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_tempFilePath = Path.Combine(Application.ExecutablePath, "..", "MyGuiNetTextEditorSyncFile.txt");

			File.WriteAllText(_tempFilePath, mainTextBox.Text);
			// Open the temporary file in the external editor
			Process.Start(new ProcessStartInfo
			{
				FileName = _tempFilePath,
				UseShellExecute = true
			});

			// Set up the file watcher
			_watcher = new FileSystemWatcher
			{
				Path = Path.GetDirectoryName(Application.ExecutablePath),
				Filter = Path.GetFileName(_tempFilePath),
				NotifyFilter = NotifyFilters.LastWrite,
				EnableRaisingEvents = true
			};
			_watcher.Changed += OnSyncFileChanged;
		}

		private void OnSyncFileChanged(object sender, FileSystemEventArgs e)
		{
			// Read changes from the file and update the form's content
			if (!mainTextBox.Created)
			{
				return;
			}
			try
			{
				using (var stream = new FileStream(_tempFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (var reader = new StreamReader(stream))
				{
					Invoke(new Action(() => mainTextBox.Text = reader.ReadToEnd()));
				}
			}
			catch (IOException ex)
			{
				// Handle the case where the file is temporarily unavailable
				MessageBox.Show($"Error reading sync file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FormTextEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Util.IsValidFile(_tempFilePath))
			{
				File.Delete(_tempFilePath);
			}
		}

		private void interfaceTagToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Form1.tagForm.ShowDialog() == DialogResult.OK)
			{
				mainTextBox.Paste("#{" + Form1.tagForm.outcome + "}");
			}
		}

		private void colorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_picker.ShowDialog() == DialogResult.OK)
			{
				mainTextBox.Paste("#" + Util.ColorToHexString(_picker.Color));
			}
		}
	}
}
