using System.Data;

namespace MyGui.net
{
	public partial class FormSkin : Form
	{
		public string outcome = "";

		private BindingSource bindingSource;

		public FormSkin()
		{
			InitializeComponent();

			bindingSource = new BindingSource();

			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Correct Type", typeof(string));
			dataTable.Columns.Add("Texture Path", typeof(string));

			foreach (var kv in Form1.AllResources)
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
	}
}
