﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserverTypes;
using Translation = ElectronicObserver.Properties.Window.Dialog.DialogDevelopmentRecordViewer;

namespace ElectronicObserver.Window.Dialog;

public partial class DialogDevelopmentRecordViewer : Form
{

	private DevelopmentRecord _record;

	private string NameAny => Translation.NameAny;
	private string NameNotExist => Translation.NameNotExist; //(失敗)
	private string NameExist => Translation.NameExist;


	private class SearchArgument
	{
		public int EquipmentCategory;
		public string EquipmentName;
		public int SecretaryCategory;
		public string SecretaryName;
		public DateTime DateBegin;
		public DateTime DateEnd;
		public string Recipe;
		public bool MergeRows;
		public DataGridViewRow BaseRow;
	}


	public DialogDevelopmentRecordViewer()
	{
		InitializeComponent();

		_record = RecordManager.Instance.Development;

		Translate();
	}

	public void Translate()
	{
		ButtonRun.Text = Translation.ButtonRun;
		MergeRows.Text = Translation.MergeRows;
		label6.Text = Translation.Recipe;
		label5.Text = Translation.Flagship;
		label4.Text = Translation.Until;
		label3.Text = Translation.From;
		label2.Text = Translation.Type;
		label1.Text = Translation.EquipmentName;

		RecordView_Name.HeaderText = Translation.RecordView_Name;
		RecordView_Date.HeaderText = Translation.RecordView_Date;
		RecordView_Recipe.HeaderText = Translation.RecordView_Recipe;
		RecordView_FlagshipType.HeaderText = Translation.RecordView_FlagshipType;
		RecordView_Flagship.HeaderText = Translation.RecordView_Flagship;
		RecordView_Detail.HeaderText = Translation.RecordView_Detail;

		Text = Translation.Title;
	}

	private void DialogDevelopmentRecordViewer_Load(object sender, EventArgs e)
	{

		var includedEquipmentNames = _record.Record
			.Select(r => r.EquipmentName)
			.Distinct()
			.Except(new[] { NameNotExist });

		var includedEquipmentObjects = includedEquipmentNames
			.Select(name => KCDatabase.Instance.MasterEquipments.Values.FirstOrDefault(eq => eq.NameEN == name))
			.Where(s => s != null);

		var removedEquipments = includedEquipmentNames.Except(includedEquipmentObjects.Select(eq => eq.NameEN));

		var includedSecretaryNames = _record.Record
			.Select(r => r.FlagshipName).Distinct();

		var includedSecretaryObjects = includedSecretaryNames
			.Select(name => KCDatabase.Instance.MasterShips.Values.FirstOrDefault(ship => ship.NameWithClass == name))
			.Where(s => s != null);

		var removedSecretaryNames = includedSecretaryNames.Except(includedSecretaryObjects.Select(s => s.NameWithClass));


		{
			DataTable dt = new DataTable();
			dt.Columns.AddRange(new DataColumn[] {
				new DataColumn( "Value", typeof( int ) ),
				new DataColumn( "Display", typeof( string ) ),
			});
			dt.Rows.Add(-1, NameAny);
			foreach (var eq in includedEquipmentObjects
				.GroupBy(eq => eq.CategoryType, (key, eq) => eq.First())
				.OrderBy(eq => eq.CategoryType))
			{
				dt.Rows.Add(eq.CategoryType, eq.CategoryTypeInstance.NameEN);
			}
			dt.AcceptChanges();
			EquipmentCategory.DisplayMember = "Display";
			EquipmentCategory.ValueMember = "Value";
			EquipmentCategory.DataSource = dt;
			EquipmentCategory.SelectedIndex = 0;
		}

		{
			EquipmentName.Items.Add(NameAny);
			EquipmentName.Items.Add(NameExist);
			EquipmentName.Items.Add(NameNotExist);
			EquipmentName.Items.AddRange(includedEquipmentObjects
				.OrderBy(eq => eq.EquipmentID)
				.OrderBy(eq => eq.CategoryType)
				.Select(eq => eq.NameEN)
				.Union(removedEquipments.OrderBy(s => s))
				.ToArray());
			EquipmentName.SelectedIndex = 0;
		}

		{
			DataTable dt = new DataTable();
			dt.Columns.AddRange(new DataColumn[] {
				new DataColumn( "Value", typeof( int ) ),
				new DataColumn( "Display", typeof( string ) ),
			});
			dt.Rows.Add(-1, NameAny);
			foreach (var ship in includedSecretaryObjects
				.GroupBy(s => s.ShipType, (key, s) => s.First())
				.OrderBy(s => s.ShipType))
			{
				dt.Rows.Add(ship.ShipType, ship.ShipTypeName);
			}
			dt.AcceptChanges();
			SecretaryCategory.DisplayMember = "Display";
			SecretaryCategory.ValueMember = "Value";
			SecretaryCategory.DataSource = dt;
			SecretaryCategory.SelectedIndex = 0;
		}

		{
			SecretaryName.Items.Add(NameAny);
			SecretaryName.Items.AddRange(includedSecretaryObjects
				.OrderBy(s => s.NameReading)
				.OrderBy(s => s.ShipType)
				.Select(s => s.NameWithClass)
				.Union(removedSecretaryNames.OrderBy(s => s))
				.ToArray()
			);
			SecretaryName.SelectedIndex = 0;
		}

		DateBegin.Value = DateBegin.MinDate = DateEnd.MinDate = _record.Record.First().Date.Date;
		DateEnd.Value = DateBegin.MaxDate = DateEnd.MaxDate = DateTime.Now.AddDays(1).Date;

		//checkme
		Recipe.Items.Add(NameAny);
		Recipe.Items.AddRange(_record.Record
			.Select(r => GetRecipeStringForSorting(r))
			.Distinct()
			.OrderBy(s => s)
			.Select(r => GetRecipeString(GetResources(r)))
			.ToArray());
		Recipe.SelectedIndex = 0;


		foreach (DataGridViewColumn column in RecordView.Columns)
			column.Width = 20;

		Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)IconContent.FormDevelopmentRecord]);
	}


	private string GetRecipeString(int[] resources)
	{
		return string.Join("/", resources);
	}

	private string GetRecipeString(int fuel, int ammo, int steel, int bauxite)
	{
		return GetRecipeString(new int[] { fuel, ammo, steel, bauxite });
	}

	private string GetRecipeString(DevelopmentRecord.DevelopmentElement record)
	{
		return GetRecipeString(new int[] { record.Fuel, record.Ammo, record.Steel, record.Bauxite });
	}

	private string GetRecipeStringForSorting(int[] resources)
	{
		return string.Join("/", resources.Select(r => r.ToString("D4")));
	}

	private string GetRecipeStringForSorting(int fuel, int ammo, int steel, int bauxite)
	{
		return GetRecipeStringForSorting(new int[] { fuel, ammo, steel, bauxite });
	}

	private string GetRecipeStringForSorting(DevelopmentRecord.DevelopmentElement record)
	{
		return GetRecipeStringForSorting(new int[] { record.Fuel, record.Ammo, record.Steel, record.Bauxite });
	}

	private int[] GetResources(string recipe)
	{
		return recipe.Split("/".ToCharArray()).Select(s => int.Parse(s)).ToArray();
	}


	private void DialogDevelopmentRecordViewer_FormClosed(object sender, FormClosedEventArgs e)
	{
		ResourceManager.DestroyIcon(Icon);
	}


	private void ButtonRun_Click(object sender, EventArgs e)
	{

		if (Searcher.IsBusy)
		{
			if (MessageBox.Show(EncycloRes.InterruptSearch, EncycloRes.Searching, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
				== System.Windows.Forms.DialogResult.Yes)
			{
				Searcher.CancelAsync();
			}
			return;
		}

		RecordView.Rows.Clear();

		var row = new DataGridViewRow();
		row.CreateCells(RecordView);


		var args = new SearchArgument
		{
			EquipmentCategory = (int)EquipmentCategory.SelectedValue,
			EquipmentName = (string)EquipmentName.SelectedItem,
			SecretaryCategory = (int)SecretaryCategory.SelectedValue,
			SecretaryName = (string)SecretaryName.SelectedItem,
			DateBegin = DateBegin.Value,
			DateEnd = DateEnd.Value,
			Recipe = (string)Recipe.SelectedItem,
			MergeRows = MergeRows.Checked,
			BaseRow = row
		};

		RecordView.Tag = args;


		if (!MergeRows.Checked)
		{
			RecordView_Header.Width = 50;
			RecordView_Header.HeaderText = "";
			RecordView_Name.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			RecordView_Name.HeaderText = Translation.Equipment;
			RecordView_Date.Width = 140;
			RecordView_Date.Visible = true;
			RecordView_Recipe.Width = 95;
			RecordView_Recipe.Visible = true;
			RecordView_FlagshipType.Width = 40;
			RecordView_FlagshipType.Visible = true;
			RecordView_Flagship.Width = 105;
			RecordView_Flagship.Visible = true;
			RecordView_Detail.Visible = false;
		}
		else
		{
			RecordView_Header.Width = 150;
			RecordView_Header.HeaderText = EncycloRes.Tries;
			RecordView_Name.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
			RecordView_Name.Width = 160;
			RecordView_Name.HeaderText = ((EquipmentName.Text != NameAny && EquipmentName.Text != NameExist) || (int)EquipmentCategory.SelectedValue != -1) ? Translation.Recipe : Translation.Equipment;
			RecordView_Date.Visible = false;
			RecordView_Recipe.Visible = false;
			RecordView_FlagshipType.Visible = false;
			RecordView_Flagship.Visible = false;
			RecordView_Detail.HeaderText = (SecretaryName.Text != NameAny || (int)SecretaryCategory.SelectedValue != -1) ? Translation.RecipeTries : Translation.ShipType;
			RecordView_Detail.Visible = true;
		}
		RecordView.ColumnHeadersVisible = true;

		StatusInfo.Text = EncycloRes.Searching + "...";
		StatusInfo.Tag = DateTime.Now;

		Searcher.RunWorkerAsync(args);

	}

	private void EquipmentCategory_SelectedIndexChanged(object sender, EventArgs e)
	{

		string name = (string)EquipmentName.SelectedItem;
		var category = (EquipmentTypes)EquipmentCategory.SelectedValue;

		if ((int)category != -1 && name != NameAny && name != NameExist)
		{
			var eq = KCDatabase.Instance.MasterEquipments.Values.FirstOrDefault(eqm => eqm.NameEN == name);

			if (eq == null || eq.CategoryType != category)
				EquipmentName.SelectedIndex = 0;

		}
	}

	private void EquipmentName_SelectedIndexChanged(object sender, EventArgs e)
	{

		string name = (string)EquipmentName.SelectedItem;
		var category = (EquipmentTypes)EquipmentCategory.SelectedValue;

		if ((int)category != -1 && name != NameAny && name != NameExist)
		{
			var eq = KCDatabase.Instance.MasterEquipments.Values.FirstOrDefault(eqm => eqm.NameEN == name);

			if (eq == null || eq.CategoryType != category)
				EquipmentCategory.SelectedIndex = 0;

		}
	}

	private void SecretaryCategory_SelectedIndexChanged(object sender, EventArgs e)
	{

		string name = (string)SecretaryName.SelectedItem;
		var category = (ShipTypes)SecretaryCategory.SelectedValue;

		if (name != NameAny && (int)category != -1)
		{
			var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.NameWithClass == name);

			if (ship == null || ship.ShipType != category)
				SecretaryName.SelectedIndex = 0;
		}
	}

	private void SecretaryName_SelectedIndexChanged(object sender, EventArgs e)
	{

		string name = (string)SecretaryName.SelectedItem;
		var category = (ShipTypes)SecretaryCategory.SelectedValue;

		if (name != NameAny && (int)category != -1)
		{
			var ship = KCDatabase.Instance.MasterShips.Values.FirstOrDefault(s => s.NameWithClass == name);

			if (ship == null || ship.ShipType != category)
				SecretaryCategory.SelectedIndex = 0;
		}
	}

	private void Searcher_DoWork(object sender, DoWorkEventArgs e)
	{

		var args = (SearchArgument)e.Argument;

		var records = RecordManager.Instance.Development.Record;
		var rows = new LinkedList<DataGridViewRow>();

		int prioritySecretary =
			args.SecretaryName != NameAny ? 2 :
			args.SecretaryCategory != -1 ? 1 : 0;

		int priorityEquipment =
			args.EquipmentName != NameAny && args.EquipmentName != NameExist ? 2 :
			args.EquipmentCategory != -1 ? 1 : 0;


		int i = 0;
		var counts = new Dictionary<string, int>();
		var allcounts = new Dictionary<string, int>();
		var countsdetail = new Dictionary<string, Dictionary<string, int>>();

		foreach (var r in records)
		{

			#region Filtering

			var eq = KCDatabase.Instance.MasterEquipments[r.EquipmentID];
			var secretary = KCDatabase.Instance.MasterShips[r.FlagshipID];
			string currentRecipe = GetRecipeString(r.Fuel, r.Ammo, r.Steel, r.Bauxite);
			var shiptype = KCDatabase.Instance.ShipTypes[r.FlagshipType];


			if (eq != null && eq.NameEN != r.EquipmentName) eq = null;
			if (secretary != null && secretary.NameEN != r.FlagshipName) secretary = null;


			if (r.Date < args.DateBegin || args.DateEnd < r.Date)
				continue;

			if (args.SecretaryCategory != -1 && args.SecretaryCategory != r.FlagshipType)
				continue;

			if (args.SecretaryName != NameAny && args.SecretaryName != r.FlagshipName)
				continue;



			if (args.MergeRows)
			{

				string key;

				if (priorityEquipment > 0)
					key = currentRecipe;
				else
					key = r.EquipmentName;

				if (!allcounts.ContainsKey(key))
				{
					allcounts.Add(key, 1);

				}
				else
				{
					allcounts[key]++;
				}

			}



			if (args.EquipmentCategory != -1 && (eq == null || args.EquipmentCategory != (int)eq.CategoryType))
				continue;

			switch (args.EquipmentName)
			{
				case string s when s == NameAny:
					break;
				case string s when s == NameExist:
					if (r.EquipmentID == -1)
						continue;
					break;
				case string s when s == NameNotExist:
					if (r.EquipmentID != -1)
						continue;
					break;
				default:
					if (args.EquipmentName != r.EquipmentName)
						continue;
					break;
			}

			if (args.Recipe != NameAny && args.Recipe != currentRecipe)
				continue;

			#endregion


			if (!args.MergeRows)
			{
				var row = (DataGridViewRow)args.BaseRow.Clone();

				row.SetValues(
					i + 1,
					r.EquipmentName,
					r.Date,
					GetRecipeString(r),
					shiptype?.NameEN ?? Translation.Unknown,
					r.FlagshipName,
					null
				);

				row.Cells[1].Tag = (eq?.EquipmentID ?? 0) + 1000 * ((int?)eq?.CategoryType ?? 0);
				row.Cells[3].Tag = GetRecipeStringForSorting(r);
				row.Cells[4].Tag = shiptype?.TypeID ?? 0;
				row.Cells[5].Tag = ((int?)secretary?.ShipType ?? 0).ToString("D4") + (secretary?.NameReading ?? r.FlagshipName);

				rows.AddLast(row);

			}
			else
			{

				string key;
				if (priorityEquipment > 0)
					key = currentRecipe;
				else
					key = r.EquipmentName;

				if (!counts.ContainsKey(key))
				{
					counts.Add(key, 1);

				}
				else
				{
					counts[key]++;
				}



				if (priorityEquipment > 0)
					key = currentRecipe;
				else
					key = r.EquipmentName;

				string key2;
				if (prioritySecretary > 0)
					key2 = currentRecipe;
				else
					key2 = shiptype?.NameEN ?? Translation.Unknown;

				if (!countsdetail.ContainsKey(key))
				{
					countsdetail.Add(key, new Dictionary<string, int>());
				}
				if (!countsdetail[key].ContainsKey(key2))
				{
					countsdetail[key].Add(key2, 1);
				}
				else
				{
					countsdetail[key][key2]++;
				}

			}

			if (Searcher.CancellationPending)
				break;

			i++;
		}


		if (args.MergeRows)
		{

			int sum = counts.Values.Sum();

			foreach (var c in counts)
			{
				var row = (DataGridViewRow)args.BaseRow.Clone();

				if (priorityEquipment > 0)
				{

					row.SetValues(
						c.Value,
						c.Key,
						null,
						null,
						null,
						null,
						string.Join(", ", countsdetail[c.Key].OrderByDescending(p => p.Value).Select(d => string.Format("{0}({1})", d.Key, d.Value)))
					);

					row.Cells[0].Tag = allcounts[c.Key];
					row.Cells[1].Tag = GetRecipeStringForSorting(GetResources(c.Key));

				}
				else
				{

					row.SetValues(
						c.Value,
						c.Key,
						null,
						null,
						null,
						null,
						string.Join(", ", countsdetail[c.Key].OrderByDescending(p => p.Value).Select(d => string.Format("{0}({1})", d.Key, d.Value)))
					);

					var eq = KCDatabase.Instance.MasterEquipments.Values.FirstOrDefault(eqm => eqm.NameEN == c.Key);
					row.Cells[0].Tag = (double)c.Value / sum;
					row.Cells[1].Tag = (eq?.EquipmentID ?? 0) + 1000 * ((int?)eq?.CategoryType ?? 0);
				}

				rows.AddLast(row);

				if (Searcher.CancellationPending)
					break;
			}

		}



		e.Result = rows.ToArray();

	}

	private void Searcher_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{

		if (!e.Cancelled)
		{
			var rows = (DataGridViewRow[])e.Result;

			RecordView.Rows.AddRange(rows);
			RecordView.Sort(RecordView.SortedColumn ?? RecordView_Header,
				RecordView.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);

			StatusInfo.Text = EncycloRes.SearchComplete + "(" + (int)(DateTime.Now - (DateTime)StatusInfo.Tag).TotalMilliseconds + " ms)";

		}
		else
		{

			StatusInfo.Text = EncycloRes.SearchCancelled;
		}

	}

	private void RecordView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
	{

		object tag1 = RecordView[e.Column.Index, e.RowIndex1].Tag;
		object tag2 = RecordView[e.Column.Index, e.RowIndex2].Tag;

		if (tag1 != null && (tag1 is double || tag1 is int) && e.CellValue1 is int)
		{
			double c1 = 0, c2 = 0;

			if (tag1 is double)
			{
				c1 = (double)tag1;
				c2 = (double)tag2;
			}
			else if (tag1 is int)
			{
				c1 = (double)(int)e.CellValue1 / Math.Max((int)tag1, 1);
				c2 = (double)(int)e.CellValue2 / Math.Max((int)tag2, 1);
			}


			if (Math.Abs(c1 - c2) < 0.000001)
				e.SortResult = (int)e.CellValue1 - (int)e.CellValue2;
			else if (c1 < c2)
				e.SortResult = -1;
			else
				e.SortResult = 1;
			e.Handled = true;

		}
		else if (tag1 is string)
		{
			e.SortResult = ((IComparable)tag1).CompareTo(tag2);
			e.Handled = true;
		}
		else if (tag1 is int)
		{
			e.SortResult = (int)tag1 - (int)tag2;
			e.Handled = true;
		}


		if (!e.Handled)
		{
			e.SortResult = ((IComparable)e.CellValue1 ?? 0).CompareTo(e.CellValue2 ?? 0);
			e.Handled = true;
		}

		if (e.SortResult == 0)
		{
			e.SortResult = (int)(RecordView.Rows[e.RowIndex1].Tag ?? 0) - (int)(RecordView.Rows[e.RowIndex2].Tag ?? 0);
		}
	}

	private void RecordView_Sorted(object sender, EventArgs e)
	{

		for (int i = 0; i < RecordView.Rows.Count; i++)
		{
			RecordView.Rows[i].Tag = i;
		}
	}

	private void RecordView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{

		if (e.ColumnIndex == RecordView_Header.Index)
		{
			object tag = RecordView[e.ColumnIndex, e.RowIndex].Tag;

			if (tag != null)
			{
				if (tag is double)
				{
					e.Value = string.Format("{0} ({1:p1})", e.Value, (double)tag);
					e.FormattingApplied = true;
				}
				else if (tag is int)
				{
					e.Value = string.Format("{0}/{1} ({2:p1})", e.Value, (int)tag, (double)(int)e.Value / (int)tag);
					e.FormattingApplied = true;
				}
			}

		}
		else if (e.ColumnIndex == RecordView_Date.Index)
		{

			if (e.Value is DateTime)
			{
				e.Value = DateTimeHelper.TimeToCSVString((DateTime)e.Value);
				e.FormattingApplied = true;
			}
		}

	}


	private void RecordView_SelectionChanged(object sender, EventArgs e)
	{
		var args = RecordView.Tag as SearchArgument;
		if (args == null)
			return;

		int selectedCount = RecordView.Rows.GetRowCount(DataGridViewElementStates.Selected);

		if (selectedCount == 0)
			return;

		if (args.MergeRows)
		{
			int count = RecordView.SelectedRows.OfType<DataGridViewRow>().Select(r => (int)r.Cells[RecordView_Header.Index].Value).Sum();
			int allcount = RecordView.Rows.OfType<DataGridViewRow>().Select(r => (int)r.Cells[RecordView_Header.Index].Value).Sum();

			StatusInfo.Text = string.Format(Translation.SelectedItems + ": {0} / {1} ({2:p1})",
				count, allcount, (double)count / allcount);
		}
		else
		{
			int allcount = RecordView.RowCount;
			StatusInfo.Text = string.Format(Translation.SelectedItems + ": {0} / {1} ({2:p1})",
				selectedCount, allcount, (double)selectedCount / allcount);
		}
	}
}
