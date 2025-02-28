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
using ElectronicObserver.Utility.Data;
using ElectronicObserverTypes;
using Translation = ElectronicObserver.Properties.Window.Dialog.DialogExpChecker;

namespace ElectronicObserver.Window.Dialog;

public partial class DialogExpChecker : Form
{
	private string DefaultTitle => Translation.Title;
	private DataGridViewCellStyle CellStyleModernized;


	private int DefaultShipID = -1;



	private class ComboShipData
	{
		public ShipData Ship;

		public ComboShipData(ShipData ship)
		{
			Ship = ship;
		}

		public override string ToString() => $"{Ship.MasterShip.ShipTypeName} {Ship.NameWithLevel}";
	}

	private class ASWEquipmentData
	{
		public int ID;
		public int ASW;
		public string Name;
		public bool IsSonar;
		public int Count;

		public override string ToString() => Name;
	}






	public DialogExpChecker()
	{
		InitializeComponent();

		CellStyleModernized = new DataGridViewCellStyle(ColumnLevel.DefaultCellStyle);
		CellStyleModernized.BackColor =
			CellStyleModernized.SelectionBackColor = Color.LightGreen;

		Translate();
	}

	public DialogExpChecker(int shipID) : this()
	{
		DefaultShipID = shipID;
		Text = DefaultTitle;
	}

	public void Translate()
	{
		groupBox1.Text = Translation.DisplayCriteria;
		label3.Text = Translation.Ship;
		SearchInFleet.Text = Translation.SearchInFleet;
		ToolTipInfo.SetToolTip(SearchInFleet, Translation.SearchInFleetToolTip);
		label1.Text = Translation.SortieExp;
		ShowAllASWEquipments.Text = Translation.ShowAllASWEquipments;
		ToolTipInfo.SetToolTip(ShowAllASWEquipments, Translation.ShowAllASWEquipmentsToolTip);

		ShowAllLevel.Text = Translation.ShowAllLevel;
		ToolTipInfo.SetToolTip(ShowAllLevel, Translation.ShowAllLevelToolTip);

		label2.Text = Translation.AswOffset;
		ToolTipInfo.SetToolTip(ASWModernization, Translation.ASWModernizationToolTip);

		ToolTipInfo.SetToolTip(ExpUnit, Translation.ExpUnitToolTip);
		GroupExp.Text = Translation.GroupExp;

		ColumnExp.HeaderText = Translation.GroupExp;
		ColumnSortieCount.HeaderText = Translation.ColumnSortieCount;
		ColumnASW.HeaderText = Translation.ASW;
		ColumnEquipment.HeaderText = Translation.ColumnEquipment;

		Text = Translation.Title;
	}

	private void DialogExpChecker_Load(object sender, EventArgs e)
	{
		var ships = KCDatabase.Instance.Ships.Values;

		if (!ships.Any())
		{
			MessageBox.Show(new Form { TopMost = true },Translation.NoShipsAvailable, Translation.ShipsUnavailable, MessageBoxButtons.OK, MessageBoxIcon.Error);
			Close();
			return;
		}

		LabelAlert.Text = "";
		SearchInFleet_CheckedChanged(this, new EventArgs());
		ExpUnit.Value = Utility.Configuration.Config.Control.ExpCheckerExpUnit;

		if (DefaultShipID != -1)
			TextShip.SelectedItem = TextShip.Items.OfType<ComboShipData>().FirstOrDefault(f => f.Ship.MasterID == DefaultShipID);


		this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)IconContent.FormExpChecker]);
	}

	private void DialogExpChecker_FormClosed(object sender, FormClosedEventArgs e)
	{
		Utility.Configuration.Config.Control.ExpCheckerExpUnit = (int)ExpUnit.Value;
		ResourceManager.DestroyIcon(Icon);
	}



	private void UpdateLevelView()
	{
		var selectedShip = (TextShip.SelectedItem as ComboShipData)?.Ship;

		if (selectedShip == null)
		{
			System.Media.SystemSounds.Asterisk.Play();
			return;
		}


		LevelView.SuspendLayout();

		LevelView.Rows.Clear();


		// 空母系は面倒なので省略
		int openingASWborder = selectedShip.MasterShip.ShipType == ShipTypes.Escort ? 60 : 100;

		var ASWEquipmentPairs = new Dictionary<int, string>();
		if (ShowAllASWEquipments.Checked)
		{

			var had = KCDatabase.Instance.Equipments.Values
				.Where(eq => eq.MasterEquipment.CategoryType == EquipmentTypes.Sonar || eq.MasterEquipment.CategoryType == EquipmentTypes.DepthCharge)
				.GroupBy(eq => eq.EquipmentID)
				.Select(g => new ASWEquipmentData { ID = g.Key, ASW = g.First().MasterEquipment.ASW, Name = g.First().MasterEquipment.NameEN, IsSonar = g.First().MasterEquipment.IsSonar, Count = g.Count() })
				.Concat(new[] { new ASWEquipmentData { ID = -1, ASW = 0, Name = "", Count = 99, IsSonar = false } })
				.OrderByDescending(a => a.ASW)
				.ToArray();

			var stack = Enumerable.Repeat(0, selectedShip.SlotSize).ToArray();

			var pair = new Dictionary<int, List<ASWEquipmentData[]>>();

			if (had.Length > 0 && stack.Length > 0)
			{

				while (stack[0] != -1)
				{
					var convert = stack.Select(i => had[i]).ToArray();


					if (convert.Any(c => c.IsSonar) && stack.GroupBy(s => s).All(s => had[s.Key].Count >= s.Count()))
					{
						int aswsum = convert.Sum(c => c.ASW);

						if (!pair.ContainsKey(aswsum))
							pair.Add(aswsum, new List<ASWEquipmentData[]>() { convert });
						else
							pair[aswsum].Add(convert);
					}

					for (int p = stack.Length - 1; p >= 0; p--)
					{
						stack[p]++;
						if (stack[p] < had.Length)
							break;
						stack[p] = -1;
					}
					for (int p = 1; p < stack.Length; p++)
					{
						if (stack[p] == -1)
							stack[p] = stack[p - 1];
					}
				}
			}

			foreach (var x in pair)
			{
				// 要するに下のようなフォーマットにする
				ASWEquipmentPairs.Add(openingASWborder - x.Key,
					string.Join(", ",
						x.Value.OrderBy(a => a.Count(b => b.ID > 0))
							.Select(a => $"[{string.Join(", ", a.Where(b => b.ID > 0).GroupBy(b => b.ID).Select(b => b.Count() == 1 ? b.First().Name : $"{b.First().Name}x{b.Count()}"))}]")));
			}
		}
		else
		{
			string hfdf = KCDatabase.Instance.MasterEquipments[(int)EquipmentId.Sonar_HFDF_Type144147ASDIC].NameEN;
			string aswTorpedo = KCDatabase.Instance.MasterEquipments[(int)EquipmentId.DepthCharge_LightweightASWTorpedo_InitialTestModel].NameEN;
			string rur = KCDatabase.Instance.MasterEquipments[(int)EquipmentId.DepthCharge_RUR4AWeaponAlphaKai].NameEN;
			string aswRocket = KCDatabase.Instance.MasterEquipments[(int)EquipmentId.DepthCharge_Prototype15cm9tubeASWRocketLauncher].NameEN;
			string type4 = KCDatabase.Instance.MasterEquipments[(int)EquipmentId.Sonar_Type4PassiveSONAR].NameEN;
			string type3dc = KCDatabase.Instance.MasterEquipments[(int)EquipmentId.DepthCharge_Type3DepthChargeProjector].NameEN;
			string type3 = KCDatabase.Instance.MasterEquipments[(int)EquipmentId.Sonar_Type3ActiveSONAR].NameEN;
			string type2dc = KCDatabase.Instance.MasterEquipments[(int)EquipmentId.DepthCharge_Type2DepthCharge].NameEN;


			if (selectedShip.SlotSize >= 4)
			{
				ASWEquipmentPairs.Add(openingASWborder - 67, $"[{hfdf}, {aswTorpedo}, {rur}, {aswRocket}]");
				ASWEquipmentPairs.Add(openingASWborder - 51, $"[{type4}x3, {aswRocket}]");
				ASWEquipmentPairs.Add(openingASWborder - 48, $"[{type4}x4]");
				ASWEquipmentPairs.Add(openingASWborder - 44, $"[{type4}x3, {type3dc}]");
			}
			if (selectedShip.SlotSize >= 3)
			{
				ASWEquipmentPairs.Add(openingASWborder - 52, $"[{hfdf}, {aswTorpedo}, {rur}]");
				ASWEquipmentPairs.Add(openingASWborder - 47, $"[{hfdf}, {rur}, {aswRocket}]");

				ASWEquipmentPairs.Add(openingASWborder - 39, $"[{type4}x2, {aswRocket}]");
				ASWEquipmentPairs.Add(openingASWborder - 36, $"[{type4}x3]");
				ASWEquipmentPairs.Add(openingASWborder - 32, $"[{type4}x2, {type3dc}]");
				ASWEquipmentPairs.Add(openingASWborder - 28, $"[{type3}x2, {type3dc}]");
				ASWEquipmentPairs.Add(openingASWborder - 27, $"[{type4}, {type3dc}, {type2dc}]");
			}
			if (selectedShip.SlotSize >= 2)
			{
				ASWEquipmentPairs.Add(openingASWborder - 35, $"[{hfdf}, {aswTorpedo}]");
				if (ASWEquipmentPairs.ContainsKey(openingASWborder - 32))
					ASWEquipmentPairs[openingASWborder - 32] += $", [{hfdf}, {rur}]";
				else
					ASWEquipmentPairs.Add(openingASWborder - 32, $"[{hfdf}, {rur}]");
				if (ASWEquipmentPairs.ContainsKey(openingASWborder - 27))
					ASWEquipmentPairs[openingASWborder - 27] += $", [{type4}, {aswRocket}]";
				else
					ASWEquipmentPairs.Add(openingASWborder - 27, $"[{type4}, {aswRocket}]");
				ASWEquipmentPairs.Add(openingASWborder - 20, $"[{type4}, {type3dc}]");
				ASWEquipmentPairs.Add(openingASWborder - 18, $"[{type3}, {type3dc}]");
			}
			ASWEquipmentPairs.Add(openingASWborder - 15, $"[{hfdf}]");
			ASWEquipmentPairs.Add(openingASWborder - 12, $"[{type4}]");
		}


		var aswdata = selectedShip.MasterShip.ASW;
		int aswmin = aswdata.Minimum;
		int aswmax = aswdata.Maximum;
		int aswmod = (int)ASWModernization.Value;
		int currentlv = selectedShip.Level;
		int minlv = ShowAllLevel.Checked ? 1 : (currentlv + 1);
		int unitexp = Math.Max((int)ExpUnit.Value, 1);
		var remodelLevelTable = GetRemodelLevelTable(selectedShip.MasterShip);


		if (!aswdata.IsAvailable)
			LabelAlert.Text = Translation.AswUnknown;
		else if (!aswdata.IsDetermined)
			LabelAlert.Text = Translation.AswApproximated;
		else
			LabelAlert.Text = "";


		var rows = new DataGridViewRow[ExpTable.ShipMaximumLevel - (minlv - 1)];

		for (int lv = minlv; lv <= ExpTable.ShipMaximumLevel; lv++)
		{
			int asw = aswmin + ((aswmax - aswmin) * lv / 99) + aswmod;

			int needexp = ExpTable.ShipExp[lv].Total - selectedShip.ExpTotal;

			var row = new DataGridViewRow();
			row.CreateCells(LevelView);
			row.SetValues(
				lv,
				Math.Max(needexp, 0),
				Math.Max((int)Math.Ceiling((double)needexp / unitexp), 0),
				!aswdata.IsAvailable ? -1 : asw,
				!aswdata.IsAvailable ? "-" : (ASWEquipmentPairs.Where(k => asw >= k.Key).OrderByDescending(p => p.Key).FirstOrDefault().Value ?? "-")
			);


			if (remodelLevelTable.Contains(lv))
			{
				row.Cells[ColumnLevel.Index].Style = CellStyleModernized;
			}

			rows[lv - minlv] = row;
		}

		LevelView.Rows.AddRange(rows);

		LevelView.ResumeLayout();


		Text = DefaultTitle + " - " + selectedShip.NameWithLevel;
		GroupExp.Text = $"{selectedShip.NameWithLevel}: Exp. {selectedShip.ExpTotal}, {Translation.ASW} {selectedShip.ASWBase} ({Translation.Modernization}+{selectedShip.ASWModernized})";
	}



	private void SearchInFleet_CheckedChanged(object sender, EventArgs e)
	{
		var ships = KCDatabase.Instance.Ships.Values;

		TextShip.Items.Clear();

		if (SearchInFleet.Checked)
		{
			TextShip.Items.AddRange(ships
				.Where(s => s.Fleet != -1)
				.OrderBy(s => s.FleetWithIndex)
				.Select(s => new ComboShipData(s))
				.ToArray());
		}
		else
		{
			TextShip.Items.AddRange(ships
				.OrderBy(s => s.MasterShip.ShipType)
				.ThenByDescending(s => s.Level)
				.Select(s => new ComboShipData(s))
				.ToArray());
		}

		TextShip.SelectedIndex = 0;
	}

	private void TextShip_SelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedShip = (TextShip.SelectedItem as ComboShipData)?.Ship;

		if (selectedShip == null)
			return;

		ASWModernization.Value = selectedShip.ASWModernized;

		UpdateLevelView();
	}

	private void ShowAllLevel_CheckedChanged(object sender, EventArgs e)
	{
		UpdateLevelView();
	}

	private void ShowAllASWEquipments_CheckedChanged(object sender, EventArgs e)
	{
		UpdateLevelView();
	}

	private void ExpUnit_ValueChanged(object sender, EventArgs e)
	{
		UpdateLevelView();
	}

	private void ASWModernization_ValueChanged(object sender, EventArgs e)
	{
		UpdateLevelView();
	}

	private int[] GetRemodelLevelTable(IShipDataMaster ship)
	{
		while (ship.RemodelBeforeShip != null)
			ship = ship.RemodelBeforeShip;

		var list = new LinkedList<int>();

		while (ship != null)
		{
			list.AddLast(ship.RemodelAfterLevel);
			ship = ship.RemodelAfterShip;
			if (list.Last() >= ship.RemodelAfterLevel)
				break;
		}

		return list.ToArray();
	}


	private void LevelView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{
		if (e.ColumnIndex == ColumnASW.Index)
		{
			e.Value = (int)e.Value == -1 ? "???" : e.Value.ToString();
			e.FormattingApplied = true;
		}
	}
}
