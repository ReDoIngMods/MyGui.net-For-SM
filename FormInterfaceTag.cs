using MyGui.net.Properties;
using System.Data;
using System.Diagnostics;

namespace MyGui.net
{
	public partial class FormInterfaceTag : Form
	{

		public string outcome = "";

		private static BindingSource bindingSource;

		public FormInterfaceTag()
		{
			InitializeComponent();
			ReloadCache();
		}

		public void ReloadCache()
		{
			Util.languageTags = new();
			Util.GetLanguageTagString("a", Settings.Default.ReferenceLanguage, Form1.ScrapMechanicPath); //TODO: create specialized load Interface Tags function

			bindingSource = new BindingSource();

			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Tag", typeof(string));
			dataTable.Columns.Add("Text", typeof(string));

			foreach (var kv in Util.languageTags)
			{
				dataTable.Rows.Add(kv.Key, kv.Value);
			}

			DataView dataView = new DataView(dataTable);

			bindingSource.DataSource = dataView;
			dataGridView1.DataSource = bindingSource;
		}

		private void FormInterfaceTag_Load(object sender, EventArgs e)
		{
			DialogResult = DialogResult.None;
			outcome = "";
		}

		private void FormInterfaceTag_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (dataGridView1.SelectedCells.Count < 1)
			{
				outcome = "";
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
				bindingSource.Filter = $"Tag LIKE '*{searchValue}*' OR Text LIKE '*{searchValue}*'";
				dataGridView1.Refresh();
			}
		}

		private void FormInterfaceTag_KeyDown(object sender, KeyEventArgs e)
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
