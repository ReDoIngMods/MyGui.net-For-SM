using MyGui.net.Properties;
using System.Data;
using System.Diagnostics;

namespace MyGui.net
{
	public partial class FormFont : Form
	{

		public string outcome = "";

		private static BindingSource bindingSource;

		public FormFont()
		{
			InitializeComponent();
			ReloadCache();
		}

		public void ReloadCache()
		{

			bindingSource = new BindingSource();

			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Font", typeof(string));
			dataTable.Columns.Add("Size", typeof(string));
			dataTable.Columns.Add("Spacing", typeof(string));
			dataTable.Columns.Add("Font File", typeof(string));
			dataTable.Columns.Add("Available Characters", typeof(string));

			foreach (var kv in RenderBackend._allFonts)
			{
				dataTable.Rows.Add(kv.Key, kv.Value.size, kv.Value.letterSpacing ?? 1, kv.Value.source, kv.Value.allowedChars);
			}

			DataView dataView = new DataView(dataTable);

			bindingSource.DataSource = dataView;
			dataGridView1.DataSource = bindingSource;
		}

		private void FormFont_Load(object sender, EventArgs e)
		{
			DialogResult = DialogResult.None;
			outcome = "";
		}

		private void FormFont_FormClosing(object sender, FormClosingEventArgs e)
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
				bindingSource.Filter = $"Font LIKE '*{searchValue}*'";
				dataGridView1.Refresh();
			}
		}

		private void FormFont_KeyDown(object sender, KeyEventArgs e)
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
	}
}
