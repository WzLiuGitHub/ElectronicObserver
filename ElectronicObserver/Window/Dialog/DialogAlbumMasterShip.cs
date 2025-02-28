﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ElectronicObserverTypes;
using static ElectronicObserver.Resource.Record.ShipParameterRecord;
using AlbumMasterTranslation = ElectronicObserver.Properties.Window.Dialog.DialogAlbumMasterEquipment;
using Translation = ElectronicObserver.Properties.Window.Dialog.DialogAlbumMasterShip;

namespace ElectronicObserver.Window.Dialog;

public partial class DialogAlbumMasterShip : Form
{

	private int _shipID;

	private ImageLabel[] Aircrafts;
	private ImageLabel[] Equipments;

	private int loadingResourceShipID;


	public DialogAlbumMasterShip()
	{
		InitializeComponent();

		Aircrafts = new ImageLabel[] { Aircraft1, Aircraft2, Aircraft3, Aircraft4, Aircraft5 };
		Equipments = new ImageLabel[] { Equipment1, Equipment2, Equipment3, Equipment4, Equipment5 };

		loadingResourceShipID = -1;

		TitleHP.ImageList =
			TitleFirepower.ImageList =
				TitleTorpedo.ImageList =
					TitleAA.ImageList =
						TitleArmor.ImageList =
							TitleASW.ImageList =
								TitleEvasion.ImageList =
									TitleLOS.ImageList =
										TitleLuck.ImageList =
											Accuracy.ImageList =
												TitleSpeed.ImageList =
													TitleRange.ImageList =
														Rarity.ImageList =
															Fuel.ImageList =
																Ammo.ImageList =
																	TitleBuildingTime.ImageList =
																		MaterialFuel.ImageList =
																			MaterialAmmo.ImageList =
																				MaterialSteel.ImageList =
																					MaterialBauxite.ImageList =
																						PowerUpFirepower.ImageList =
																							PowerUpTorpedo.ImageList =
																								PowerUpAA.ImageList =
																									PowerUpArmor.ImageList =
																										RemodelBeforeLevel.ImageList =
																											RemodelBeforeAmmo.ImageList =
																												RemodelBeforeSteel.ImageList =
																													RemodelAfterLevel.ImageList =
																														RemodelAfterAmmo.ImageList =
																															RemodelAfterSteel.ImageList =
																																ResourceManager.Instance.Icons;

		TitleAirSuperiority.ImageList =
			TitleDayAttack.ImageList =
				TitleNightAttack.ImageList =
					Equipment1.ImageList =
						Equipment2.ImageList =
							Equipment3.ImageList =
								Equipment4.ImageList =
									Equipment5.ImageList =
										ResourceManager.Instance.Equipments;

		TitleHP.ImageIndex = (int)IconContent.ParameterHP;
		TitleFirepower.ImageIndex = (int)IconContent.ParameterFirepower;
		TitleTorpedo.ImageIndex = (int)IconContent.ParameterTorpedo;
		TitleAA.ImageIndex = (int)IconContent.ParameterAA;
		TitleArmor.ImageIndex = (int)IconContent.ParameterArmor;
		TitleASW.ImageIndex = (int)IconContent.ParameterASW;
		TitleEvasion.ImageIndex = (int)IconContent.ParameterEvasion;
		TitleLOS.ImageIndex = (int)IconContent.ParameterLOS;
		TitleLuck.ImageIndex = (int)IconContent.ParameterLuck;
		Accuracy.ImageIndex = (int)IconContent.ParameterAccuracy;
		TitleSpeed.ImageIndex = (int)IconContent.ParameterSpeed;
		TitleRange.ImageIndex = (int)IconContent.ParameterRange;
		Fuel.ImageIndex = (int)IconContent.ResourceFuel;
		Ammo.ImageIndex = (int)IconContent.ResourceAmmo;
		TitleBuildingTime.ImageIndex = (int)IconContent.FormArsenal;
		MaterialFuel.ImageIndex = (int)IconContent.ResourceFuel;
		MaterialAmmo.ImageIndex = (int)IconContent.ResourceAmmo;
		MaterialSteel.ImageIndex = (int)IconContent.ResourceSteel;
		MaterialBauxite.ImageIndex = (int)IconContent.ResourceBauxite;
		PowerUpFirepower.ImageIndex = (int)IconContent.ParameterFirepower;
		PowerUpTorpedo.ImageIndex = (int)IconContent.ParameterTorpedo;
		PowerUpAA.ImageIndex = (int)IconContent.ParameterAA;
		PowerUpArmor.ImageIndex = (int)IconContent.ParameterArmor;
		RemodelBeforeAmmo.ImageIndex = (int)IconContent.ResourceAmmo;
		RemodelBeforeSteel.ImageIndex = (int)IconContent.ResourceSteel;
		RemodelAfterAmmo.ImageIndex = (int)IconContent.ResourceAmmo;
		RemodelAfterSteel.ImageIndex = (int)IconContent.ResourceSteel;
		TitleAirSuperiority.ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter;
		TitleDayAttack.ImageIndex = (int)ResourceManager.EquipmentContent.Seaplane;
		TitleNightAttack.ImageIndex = (int)ResourceManager.EquipmentContent.Torpedo;

		ParameterLevel.Value = ParameterLevel.Maximum = ExpTable.ShipMaximumLevel;


		TableBattle.Visible = false;
		BasePanelShipGirl.Visible = false;


		ControlHelper.SetDoubleBuffered(TableShipName);
		ControlHelper.SetDoubleBuffered(TableParameterMain);
		ControlHelper.SetDoubleBuffered(TableParameterSub);
		ControlHelper.SetDoubleBuffered(TableConsumption);
		ControlHelper.SetDoubleBuffered(TableEquipment);
		ControlHelper.SetDoubleBuffered(TableArsenal);
		ControlHelper.SetDoubleBuffered(TableRemodel);
		ControlHelper.SetDoubleBuffered(TableBattle);

		ControlHelper.SetDoubleBuffered(ShipView);


		//ShipView Initialize
		ShipView.SuspendLayout();

		ShipView_ShipID.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
		ShipView_ShipType.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;


		ShipView.Rows.Clear();

		List<DataGridViewRow> rows = new List<DataGridViewRow>(KCDatabase.Instance.MasterShips.Values.Count(s => s.Name != "なし"));

		foreach (var ship in KCDatabase.Instance.MasterShips.Values)
		{

			if (ship.Name == "なし") continue;

			DataGridViewRow row = new DataGridViewRow();
			row.CreateCells(ShipView);
			row.SetValues(ship.ShipID, ship.ShipTypeName, ship.NameWithClass);
			row.Cells[ShipView_ShipType.Index].Tag = ship.ShipType;
			row.Cells[ShipView_Name.Index].Tag = ship.IsAbyssalShip ? null : ship.NameEN;
			rows.Add(row);

		}
		ShipView.Rows.AddRange(rows.ToArray());

		ShipView_ShipID.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
		ShipView_ShipType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

		ShipView.Sort(ShipView_ShipID, ListSortDirection.Ascending);
		ShipView.ResumeLayout();

		Translate();
	}

	public DialogAlbumMasterShip(int shipID)
		: this()
	{

		UpdateAlbumPage(shipID);


		if (KCDatabase.Instance.MasterShips.ContainsKey(shipID))
		{
			var row = ShipView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[ShipView_ShipID.Index].Value == shipID);
			if (row != null)
				ShipView.FirstDisplayedScrollingRowIndex = row.Index;
		}

	}

	public void Translate()
	{
		StripMenu_File.Text = AlbumMasterTranslation.StripMenu_File;
		StripMenu_File_OutputCSVUser.Text = AlbumMasterTranslation.StripMenu_File_OutputCSVUser;
		StripMenu_File_OutputCSVData.Text = AlbumMasterTranslation.StripMenu_File_OutputCSVData;
		StripMenu_File_MergeDefaultRecord.Text = Translation.StripMenu_File_MergeDefaultRecord;

		StripMenu_Edit.Text = AlbumMasterTranslation.StripMenu_Edit;
		StripMenu_Edit_EditParameter.Text = Translation.StripMenu_Edit_EditParameter;
		StripMenu_Edit_CopyShipName.Text = Translation.StripMenu_Edit_CopyShipName;
		StripMenu_Edit_CopyShipData.Text = Translation.StripMenu_Edit_CopyShipData;
		StripMenu_Edit_GoogleShipName.Text = Translation.StripMenu_Edit_GoogleShipName;
		StripMenu_Edit_CopySpecialEquipmentTable.Text = Translation.StripMenu_Edit_CopySpecialEquipmentTable;

		StripMenu_View.Text = AlbumMasterTranslation.StripMenu_View;
		StripMenu_View_ShowAppearingArea.Text = AlbumMasterTranslation.StripMenu_View_ShowAppearingArea;
		StripMenu_View_ShowShipGraphicViewer.Text = Translation.StripMenu_View_ShowShipGraphicViewer;

		ShipView_ShipType.HeaderText = EncycloRes.ShipType;
		ShipView_Name.HeaderText = EncycloRes.ShipName;

		TitleDayAttack.Text = EncycloRes.DayAttack;
		TitleNightAttack.Text = EncycloRes.NightAttack;
		TitleAirSuperiority.Text = EncycloRes.AirPower;

		ToolTipInfo.SetToolTip(ParameterLevel, Translation.ParameterLevelToolTip);

		imageLabel2.Text = AlbumMasterTranslation.LibraryId;

		TitleRange.Text = AlbumMasterTranslation.TitleRange;
		TitleSpeed.Text = AlbumMasterTranslation.TitleSpeed;
		TitleLOS.Text = AlbumMasterTranslation.TitleLOS;
		TitleFirepower.Text = AlbumMasterTranslation.TitleFirepower;
		TitleTorpedo.Text = AlbumMasterTranslation.TitleTorpedo;
		TitleAA.Text = AlbumMasterTranslation.TitleAA;
		TitleArmor.Text = AlbumMasterTranslation.TitleArmor;
		TitleASW.Text = AlbumMasterTranslation.TitleASW;
		TitleEvasion.Text = AlbumMasterTranslation.TitleEvasion;

		TitleParameterMax.Text = EncycloRes.Maximum;
		TitleParameterMin.Text = EncycloRes.Initial;
		TitleHP.Text = Translation.TitleHP;

		ToolTipInfo.SetToolTip(ShipBanner, Translation.ShipBannerToolTip);

		SaveCSVDialog.Title = AlbumMasterTranslation.SaveCSVDialog;

		Text = EncycloRes.ShipEncyclopedia;
	}

	private void DialogAlbumMasterShip_Load(object sender, EventArgs e)
	{

		this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)IconContent.FormAlbumShip]);

	}




	private void ShipView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
	{

		if (e.Column.Index == ShipView_ShipType.Index)
		{
			e.SortResult = (int)ShipView[e.Column.Index, e.RowIndex1].Tag - (int)ShipView[e.Column.Index, e.RowIndex2].Tag;

		}
		else if (e.Column.Index == ShipView_Name.Index)
		{

			// 艦娘優先; 艦娘同士なら読みで比べる、深海棲艦同士なら名前で比べる

			string tag1 = ShipView[e.Column.Index, e.RowIndex1].Tag as string;
			string tag2 = ShipView[e.Column.Index, e.RowIndex2].Tag as string;

			if (tag1 != null)
			{
				if (tag2 != null)
					e.SortResult = tag1.CompareTo(tag2);
				else
					e.SortResult = -1;
			}
			else
			{
				if (tag2 != null)
					e.SortResult = 1;
				else
					e.SortResult = 0;
			}

			if (e.SortResult == 0)
				e.SortResult = ((string)e.CellValue1).CompareTo(e.CellValue2);

		}
		else
		{
			e.SortResult = ((IComparable)e.CellValue1).CompareTo(e.CellValue2);
		}

		if (e.SortResult == 0)
		{
			e.SortResult = (int)(ShipView.Rows[e.RowIndex1].Tag ?? 0) - (int)(ShipView.Rows[e.RowIndex2].Tag ?? 0);
		}

		e.Handled = true;
	}

	private void ShipView_Sorted(object sender, EventArgs e)
	{

		for (int i = 0; i < ShipView.Rows.Count; i++)
		{
			ShipView.Rows[i].Tag = i;
		}

	}



	private void ShipView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
	{

		if (e.RowIndex >= 0)
		{
			int shipID = (int)ShipView.Rows[e.RowIndex].Cells[0].Value;

			if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
			{
				Cursor = Cursors.AppStarting;
				new DialogAlbumMasterShip(shipID).Show(Owner);
				Cursor = Cursors.Default;

			}
			else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
			{
				UpdateAlbumPage(shipID);
			}
		}

	}




	private void UpdateAlbumPage(int shipID)
	{

		KCDatabase db = KCDatabase.Instance;
		ShipDataMaster ship = db.MasterShips[shipID];

		if (ship == null) return;


		BasePanelShipGirl.SuspendLayout();

		//header
		TableShipName.SuspendLayout();
		_shipID = shipID;
		ShipID.Text = ship.ShipID.ToString();
		AlbumNo.Text = ship.AlbumNo.ToString();

		ResourceName.Text = $"{ship.ResourceName} {ship.ResourceGraphicVersion}/{ship.ResourceVoiceVersion}/{ship.ResourcePortVoiceVersion}";
		ToolTipInfo.SetToolTip(ResourceName, string.Format(Translation.ResourceNameToolTip,
			ship.ResourceName, ship.ResourceGraphicVersion, ship.ResourceVoiceVersion, ship.ResourcePortVoiceVersion, Constants.GetVoiceFlag(ship.VoiceFlag)));


		ShipType.Text = ship.IsLandBase ? "Land Base" : ship.ShipTypeName;
		{
			string shipClassName = Constants.GetShipClass(ship.ShipClass);
			bool isShipClassUnknown = shipClassName == "不明";

			ShipType.Text = (ship.IsAbyssalShip ? "深海" : isShipClassUnknown ? "" : shipClassName) + (ship.IsLandBase ? "陸上施設" : ship.ShipTypeName);

			var tip = new StringBuilder();
			if (ship.IsAbyssalShip)
				tip.AppendLine($"{Translation.ShipClassId}: {ship.ShipClass}");
			else if (Constants.GetShipClass(ship.ShipClass) == "不明" || Constants.GetShipClass(ship.ShipClass) == "Unknown")
				tip.AppendLine($"{Translation.ShipClassUnknown}: {ship.ShipClass}");
			else
				tip.AppendLine($"{shipClassName}: {ship.ShipClass}");

			tip.AppendLine();
			tip.AppendLine($"{AlbumMasterTranslation.Equippable}:");
			tip.AppendLine(GetEquippableString(shipID));

			ToolTipInfo.SetToolTip(ShipType, tip.ToString());
		}
		ShipName.Text = ship.NameWithClass;
		ShipName.ForeColor = ship.GetShipNameColor();
		if (ShipName.ForeColor == Color.FromArgb(0xFF, 0xFF, 0xFF))
		{
			ShipName.ForeColor = SystemColors.ControlText;
		}

		ToolTipInfo.SetToolTip(ShipName, (!ship.IsAbyssalShip ? ship.NameReading + "\r\n" : "") + AlbumMasterTranslation.RightClickToCopy);
		TableShipName.ResumeLayout();


		//main parameter
		TableParameterMain.SuspendLayout();

		if (!ship.IsAbyssalShip)
		{

			TitleParameterMin.Text = EncycloRes.Initial;
			TitleParameterMax.Text = EncycloRes.Maximum;

			HPMin.Text = ship.HPMin.ToString();
			HPMax.Text = ship.HPMaxMarried.ToString();
			ToolTipInfo.SetToolTip(HPMin, string.Format(Translation.HpMinToolTip, ship.HPMaxModernized, ship.HPMaxModernizable));
			ToolTipInfo.SetToolTip(HPMax, string.Format(Translation.HpMaxToolTip, ship.HPMaxMarriedModernized, ship.HPMaxMarriedModernizable, ship.HPMax));

			FirepowerMin.Text = ship.FirepowerMin.ToString();
			FirepowerMax.Text = ship.FirepowerMax.ToString();

			TorpedoMin.Text = ship.TorpedoMin.ToString();
			TorpedoMax.Text = ship.TorpedoMax.ToString();

			AAMin.Text = ship.AAMin.ToString();
			AAMax.Text = ship.AAMax.ToString();

			ArmorMin.Text = ship.ArmorMin.ToString();
			ArmorMax.Text = ship.ArmorMax.ToString();

			ASWMin.Text = GetParameterMinBound(ship.ASW);
			ASWMax.Text = GetParameterMax(ship.ASW);

			EvasionMin.Text = GetParameterMinBound(ship.Evasion);
			EvasionMax.Text = GetParameterMax(ship.Evasion);

			LOSMin.Text = GetParameterMinBound(ship.LOS);
			LOSMax.Text = GetParameterMax(ship.LOS);

			LuckMin.Text = ship.LuckMin.ToString();
			LuckMax.Text = ship.LuckMax.ToString();

			Accuracy.Visible = false;
		}
		else
		{

			int hp = ship.HPMin;
			int firepower = ship.FirepowerMax;
			int torpedo = ship.TorpedoMax;
			int aa = ship.AAMax;
			int armor = ship.ArmorMax;
			int asw = ship.ASW?.IsDetermined ?? false ? ship.ASW.Maximum : 0;
			int evasion = ship.Evasion?.IsDetermined ?? false ? ship.Evasion.Maximum : 0;
			int los = ship.LOS?.IsDetermined ?? false ? ship.LOS.Maximum : 0;
			int luck = ship.LuckMax;
			int accuracy = 0;

			if (ship.DefaultSlot != null)
			{
				int count = ship.DefaultSlot.Count;
				for (int i = 0; i < count; i++)
				{
					var eq = KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[i]];
					if (eq == null)
						continue;

					firepower += eq.Firepower;
					torpedo += eq.Torpedo;
					aa += eq.AA;
					armor += eq.Armor;
					asw += eq.ASW;
					evasion += eq.Evasion;
					los += eq.LOS;
					luck += eq.Luck;
					accuracy += eq.Accuracy;
				}
			}

			TitleParameterMin.Text = EncycloRes.BaseValue;
			TitleParameterMax.Text = EncycloRes.WithEquipValue;

			HPMin.Text = ship.HPMin > 0 ? ship.HPMin.ToString() : "???";
			HPMax.Text = hp > 0 ? hp.ToString() : "???";
			ToolTipInfo.SetToolTip(HPMin, null);
			ToolTipInfo.SetToolTip(HPMax, null);

			FirepowerMin.Text = ship.FirepowerMax.ToString();
			FirepowerMax.Text = firepower.ToString();

			TorpedoMin.Text = ship.TorpedoMax.ToString();
			TorpedoMax.Text = torpedo.ToString();

			AAMin.Text = ship.AAMax.ToString();
			AAMax.Text = aa.ToString();

			ArmorMin.Text = ship.ArmorMax.ToString();
			ArmorMax.Text = armor.ToString();

			if (ship.ASW?.IsDetermined ?? false)
			{
				ASWMin.Text = ship.ASW.Maximum.ToString();
				ASWMax.Text = asw.ToString();
			}
			else
			{
				ASWMin.Text = "???";
				ASWMax.Text = asw.ToString("+0;-0");
			}

			if (ship.Evasion?.IsDetermined ?? false)
			{
				EvasionMin.Text = ship.Evasion.Maximum.ToString();
				EvasionMax.Text = evasion.ToString();
			}
			else
			{
				EvasionMin.Text = "???";
				EvasionMax.Text = evasion.ToString("+0;-0");
			}

			if (ship.LOS?.IsDetermined ?? false)
			{
				LOSMin.Text = ship.LOS.Maximum.ToString();
				LOSMax.Text = los.ToString();
			}
			else
			{
				LOSMin.Text = "???";
				LOSMax.Text = los.ToString("+0;-0");
			}

			LuckMin.Text = ship.LuckMax > 0 ? ship.LuckMax.ToString() : "???";
			LuckMax.Text = luck > 0 ? luck.ToString() : "???";

			Accuracy.Text = accuracy.ToString("+0;-0");
			Accuracy.Visible = true;

		}
		UpdateLevelParameter(ship.ShipID);

		TableParameterMain.ResumeLayout();


		//sub parameter
		TableParameterSub.SuspendLayout();

		Speed.Text = Constants.GetSpeed(ship.Speed);
		if (!ship.IsAbyssalShip)
		{
			Range.Text = Constants.GetRange(ship.Range);
			ToolTipInfo.SetToolTip(Range, null);
		}
		else
		{
			var availableEquipments = (ship.DefaultSlot ?? Enumerable.Repeat(-1, 5))
				.Select(id => KCDatabase.Instance.MasterEquipments[id])
				.Where(eq => eq != null);
			Range.Text = Constants.GetRange(Math.Max(ship.Range, availableEquipments.Any() ? availableEquipments.Max(eq => eq.Range) : 0));
			ToolTipInfo.SetToolTip(Range, $"{Translation.DefaultRange}: {Constants.GetRange(ship.Range)}");
		}
		Rarity.Text = Constants.GetShipRarity(ship.Rarity);
		Rarity.ImageIndex = (int)IconContent.RarityRed + ship.Rarity;

		TableParameterSub.ResumeLayout();

		TableConsumption.SuspendLayout();

		Fuel.Text = ship.Fuel.ToString();
		Ammo.Text = ship.Ammo.ToString();

		string tooltiptext = string.Format(
			EncycloRes.RepairTooltip,
			(ship.Fuel * 0.06),
			(ship.Fuel * 0.032),
			(int)(ship.Fuel * 0.06 * (ship.HPMaxMarried - 1)),
			(int)(ship.Fuel * 0.032 * (ship.HPMaxMarried - 1))
		);

		ToolTipInfo.SetToolTip(TableConsumption, tooltiptext);
		ToolTipInfo.SetToolTip(TitleConsumption, tooltiptext);
		ToolTipInfo.SetToolTip(Fuel, tooltiptext);
		ToolTipInfo.SetToolTip(Ammo, tooltiptext);

		TableConsumption.ResumeLayout();

		Description.Text = ship.MessageAlbum != "" ? FormatDescription(ship.MessageAlbum) : FormatDescription(ship.MessageGet);
		Description.Tag = ship.MessageAlbum != "" ? 1 : 0;


		//equipment
		TableEquipment.SuspendLayout();

		for (int i = 0; i < Equipments.Length; i++)
		{

			if (ship.Aircraft[i] > 0 || i < ship.SlotSize)
				Aircrafts[i].Text = ship.Aircraft[i].ToString();
			else
				Aircrafts[i].Text = "";


			ToolTipInfo.SetToolTip(Equipments[i], null);

			if (ship.DefaultSlot == null)
			{
				if (i < ship.SlotSize)
				{
					Equipments[i].Text = "???";
					Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Unknown;
				}
				else
				{
					Equipments[i].Text = "";
					Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Locked;
				}

			}
			else if (ship.DefaultSlot[i] != -1)
			{
				EquipmentDataMaster eq = db.MasterEquipments[ship.DefaultSlot[i]];
				if (eq == null)
				{
					// 破損データが入っていた場合
					Equipments[i].Text = Translation.Empty;
					Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Nothing;

				}
				else
				{

					Equipments[i].Text = eq.NameEN;

					int eqicon = eq.EquipmentType[3];
					if (eqicon >= (int)ResourceManager.EquipmentContent.Locked)
						eqicon = (int)ResourceManager.EquipmentContent.Unknown;

					Equipments[i].ImageIndex = eqicon;

					{
						StringBuilder sb = new StringBuilder();

						sb.AppendFormat("{0} {1} (ID: {2})\r\n", eq.CategoryTypeInstance.NameEN, eq.NameEN, eq.EquipmentID);
						if (eq.Firepower != 0) sb.AppendFormat(Translation.Firepower + " {0:+0;-0}\r\n", eq.Firepower);
						if (eq.Torpedo != 0) sb.AppendFormat(Translation.Torpedo + " {0:+0;-0}\r\n", eq.Torpedo);
						if (eq.AA != 0) sb.AppendFormat(Translation.AA + " {0:+0;-0}\r\n", eq.AA);
						if (eq.Armor != 0) sb.AppendFormat(Translation.Armor + " {0:+0;-0}\r\n", eq.Armor);
						if (eq.ASW != 0) sb.AppendFormat(Translation.ASW + " {0:+0;-0}\r\n", eq.ASW);
						if (eq.Evasion != 0) sb.AppendFormat("{0} {1:+0;-0}\r\n", eq.CategoryType == EquipmentTypes.Interceptor ? Translation.Interception : Translation.Evasion, eq.Evasion);
						if (eq.LOS != 0) sb.AppendFormat(Translation.LOS + " {0:+0;-0}\r\n", eq.LOS);
						if (eq.Accuracy != 0) sb.AppendFormat("{0} {1:+0;-0}\r\n", eq.CategoryType == EquipmentTypes.Interceptor ? Translation.AntiBomb : Translation.Accuracy, eq.Accuracy);
						if (eq.Bomber != 0) sb.AppendFormat(Translation.Bombing + " {0:+0;-0}\r\n", eq.Bomber);
						sb.AppendLine(Translation.RightClickToOpenInNewWindow);

						ToolTipInfo.SetToolTip(Equipments[i], sb.ToString());
					}
				}

			}
			else if (i < ship.SlotSize)
			{
				Equipments[i].Text = Translation.Empty;
				Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Nothing;

			}
			else
			{
				Equipments[i].Text = "";
				Equipments[i].ImageIndex = (int)ResourceManager.EquipmentContent.Locked;
			}
		}

		TableEquipment.ResumeLayout();


		//arsenal
		TableArsenal.SuspendLayout();
		BuildingTime.Text = DateTimeHelper.ToTimeRemainString(new TimeSpan(0, ship.BuildingTime, 0));

		MaterialFuel.Text = ship.Material[0].ToString();
		MaterialAmmo.Text = ship.Material[1].ToString();
		MaterialSteel.Text = ship.Material[2].ToString();
		MaterialBauxite.Text = ship.Material[3].ToString();

		PowerUpFirepower.Text = ship.PowerUp[0].ToString();
		PowerUpTorpedo.Text = ship.PowerUp[1].ToString();
		PowerUpAA.Text = ship.PowerUp[2].ToString();
		PowerUpArmor.Text = ship.PowerUp[3].ToString();

		TableArsenal.ResumeLayout();


		//remodel
		if (!ship.IsAbyssalShip)
		{

			TableRemodel.SuspendLayout();

			if (ship.RemodelBeforeShipID == 0)
			{
				RemodelBeforeShipName.Text = Translation.Empty;
				ToolTipInfo.SetToolTip(RemodelBeforeShipName, null);
				RemodelBeforeLevel.Text = "";
				RemodelBeforeLevel.ImageIndex = -1;
				ToolTipInfo.SetToolTip(RemodelBeforeLevel, null);
				RemodelBeforeAmmo.Text = "-";
				RemodelBeforeSteel.Text = "-";
			}
			else
			{
				IShipDataMaster sbefore = ship.RemodelBeforeShip;
				RemodelBeforeShipName.Text = sbefore.NameEN;
				ToolTipInfo.SetToolTip(RemodelBeforeShipName, Translation.RemodelBeforeShipNameToolTip);
				RemodelBeforeLevel.Text = string.Format("Lv. {0}", sbefore.RemodelAfterLevel);
				RemodelBeforeLevel.ImageIndex = GetRemodelItemImageIndex(sbefore);
				ToolTipInfo.SetToolTip(RemodelBeforeLevel, GetRemodelItem(sbefore));
				RemodelBeforeAmmo.Text = sbefore.RemodelAmmo.ToString();
				RemodelBeforeSteel.Text = sbefore.RemodelSteel.ToString();
			}

			if (ship.RemodelAfterShipID == 0)
			{
				RemodelAfterShipName.Text = Translation.Empty;
				ToolTipInfo.SetToolTip(RemodelAfterShipName, null);
				RemodelAfterLevel.Text = "";
				RemodelAfterLevel.ImageIndex = -1;
				ToolTipInfo.SetToolTip(RemodelAfterLevel, null);
				RemodelAfterAmmo.Text = "-";
				RemodelAfterSteel.Text = "-";
			}
			else
			{
				RemodelAfterShipName.Text = ship.RemodelAfterShip.NameEN;
				ToolTipInfo.SetToolTip(RemodelAfterShipName, Translation.RemodelBeforeShipNameToolTip);
				RemodelAfterLevel.Text = string.Format("Lv. {0}", ship.RemodelAfterLevel);
				RemodelAfterLevel.ImageIndex = GetRemodelItemImageIndex(ship);
				ToolTipInfo.SetToolTip(RemodelAfterLevel, GetRemodelItem(ship));
				RemodelAfterAmmo.Text = ship.RemodelAmmo.ToString();
				RemodelAfterSteel.Text = ship.RemodelSteel.ToString();
			}
			TableRemodel.ResumeLayout();


			TableRemodel.Visible = true;
			TableBattle.Visible = false;


		}
		else
		{

			TableBattle.SuspendLayout();

			AirSuperiority.Text = Calculator.GetAirSuperiority(ship).ToString();
			DayAttack.Text = Constants.GetDayAttackKind(Calculator.GetDayAttackKind(ship.DefaultSlot?.ToArray(), ship.ShipID, -1));
			NightAttack.Text = Constants.GetNightAttackKind(Calculator.GetNightAttackKind(ship.DefaultSlot?.ToArray(), ship.ShipID, -1));

			TableBattle.ResumeLayout();

			TableRemodel.Visible = false;
			TableBattle.Visible = true;

		}


		if (ShipBanner.Image != null)
		{
			var img = ShipBanner.Image;
			ShipBanner.Image = null;
			img.Dispose();
		}
		if (!ImageLoader.IsBusy)
		{
			loadingResourceShipID = ship.ShipID;
			ImageLoader.RunWorkerAsync(ship.ShipID);
		}



		BasePanelShipGirl.ResumeLayout();
		BasePanelShipGirl.Visible = true;


		this.Text = EncycloRes.ShipEncyclopedia + " - " + ship.NameWithClass;

	}


	private void UpdateLevelParameter(int shipID)
	{

		ShipDataMaster ship = KCDatabase.Instance.MasterShips[shipID];

		if (ship == null)
			return;

		if (!ship.IsAbyssalShip)
		{
			ASWLevel.Text = EstimateParameter((int)ParameterLevel.Value, ship.ASW);
			EvasionLevel.Text = EstimateParameter((int)ParameterLevel.Value, ship.Evasion);
			LOSLevel.Text = EstimateParameter((int)ParameterLevel.Value, ship.LOS);
			ASWLevel.Visible =
				ASWSeparater.Visible =
					EvasionLevel.Visible =
						EvasionSeparater.Visible =
							LOSLevel.Visible =
								LOSSeparater.Visible = true;

		}
		else
		{
			ASWLevel.Visible =
				ASWSeparater.Visible =
					EvasionLevel.Visible =
						EvasionSeparater.Visible =
							LOSLevel.Visible =
								LOSSeparater.Visible = false;
		}
	}

	private string EstimateParameter(int level, IParameter param)
	{

		if (param == null || param.Maximum == Parameter.MaximumDefault)
			return "???";

		int min = (int)(param.MinimumEstMin + (param.Maximum - param.MinimumEstMin) * level / 99.0);
		int max = (int)(param.MinimumEstMax + (param.Maximum - param.MinimumEstMax) * level / 99.0);

		if (min == max)
			return min.ToString();
		else
			return $"{Math.Min(min, max)}～{Math.Max(min, max)}";
	}


	private string GetParameterMinBound(IParameter param)
	{

		if (param == null || param.MinimumEstMax == Parameter.MaximumDefault)
			return "???";
		else if (param.MinimumEstMin == param.MinimumEstMax)
			return param.MinimumEstMin.ToString();
		else if (param.MinimumEstMin == Parameter.MinimumDefault && param.MinimumEstMax == param.Maximum)
			return "???";
		else
			return $"{param.MinimumEstMin}～{param.MinimumEstMax}";

	}

	private string GetParameterMax(IParameter param)
	{

		if (param == null || param.Maximum == Parameter.MaximumDefault)
			return "???";
		else
			return param.Maximum.ToString();

	}

	private string GetEquippableString(int shipID)
	{
		var db = KCDatabase.Instance;
		var ship = db.MasterShips[shipID];
		if (ship == null)
			return "";

		return string.Join("\r\n", ship.EquippableCategories.Select(id => db.EquipmentTypes[id].NameEN)
			.Concat(db.MasterEquipments.Values.Where(eq => eq.EquippableShipsAtExpansion.Contains(shipID)).Select(eq => eq.NameEN + $" ({Translation.ReinforcementSlot})")));
	}


	private void ParameterLevel_ValueChanged(object sender, EventArgs e)
	{
		if (_shipID != -1)
		{
			LevelTimer.Start();
			//UpdateLevelParameter( _shipID );
		}
	}

	private void LevelTimer_Tick(object sender, EventArgs e)
	{
		if (_shipID != -1)
			UpdateLevelParameter(_shipID);
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

	private void TableConsumption_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
	}

	private void TableEquipment_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
	}

	private void TableArsenal_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
	}

	private void TableRemodel_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		if (e.Row % 2 == 1)
			e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
	}



	private void RemodelBeforeShipName_MouseClick(object sender, MouseEventArgs e)
	{

		if (_shipID == -1) return;
		var ship = KCDatabase.Instance.MasterShips[_shipID];

		if (ship != null && ship.RemodelBeforeShipID != 0)
		{

			if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
				new DialogAlbumMasterShip(ship.RemodelBeforeShipID).Show(Owner);

			else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
				UpdateAlbumPage(ship.RemodelBeforeShipID);
		}
	}

	private void RemodelAfterShipName_MouseClick(object sender, MouseEventArgs e)
	{

		if (_shipID == -1) return;
		var ship = KCDatabase.Instance.MasterShips[_shipID];

		if (ship != null && ship.RemodelAfterShipID != 0)
		{

			if ((e.Button & System.Windows.Forms.MouseButtons.Right) != 0)
				new DialogAlbumMasterShip(ship.RemodelAfterShipID).Show(Owner);

			else if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
				UpdateAlbumPage(ship.RemodelAfterShipID);
		}
	}



	private void Equipment_MouseClick(object sender, MouseEventArgs e)
	{

		if (e.Button == System.Windows.Forms.MouseButtons.Right)
		{

			for (int i = 0; i < Equipments.Length; i++)
			{
				if (sender == Equipments[i])
				{

					if (_shipID != -1)
					{
						ShipDataMaster ship = KCDatabase.Instance.MasterShips[_shipID];

						if (ship != null && ship.DefaultSlot != null && i < ship.DefaultSlot.Count && KCDatabase.Instance.MasterEquipments.ContainsKey(ship.DefaultSlot[i]))
						{
							Cursor = Cursors.AppStarting;
							new DialogAlbumMasterEquipment(ship.DefaultSlot[i]).Show(Owner);
							Cursor = Cursors.Default;
						}
					}
				}
			}

		}
	}


	private static int GetRemodelItemImageIndex(IShipDataMaster ship)
	{
		return
			ship.NeedCatapult > 0 ? (int)IconContent.ItemCatapult :
			ship.NeedActionReport > 0 ? (int)IconContent.ItemActionReport :
			ship.NeedBlueprint > 0 ? (int)IconContent.ItemBlueprint :
			ship.NeedAviationMaterial > 0 ? (int)IconContent.ItemAviationMaterial :
			-1;
	}

	private static string GetRemodelItem(IShipDataMaster ship)
	{
		StringBuilder sb = new StringBuilder();
		if (ship.NeedBlueprint > 0)
			sb.AppendLine(EncycloRes.Blueprint + ": " + ship.NeedBlueprint);
		if (ship.NeedCatapult > 0)
			sb.AppendLine(EncycloRes.PrototypeCatapult + ": " + ship.NeedCatapult);
		if (ship.NeedActionReport > 0)
			sb.AppendLine($"{Translation.ActionReport}: " + ship.NeedActionReport);
		if (ship.NeedAviationMaterial > 0)
			sb.AppendLine($"{Translation.AviationMaterial}: " + ship.NeedAviationMaterial);

		return sb.ToString();
	}


	private void StripMenu_File_OutputCSVUser_Click(object sender, EventArgs e)
	{

		if (SaveCSVDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		{

			try
			{

				using (StreamWriter sw = new StreamWriter(SaveCSVDialog.FileName, false, Utility.Configuration.Config.Log.FileEncoding))
				{

					sw.WriteLine("艦船ID,図鑑番号,艦型,艦種,艦名,読み,ソート順,改装前,改装後,改装Lv,改装弾薬,改装鋼材,改装設計図,カタパルト,戦闘詳報,新型航空兵装資材,改装段階,耐久初期,耐久結婚,耐久最大,火力初期,火力最大,雷装初期,雷装最大,対空初期,対空最大,装甲初期,装甲最大,対潜初期,対潜最大,回避初期,回避最大,索敵初期,索敵最大,運初期,運最大,速力,射程,レア,スロット数,搭載機数1,搭載機数2,搭載機数3,搭載機数4,搭載機数5,初期装備1,初期装備2,初期装備3,初期装備4,初期装備5,建造時間,解体燃料,解体弾薬,解体鋼材,解体ボーキ,改修火力,改修雷装,改修対空,改修装甲,ドロップ文章,図鑑文章,搭載燃料,搭載弾薬,ボイス,リソース名,画像バージョン,ボイスバージョン,母港ボイスバージョン");

					foreach (ShipDataMaster ship in KCDatabase.Instance.MasterShips.Values)
					{

						if (ship.Name == "なし") continue;

						sw.WriteLine(string.Join(",",
							ship.ShipID,
							ship.AlbumNo,
							ship.IsAbyssalShip ? "深海棲艦" : Constants.GetShipClass(ship.ShipClass),
							CsvHelper.EscapeCsvCell(ship.ShipTypeName),
							CsvHelper.EscapeCsvCell(ship.NameEN),
							CsvHelper.EscapeCsvCell(ship.NameReading),
							ship.SortID,
							CsvHelper.EscapeCsvCell(ship.RemodelBeforeShipID > 0 ? ship.RemodelBeforeShip.NameEN : "-"),
							CsvHelper.EscapeCsvCell(ship.RemodelAfterShipID > 0 ? ship.RemodelAfterShip.NameEN : "-"),
							ship.RemodelAfterLevel,
							ship.RemodelAmmo,
							ship.RemodelSteel,
							ship.NeedBlueprint > 0 ? ship.NeedBlueprint + "枚" : "-",
							ship.NeedCatapult > 0 ? ship.NeedCatapult + "個" : "-",
							ship.NeedActionReport > 0 ? ship.NeedActionReport + "枚" : "-",
							ship.NeedAviationMaterial > 0 ? ship.NeedAviationMaterial + "個" : "-",
							ship.RemodelTier,
							ship.HPMin,
							ship.HPMaxMarried,
							ship.HPMaxMarriedModernized,
							ship.FirepowerMin,
							ship.FirepowerMax,
							ship.TorpedoMin,
							ship.TorpedoMax,
							ship.AAMin,
							ship.AAMax,
							ship.ArmorMin,
							ship.ArmorMax,
							ship.ASW != null && !ship.ASW.IsMinimumDefault ? ship.ASW.Minimum.ToString() : "???",
							ship.ASW != null && !ship.ASW.IsMaximumDefault ? ship.ASW.Maximum.ToString() : "???",
							ship.Evasion != null && !ship.Evasion.IsMinimumDefault ? ship.Evasion.Minimum.ToString() : "???",
							ship.Evasion != null && !ship.Evasion.IsMaximumDefault ? ship.Evasion.Maximum.ToString() : "???",
							ship.LOS != null && !ship.LOS.IsMinimumDefault ? ship.LOS.Minimum.ToString() : "???",
							ship.LOS != null && !ship.LOS.IsMaximumDefault ? ship.LOS.Maximum.ToString() : "???",
							ship.LuckMin,
							ship.LuckMax,
							Constants.GetSpeed(ship.Speed),
							Constants.GetRange(ship.Range),
							Constants.GetShipRarity(ship.Rarity),
							ship.SlotSize,
							ship.Aircraft[0],
							ship.Aircraft[1],
							ship.Aircraft[2],
							ship.Aircraft[3],
							ship.Aircraft[4],
							ship.DefaultSlot != null ? (ship.DefaultSlot[0] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[0]].NameEN : (ship.SlotSize > 0 ? "(なし)" : "")) : "???",
							ship.DefaultSlot != null ? (ship.DefaultSlot[1] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[1]].NameEN : (ship.SlotSize > 1 ? "(なし)" : "")) : "???",
							ship.DefaultSlot != null ? (ship.DefaultSlot[2] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[2]].NameEN : (ship.SlotSize > 2 ? "(なし)" : "")) : "???",
							ship.DefaultSlot != null ? (ship.DefaultSlot[3] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[3]].NameEN : (ship.SlotSize > 3 ? "(なし)" : "")) : "???",
							ship.DefaultSlot != null ? (ship.DefaultSlot[4] != -1 ? KCDatabase.Instance.MasterEquipments[ship.DefaultSlot[4]].NameEN : (ship.SlotSize > 4 ? "(なし)" : "")) : "???",
							DateTimeHelper.ToTimeRemainString(TimeSpan.FromMinutes(ship.BuildingTime)),
							ship.Material[0],
							ship.Material[1],
							ship.Material[2],
							ship.Material[3],
							ship.PowerUp[0],
							ship.PowerUp[1],
							ship.PowerUp[2],
							ship.PowerUp[3],
							CsvHelper.EscapeCsvCell(ship.MessageGet),
							CsvHelper.EscapeCsvCell(ship.MessageAlbum),
							ship.Fuel,
							ship.Ammo,
							Constants.GetVoiceFlag(ship.VoiceFlag),
							CsvHelper.EscapeCsvCell(ship.ResourceName),
							ship.ResourceGraphicVersion,
							ship.ResourceVoiceVersion,
							ship.ResourcePortVoiceVersion
						));

					}

				}

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, Translation.CsvExportFailed);
				MessageBox.Show(Translation.CsvExportFailed + "\r\n" + ex.Message, AlbumMasterTranslation.DialogTitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

					sw.WriteLine(string.Format("艦船ID,図鑑番号,艦名,読み,艦種,艦型,ソート順,改装前,改装後,改装Lv,改装弾薬,改装鋼材,改装設計図,カタパルト,戦闘詳報,新型航空兵装資材,改装段階,耐久初期,耐久最大,耐久結婚,耐久改修,火力初期,火力最大,雷装初期,雷装最大,対空初期,対空最大,装甲初期,装甲最大,対潜初期最小,対潜初期最大,対潜最大,対潜{0}最小,対潜{0}最大,回避初期最小,回避初期最大,回避最大,回避{0}最小,回避{0}最大,索敵初期最小,索敵初期最大,索敵最大,索敵{0}最小,索敵{0}最大,運初期,運最大,速力,射程,レア,スロット数,搭載機数1,搭載機数2,搭載機数3,搭載機数4,搭載機数5,初期装備1,初期装備2,初期装備3,初期装備4,初期装備5,建造時間,解体燃料,解体弾薬,解体鋼材,解体ボーキ,改修火力,改修雷装,改修対空,改修装甲,ドロップ文章,図鑑文章,搭載燃料,搭載弾薬,ボイス,リソース名,画像バージョン,ボイスバージョン,母港ボイスバージョン", ExpTable.ShipMaximumLevel));

					foreach (ShipDataMaster ship in KCDatabase.Instance.MasterShips.Values)
					{

						sw.WriteLine(string.Join(",",
							ship.ShipID,
							ship.AlbumNo,
							CsvHelper.EscapeCsvCell(ship.NameEN),
							CsvHelper.EscapeCsvCell(ship.NameReading),
							(int)ship.ShipType,
							ship.ShipClass,
							ship.SortID,
							ship.RemodelBeforeShipID,
							ship.RemodelAfterShipID,
							ship.RemodelAfterLevel,
							ship.RemodelAmmo,
							ship.RemodelSteel,
							ship.NeedBlueprint,
							ship.NeedCatapult,
							ship.NeedActionReport,
							ship.NeedAviationMaterial,
							ship.RemodelTier,
							ship.HPMin,
							ship.HPMax,
							ship.HPMaxMarried,
							ship.HPMaxMarriedModernized,
							ship.FirepowerMin,
							ship.FirepowerMax,
							ship.TorpedoMin,
							ship.TorpedoMax,
							ship.AAMin,
							ship.AAMax,
							ship.ArmorMin,
							ship.ArmorMax,
							ship.ASW?.MinimumEstMin ?? Parameter.MinimumDefault,
							ship.ASW?.MinimumEstMax ?? Parameter.MaximumDefault,
							ship.ASW?.Maximum ?? Parameter.MaximumDefault,
							ship.ASW?.GetEstParameterMin(ExpTable.ShipMaximumLevel) ?? Parameter.MinimumDefault,
							ship.ASW?.GetEstParameterMax(ExpTable.ShipMaximumLevel) ?? Parameter.MaximumDefault,
							ship.Evasion?.MinimumEstMin ?? Parameter.MinimumDefault,
							ship.Evasion?.MinimumEstMax ?? Parameter.MaximumDefault,
							ship.Evasion?.Maximum ?? Parameter.MaximumDefault,
							ship.Evasion?.GetEstParameterMin(ExpTable.ShipMaximumLevel) ?? Parameter.MinimumDefault,
							ship.Evasion?.GetEstParameterMax(ExpTable.ShipMaximumLevel) ?? Parameter.MaximumDefault,
							ship.LOS?.MinimumEstMin ?? Parameter.MinimumDefault,
							ship.LOS?.MinimumEstMax ?? Parameter.MaximumDefault,
							ship.LOS?.Maximum ?? Parameter.MaximumDefault,
							ship.LOS?.GetEstParameterMin(ExpTable.ShipMaximumLevel) ?? Parameter.MinimumDefault,
							ship.LOS?.GetEstParameterMax(ExpTable.ShipMaximumLevel) ?? Parameter.MaximumDefault,
							ship.LuckMin,
							ship.LuckMax,
							ship.Speed,
							ship.Range,
							ship.Rarity,
							ship.SlotSize,
							ship.Aircraft[0],
							ship.Aircraft[1],
							ship.Aircraft[2],
							ship.Aircraft[3],
							ship.Aircraft[4],
							ship.DefaultSlot?[0] ?? -1,
							ship.DefaultSlot?[1] ?? -1,
							ship.DefaultSlot?[2] ?? -1,
							ship.DefaultSlot?[3] ?? -1,
							ship.DefaultSlot?[4] ?? -1,
							ship.BuildingTime,
							ship.Material[0],
							ship.Material[1],
							ship.Material[2],
							ship.Material[3],
							ship.PowerUp[0],
							ship.PowerUp[1],
							ship.PowerUp[2],
							ship.PowerUp[3],
							CsvHelper.EscapeCsvCell(ship.MessageGet),
							CsvHelper.EscapeCsvCell(ship.MessageAlbum),
							ship.Fuel,
							ship.Ammo,
							ship.VoiceFlag,
							CsvHelper.EscapeCsvCell(ship.ResourceName),
							ship.ResourceGraphicVersion,
							ship.ResourceVoiceVersion,
							ship.ResourcePortVoiceVersion
						));

					}

				}

			}
			catch (Exception ex)
			{

				Utility.ErrorReporter.SendErrorReport(ex, Translation.CsvExportFailed);
				MessageBox.Show(Translation.CsvExportFailed + "\r\n" + ex.Message, AlbumMasterTranslation.DialogTitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}

	}



	private void DialogAlbumMasterShip_FormClosed(object sender, FormClosedEventArgs e)
	{

		ResourceManager.DestroyIcon(Icon);

	}



	private void Description_Click(object sender, EventArgs e)
	{

		int tag = Description.Tag as int? ?? 0;
		ShipDataMaster ship = KCDatabase.Instance.MasterShips[_shipID];

		if (ship == null) return;

		if (tag == 0 && ship.MessageAlbum.Length > 0)
		{
			Description.Text = FormatDescription(ship.MessageAlbum);
			Description.Tag = 1;
		}
		else
		{
			Description.Text = FormatDescription(ship.MessageGet);
			Description.Tag = 0;
		}
	}

	private string FormatDescription(string description)
	{
		// 本家の改行がアレなので、区切り文字+改行 以外の改行を削除する
		var regex = new Regex(@"([^、。,\.！？!\?])\r\n");
		return regex.Replace(description, "$1");
	}


	private void ResourceName_MouseClick(object sender, MouseEventArgs e)
	{

		if (e.Button == System.Windows.Forms.MouseButtons.Right)
		{

			var ship = KCDatabase.Instance.MasterShips[_shipID];
			if (ship != null)
			{
				Clipboard.SetData(DataFormats.StringFormat, ship.ResourceName);
			}
		}

	}



	private void StripMenu_Edit_EditParameter_Click(object sender, EventArgs e)
	{

		if (_shipID <= 0)
		{
			MessageBox.Show(Translation.SelectAShip, AlbumMasterTranslation.DialogTitleError, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			return;
		}

		using (var dialog = new DialogAlbumShipParameter(_shipID))
		{
			dialog.ShowDialog(this);
			UpdateAlbumPage(_shipID);
		}

	}



	private void ImageLoader_DoWork(object sender, DoWorkEventArgs e)
	{
		//System.Threading.Thread.Sleep( 2000 );		// for test

		try
		{
			e.Result = KCResourceHelper.LoadShipImage(e.Argument as int? ?? 0, false, KCResourceHelper.ResourceTypeShipBanner);
		}
		catch (Exception)
		{
			e.Result = null;
		}

	}

	private void ImageLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{

		if (ShipBanner.Image != null)
		{
			var img = ShipBanner.Image;
			ShipBanner.Image = null;
			img.Dispose();
		}

		if (loadingResourceShipID != _shipID)
		{
			if (e.Result != null)
				((Bitmap)e.Result).Dispose();

			if (!ImageLoader.IsBusy)
			{
				loadingResourceShipID = _shipID;
				var ship = KCDatabase.Instance.MasterShips[_shipID];
				if (ship != null)
					ImageLoader.RunWorkerAsync(_shipID);
			}

			return;
		}

		if (e.Result != null)
		{
			ShipBanner.Image = e.Result as Bitmap;
			loadingResourceShipID = -1;
		}

	}



	private void TextSearch_TextChanged(object sender, EventArgs e)
	{

		if (string.IsNullOrWhiteSpace(TextSearch.Text))
			return;


		bool Search(string searchWord)
		{
			var target =
				ShipView.Rows.OfType<DataGridViewRow>()
					.Select(r => KCDatabase.Instance.MasterShips[(int)r.Cells[ShipView_ShipID.Index].Value])
					.FirstOrDefault(
						ship =>
							Calculator.ToHiragana(ship.NameWithClass.ToLower()).StartsWith(searchWord) ||
							Calculator.ToHiragana(ship.NameReading.ToLower()).StartsWith(searchWord));

			if (target != null)
			{
				ShipView.FirstDisplayedScrollingRowIndex = ShipView.Rows.OfType<DataGridViewRow>().First(r => (int)r.Cells[ShipView_ShipID.Index].Value == target.ShipID).Index;
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

	private void StripMenu_Edit_CopyShipName_Click(object sender, EventArgs e)
	{
		var ship = KCDatabase.Instance.MasterShips[_shipID];
		if (ship != null)
			Clipboard.SetText(ship.NameWithClass);
		else
			System.Media.SystemSounds.Exclamation.Play();
	}

	private void ShipName_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == System.Windows.Forms.MouseButtons.Right)
		{
			var ship = KCDatabase.Instance.MasterShips[_shipID];
			if (ship != null)
				Clipboard.SetText(ship.NameWithClass);
			else
				System.Media.SystemSounds.Exclamation.Play();
		}
	}

	private void StripMenu_Edit_CopyShipData_Click(object sender, EventArgs e)
	{
		var ship = KCDatabase.Instance.MasterShips[_shipID];
		if (ship == null)
		{
			System.Media.SystemSounds.Exclamation.Play();
			return;
		}

		var sb = new StringBuilder();

		var slot = (ship.DefaultSlot ?? Enumerable.Repeat(-2, 5)).ToArray();

		sb.AppendFormat("{0} {1}\r\n", ship.ShipTypeName, ship.NameWithClass);
		sb.AppendFormat("ID: {0} / 図鑑番号: {1} / リソース: {2} ver. {3} / {4} / {5} ({6})\r\n", ship.ShipID, ship.AlbumNo,
			ship.ResourceName, ship.ResourceGraphicVersion, ship.ResourceVoiceVersion, ship.ResourcePortVoiceVersion,
			Constants.GetVoiceFlag(ship.VoiceFlag));
		sb.AppendLine();
		if (!ship.IsAbyssalShip)
		{
			sb.AppendFormat("耐久: {0} / {1}\r\n", ship.HPMin, ship.HPMaxMarried);
			sb.AppendFormat("火力: {0} / {1}\r\n", ship.FirepowerMin, ship.FirepowerMax);
			sb.AppendFormat("雷装: {0} / {1}\r\n", ship.TorpedoMin, ship.TorpedoMax);
			sb.AppendFormat("対空: {0} / {1}\r\n", ship.AAMin, ship.AAMax);
			sb.AppendFormat("装甲: {0} / {1}\r\n", ship.ArmorMin, ship.ArmorMax);
			sb.AppendFormat("対潜: {0} / {1}\r\n", GetParameterMinBound(ship.ASW), GetParameterMax(ship.ASW));
			sb.AppendFormat("回避: {0} / {1}\r\n", GetParameterMinBound(ship.Evasion), GetParameterMax(ship.Evasion));
			sb.AppendFormat("索敵: {0} / {1}\r\n", GetParameterMinBound(ship.LOS), GetParameterMax(ship.LOS));
			sb.AppendFormat("運: {0} / {1}\r\n", ship.LuckMin, ship.LuckMax);
			sb.AppendFormat("速力: {0} / 射程: {1}\r\n", Constants.GetSpeed(ship.Speed), Constants.GetRange(ship.Range));
			sb.AppendFormat("搭載資源: 燃料 {0} / 弾薬 {1}\r\n", ship.Fuel, ship.Ammo);
			sb.AppendFormat("レアリティ: {0}\r\n", Constants.GetShipRarity(ship.Rarity));
		}
		else
		{
			var availableEquipments = slot.Select(id => KCDatabase.Instance.MasterEquipments[id]).Where(eq => eq != null);
			int luckSum = ship.LuckMax + availableEquipments.Sum(eq => eq.Luck);
			sb.AppendFormat("耐久: {0}\r\n", ship.HPMin > 0 ? ship.HPMin.ToString() : "???");
			sb.AppendFormat("火力: {0} / {1}\r\n", ship.FirepowerMin, ship.FirepowerMax + availableEquipments.Sum(eq => eq.Firepower));
			sb.AppendFormat("雷装: {0} / {1}\r\n", ship.TorpedoMin, ship.TorpedoMax + availableEquipments.Sum(eq => eq.Torpedo));
			sb.AppendFormat("対空: {0} / {1}\r\n", ship.AAMin, ship.AAMax + availableEquipments.Sum(eq => eq.AA));
			sb.AppendFormat("装甲: {0} / {1}\r\n", ship.ArmorMin, ship.ArmorMax + availableEquipments.Sum(eq => eq.Armor));
			sb.AppendFormat("対潜: {0} / {1}\r\n", GetParameterMax(ship.ASW), (ship.ASW != null && !ship.ASW.IsMaximumDefault ? ship.ASW.Maximum : 0) + availableEquipments.Sum(eq => eq.ASW));
			sb.AppendFormat("回避: {0} / {1}\r\n", GetParameterMax(ship.Evasion), (ship.Evasion != null && !ship.Evasion.IsMaximumDefault ? ship.Evasion.Maximum : 0) + availableEquipments.Sum(eq => eq.Evasion));
			sb.AppendFormat("索敵: {0} / {1}\r\n", GetParameterMax(ship.LOS), (ship.LOS != null && !ship.LOS.IsMaximumDefault ? ship.LOS.Maximum : 0) + availableEquipments.Sum(eq => eq.LOS));
			sb.AppendFormat("運: {0} / {1}\r\n", ship.LuckMin > 0 ? ship.LuckMin.ToString() : "???", luckSum > 0 ? luckSum.ToString() : "???");
			sb.AppendFormat("速力: {0} / 射程: {1}\r\n", Constants.GetSpeed(ship.Speed),
				Constants.GetRange(Math.Max(ship.Range, availableEquipments.Any() ? availableEquipments.Max(eq => eq.Range) : 0)));
			if (ship.Fuel > 0 || ship.Ammo > 0)
				sb.AppendFormat("搭載資源: 燃料 {0} / 弾薬 {1}\r\n", ship.Fuel, ship.Ammo);
			if (ship.Rarity > 0)
				sb.AppendFormat("レアリティ: {0}\r\n", Constants.GetShipRarity(ship.Rarity));
		}
		sb.AppendLine();
		sb.AppendLine("初期装備:");
		{
			for (int i = 0; i < slot.Length; i++)
			{
				string name;
				var eq = KCDatabase.Instance.MasterEquipments[slot[i]];
				if (eq == null && i >= ship.SlotSize)
					continue;

				if (eq != null)
					name = eq.NameEN;
				else if (slot[i] == -1)
					name = "(なし)";
				else
					name = "(不明)";

				sb.AppendFormat("[{0}] {1}\r\n", ship.Aircraft[i], name);
			}
		}
		sb.AppendLine();
		if (!ship.IsAbyssalShip)
		{
			sb.AppendFormat("建造時間: {0}\r\n", DateTimeHelper.ToTimeRemainString(TimeSpan.FromMinutes(ship.BuildingTime)));
			sb.AppendFormat("解体資源: {0}\r\n", string.Join(" / ", ship.Material));
			sb.AppendFormat("改修強化: {0}\r\n", string.Join(" / ", ship.PowerUp));
			if (ship.RemodelBeforeShipID != 0)
			{
				var before = ship.RemodelBeforeShip;
				var append = new List<string>(4)
				{
					"弾薬 " + before.RemodelAmmo,
					"鋼材 " + before.RemodelSteel
				};
				if (before.NeedBlueprint > 0)
					append.Add("要改装設計図");
				if (before.NeedCatapult > 0)
					append.Add("要カタパルト");
				if (before.NeedActionReport > 0)
					append.Add("要戦闘詳報");
				if (before.NeedAviationMaterial > 0)
					append.Add("要新型航空兵装資材");

				sb.AppendFormat("改造前: {0} Lv. {1} ({2})\r\n",
					before.NameWithClass, before.RemodelAfterLevel, string.Join(", ", append));
			}
			else
			{
				sb.AppendLine("改造前: (なし)");
			}
			if (ship.RemodelAfterShipID != 0)
			{
				var append = new List<string>(4)
				{
					"弾薬 " + ship.RemodelAmmo,
					"鋼材 " + ship.RemodelSteel
				};
				if (ship.NeedBlueprint > 0)
					append.Add("要改装設計図");
				if (ship.NeedCatapult > 0)
					append.Add("要カタパルト");
				if (ship.NeedActionReport > 0)
					append.Add("要戦闘詳報");
				if (ship.NeedAviationMaterial > 0)
					append.Add("要新型航空兵装資材");

				sb.AppendFormat("改造後: {0} Lv. {1} ({2})\r\n",
					ship.RemodelAfterShip.NameWithClass, ship.RemodelAfterLevel, string.Join(", ", append));
			}
			else
			{
				sb.AppendLine("改造後: (なし)");
			}
			sb.AppendLine();
			sb.AppendFormat("図鑑文章: \r\n{0}\r\n\r\n入手文章: \r\n{1}\r\n\r\n",
				!string.IsNullOrWhiteSpace(ship.MessageAlbum) ? ship.MessageAlbum : "(不明)",
				!string.IsNullOrWhiteSpace(ship.MessageGet) ? ship.MessageGet : "(不明)");
		}

		sb.AppendLine("出現海域:");
		{
			string result = GetAppearingArea(ship.ShipID);
			if (string.IsNullOrEmpty(result))
				result = "(不明)";
			sb.AppendLine(result);
		}

		Clipboard.SetText(sb.ToString());
	}



	private string GetAppearingArea(int shipID)
	{

		var ship = KCDatabase.Instance.MasterShips[shipID];
		if (ship == null)
			return string.Empty;

		var sb = new StringBuilder();

		if (!ship.IsAbyssalShip)
		{

			foreach (var record in RecordManager.Instance.ShipDrop.Record
				.Where(s => s.ShipID == shipID && s.EnemyFleetID != 0)
				.Select(s => new
				{
					s.MapAreaID,
					s.MapInfoID,
					s.CellID,
					s.Difficulty,
					EnemyFleetName = RecordManager.Instance.EnemyFleet.Record.ContainsKey(s.EnemyFleetID) ?
						RecordManager.Instance.EnemyFleet.Record[s.EnemyFleetID].FleetName : Translation.Unknown
				})
				.Distinct()
				.OrderBy(r => r.MapAreaID)
				.ThenBy(r => r.MapInfoID)
				.ThenBy(r => r.CellID)
				.ThenBy(r => r.Difficulty)
			)
			{
				sb.AppendFormat("{0}-{1}-{2}{3} ({4})\r\n",
					record.MapAreaID, record.MapInfoID, record.CellID, record.Difficulty > 0 ? " [" + Constants.GetDifficulty(record.Difficulty) + "]" : "", record.EnemyFleetName);
			}

			foreach (var record in RecordManager.Instance.Construction.Record
				.Where(s => s.ShipID == shipID)
				.Select(s => new
				{
					s.Fuel,
					s.Ammo,
					s.Steel,
					s.Bauxite,
					s.DevelopmentMaterial
				})
				.Distinct()
				.OrderBy(r => r.Fuel)
				.ThenBy(r => r.Ammo)
				.ThenBy(r => r.Steel)
				.ThenBy(r => r.Bauxite)
				.ThenBy(r => r.DevelopmentMaterial)
			)
			{
				sb.AppendFormat(Translation.Recipe + " {0} / {1} / {2} / {3} - {4}\r\n",
					record.Fuel, record.Ammo, record.Steel, record.Bauxite, record.DevelopmentMaterial);
			}

		}
		else
		{

			foreach (var record in RecordManager.Instance.EnemyFleet.Record.Values
				.Where(r => r.FleetMember.Contains(shipID))
				.Select(s => new
				{
					s.MapAreaID,
					s.MapInfoID,
					s.CellID,
					s.Difficulty,
					EnemyFleetName = !string.IsNullOrWhiteSpace(s.FleetName) ? s.FleetName : Translation.Unknown
				})
				.Distinct()
				.OrderBy(r => r.MapAreaID)
				.ThenBy(r => r.MapInfoID)
				.ThenBy(r => r.CellID)
				.ThenBy(r => r.Difficulty)
			)
			{
				sb.AppendFormat("{0}-{1}-{2}{3} ({4})\r\n",
					record.MapAreaID, record.MapInfoID, record.CellID, record.Difficulty > 0 ? " [" + Constants.GetDifficulty(record.Difficulty) + "]" : "", record.EnemyFleetName);
			}

		}

		return sb.ToString();
	}

	private void StripMenu_View_ShowAppearingArea_Click(object sender, EventArgs e)
	{
		var ship = KCDatabase.Instance.MasterShips[_shipID];
		if (ship == null)
		{
			System.Media.SystemSounds.Exclamation.Play();
			return;
		}

		string result = GetAppearingArea(ship.ShipID);

		if (string.IsNullOrEmpty(result))
			result = string.Format(Translation.FailedToFindMapOrRecipe, ship.NameWithClass);

		MessageBox.Show(result, Translation.MapOrRecipeSearchCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
	}



	private void ShipBanner_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == System.Windows.Forms.MouseButtons.Right)
		{

			StripMenu_View_ShowShipGraphicViewer.PerformClick();
		}
	}

	private void StripMenu_View_ShowShipGraphicViewer_Click(object sender, EventArgs e)
	{
		var ship = KCDatabase.Instance.MasterShips[_shipID];
		if (ship != null)
		{
			new DialogShipGraphicViewer(ship.ShipID).Show(Owner);
		}
		else
		{
			MessageBox.Show(Translation.SpecifyTargetShip, Translation.NoShipSelectedCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}


	private void StripMenu_Edit_GoogleShipName_Click(object sender, EventArgs e)
	{
		var ship = KCDatabase.Instance.MasterShips[_shipID];
		if (ship == null)
		{
			System.Media.SystemSounds.Exclamation.Play();
			return;
		}

		try
		{
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = @"https://www.duckduckgo.com/?q=" + Uri.EscapeDataString(ship.NameWithClass) + AlbumMasterTranslation.KancolleSpecifier,
				UseShellExecute = true
			};
			// google <艦船名> 艦これ
			Process.Start(psi);
		}
		catch (Exception ex)
		{
			Utility.ErrorReporter.SendErrorReport(ex, AlbumMasterTranslation.FailedToSearchOnWeb);
		}
	}

	private void StripMenu_Edit_CopySpecialEquipmentTable_Click(object sender, EventArgs e)
	{
		var sb = new StringBuilder();
		sb.AppendLine("|Ship ID|Ship|Equipable|Unequipable|");
		sb.AppendLine("|--:|:--|:--|:--|");

		foreach (var ship in KCDatabase.Instance.MasterShips.Values)
		{
			if (ship.SpecialEquippableCategories == null)
				continue;

			var add = ship.SpecialEquippableCategories.Except(ship.ShipTypeInstance.EquippableCategories);
			var sub = ship.ShipTypeInstance.EquippableCategories.Except(ship.SpecialEquippableCategories);

			sb.AppendLine($"|{ship.ShipID}|{ship.NameWithClass}|{string.Join(", ", add.Select(id => KCDatabase.Instance.EquipmentTypes[id].NameEN))}|{string.Join(", ", sub.Select(id => KCDatabase.Instance.EquipmentTypes[id].NameEN))}|");
		}

		sb.AppendLine();

		{
			var nyan = new Dictionary<int, List<int>>();

			foreach (var eq in KCDatabase.Instance.MasterEquipments.Values)
			{
				if (!(eq.EquippableShipsAtExpansion?.Any() ?? false))
					continue;

				foreach (var shipid in eq.EquippableShipsAtExpansion)
				{
					if (nyan.ContainsKey(shipid))
						nyan[shipid].Add(eq.EquipmentID);
					else
						nyan.Add(shipid, new List<int>() { eq.EquipmentID });
				}
			}

			sb.AppendLine("|Ship ID|Ship|Equipable Eq ID|Equipable|");
			sb.AppendLine("|--:|:--|:--|:--|");

			foreach (var pair in nyan.OrderBy(p => p.Key))
			{
				sb.AppendLine($"|{pair.Key}|{KCDatabase.Instance.MasterShips[pair.Key].NameWithClass}|{string.Join(", ", pair.Value)}|{string.Join(", ", pair.Value.Select(id => KCDatabase.Instance.MasterEquipments[id].NameEN))}|");
			}

		}
		Clipboard.SetText(sb.ToString());
	}

	private void StripMenu_File_MergeDefaultRecord_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("デフォルトレコードの情報をもとに、艦船レコードを更新します。\r\nよろしいですか？", "レコード更新確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
		{
			return;
		}

		var parameterRecord = RecordManager.Instance.ShipParameter;


		string temporaryPath = null;
		try
		{
			temporaryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Resource.ResourceManager.CopyFromArchive("Record/" + parameterRecord.FileName, temporaryPath, false, true);

			int count = 0;
			using (var reader = new StreamReader(temporaryPath, Utility.Configuration.Config.Log.FileEncoding))
			{
				while (!reader.EndOfStream)
				{
					count += parameterRecord.Merge(reader.ReadLine()) ? 1 : 0;
				}
			}

			if (count == 0)
				Utility.Logger.Add(2, "更新できるレコードがありませんでした。お使いのデータは十分に更新されています。");
			else
				Utility.Logger.Add(2, count + " 件の艦船レコードの更新が完了しました。開き直すと反映されます。");
		}
		catch (Exception ex)
		{
			Utility.ErrorReporter.SendErrorReport(ex, "デフォルトレコードとのマージに失敗しました。");
		}
		finally
		{
			if (temporaryPath != null)
				File.Delete(temporaryPath);
		}

	}
}
