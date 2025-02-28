﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Utility.Storage;
using ElectronicObserver.Window.Control;
using ElectronicObserver.Window.Support;
using ElectronicObserver.Window.Tools.DialogAlbumMasterShip;
using ElectronicObserverTypes;
using Translation = ElectronicObserver.Properties.Window.Dialog.DialogAlbumMasterEquipment;

namespace ElectronicObserver.Window.Dialog;

public partial class DialogAlbumMasterEquipment : Form
{


	public DialogAlbumMasterEquipment()
	{
		InitializeComponent();

		TitleFirepower.ImageList =
			TitleTorpedo.ImageList =
				TitleAA.ImageList =
					TitleArmor.ImageList =
						TitleASW.ImageList =
							TitleEvasion.ImageList =
								TitleLOS.ImageList =
									TitleAccuracy.ImageList =
										TitleBomber.ImageList =
											TitleSpeed.ImageList =
												TitleRange.ImageList =
													TitleAircraftCost.ImageList =
														TitleAircraftDistance.ImageList =
															Rarity.ImageList =
																MaterialFuel.ImageList =
																	MaterialAmmo.ImageList =
																		MaterialSteel.ImageList =
																			MaterialBauxite.ImageList =
																				ResourceManager.Instance.Icons;

		EquipmentType.ImageList = ResourceManager.Instance.Equipments;

		TitleFirepower.ImageIndex = (int)IconContent.ParameterFirepower;
		TitleTorpedo.ImageIndex = (int)IconContent.ParameterTorpedo;
		TitleAA.ImageIndex = (int)IconContent.ParameterAA;
		TitleArmor.ImageIndex = (int)IconContent.ParameterArmor;
		TitleASW.ImageIndex = (int)IconContent.ParameterASW;
		TitleEvasion.ImageIndex = (int)IconContent.ParameterEvasion;
		TitleLOS.ImageIndex = (int)IconContent.ParameterLOS;
		TitleAccuracy.ImageIndex = (int)IconContent.ParameterAccuracy;
		TitleBomber.ImageIndex = (int)IconContent.ParameterBomber;
		TitleSpeed.ImageIndex = (int)IconContent.ParameterSpeed;
		TitleRange.ImageIndex = (int)IconContent.ParameterRange;
		TitleAircraftCost.ImageIndex = (int)IconContent.ParameterAircraftCost;
		TitleAircraftDistance.ImageIndex = (int)IconContent.ParameterAircraftDistance;
		MaterialFuel.ImageIndex = (int)IconContent.ResourceFuel;
		MaterialAmmo.ImageIndex = (int)IconContent.ResourceAmmo;
		MaterialSteel.ImageIndex = (int)IconContent.ResourceSteel;
		MaterialBauxite.ImageIndex = (int)IconContent.ResourceBauxite;


		BasePanelEquipment.Visible = false;


		ControlHelper.SetDoubleBuffered(TableEquipmentName);
		ControlHelper.SetDoubleBuffered(TableParameterMain);
		ControlHelper.SetDoubleBuffered(TableParameterSub);
		ControlHelper.SetDoubleBuffered(TableArsenal);

		ControlHelper.SetDoubleBuffered(EquipmentView);


		//Initialize EquipmentView
		EquipmentView.SuspendLayout();

		EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
		EquipmentView_Icon.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
		//EquipmentView_Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;


		EquipmentView.Rows.Clear();

		List<DataGridViewRow> rows = new List<DataGridViewRow>(KCDatabase.Instance.MasterEquipments.Values.Count(s => s.Name != "なし"));

		foreach (var eq in KCDatabase.Instance.MasterEquipments.Values)
		{

			if (eq.Name == "なし") continue;

			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(EquipmentView);
			row.SetValues(eq.EquipmentID, eq.IconType, eq.CategoryTypeInstance.NameEN, eq.NameEN);
			rows.Add(row);

		}
		EquipmentView.Rows.AddRange(rows.ToArray());

		EquipmentView_ID.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
		EquipmentView_Icon.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
		//EquipmentView_Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

		EquipmentView.Sort(EquipmentView_ID, ListSortDirection.Ascending);
		EquipmentView.ResumeLayout();

		Translate();
	}

	public DialogAlbumMasterEquipment(int equipmentID)
		: this()
	{

		UpdateAlbumPage(equipmentID);


		if (KCDatabase.Instance.MasterEquipments.ContainsKey(equipmentID))
		{
			var row = EquipmentView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[EquipmentView_ID.Index].Value == equipmentID);
			if (row != null)
				EquipmentView.FirstDisplayedScrollingRowIndex = row.Index;
		}
	}



	private void DialogAlbumMasterEquipment_Load(object sender, EventArgs e)
	{

		this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)IconContent.FormAlbumEquipment]);

	}


	public void Translate()
	{
		StripMenu_File.Text = Translation.StripMenu_File;
		StripMenu_File_OutputCSVUser.Text = Translation.StripMenu_File_OutputCSVUser;
		StripMenu_File_OutputCSVData.Text = Translation.StripMenu_File_OutputCSVData;

		StripMenu_Edit.Text = Translation.StripMenu_Edit;
		StripMenu_Edit_CopyEquipmentName.Text = Translation.StripMenu_Edit_CopyEquipmentName;
		StripMenu_Edit_CopyEquipmentData.Text = Translation.StripMenu_Edit_CopyEquipmentData;
		StripMenu_Edit_GoogleEquipmentName.Text = Translation.StripMenu_Edit_GoogleEquipmentName;

		StripMenu_View.Text = Translation.StripMenu_View;
		StripMenu_View_ShowAppearingArea.Text = Translation.StripMenu_View_ShowAppearingArea;

		EquipmentView_Type.HeaderText = Translation.EquipmentView_Type;
		EquipmentView_Name.HeaderText = Translation.EquipmentView_Name;

		TitleAircraftCost.Text = Translation.TitleAircraftCost;
		TitleAircraftDistance.Text = Translation.TitleAircraftDistance;
		imageLabel1.Text = Translation.InitialEquipmentShip;
		imageLabel2.Text = Translation.LibraryId;
		Description.Text = Translation.Description;
		EquipmentType.Text = Translation.EquipmentType;
		EquipmentName.Text = Translation.EquipmentName;

		TitleRange.Text = Translation.TitleRange;
		TitleSpeed.Text = Translation.TitleSpeed;
		TitleBomber.Text = Translation.TitleBomber;
		TitleLOS.Text = Translation.TitleLOS;
		TitleFirepower.Text = Translation.TitleFirepower;
		TitleTorpedo.Text = Translation.TitleTorpedo;
		TitleAA.Text = Translation.TitleAA;
		TitleArmor.Text = Translation.TitleArmor;
		TitleASW.Text = Translation.TitleASW;
		TitleEvasion.Text = Translation.TitleEvasion;
		TitleAccuracy.Text = Translation.TitleAccuracy;

		SaveCSVDialog.Title = Translation.SaveCSVDialog;

		Text = Translation.Title;
	}

	private void EquipmentView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
	{

		if (e.Column.Name == EquipmentView_Type.Name)
		{
			e.SortResult =
				KCDatabase.Instance.MasterEquipments[(int)EquipmentView.Rows[e.RowIndex1].Cells[0].Value].EquipmentType[2] -
				KCDatabase.Instance.MasterEquipments[(int)EquipmentView.Rows[e.RowIndex2].Cells[0].Value].EquipmentType[2];
		}
		else
		{
			e.SortResult = ((IComparable)e.CellValue1).CompareTo(e.CellValue2);
		}

		if (e.SortResult == 0)
		{
			e.SortResult = (int)(EquipmentView.Rows[e.RowIndex1].Tag ?? 0) - (int)(EquipmentView.Rows[e.RowIndex2].Tag ?? 0);
		}

		e.Handled = true;
	}

	private void EquipmentView_Sorted(object sender, EventArgs e)
	{

		for (int i = 0; i < EquipmentView.Rows.Count; i++)
		{
			EquipmentView.Rows[i].Tag = i;
		}
	}


	private void EquipmentView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{

		if (e.ColumnIndex == EquipmentView_Icon.Index)
		{
			e.Value = ResourceManager.GetEquipmentImage((int)e.Value);
			e.FormattingApplied = true;
		}

	}



	private void EquipmentView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
	{

		if (e.RowIndex >= 0)
		{
			int equipmentID = (int)EquipmentView.Rows[e.RowIndex].Cells[0].Value;

			if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
			{
				Cursor = Cursors.AppStarting;
				new DialogAlbumMasterEquipment(equipmentID).Show(Owner);
				Cursor = Cursors.Default;

			}
			else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
			{
				UpdateAlbumPage(equipmentID);
			}
		}

	}




	private void UpdateAlbumPage(int equipmentID)
	{

		KCDatabase db = KCDatabase.Instance;
		EquipmentDataMaster eq = db.MasterEquipments[equipmentID];

		if (eq == null) return;


		BasePanelEquipment.SuspendLayout();


		//header
		EquipmentID.Tag = equipmentID;
		EquipmentID.Text = eq.EquipmentID.ToString();
		ToolTipInfo.SetToolTip(EquipmentID, string.Format("Type: [ {0} ]", string.Join(", ", eq.EquipmentType)));
		AlbumNo.Text = eq.AlbumNo.ToString();


		TableEquipmentName.SuspendLayout();

		EquipmentType.Text = eq.CategoryTypeInstance.NameEN;

		{
			int eqicon = eq.IconType;
			if (eqicon >= (int)ResourceManager.EquipmentContent.Locked)
				eqicon = (int)ResourceManager.EquipmentContent.Unknown;
			EquipmentType.ImageIndex = eqicon;

			ToolTipInfo.SetToolTip(EquipmentType, GetEquippableShips(equipmentID));
		}
		EquipmentName.Text = eq.NameEN;
		ToolTipInfo.SetToolTip(EquipmentName, Translation.RightClickToCopy);

		TableEquipmentName.ResumeLayout();


		//main parameter
		TableParameterMain.SuspendLayout();

		SetParameterText(Firepower, eq.Firepower);
		SetParameterText(Torpedo, eq.Torpedo);
		SetParameterText(AA, eq.AA);
		SetParameterText(Armor, eq.Armor);
		SetParameterText(ASW, eq.ASW);
		SetParameterText(Evasion, eq.Evasion);
		SetParameterText(LOS, eq.LOS);
		SetParameterText(Accuracy, eq.Accuracy);
		SetParameterText(Bomber, eq.Bomber);

		if (eq.CategoryType == EquipmentTypes.Interceptor)
		{
			TitleAccuracy.Text = Translation.AntiBomber;
			TitleAccuracy.ImageIndex = (int)IconContent.ParameterAntiBomber;
			TitleEvasion.Text = Translation.Interception;
			TitleEvasion.ImageIndex = (int)IconContent.ParameterInterception;
		}
		else
		{
			TitleAccuracy.Text = Translation.TitleAccuracy;
			TitleAccuracy.ImageIndex = (int)IconContent.ParameterAccuracy;
			TitleEvasion.Text = Translation.TitleEvasion;
			TitleEvasion.ImageIndex = (int)IconContent.ParameterEvasion;
		}

		TableParameterMain.ResumeLayout();


		//sub parameter
		TableParameterSub.SuspendLayout();

		Speed.Text = EncycloRes.None; //Constants.GetSpeed( eq.Speed );
		Range.Text = Constants.GetRange(eq.Range);
		Rarity.Text = Constants.GetEquipmentRarity(eq.Rarity);
		Rarity.ImageIndex = (int)IconContent.RarityRed + Constants.GetEquipmentRarityID(eq.Rarity);     //checkme

		TableParameterSub.ResumeLayout();


		// aircraft
		if (eq.IsAircraft)
		{
			TableAircraft.SuspendLayout();
			AircraftCost.Text = eq.AircraftCost.ToString();
			ToolTipInfo.SetToolTip(AircraftCost, EncycloRes.AircraftCostHint + "：" + ((eq.IsCombatAircraft ? 18 : 4) * eq.AircraftCost));
			AircraftDistance.Text = eq.AircraftDistance.ToString();
			TableAircraft.ResumeLayout();
			TableAircraft.Visible = true;
		}
		else
		{
			TableAircraft.Visible = false;
		}


		//default equipment
		DefaultSlots.BeginUpdate();
		DefaultSlots.Items.Clear();
		foreach (var ship in KCDatabase.Instance.MasterShips.Values)
		{
			if (ship.DefaultSlot != null && ship.DefaultSlot.Contains(equipmentID))
			{
				DefaultSlots.Items.Add(ship);
			}
		}
		DefaultSlots.EndUpdate();


		Description.Text = eq.Message;


		//arsenal
		TableArsenal.SuspendLayout();

		MaterialFuel.Text = eq.Material[0].ToString();
		MaterialAmmo.Text = eq.Material[1].ToString();
		MaterialSteel.Text = eq.Material[2].ToString();
		MaterialBauxite.Text = eq.Material[3].ToString();

		TableArsenal.ResumeLayout();



		//装備画像を読み込んでみる
		{
			var img =
				KCResourceHelper.LoadEquipmentImage(equipmentID, KCResourceHelper.ResourceTypeEquipmentCard) ??
				KCResourceHelper.LoadEquipmentImage(equipmentID, KCResourceHelper.ResourceTypeEquipmentCardSmall);

			if (img != null)
			{
				EquipmentImage.Image?.Dispose();
				EquipmentImage.Image = img;
			}
			else
			{
				EquipmentImage.Image?.Dispose();
				EquipmentImage.Image = null;
			}
		}


		BasePanelEquipment.ResumeLayout();
		BasePanelEquipment.Visible = true;


		this.Text = EncycloRes.EquipmentEncyclopedia + " - " + eq.NameEN;

	}


	private void SetParameterText(ImageLabel label, int value)
	{

		if (value > 0)
		{
			label.ForeColor = SystemColors.ControlText;
			label.Text = "+" + value.ToString();
		}
		else if (value == 0)
		{
			label.ForeColor = Color.Silver;
			label.Text = "0";
		}
		else
		{
			label.ForeColor = Color.Red;
			label.Text = value.ToString();
		}

	}

	private string GetEquippableShips(int equipmentID)
	{
		var db = KCDatabase.Instance;

		var sb = new StringBuilder();
		sb.AppendLine($"{Translation.Equippable}:");

		var eq = db.MasterEquipments[equipmentID];
		if (eq == null)
			return sb.ToString();

		int eqCategory = (int)eq.CategoryType;

		var specialShips = new Dictionary<ShipTypes, List<string>>();

		foreach (var ship in db.MasterShips.Values.Where(s => s.SpecialEquippableCategories != null))
		{
			bool usual = ship.ShipTypeInstance.EquippableCategories.Contains(eqCategory);
			bool special = ship.SpecialEquippableCategories.Contains(eqCategory);

			if (usual != special)
			{
				if (specialShips.ContainsKey(ship.ShipType))
					specialShips[ship.ShipType].Add(ship.NameWithClass);
				else
					specialShips.Add(ship.ShipType, new List<string>(new[] { ship.NameWithClass }));
			}
		}

		foreach (var shiptype in db.ShipTypes.Values)
		{
			if (shiptype.EquippableCategories.Contains(eqCategory))
			{
				sb.Append(shiptype.NameEN);

				if (specialShips.ContainsKey(shiptype.Type))
				{
					sb.Append(" (").Append(string.Join(", ", specialShips[shiptype.Type])).Append($"{Translation.Excluding})");
				}

				sb.AppendLine();
			}
			else
			{
				if (specialShips.ContainsKey(shiptype.Type))
				{
					sb.Append("○ ").AppendLine(string.Join(", ", specialShips[shiptype.Type]));
				}
			}
		}

		if (eq.EquippableShipsAtExpansion.Any())
			sb.Append($"[{Translation.ExpansionSlot}] ").AppendLine(string.Join(", ", eq.EquippableShipsAtExpansion.Select(id => db.MasterShips[id].NameWithClass)));

		return sb.ToString();
	}


	private void DefaultSlots_MouseDown(object sender, MouseEventArgs e)
	{

		if (e.Button == System.Windows.Forms.MouseButtons.Right)
		{
			int index = DefaultSlots.IndexFromPoint(e.Location);
			if (index >= 0)
			{
				Cursor = Cursors.AppStarting;
				new DialogAlbumMasterShipWpf(((ShipDataMaster)DefaultSlots.Items[index]).ShipID).Show();
				Cursor = Cursors.Default;
			}
		}
	}



	private void TableParameterMain_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
		/*/
		if ( e.Column == 0 )
			e.Graphics.DrawLine( Pens.Silver, e.CellBounds.Right - 1, e.CellBounds.Y, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1 );
		//*/
	}

	private void TableParameterSub_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
	}



	private void TableArsenal_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
	}

	private void TableAircraft_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
	}



	private void StripMenu_File_OutputCSVUser_Click(object sender, EventArgs e)
	{

		if (SaveCSVDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		{

			try
			{

				using (StreamWriter sw = new StreamWriter(SaveCSVDialog.FileName, false, Utility.Configuration.Config.Log.FileEncoding))
				{

					sw.WriteLine("装備ID,図鑑番号,装備種,装備名,大分類,図鑑カテゴリID,カテゴリID,アイコンID,航空機グラフィックID,火力,雷装,対空,装甲,対潜,回避,索敵,運,命中,爆装,射程,レア,廃棄燃料,廃棄弾薬,廃棄鋼材,廃棄ボーキ,図鑑文章,戦闘行動半径,配置コスト");

					foreach (EquipmentDataMaster eq in KCDatabase.Instance.MasterEquipments.Values)
					{

						sw.WriteLine(string.Join(",",
							eq.EquipmentID,
							eq.AlbumNo,
							CsvHelper.EscapeCsvCell(eq.CategoryTypeInstance.NameEN),
							CsvHelper.EscapeCsvCell(eq.NameEN),
							eq.EquipmentType[0],
							eq.EquipmentType[1],
							eq.EquipmentType[2],
							eq.EquipmentType[3],
							eq.EquipmentType[4],
							eq.Firepower,
							eq.Torpedo,
							eq.AA,
							eq.Armor,
							eq.ASW,
							eq.Evasion,
							eq.LOS,
							eq.Luck,
							eq.Accuracy,
							eq.Bomber,
							Constants.GetRange(eq.Range),
							Constants.GetEquipmentRarity(eq.Rarity),
							eq.Material[0],
							eq.Material[1],
							eq.Material[2],
							eq.Material[3],
							CsvHelper.EscapeCsvCell(eq.Message),
							eq.AircraftDistance,
							eq.AircraftCost
						));

					}

				}

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, EncycloRes.FailedOutputEquipCSV);
				MessageBox.Show(EncycloRes.FailedOutputEquipCSV + "\r\n" + ex.Message, Translation.DialogTitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}


	}


	private void StripMenu_File_OutputCSVData_Click(object sender, EventArgs e)
	{

		if (SaveCSVDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		{

			try
			{

				using (StreamWriter sw = new StreamWriter(SaveCSVDialog.FileName, false, Utility.Configuration.Config.Log.FileEncoding))
				{

					sw.WriteLine("装備ID,図鑑番号,装備名,装備種1,装備種2,装備種3,装備種4,装備種5,火力,雷装,対空,装甲,対潜,回避,索敵,運,命中,爆装,射程,レア,廃棄燃料,廃棄弾薬,廃棄鋼材,廃棄ボーキ,図鑑文章,戦闘行動半径,配置コスト");

					foreach (EquipmentDataMaster eq in KCDatabase.Instance.MasterEquipments.Values)
					{

						sw.WriteLine(string.Join(",",
							eq.EquipmentID,
							eq.AlbumNo,
							CsvHelper.EscapeCsvCell(eq.NameEN),
							eq.EquipmentType[0],
							eq.EquipmentType[1],
							eq.EquipmentType[2],
							eq.EquipmentType[3],
							eq.EquipmentType[4],
							eq.Firepower,
							eq.Torpedo,
							eq.AA,
							eq.Armor,
							eq.ASW,
							eq.Evasion,
							eq.LOS,
							eq.Luck,
							eq.Accuracy,
							eq.Bomber,
							eq.Range,
							eq.Rarity,
							eq.Material[0],
							eq.Material[1],
							eq.Material[2],
							eq.Material[3],
							CsvHelper.EscapeCsvCell(eq.Message),
							eq.AircraftDistance,
							eq.AircraftCost
						));

					}

				}

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, Translation.FailedToExportCsv);
				MessageBox.Show($"{Translation.FailedToExportCsv}\r\n" + ex.Message, Translation.DialogTitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

	}


	private void TextSearch_TextChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(TextSearch.Text))
			return;


		bool Search(string searchWord)
		{
			var target =
				EquipmentView.Rows.OfType<DataGridViewRow>()
					.Select(r => KCDatabase.Instance.MasterEquipments[(int)r.Cells[EquipmentView_ID.Index].Value])
					.FirstOrDefault(
						eq => Calculator.ToHiragana(eq.NameEN.ToLower()).Contains(searchWord));

			if (target != null)
			{
				EquipmentView.FirstDisplayedScrollingRowIndex = EquipmentView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[EquipmentView_ID.Index].Value == target.EquipmentID).Index;
				return true;
			}
			return false;
		}

		if (!Search(Calculator.ToHiragana(TextSearch.Text.ToLower())))
			Search(Calculator.RomaToHira(TextSearch.Text));
	}

	private void TextSearch_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Enter)
		{
			TextSearch_TextChanged(sender, e);
			e.SuppressKeyPress = true;
			e.Handled = true;
		}
	}




	private void DialogAlbumMasterEquipment_FormClosed(object sender, FormClosedEventArgs e)
	{

		ResourceManager.DestroyIcon(Icon);

	}

	private void StripMenu_Edit_CopyEquipmentName_Click(object sender, EventArgs e)
	{
		var eq = KCDatabase.Instance.MasterEquipments[EquipmentID.Tag as int? ?? -1];
		if (eq != null)
			Clipboard.SetText(eq.NameEN);
		else
			System.Media.SystemSounds.Exclamation.Play();
	}

	private void EquipmentName_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == System.Windows.Forms.MouseButtons.Right)
		{
			var eq = KCDatabase.Instance.MasterEquipments[EquipmentID.Tag as int? ?? -1];
			if (eq != null)
				Clipboard.SetText(eq.NameEN);
			else
				System.Media.SystemSounds.Exclamation.Play();
		}
	}

	private void StripMenu_Edit_CopyEquipmentData_Click(object sender, EventArgs e)
	{
		var eq = KCDatabase.Instance.MasterEquipments[EquipmentID.Tag as int? ?? -1];
		if (eq == null)
		{
			System.Media.SystemSounds.Exclamation.Play();
			return;
		}

		var sb = new StringBuilder();

		sb.AppendFormat("{0} {1}\r\n", eq.CategoryTypeInstance.NameEN, eq.NameEN);
		sb.AppendFormat("ID: {0} / 図鑑番号: {1} / カテゴリID: [{2}]\r\n", eq.EquipmentID, eq.AlbumNo, string.Join(", ", eq.EquipmentType));

		sb.AppendLine();

		if (eq.Firepower != 0) sb.AppendFormat("火力: {0:+0;-0;0}\r\n", eq.Firepower);
		if (eq.Torpedo != 0) sb.AppendFormat("雷装: {0:+0;-0;0}\r\n", eq.Torpedo);
		if (eq.AA != 0) sb.AppendFormat("対空: {0:+0;-0;0}\r\n", eq.AA);
		if (eq.Armor != 0) sb.AppendFormat("装甲: {0:+0;-0;0}\r\n", eq.Armor);
		if (eq.ASW != 0) sb.AppendFormat("対潜: {0:+0;-0;0}\r\n", eq.ASW);
		if (eq.Evasion != 0) sb.AppendFormat("{0}: {1:+0;-0;0}\r\n", eq.CategoryType == EquipmentTypes.Interceptor ? "迎撃" : "回避", eq.Evasion);
		if (eq.LOS != 0) sb.AppendFormat("索敵: {0:+0;-0;0}\r\n", eq.LOS);
		if (eq.Accuracy != 0) sb.AppendFormat("{0}: {1:+0;-0;0}\r\n", eq.CategoryType == EquipmentTypes.Interceptor ? "対爆" : "命中", eq.Accuracy);
		if (eq.Bomber != 0) sb.AppendFormat("爆装: {0:+0;-0;0}\r\n", eq.Bomber);
		if (eq.Luck != 0) sb.AppendFormat("運: {0:+0;-0;0}\r\n", eq.Luck);

		if (eq.Range > 0) sb.Append("射程: ").AppendLine(Constants.GetRange(eq.Range));

		if (eq.AircraftCost > 0) sb.AppendFormat("配備コスト: {0}\r\n", eq.AircraftCost);
		if (eq.AircraftDistance > 0) sb.AppendFormat("戦闘行動半径: {0}\r\n", eq.AircraftDistance);

		sb.AppendLine();

		sb.AppendFormat("レアリティ: {0}\r\n", Constants.GetEquipmentRarity(eq.Rarity));
		sb.AppendFormat("廃棄資材: {0}\r\n", string.Join(" / ", eq.Material));

		sb.AppendLine();

		sb.AppendFormat("図鑑説明: \r\n{0}\r\n",
			!string.IsNullOrWhiteSpace(eq.Message) ? eq.Message : "(不明)");

		sb.AppendLine();

		sb.AppendLine("初期装備/開発:");
		string result = GetAppearingArea(eq.EquipmentID);
		if (string.IsNullOrWhiteSpace(result))
			result = "(不明)\r\n";
		sb.AppendLine(result);


		Clipboard.SetText(sb.ToString());
	}


	private string GetAppearingArea(int equipmentID)
	{
		var sb = new StringBuilder();

		foreach (var ship in KCDatabase.Instance.MasterShips.Values
			.Where(s => s.DefaultSlot != null && s.DefaultSlot.Contains(equipmentID)))
		{
			sb.AppendLine(ship.NameWithClass);
		}

		foreach (var record in RecordManager.Instance.Development.Record
			.Where(r => r.EquipmentID == equipmentID)
			.Select(r => new
			{
				r.Fuel,
				r.Ammo,
				r.Steel,
				r.Bauxite
			})
			.Distinct()
			.OrderBy(r => r.Fuel)
			.ThenBy(r => r.Ammo)
			.ThenBy(r => r.Steel)
			.ThenBy(r => r.Bauxite)
		)
		{
			sb.AppendFormat(Translation.Recipe + " {0} / {1} / {2} / {3}\r\n",
				record.Fuel, record.Ammo, record.Steel, record.Bauxite);
		}

		return sb.ToString();
	}

	private void StripMenu_View_ShowAppearingArea_Click(object sender, EventArgs e)
	{

		int eqID = EquipmentID.Tag as int? ?? -1;
		var eq = KCDatabase.Instance.MasterEquipments[eqID];

		if (eq == null)
		{
			System.Media.SystemSounds.Exclamation.Play();
			return;
		}

		string result = GetAppearingArea(eqID);

		if (string.IsNullOrWhiteSpace(result))
		{
			result = string.Format(Translation.FailedToFindShipOrRecipe, eq.NameEN);
		}

		MessageBox.Show(result, Translation.ShipOrRecipeCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
	}


	private void StripMenu_Edit_GoogleEquipmentName_Click(object sender, EventArgs e)
	{
		var eq = KCDatabase.Instance.MasterEquipments[EquipmentID.Tag as int? ?? -1];
		if (eq == null)
		{
			System.Media.SystemSounds.Exclamation.Play();
			return;
		}

		try
		{
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = @"https://www.duckduckgo.com/?q=" + Uri.EscapeDataString(eq.NameEN) + Translation.KancolleSpecifier,
				UseShellExecute = true
			};
			// google <装備名> 艦これ
			Process.Start(psi);

		}
		catch (Exception ex)
		{
			Utility.ErrorReporter.SendErrorReport(ex, Translation.FailedToSearchOnWeb);
		}
	}

}
