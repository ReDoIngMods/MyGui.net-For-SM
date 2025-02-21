using MyGui.net.Properties;
using SkiaSharp;
using System.Data;

namespace MyGui.net
{
	public partial class FormSkin : Form
	{
		readonly Dictionary<string, string> _catOverrides = new() {
			{"WhiteSkin", "Neutral"},
			{"ButtonImage", "Neutral"},
			{"TextBox", "Neutral"},
			{"EditBoxEmpty", "Neutral"},
			{"WordWrapEmpty", "Neutral"},
			{"ImageBox", "Neutral"}
		};

		public string outcome = "";

		private BindingSource bindingSource;

		public FormSkin()
		{
			InitializeComponent();

			bindingSource = new BindingSource();

			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Category", typeof(string));
			dataTable.Columns.Add("Correct Type", typeof(string));
			dataTable.Columns.Add("Texture Path", typeof(string));

			foreach (var kv in RenderBackend.AllResources)
			{
				MyGuiResource res = kv.Value;
				string texPath = res.path ?? (res.resourceLayout != null ? "Resource Layout" : "Texture-less");
				string catStr = _catOverrides.TryGetValue(res.name, out string val) ? val : ((res.pathSpecial == null || (res.path == null && res.resourceLayout == null)) ? "Neutral" : 
					(
						Util.IsAnyOf<string>(Path.GetFileName(res.pathSpecial), ["ScrapMekSkin.xml", "ScrapMekTemplate.xml"]) ? "Old Scrap Mechanic" : 
						(
							Util.IsAnyOf<string>(Path.GetFileName(res.pathSpecial), ["MyGUI_BlackOrangeSkins.xml", "MyGUI_BlackOrangeTemplates.xml"]) ? "Old MyGui" : "Modern Scrap Mechanic"
						)
					));
				if (Settings.Default.HideOldMyGuiWidgetSkins && catStr == "Old MyGui")
				{
					continue;
				}
				dataTable.Rows.Add(kv.Key, catStr, res.correctType == "" ? "Any" : res.correctType, texPath);
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
			if (dataGridView1.SelectedCells.Count < 1)
			{
				outcome = "";
				DialogResult = DialogResult.Cancel;
				return;
			}
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
			var selecteditemResource = RenderBackend.AllResources[selectedItem];

			// Get the control's size
			var controlWidth = previewViewport.Width;
			var controlHeight = previewViewport.Height;

			// Apply viewport transformations

			bool useTileSize = selecteditemResource.tileSize != null;

			Point widgetSize = useTileSize ? Util.GetWidgetPos(true, selecteditemResource.tileSize, new(1, 1)) : (selecteditemResource.resourceLayout != null ? selecteditemResource.resourceLayout[0].size : new(100, 100));

			widgetSize.X = (int)viewportWidgetSizeX.Value;
			widgetSize.Y = (int)viewportWidgetSizeY.Value;

			int maxSizeAxis = Math.Max(widgetSize.X, widgetSize.Y);

			float viewportZoom = Math.Min(controlWidth / (float)widgetSize.X, controlHeight / (float)widgetSize.Y);


			Point widgetPos = new((int)((controlWidth / viewportZoom - widgetSize.X) / 2), (int)((controlHeight / viewportZoom - widgetSize.Y) / 2));

			_viewportMatrix = SKMatrix.CreateScale(viewportZoom, viewportZoom);
			canvas.SetMatrix(_viewportMatrix);

			//canvas.ClipRect(new SKRect(0, 0, controlWidth / viewportZoom, controlHeight / viewportZoom));

			var widget = new MyGuiWidgetData()
			{
				size = widgetSize,
				position = widgetPos,
				skin = selectedItem
			};

			RenderBackend.DrawWidget(canvas, widget, new SKPoint(0, 0), forceDebug: showDebugCheckBox.Checked);
		}

		private void dataGridView1_SelectionChanged(object sender, EventArgs e)
		{
			if (dataGridView1.SelectedCells.Count > 0)
			{
				var selectedItem = dataGridView1.SelectedCells[0].Value.ToString();
				if (RenderBackend.AllResources.ContainsKey(selectedItem))
				{
					var selecteditemResource = RenderBackend.AllResources[selectedItem];
					if (alwaysSetDefaultSize.Checked)
					{
						bool useTileSize = selecteditemResource.tileSize != null;

						Point widgetSize = useTileSize ? Util.GetWidgetPos(true, selecteditemResource.tileSize, new(1, 1)) : (selecteditemResource.resourceLayout != null ? selecteditemResource.resourceLayout[0].size : new(100, 100));

						viewportWidgetSizeX.Value = widgetSize.X;
						viewportWidgetSizeY.Value = widgetSize.Y;
					}
					resourceInfoLabel.Text = $"Skin Info:\n● Name: \"{selecteditemResource.name}\"\n● Skin Type: \"{(selecteditemResource.resourceLayout == null ? "ResourceSkin" : "ResourceLayout")}\"";
					if (selecteditemResource.basisSkins != null)
					{
						resourceInfoLabel.Text += $"\n    ● Basis Skin Count: {selecteditemResource.basisSkins.Count}";
					}
					copyResourceLayoutButton.Enabled = selecteditemResource.resourceLayout != null;
				}
			}
			previewViewport.Refresh();
		}

		bool _viewportWidgetSizeBlockValueChanged = false;

		private decimal _prevValueX;
		private decimal _prevValueY;

		private void viewportWidgetSizeX_ValueChanged(object sender, EventArgs e)
		{
			previewViewport.Refresh();
			if (_viewportWidgetSizeBlockValueChanged) return;

			decimal delta = viewportWidgetSizeX.Value - _prevValueX;
			_prevValueX = viewportWidgetSizeX.Value;

			if (Util.IsKeyPressed(Keys.ShiftKey))
			{
				_viewportWidgetSizeBlockValueChanged = true;
				viewportWidgetSizeY.Value = Math.Clamp(viewportWidgetSizeY.Value + delta, viewportWidgetSizeY.Minimum, viewportWidgetSizeY.Maximum);
				_viewportWidgetSizeBlockValueChanged = false;
			}
		}

		private void viewportWidgetSizeY_ValueChanged(object sender, EventArgs e)
		{
			previewViewport.Refresh();
			if (_viewportWidgetSizeBlockValueChanged) return;

			decimal delta = viewportWidgetSizeY.Value - _prevValueY;
			_prevValueY = viewportWidgetSizeY.Value;

			if (Util.IsKeyPressed(Keys.ShiftKey))
			{
				_viewportWidgetSizeBlockValueChanged = true;
				viewportWidgetSizeX.Value = Math.Clamp(viewportWidgetSizeX.Value + delta, viewportWidgetSizeX.Minimum, viewportWidgetSizeX.Maximum);
				_viewportWidgetSizeBlockValueChanged = false;
			}
		}

		private void viewportWidgetSizePreffered_Click(object sender, EventArgs e)
		{
			if (dataGridView1.SelectedCells.Count > 0)
			{
				var selectedItem = dataGridView1.SelectedCells[0].Value.ToString();
				var selecteditemResource = RenderBackend.AllResources[selectedItem];
				bool useTileSize = selecteditemResource.tileSize != null;

				Point widgetSize = useTileSize ? Util.GetWidgetPos(true, selecteditemResource.tileSize, new(1, 1)) : (selecteditemResource.resourceLayout != null ? selecteditemResource.resourceLayout[0].size : new(100, 100));

				viewportWidgetSizeX.Value = widgetSize.X;
				viewportWidgetSizeY.Value = widgetSize.Y;
			}
		}

		private void showDebugCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			previewViewport.Refresh();
		}

		private void copyResourceLayoutButton_Click(object sender, EventArgs e)
		{
			var selectedItem = dataGridView1.SelectedCells[0].Value.ToString();
			var selecteditemResource = RenderBackend.AllResources[selectedItem];

			if (Clipboard.ContainsText())
			{
				Clipboard.Clear();
			}
			Clipboard.SetText(Util.ExportLayoutToXmlString(selecteditemResource.resourceLayout, new Point(1, 1), true, false), TextDataFormat.Text);
		}
	}
}
