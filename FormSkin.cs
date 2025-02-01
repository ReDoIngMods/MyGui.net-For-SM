using MyGui.net.Properties;
using SkiaSharp;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Security.Policy;

namespace MyGui.net
{
	public partial class FormSkin : Form
	{
		public string outcome = "";

		//private SkiaSharp.Views.Desktop.SKGLControl previewViewport;

		private BindingSource bindingSource;

		public FormSkin()
		{
			InitializeComponent();

			bindingSource = new BindingSource();

			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Correct Type", typeof(string));
			dataTable.Columns.Add("Texture Path", typeof(string));

			foreach (var kv in RenderBackend.AllResources)
			{
				MyGuiResource res = kv.Value;
				dataTable.Rows.Add(kv.Key, res.correctType == "" ? "Any" : res.correctType, res.path ?? "Texture-less");
			}

			DataView dataView = new DataView(dataTable);

			bindingSource.DataSource = dataView;
			dataGridView1.DataSource = bindingSource;
		}

		private void FormSkin_Load(object sender, EventArgs e)
		{
			DialogResult = DialogResult.None;
			outcome = "";
		}

		private void FormSkin_FormClosing(object sender, FormClosingEventArgs e)
		{
			outcome = dataGridView1.SelectedCells[0].Value.ToString();
		}

		private void searchBox_TextChanged(object senderAny, EventArgs e)
		{
			string searchValue = searchBox.Text.Trim().ToLower();

			// Apply the filter to the BindingSource
			if (string.IsNullOrEmpty(searchValue))
			{
				bindingSource.RemoveFilter(); // No filter if the search box is empty
				dataGridView1.Refresh();
			}
			else
			{
				bindingSource.Filter = $"Name LIKE '*{searchValue}*'";
				dataGridView1.Refresh();
			}
		}

		private void FormSkin_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				dataGridView1_CellMouseDoubleClick(null, new DataGridViewCellMouseEventArgs(1, 1, 0, 0, new(MouseButtons.Left, 2, 1, 1, 1)));
				e.Handled = true;
			}
		}

		private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
			{
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		SKMatrix _viewportMatrix = SKMatrix.Identity;

		private void previewViewport_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
		{
			SKCanvas canvas = e.Surface.Canvas;
			canvas.Clear(new SKColor(105, 105, 105));

			if (dataGridView1.SelectedCells.Count < 1)
			{
				return;
			}

			var selectedItem = dataGridView1.SelectedCells[0].Value.ToString();
			Debug.WriteLine(selectedItem);
			var selecteditemResource = RenderBackend.AllResources[selectedItem];

			// Get the control's size
			var controlWidth = previewViewport.Width;
			var controlHeight = previewViewport.Height;

			// Apply viewport transformations

			bool useTileSize = selecteditemResource.tileSize != null;

			Point widgetSize = useTileSize ? Util.GetWidgetPos(true, selecteditemResource.tileSize, new(1, 1)) : new(controlWidth - 10, controlHeight - 10);
			int maxSizeAxis = Math.Max(widgetSize.X, widgetSize.Y);

			float viewportZoom = (Math.Min(controlWidth, controlHeight) * 0.9f) / maxSizeAxis;

			Point widgetPos = useTileSize ? new((int)((controlWidth / viewportZoom - widgetSize.X) / 2), (int)((controlHeight / viewportZoom - widgetSize.Y) / 2)) : new(5, 5);

			_viewportMatrix = SKMatrix.CreateScale(viewportZoom, viewportZoom);
			canvas.SetMatrix(_viewportMatrix);

			canvas.ClipRect(new SKRect(0, 0, controlWidth / viewportZoom, controlHeight / viewportZoom));

			var widget = new MyGuiWidgetData()
			{
				size = widgetSize,
				position = widgetPos,
				skin = selectedItem
			};

			RenderBackend.DrawWidget(canvas, widget, new SKPoint(0, 0));
		}

		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			previewViewport.Refresh();
		}
	}
}
