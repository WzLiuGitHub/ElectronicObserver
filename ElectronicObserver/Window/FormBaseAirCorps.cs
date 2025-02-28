﻿using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Window.Control;
using ElectronicObserver.Window.Support;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window;

public partial class FormBaseAirCorps : DockContent
{


	private class TableBaseAirCorpsControl : IDisposable
	{

		public ImageLabel Name;
		public ImageLabel ActionKind;
		public ImageLabel AirSuperiority;
		public ImageLabel Distance;
		public ShipStatusEquipment Squadrons;

		public ToolTip ToolTipInfo;

		public TableBaseAirCorpsControl(FormBaseAirCorps parent)
		{

			#region Initialize

			Name = new ImageLabel
			{
				Name = "Name",
				Text = "*",
				Anchor = AnchorStyles.Left,
				TextAlign = ContentAlignment.MiddleLeft,
				ImageAlign = ContentAlignment.MiddleRight,
				ImageList = ResourceManager.Instance.Icons,
				Padding = new Padding(2, 2, 2, 2),
				Margin = new Padding(2, 1, 2, 1),      // ここを 2,0,2,0 にすると境界線の描画に問題が出るので
				AutoSize = true,
				ContextMenuStrip = parent.ContextMenuBaseAirCorps,
				Visible = false,
				Cursor = Cursors.Help
			};

			ActionKind = new ImageLabel
			{
				Text = "*",
				Anchor = AnchorStyles.Left,
				TextAlign = ContentAlignment.MiddleLeft,
				ImageAlign = ContentAlignment.MiddleCenter,
				//ActionKind.ImageList =
				Padding = new Padding(2, 2, 2, 2),
				Margin = new Padding(2, 0, 2, 0),
				AutoSize = true,
				Visible = false
			};

			AirSuperiority = new ImageLabel
			{
				Text = "*",
				Anchor = AnchorStyles.Left,
				TextAlign = ContentAlignment.MiddleLeft,
				ImageAlign = ContentAlignment.MiddleLeft,
				ImageList = ResourceManager.Instance.Equipments,
				ImageIndex = (int)ResourceManager.EquipmentContent.CarrierBasedFighter,
				Padding = new Padding(2, 2, 2, 2),
				Margin = new Padding(2, 0, 2, 0),
				AutoSize = true,
				Visible = false
			};

			Distance = new ImageLabel
			{
				Text = "*",
				Anchor = AnchorStyles.Left,
				TextAlign = ContentAlignment.MiddleLeft,
				ImageAlign = ContentAlignment.MiddleLeft,
				ImageList = ResourceManager.Instance.Icons,
				ImageIndex = (int)IconContent.ParameterAircraftDistance,
				Padding = new Padding(2, 2, 2, 2),
				Margin = new Padding(2, 0, 2, 0),
				AutoSize = true,
				Visible = false
			};

			Squadrons = new ShipStatusEquipment
			{
				Anchor = AnchorStyles.Left,
				Padding = new Padding(0, 1, 0, 2),
				Margin = new Padding(2, 0, 2, 0),
				Size = new Size(40, 20),
				AutoSize = true,
				Visible = false
			};
			Squadrons.ResumeLayout();

			ConfigurationChanged(parent);

			ToolTipInfo = parent.ToolTipInfo;

			#endregion

		}


		public TableBaseAirCorpsControl(FormBaseAirCorps parent, TableLayoutPanel table, int row)
			: this(parent)
		{
			AddToTable(table, row);
		}

		public void AddToTable(TableLayoutPanel table, int row)
		{

			table.SuspendLayout();

			table.Controls.Add(Name, 0, row);
			table.Controls.Add(ActionKind, 1, row);
			table.Controls.Add(AirSuperiority, 2, row);
			table.Controls.Add(Distance, 3, row);
			table.Controls.Add(Squadrons, 4, row);
			table.ResumeLayout();

			ControlHelper.SetTableRowStyle(table, row, ControlHelper.GetDefaultRowStyle());
		}


		public void Update(int baseAirCorpsID)
		{

			KCDatabase db = KCDatabase.Instance;
			var corps = db.BaseAirCorps[baseAirCorpsID];

			if (corps == null)
			{
				baseAirCorpsID = -1;

			}
			else
			{

				Name.Text = string.Format("#{0} - {1}", corps.MapAreaID, corps.Name);
				Name.Tag = corps.MapAreaID;
				var sb = new StringBuilder();


				string areaName = KCDatabase.Instance.MapArea.ContainsKey(corps.MapAreaID) ? KCDatabase.Instance.MapArea[corps.MapAreaID].NameEN : "Unknown Area";

				sb.AppendLine(Properties.Window.FormBaseAirCorps.Area + areaName);

				// state

				if (corps.Squadrons.Values.Any(sq => sq != null && sq.Condition > 1))
				{
					// 疲労
					int tired = corps.Squadrons.Values.Max(sq => sq?.Condition ?? 0);

					if (tired == 2)
					{
						Name.ImageAlign = ContentAlignment.MiddleRight;
						Name.ImageIndex = (int)IconContent.ConditionTired;
						sb.AppendLine(GeneralRes.Tired);

					}
					else
					{
						Name.ImageAlign = ContentAlignment.MiddleRight;
						Name.ImageIndex = (int)IconContent.ConditionVeryTired;
						sb.AppendLine(GeneralRes.VeryTired);

					}

				}
				else if (corps.Squadrons.Values.Any(sq => sq != null && sq.AircraftCurrent < sq.AircraftMax))
				{
					// 未補給
					Name.ImageAlign = ContentAlignment.MiddleRight;
					Name.ImageIndex = (int)IconContent.FleetNotReplenished;
					sb.AppendLine(Properties.Window.FormBaseAirCorps.Unsupplied);

				}
				else
				{
					Name.ImageAlign = ContentAlignment.MiddleCenter;
					Name.ImageIndex = -1;

				}

				sb.AppendLine(string.Format(Properties.Window.FormBaseAirCorps.AirControlSummary,
					db.BaseAirCorps.Values.Where(c => c.MapAreaID == corps.MapAreaID && c.ActionKind == 2).Select(c => Calculator.GetAirSuperiority(c)).DefaultIfEmpty(0).Sum(),
					db.BaseAirCorps.Values.Where(c => c.MapAreaID == corps.MapAreaID && c.ActionKind == 2).Select(c => Calculator.GetAirSuperiority(c, isHighAltitude: true)).DefaultIfEmpty(0).Sum()
				));

				ToolTipInfo.SetToolTip(Name, sb.ToString());


				ActionKind.Text = "[" + Constants.GetBaseAirCorpsActionKind(corps.ActionKind) + "]";

				{
					int airSuperiority = Calculator.GetAirSuperiority(corps);
					if (Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange)
					{
						int airSuperiority_max = Calculator.GetAirSuperiority(corps, true);
						if (airSuperiority < airSuperiority_max)
							AirSuperiority.Text = string.Format("{0} ～ {1}", airSuperiority, airSuperiority_max);
						else
							AirSuperiority.Text = airSuperiority.ToString();
					}
					else
					{
						AirSuperiority.Text = airSuperiority.ToString();
					}

					var tip = new StringBuilder();
					tip.AppendFormat(GeneralRes.BaseTooltip,
						(int)(airSuperiority / 3.0),
						(int)(airSuperiority / 1.5),
						Math.Max((int)(airSuperiority * 1.5 - 1), 0),
						Math.Max((int)(airSuperiority * 3.0 - 1), 0));

					if (corps.ActionKind == 2)
					{
						int airSuperiorityHighAltitude = Calculator.GetAirSuperiority(corps, isHighAltitude: true);
						int airSuperiorityHighAltitudeMax = Calculator.GetAirSuperiority(corps, isAircraftLevelMaximum: true, isHighAltitude: true);

						tip.AppendFormat(GeneralRes.HighAltitudeAirState,
							Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange && airSuperiorityHighAltitude != airSuperiorityHighAltitudeMax ?
								$"{airSuperiorityHighAltitude} ～ {airSuperiorityHighAltitudeMax}" :
								airSuperiorityHighAltitude.ToString(),
							(int)(airSuperiorityHighAltitude / 3.0),
							(int)(airSuperiorityHighAltitude / 1.5),
							Math.Max((int)(airSuperiorityHighAltitude * 1.5 - 1), 0),
							Math.Max((int)(airSuperiorityHighAltitude * 3.0 - 1), 0));
					}

					ToolTipInfo.SetToolTip(AirSuperiority, tip.ToString());
				}
				int dist_text = corps.Distance;

				Distance.Text = dist_text.ToString();

				Squadrons.SetSlotList(corps);
				ToolTipInfo.SetToolTip(Squadrons, GetEquipmentString(corps));
				ToolTipInfo.SetToolTip(Distance, string.Format(Properties.Window.FormBaseAirCorps.TotalDistance, corps.Distance));

			}


			Name.Visible =
				ActionKind.Visible =
					AirSuperiority.Visible =
						Distance.Visible =
							Squadrons.Visible =
								baseAirCorpsID != -1;
		}


		public void ConfigurationChanged(FormBaseAirCorps parent)
		{

			var config = Utility.Configuration.Config;

			var mainfont = config.UI.MainFont;
			var subfont = config.UI.SubFont;

			Name.Font = mainfont;
			ActionKind.Font = mainfont;
			AirSuperiority.Font = mainfont;
			Distance.Font = mainfont;
			Squadrons.Font = subfont;

			Squadrons.ShowAircraft = config.FormFleet.ShowAircraft;
			Squadrons.ShowAircraftLevelByNumber = config.FormFleet.ShowAircraftLevelByNumber;
			Squadrons.LevelVisibility = config.FormFleet.EquipmentLevelVisibility;

		}


		private string GetEquipmentString(BaseAirCorpsData corps)
		{
			var sb = new StringBuilder();

			if (corps == null)
				return GeneralRes.BaseNotOpen;

			foreach (var squadron in corps.Squadrons.Values)
			{
				if (squadron == null)
					continue;

				var eq = squadron.EquipmentInstance;

				switch (squadron.State)
				{
					case 0:     // 未配属
					default:
						sb.AppendLine(Properties.Window.FormBaseAirCorps.Empty);
						break;

					case 1:     // 配属済み
						if (eq == null)
							goto case 0;
						sb.AppendFormat("[{0}/{1}] ",
							squadron.AircraftCurrent,
							squadron.AircraftMax);

						switch (squadron.Condition)
						{
							case 1:
							default:
								break;
							case 2:
								sb.Append("[" + GeneralRes.Tired + "] ");
								break;
							case 3:
								sb.Append("[" + GeneralRes.VeryTired + "] ");
								break;
						}

						sb.AppendFormat($"{Properties.Window.FormBaseAirCorps.Range}\n", eq.NameWithLevel, eq.MasterEquipment.AircraftDistance);
						break;

					case 2:     // 配置転換中
						sb.AppendFormat($"{GeneralRes.BaseRelocate}\n",
							DateTimeHelper.TimeToCSVString(squadron.RelocatedTime));
						break;
				}
			}

			return sb.ToString();
		}

		public void Dispose()
		{
			Name.Dispose();
			ActionKind.Dispose();
			AirSuperiority.Dispose();
			Distance.Dispose();
			Squadrons.Dispose();
		}
	}


	private TableBaseAirCorpsControl[] ControlMember;

	public FormBaseAirCorps(FormMain parent)
	{
		InitializeComponent();


		ControlMember = new TableBaseAirCorpsControl[9];
		TableMember.SuspendLayout();
		for (int i = 0; i < ControlMember.Length; i++)
		{
			ControlMember[i] = new TableBaseAirCorpsControl(this, TableMember, i);
		}
		TableMember.ResumeLayout();

		ConfigurationChanged();

		Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)IconContent.FormBaseAirCorps]);

		Translate();
	}

	public void Translate()
	{
		ContextMenuBaseAirCorps_CopyOrganization.Text = Properties.Window.FormBaseAirCorps.CopyOrganization;
		// GeneralRes.ContextMenuBaseAirCorps_DisplayRelocatedEquipments <-- dupe
		ContextMenuBaseAirCorps_DisplayRelocatedEquipments.Text = Properties.Window.FormBaseAirCorps.DisplayRelocatedEquipments;

		Text = Properties.Window.FormBaseAirCorps.Title;
	}

	private void FormBaseAirCorps_Load(object sender, EventArgs e)
	{

		var api = Observer.APIObserver.Instance;

		api["api_port/port"].ResponseReceived += Updated;
		api["api_get_member/mapinfo"].ResponseReceived += Updated;
		api["api_get_member/base_air_corps"].ResponseReceived += Updated;
		api["api_req_air_corps/change_deployment_base"].ResponseReceived += Updated;
		api["api_req_air_corps/change_name"].ResponseReceived += Updated;
		api["api_req_air_corps/set_action"].ResponseReceived += Updated;
		api["api_req_air_corps/set_plane"].ResponseReceived += Updated;
		api["api_req_air_corps/supply"].ResponseReceived += Updated;
		api["api_req_air_corps/expand_base"].ResponseReceived += Updated;

		Utility.Configuration.Instance.ConfigurationChanged += ConfigurationChanged;

	}


	private void ConfigurationChanged()
	{

		var c = Utility.Configuration.Config;

		TableMember.SuspendLayout();

		Font = c.UI.MainFont;

		foreach (var control in ControlMember)
			control.ConfigurationChanged(this);

		ControlHelper.SetTableRowStyles(TableMember, ControlHelper.GetDefaultRowStyle());

		TableMember.ResumeLayout();

		if (KCDatabase.Instance.BaseAirCorps.Any())
			Updated(null, null);
	}


	void Updated(string apiname, dynamic data)
	{

		var keys = KCDatabase.Instance.BaseAirCorps.Keys;

		if (Utility.Configuration.Config.FormBaseAirCorps.ShowEventMapOnly)
		{
			var eventAreaCorps = KCDatabase.Instance.BaseAirCorps.Values.Where(b =>
			{
				var maparea = KCDatabase.Instance.MapArea[b.MapAreaID];
				return maparea != null && maparea.MapType == 1;
			}).Select(b => b.ID);

			if (eventAreaCorps.Any())
				keys = eventAreaCorps;
		}


		TableMember.SuspendLayout();
		TableMember.RowCount = keys.Count();
		for (int i = 0; i < ControlMember.Length; i++)
		{
			ControlMember[i].Update(i < keys.Count() ? keys.ElementAt(i) : -1);
		}
		TableMember.ResumeLayout();

		// set icon
		{
			var squadrons = KCDatabase.Instance.BaseAirCorps.Values.Where(b => b != null)
				.SelectMany(b => b.Squadrons.Values)
				.Where(s => s != null);
			bool isNotReplenished = squadrons.Any(s => s.State == 1 && s.AircraftCurrent < s.AircraftMax);
			bool isTired = squadrons.Any(s => s.State == 1 && s.Condition == 2);
			bool isVeryTired = squadrons.Any(s => s.State == 1 && s.Condition == 3);

			int imageIndex;

			if (isNotReplenished)
				imageIndex = (int)IconContent.FleetNotReplenished;
			else if (isVeryTired)
				imageIndex = (int)IconContent.ConditionVeryTired;
			else if (isTired)
				imageIndex = (int)IconContent.ConditionTired;
			else
				imageIndex = (int)IconContent.FormBaseAirCorps;

			if (Icon != null) ResourceManager.DestroyIcon(Icon);
			Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[imageIndex]);
			if (Parent != null) Parent.Refresh();       //アイコンを更新するため
		}

	}


	private void ContextMenuBaseAirCorps_Opening(object sender, System.ComponentModel.CancelEventArgs e)
	{
		if (KCDatabase.Instance.BaseAirCorps.Count == 0)
		{
			e.Cancel = true;
			return;
		}

		if (ContextMenuBaseAirCorps.SourceControl.Name == "Name")
			ContextMenuBaseAirCorps_CopyOrganization.Tag = ContextMenuBaseAirCorps.SourceControl.Tag as int? ?? -1;
		else
			ContextMenuBaseAirCorps_CopyOrganization.Tag = -1;
	}

	private void ContextMenuBaseAirCorps_CopyOrganization_Click(object sender, EventArgs e)
	{

		var sb = new StringBuilder();
		int areaid = ContextMenuBaseAirCorps_CopyOrganization.Tag as int? ?? -1;

		var baseaircorps = KCDatabase.Instance.BaseAirCorps.Values;
		if (areaid != -1)
			baseaircorps = baseaircorps.Where(c => c.MapAreaID == areaid);

		foreach (var corps in baseaircorps)
		{

			string areaName = KCDatabase.Instance.MapArea.ContainsKey(corps.MapAreaID) ? KCDatabase.Instance.MapArea[corps.MapAreaID].NameEN : Properties.Window.FormBaseAirCorps.UnknownArea;

			sb.AppendFormat($"{Properties.Window.FormBaseAirCorps.CopyOrganizationFormat}\n",
				(areaid == -1 ? (areaName + "：") : "") + corps.Name,
				Constants.GetBaseAirCorpsActionKind(corps.ActionKind),
				Calculator.GetAirSuperiority(corps),
				corps.Distance);

			var sq = corps.Squadrons.Values.ToArray();

			for (int i = 0; i < sq.Length; i++)
			{
				if (i > 0)
					sb.Append(", ");

				if (sq[i] == null)
				{
					sb.Append(GeneralRes.BaseUnknown);
					continue;
				}

				switch (sq[i].State)
				{
					case 0:
						sb.Append(GeneralRes.BaseUnassigned);
						break;
					case 1:
					{
						var eq = sq[i].EquipmentInstance;

						sb.Append(eq?.NameWithLevel ?? Properties.Window.FormBaseAirCorps.Empty);

						if (sq[i].AircraftCurrent < sq[i].AircraftMax)
							sb.AppendFormat("[{0}/{1}]", sq[i].AircraftCurrent, sq[i].AircraftMax);
					}
					break;
					case 2:
						sb.Append("(" + GeneralRes.BaseRedeployment + ")");
						break;
				}
			}

			sb.AppendLine();
		}

		Clipboard.SetData(DataFormats.StringFormat, sb.ToString());
	}

	private void ContextMenuBaseAirCorps_DisplayRelocatedEquipments_Click(object sender, EventArgs e)
	{

		string message = string.Join("\r\n", KCDatabase.Instance.RelocatedEquipments.Values
			.Where(eq => eq.EquipmentInstance != null)
			.Select(eq => string.Format("{0} ({1}～)", eq.EquipmentInstance.NameWithLevel, DateTimeHelper.TimeToCSVString(eq.RelocatedTime))));

		if (message.Length == 0)
			message = GeneralRes.ContextMenuBaseAirCorps_DisplayRelocatedEquipments_Detail;

		MessageBox.Show(message, GeneralRes.ContextMenuBaseAirCorps_DisplayRelocatedEquipments_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
	}


	private void TableMember_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
	{
		e.Graphics.DrawLine(Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
	}

	protected override string GetPersistString()
	{
		return "BaseAirCorps";
	}




}
