using Cyotek.Windows.Forms;
using SkiaSharp;
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
			if (Form1.TagForm.ShowDialog() == DialogResult.OK)
			{
				mainTextBox.Paste("#{" + Form1.TagForm.outcome + "}");
			}
		}

		private void colorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_picker.ShowDialog() == DialogResult.OK)
			{
				mainTextBox.Paste("#" + Util.ColorToHexString(_picker.Color));
			}
		}

		SKMatrix _viewportMatrix = SKMatrix.Identity;

		private void previewViewport_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			SKCanvas canvas = e.Surface.Canvas;
			canvas.Clear(new SKColor(105, 105, 105));

			// Get the control's size
			var controlWidth = previewViewport.Width;
			var controlHeight = previewViewport.Height;

			_viewportMatrix = SKMatrix.CreateTranslation(-hScrollBar1.Value, -vScrollBar1.Value);
			canvas.SetMatrix(_viewportMatrix);

			//canvas.ClipRect(new SKRect(0, 0, controlWidth / viewportZoom, controlHeight / viewportZoom));

			var widget = new MyGuiWidgetData()
			{
				size = new(99999, 99999),
				position = new(0, 0),
				skin = "TextBox",
				properties = new()
				{
					["Caption"] = Util.SystemToMyGuiString(mainTextBox.Text),
					["FontName"] = Form1._currentSelectedWidget != null ? (Form1._currentSelectedWidget.properties.TryGetValue("FontName", out string fntName) ? fntName : "") : "",
					["TextColour"] = Form1._currentSelectedWidget != null ? (Form1._currentSelectedWidget.properties.TryGetValue("TextColour", out string txtClr) ? txtClr : "") : "",
					["TextShadow"] = Form1._currentSelectedWidget != null ? (Form1._currentSelectedWidget.properties.TryGetValue("TextShadow", out string txtShd) ? txtShd : "") : "",
					["TextShadowColour"] = Form1._currentSelectedWidget != null ? (Form1._currentSelectedWidget.properties.TryGetValue("TextShadowColour", out string txtShdClr) ? txtShdClr : "") : "",
					["MaxTextLength"] = Form1._currentSelectedWidget != null ? (Form1._currentSelectedWidget.properties.TryGetValue("MaxTextLength", out string txtMxLgt) ? txtMxLgt : "") : ""
				}
			};

			RenderBackend.DrawWidget(canvas, widget, new SKPoint(0, 0));
		}

		private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
		{
			previewViewport.Refresh();
		}

		private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
		{
			previewViewport.Refresh();
		}

		private void mainTextBox_TextChanged(object sender, EventArgs e)
		{
			previewViewport.Refresh();
		}
	}
}
