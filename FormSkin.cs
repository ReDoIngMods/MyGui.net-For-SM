using MyGui.net.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyGui.net
{
    public partial class FormSkin : Form
    {
		public string outcome = "";

		private BindingSource bindingSource;

		public FormSkin()
		{
			InitializeComponent();

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
			outcome = dataGridView1.SelectedCells[0].Value.ToString();
		}

		private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			DialogResult = DialogResult.OK;
			this.Close();
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
				dataGridView1_CellContentDoubleClick(null, new DataGridViewCellEventArgs(0, 0));
				e.Handled = true;
			}
		}
	}
}
