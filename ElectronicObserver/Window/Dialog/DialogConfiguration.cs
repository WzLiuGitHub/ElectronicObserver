﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectronicObserver.Notifier;
using ElectronicObserver.Observer;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Storage;
using ElectronicObserver.ViewModels;
using ElectronicObserver.Window.Control;
using Translation = ElectronicObserver.Properties.Window.Dialog.DialogConfiguration;

namespace ElectronicObserver.Window.Dialog;

public partial class DialogConfiguration : Form
{

	/// <summary> 司令部「任意アイテム表示」から除外するアイテムのIDリスト </summary>
	private readonly HashSet<int> IgnoredItems = new HashSet<int>() { 1, 2, 3, 4, 50, 51, 66, 67, 69 };

	private System.Windows.Forms.Control _UIControl;

	private Dictionary<SyncBGMPlayer.SoundHandleID, SyncBGMPlayer.SoundHandle> BGMHandles;


	private DateTime _shownTime;
	private double _playTimeCache;



	public DialogConfiguration()
	{
		InitializeComponent();

		_shownTime = DateTime.Now;

		Translate();
	}

	public DialogConfiguration(Configuration.ConfigurationData config)
		: this()
	{

		FromConfiguration(config);
	}

	public void Translate()
	{
		label19.Text = Translation.NetworkSettingsNote;
		ToolTipInfo.SetToolTip(Connection_UpstreamProxyAddress, Translation.UpstreamProxyToolTip);
		Connection_DownstreamProxyLabel.Text = Translation.Connection_DownstreamProxyLabel;
		ToolTipInfo.SetToolTip(Connection_DownstreamProxy, Translation.Connection_DownstreamProxyToolTip);
		Connection_UseSystemProxy.Text = Translation.Connection_UseSystemProxy;
		ToolTipInfo.SetToolTip(Connection_UseSystemProxy, ConfigRes.UseSystemProxyTooltip);
		ToolTipInfo.SetToolTip(Connection_UpstreamProxyPort, Translation.Connection_UpstreamProxyPortToolTip);
		Connection_UseUpstreamProxy.Text = Translation.Connection_UseUpstreamProxy;
		ToolTipInfo.SetToolTip(Connection_UseUpstreamProxy, Translation.Connection_UseUpstreamProxyToolTip);
		Connection_RegisterAsSystemProxy.Text = Translation.Connection_RegisterAsSystemProxy;
		ToolTipInfo.SetToolTip(Connection_RegisterAsSystemProxy, ConfigRes.RegSystemProxyHint);
		Connection_OutputConnectionScript.Text = ConfigRes.OutputProxyScript;
		label4.Text = ConfigRes.MayIncreaseSize;
		ToolTipInfo.SetToolTip(Connection_ApplyVersion, ConfigRes.AddVersionToFile);
		ToolTipInfo.SetToolTip(Connection_SaveOtherFile, ConfigRes.SaveAllConnectionFiles);
		ToolTipInfo.SetToolTip(Connection_SaveResponse, ConfigRes.SaveAPIResponses);
		ToolTipInfo.SetToolTip(Connection_SaveRequest, ConfigRes.SaveAPIRequests);
		Connection_SaveDataPathSearch.Text = Translation.Connection_SaveDataPathSearch;
		label3.Text = ConfigRes.SaveLocation;
		Connection_SaveReceivedData.Text = Translation.Connection_SaveReceivedData;
		ToolTipInfo.SetToolTip(Connection_Port, ConfigRes.ConnectionPort);
		label1.Text = ConfigRes.Port;

		UI_JapaneseEquipmentTypes.Text = Translation.UI_JapaneseEquipmentTypes;
		UI_JapaneseEquipmentNames.Text = Translation.UI_JapaneseEquipmentNames;
		UI_JapaneseShipTypes.Text = Translation.UI_JapaneseShipTypes;
		UI_JapaneseShipNames.Text = Translation.UI_JapaneseShipNames;
		UI_DisableOtherTranslations.Text = Translation.UI_DisableOtherTranslations;
		ToolTipInfo.SetToolTip(UI_DisableOtherTranslations, Translation.UI_DisableOtherTranslationsToolTip);
		UI_NodeNumbering.Text = Translation.UseLetterForNodes;
		label21.Text = Translation.Theme;
		UI_ThemeOptions.Items.Clear();
		UI_ThemeOptions.Items.AddRange(new object[]
		{
			Translation.Theme_Light,
			Translation.Theme_Dark,
			Translation.Theme_Custom
		});
		ToolTipInfo.SetToolTip(UI_ThemeOptions, Translation.ThemeToolTip);
		ToolTipInfo.SetToolTip(UI_RenderingTest, Translation.UI_RenderingTestToolTip);
		UI_IsLayoutFixed.Text = Translation.UI_IsLayoutFixed;
		ToolTipInfo.SetToolTip(UI_IsLayoutFixed, Translation.UI_IsLayoutFixedToolTip);
		UI_BarColorMorphing.Text = ConfigRes.UI_BarColorMorphing;
		ToolTipInfo.SetToolTip(UI_BarColorMorphing, Translation.UI_BarColorMorphingToolTip);
		label8.Text = ConfigRes.Subfont;
		label5.Text = ConfigRes.Mainfont;
		UI_LanguageLabel.Text = Translation.UI_LanguageLabel;
		UI_LanguageOptions.Items.Clear();
		UI_LanguageOptions.Items.AddRange(new object[]
		{
			Translation.Language_English,
			Translation.Language_Japanese
		});
		UI_RestartHint.Text = Translation.UI_RestartHint;

		tabPage3.Text = ConfigRes.Log;
		Log_SaveLogImmediately.Text = Translation.Log_SaveLogImmediately;
		ToolTipInfo.SetToolTip(Log_SaveLogImmediately, Translation.Log_SaveLogImmediatelyToolTip);
		Log_SaveBattleLog.Text = Translation.Log_SaveBattleLog;
		ToolTipInfo.SetToolTip(Log_SaveBattleLog, Translation.Log_SaveBattleLogToolTip);
		Log_ShowSpoiler.Text = Translation.Log_ShowSpoiler;
		ToolTipInfo.SetToolTip(Log_ShowSpoiler, Translation.Log_ShowSpoilerToolTip);
		label12.Text = ConfigRes.CorruptLogs;
		label11.Text = ConfigRes.Enocding;
		ToolTipInfo.SetToolTip(Log_FileEncodingID, ConfigRes.EncodingHint);
		Log_SaveErrorReport.Text = ConfigRes.SaveErrorReport;
		ToolTipInfo.SetToolTip(Log_SaveErrorReport, ConfigRes.SaveErrorHint);
		Log_SaveLogFlag.Text = ConfigRes.SaveLog;
		ToolTipInfo.SetToolTip(Log_LogLevel, Translation.Log_LogLevelToolTip);
		label6.Text = ConfigRes.LoggingLevel;

		tabPage4.Text = Translation.TabPage4;
		label45.Text = Translation.ApplicationId;
		ToolTipInfo.SetToolTip(Control_ApplicationID, Translation.Control_ApplicationIDToolTip);
		Control_ForceUpdate.Text = Translation.Control_ForceUpdate;
		label44.Text = Translation.TranslationURL;
		ToolTipInfo.SetToolTip(Control_translationURL, Translation.Control_translationURLToolTip);
		Control_DiscordRPCShowFCM.Text = Translation.Control_DiscordRPCShowFCM;
		ToolTipInfo.SetToolTip(Control_DiscordRPCShowFCM, Translation.Control_DiscordRPCShowFCMToolTip);
		Control_EnableDiscordRPC.Text = Translation.Control_EnableDiscordRPC;
		ToolTipInfo.SetToolTip(Control_EnableDiscordRPC, Translation.Control_EnableDiscordRPCToolTip);
		label22.Text = Translation.DiscordRPCMessage;
		ToolTipInfo.SetToolTip(Control_DiscordRPCMessage, Translation.Control_DiscordRPCMessageToolTip);
		checkBoxUseSecretaryIconForRPC.Text = Translation.checkBoxUseSecretaryIconForRPC;
		ToolTipInfo.SetToolTip(checkBoxUseSecretaryIconForRPC, Translation.checkBoxUseSecretaryIconForRPCToolTip);
		Control_EnableTsunDbSubmission.Text = Translation.Control_EnableTsunDbSubmission;
		Control_ShowExpeditionAlertDialog.Text = Translation.Control_ShowExpeditionAlertDialog;
		ToolTipInfo.SetToolTip(Control_ShowExpeditionAlertDialog, Translation.Control_ShowExpeditionAlertDialogTooltip);
		Control_ShowSallyAreaAlertDialog.Text = Translation.Control_ShowSallyAreaAlertDialog;
		ToolTipInfo.SetToolTip(Control_ShowSallyAreaAlertDialog, Translation.Control_ShowSallyAreaAlertDialogTooltip);
		Control_PowerEngagementForm.Items.Clear();
		Control_PowerEngagementForm.Items.AddRange(new object[]
		{
			Translation.Control_PowerEngagementForm_Parallel,
			Translation.Control_PowerEngagementForm_HeadOn,
			Translation.Control_PowerEngagementForm_GreenT,
			Translation.Control_PowerEngagementForm_RedT
		});
		ToolTipInfo.SetToolTip(Control_PowerEngagementForm, Translation.Control_PowerEngagementFormToolTip);
		label29.Text = Translation.FormationModifier;
		Control_UseSystemVolume.Text = Translation.Control_UseSystemVolume;
		ToolTipInfo.SetToolTip(Control_UseSystemVolume, Translation.Control_UseSystemVolumeToolTip);
		Control_RecordAutoSaving.Items.Clear();
		Control_RecordAutoSaving.Items.AddRange(new object[]
		{
			ConfigRes.ProgressAutoSaving_Disable,
			ConfigRes.ProgressAutoSaving_Hourly,
			ConfigRes.ProgressAutoSaving_Daily,
			Translation.ProgressAutoSaving_Immediately
		});
		label9.Text = Translation.LogFrequency;
		label7.Text = ConfigRes.MoraleBorder;

		tabPage5.Text = ConfigRes.Debug;
		Debug_AlertOnError.Text = ConfigRes.AlertOnError;
		Debug_LoadAPIListOnLoad.Text = Translation.Debug_LoadAPIListOnLoad;
		ToolTipInfo.SetToolTip(Debug_LoadAPIListOnLoad, Translation.Debug_LoadAPIListOnLoadToolTip);
		Debug_EnableDebugMenu.Text = Translation.Debug_EnableDebugMenu;
		ToolTipInfo.SetToolTip(Debug_EnableDebugMenu, Translation.Debug_EnableDebugMenuToolTip);

		tabPage6.Text = ConfigRes.Window;
		Life_CanCloseFloatWindowInLock.Text = ConfigRes.Life_CanCloseFloatWindowInLock;
		ToolTipInfo.SetToolTip(Life_CanCloseFloatWindowInLock, Translation.Life_CanCloseFloatWindowInLockToolTip);
		Life_LockLayout.Text = Translation.Life_LockLayout;
		ToolTipInfo.SetToolTip(Life_LockLayout, Translation.Life_LockLayoutToolTip);
		label24.Text = Translation.ClockMode;
		Life_ClockFormat.Items.Clear();
		Life_ClockFormat.Items.AddRange(new object[]
		{
			Translation.Life_ClockFormat_ServerTime,
			Translation.Life_ClockFormat_PvpReset,
			Translation.Life_ClockFormat_QuestReset
		});
		Life_ShowStatusBar.Text = Translation.Life_ShowStatusBar;
		Life_CheckUpdateInformation.Text = Translation.Life_CheckUpdateInformation;
		label14.Text = ConfigRes.LayoutFile;
		Life_TopMost.Text = Translation.Life_TopMost;
		Life_ConfirmOnClosing.Text = Translation.Life_ConfirmOnClosing;

		tabPage7.Text = Translation.Window;

		tabPage8.Text = Translation.Fleet;
		FormFleet_AppliesSallyAreaColor.Text = Translation.FormFleet_AppliesSallyAreaColor;
		ToolTipInfo.SetToolTip(FormFleet_AppliesSallyAreaColor, Translation.FormFleet_AppliesSallyAreaColorToolTip);
		label43.Text = Translation.FleetStatus;
		FormFleet_FleetStateDisplayMode.Items.Clear();
		FormFleet_FleetStateDisplayMode.Items.AddRange(new object[]
		{
			Translation.FormFleet_FleetStateDisplayMode_SingleStatus,
			Translation.FormFleet_FleetStateDisplayMode_CollapseAll,
			Translation.FormFleet_FleetStateDisplayMode_CollapseMultiple,
			Translation.FormFleet_FleetStateDisplayMode_ExpandAll
		});
		FormFleet_EmphasizesSubFleetInPort.Text = Translation.FormFleet_EmphasizesSubFleetInPort;
		ToolTipInfo.SetToolTip(FormFleet_EmphasizesSubFleetInPort, Translation.FormFleet_EmphasizesSubFleetInPortToolTip);
		FormFleet_BlinkAtDamaged.Text = Translation.FormFleet_BlinkAtDamaged;
		ToolTipInfo.SetToolTip(FormFleet_BlinkAtDamaged, Translation.FormFleet_BlinkAtDamagedToolTip);
		FormFleet_ReflectAnchorageRepairHealing.Text = Translation.FormFleet_ReflectAnchorageRepairHealing;
		ToolTipInfo.SetToolTip(FormFleet_ReflectAnchorageRepairHealing, Translation.FormFleet_ReflectAnchorageRepairHealingToolTip);
		FormFleet_ShowAirSuperiorityRange.Text = Translation.FormFleet_ShowAirSuperiorityRange;
		ToolTipInfo.SetToolTip(FormFleet_ShowAirSuperiorityRange, Translation.FormFleet_ShowAirSuperiorityRangeToolTip);
		FormFleet_ShowAircraftLevelByNumber.Text = Translation.FormFleet_ShowAircraftLevelByNumber;
		ToolTipInfo.SetToolTip(FormFleet_ShowAircraftLevelByNumber, Translation.FormFleet_ShowAircraftLevelByNumberToolTip);
		ToolTipInfo.SetToolTip(FormFleet_FixedShipNameWidth, Translation.FormFleet_FixedShipNameWidthToolTip);
		FormFleet_ShowConditionIcon.Text = Translation.FormFleet_ShowConditionIcon;
		ToolTipInfo.SetToolTip(FormFleet_ShowConditionIcon, Translation.FormFleet_ShowConditionIconToolTip);
		FormFleet_EquipmentLevelVisibility.Items.Clear();
		FormFleet_EquipmentLevelVisibility.Items.AddRange(new object[]
		{
			ConfigRes.EquipmentLevelVisibility_Hidden,
			ConfigRes.EquipmentLevelVisibility_ImprovOnly,
			ConfigRes.EquipmentLevelVisibility_ProfOnly,
			ConfigRes.EquipmentLevelVisibility_ImprovPrio,
			ConfigRes.EquipmentLevelVisibility_ProfPrio,
			ConfigRes.EquipmentLevelVisibility_Both,
			Translation.EquipmentLevelVisibility_OverlayProficiency
		});
		ToolTipInfo.SetToolTip(FormFleet_EquipmentLevelVisibility, Translation.FormFleet_EquipmentLevelVisibilityToolTip);
		label28.Text = Translation.EquipmentLevelDisplay;
		FormFleet_BlinkAtCompletion.Text = ConfigRes.FleetBlinkAtCompletion;
		ToolTipInfo.SetToolTip(FormFleet_BlinkAtCompletion, Translation.FormFleet_BlinkAtCompletionToolTip);
		FormFleet_ShowAnchorageRepairingTimer.Text = Translation.FormFleet_ShowAnchorageRepairingTimer;
		ToolTipInfo.SetToolTip(FormFleet_ShowAnchorageRepairingTimer, Translation.FormFleet_ShowAnchorageRepairingTimerToolTip);
		FormFleet_AirSuperiorityMethod.Items.Clear();
		FormFleet_AirSuperiorityMethod.Items.AddRange(new object[]
		{
			Translation.FormFleet_AirSuperiorityMethod_Disabled,
			Translation.FormFleet_AirSuperiorityMethod_Enabled
		});
		label23.Text = ConfigRes.AirSuperiorityMethod;
		FormFleet_ShowNextExp.Text = ConfigRes.ShowNextXP;
		ToolTipInfo.SetToolTip(FormFleet_ShowNextExp, Translation.FormFleet_ShowNextExpToolTip);
		FormFleet_ShortenHPBar.Text = Translation.FormFleet_ShortenHPBar;
		ToolTipInfo.SetToolTip(FormFleet_ShortenHPBar, ConfigRes.ShortenHPHint);
		FormFleet_FixShipNameWidth.Text = Translation.FormFleet_FixShipNameWidth;
		ToolTipInfo.SetToolTip(FormFleet_FixShipNameWidth, Translation.FormFleet_FixedShipNameWidthToolTip);
		FormFleet_IsScrollable.Text = Translation.FormFleet_IsScrollable;
		ToolTipInfo.SetToolTip(FormFleet_IsScrollable, Translation.FormFleet_IsScrollableToolTip);
		FormFleet_SearchingAbilityMethod.Items.Clear();
		FormFleet_SearchingAbilityMethod.Items.AddRange(new object[]
		{
			ConfigRes.Old25,
			ConfigRes.Autumn25,
			ConfigRes.New25,
			ConfigRes.Formula33,
			ConfigRes.NewFormula33
		});
		label13.Text = ConfigRes.LosFormula;
		FormFleet_ShowAircraft.Text = Translation.FormFleet_ShowAircraft;

		tabPage9.Text = ConfigRes.Arsenal;
		label38.Text = Translation.NameWidth;
		ToolTipInfo.SetToolTip(FormArsenal_MaxShipNameWidth, Translation.FormArsenal_MaxShipNameWidthToolTip);
		FormArsenal_BlinkAtCompletion.Text = ConfigRes.BlinkAtCompletion;
		FormArsenal_ShowShipName.Text = Translation.FormArsenal_ShowShipName;

		tabPage19.Text = ConfigRes.Dock;
		label39.Text = Translation.NameWidth;
		ToolTipInfo.SetToolTip(FormDock_MaxShipNameWidth, Translation.FormArsenal_MaxShipNameWidthToolTip);
		FormDock_BlinkAtCompletion.Text = ConfigRes.BlinkAtCompletion;

		tabPage16.Text = Translation.HQ;
		label34.Text = Translation.OtherItem;
		label26.Text = Translation.Show;
		FormHeadquarters_BlinkAtMaximum.Text = ConfigRes.BlinkAtMaximum;

		tabPage18.Text = GeneralRes.Compass;
		label40.Text = Translation.NameWidth;
		ToolTipInfo.SetToolTip(FormCompass_MaxShipNameWidth, Translation.FormArsenal_MaxShipNameWidthToolTip);
		FormCompass_IsScrollable.Text = Translation.FormFleet_IsScrollable;
		ToolTipInfo.SetToolTip(FormCompass_IsScrollable, Translation.FormFleet_IsScrollableToolTip);
		label2.Text = ConfigRes.CandidateDisplayCount;

		tabPage10.Text = ConfigRes.Quests;
		FormQuest_AllowUserToSortRows.Text = ConfigRes.AllowUserToSortRows;
		FormQuest_ProgressAutoSaving.Items.Clear();
		FormQuest_ProgressAutoSaving.Items.AddRange(new object[]
		{
			ConfigRes.ProgressAutoSaving_Disable,
			ConfigRes.ProgressAutoSaving_Hourly,
			ConfigRes.ProgressAutoSaving_Daily
		});
		label27.Text = ConfigRes.ProgressAutoSaving;
		groupBox1.Text = ConfigRes.Filter;
		FormQuest_ShowOther.Text = ConfigRes.Others;
		FormQuest_ShowMonthly.Text = ConfigRes.Monthly;
		FormQuest_ShowWeekly.Text = ConfigRes.Weekly;
		FormQuest_ShowDaily.Text = ConfigRes.Daily;
		FormQuest_ShowOnce.Text = ConfigRes.OneTimeOther;
		FormQuest_ShowRunningOnly.Text = ConfigRes.UnderWayOnly;

		tabPage13.Text = ConfigRes.Group;
		FormShipGroup_ShipNameSortMethod.Items.Clear();
		FormShipGroup_ShipNameSortMethod.Items.AddRange(new object[]
		{
			ConfigRes.ShipNameSortMethod_Number,
			ConfigRes.ShipNameSortMethod_Alphabet,
			Translation.ShipNameSortMethod_GameSort
		});
		label25.Text = ConfigRes.ShipNameSortMethod;
		FormShipGroup_ShowStatusBar.Text = Translation.FormShipGroup_ShowStatusBar;
		ToolTipInfo.SetToolTip(FormShipGroup_ShowStatusBar, ConfigRes.ShowStatusbarHint);
		FormShipGroup_AutoUpdate.Text = ConfigRes.AutoUpdate;
		ToolTipInfo.SetToolTip(FormShipGroup_AutoUpdate, ConfigRes.AutoUpdateHint);

		tabPage20.Text = Translation.Combat;
		FormBattle_Display7thAsSingleLine.Text = Translation.FormBattle_Display7thAsSingleLine;
		ToolTipInfo.SetToolTip(FormBattle_Display7thAsSingleLine, Translation.FormBattle_Display7thAsSingleLineToolTip);
		FormBattle_ShowShipTypeInHPBar.Text = Translation.FormBattle_ShowShipTypeInHPBar;
		ToolTipInfo.SetToolTip(FormBattle_ShowShipTypeInHPBar, Translation.FormBattle_ShowShipTypeInHPBarToolTip);
		FormBattle_ShowHPBar.Text = Translation.FormBattle_ShowHPBar;
		ToolTipInfo.SetToolTip(FormBattle_ShowHPBar, Translation.FormBattle_ShowHPBarToolTip);
		FormBattle_HideDuringBattle.Text = Translation.FormBattle_HideDuringBattle;
		ToolTipInfo.SetToolTip(FormBattle_HideDuringBattle, Translation.FormBattle_HideDuringBattleToolTip);
		FormBattle_IsScrollable.Text = Translation.FormFleet_IsScrollable;
		ToolTipInfo.SetToolTip(FormBattle_IsScrollable, Translation.FormFleet_IsScrollableToolTip);

		tabPage12.Text = Translation.Browser;
		FormBrowser_SavesBrowserLog.Text = Translation.FormBrowser_SavesBrowserLog;
		ToolTipInfo.SetToolTip(FormBrowser_SavesBrowserLog, Translation.FormBrowser_SavesBrowserLogTooltip);
		FormBrowser_ForceColorProfile.Text = Translation.FormBrowser_ForceColorProfile;
		ToolTipInfo.SetToolTip(FormBrowser_ForceColorProfile, Translation.FormBrowser_ForceColorProfileToolTip);
		FormBrowser_PreserveDrawingBuffer.Text = Translation.FormBrowser_PreserveDrawingBuffer;
		ToolTipInfo.SetToolTip(FormBrowser_PreserveDrawingBuffer, Translation.FormBrowser_PreserveDrawingBufferToolTip);
		label20.Text = Translation.RestartNotice;
		FormBrowser_HardwareAccelerationEnabled.Text = Translation.FormBrowser_HardwareAccelerationEnabled;
		ToolTipInfo.SetToolTip(FormBrowser_HardwareAccelerationEnabled, Translation.FormBrowser_HardwareAccelerationEnabledToolTip);
		FormBrowser_IsDMMreloadDialogDestroyable.Text = Translation.FormBrowser_IsDMMreloadDialogDestroyable;
		ToolTipInfo.SetToolTip(FormBrowser_IsDMMreloadDialogDestroyable, Translation.FormBrowser_IsDMMreloadDialogDestroyableToolTip);
		FormBrowser_ToolMenuDockStyle.Items.Clear();
		FormBrowser_ToolMenuDockStyle.Items.AddRange(new object[]
		{
			ConfigRes.ToolMenuAlignment_Top,
			ConfigRes.ToolMenuAlignment_Bottom,
			ConfigRes.ToolMenuAlignment_Left,
			ConfigRes.ToolMenuAlignment_Right,
			ConfigRes.ToolMenuAlignment_Invisible
		});
		label30.Text = ConfigRes.ToolMenuDockStyle;
		FormBrowser_ZoomFit.Text = ConfigRes.ZoomToFit;
		ToolTipInfo.SetToolTip(FormBrowser_ZoomFit, ConfigRes.FitHint);
		FormBrowser_AppliesStyleSheet.Text = ConfigRes.ApplyStyleSheet;
		ToolTipInfo.SetToolTip(FormBrowser_AppliesStyleSheet, ConfigRes.ApplyStyleSheetHint);
		FormBrowser_ConfirmAtRefresh.Text = Translation.FormBrowser_ConfirmAtRefresh;
		ToolTipInfo.SetToolTip(FormBrowser_ConfirmAtRefresh, Translation.FormBrowser_ConfirmAtRefreshToolTip);
		groupBox2.Text = Translation.Screenshot;
		label42.Text = Translation.Output;
		FormBrowser_ScreenShotSaveMode.Items.Clear();
		FormBrowser_ScreenShotSaveMode.Items.AddRange(new object[]
		{
			Translation.FormBrowser_ScreenShotSaveMode_ToFile,
			Translation.FormBrowser_ScreenShotSaveMode_ToClipboard,
			Translation.FormBrowser_ScreenShotSaveMode_Both
		});
		FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Text = Translation.FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration;
		ToolTipInfo.SetToolTip(FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration, Translation.FormBrowser_ScreenShotFormat_AvoidTwitterDeteriorationToolTip);
		label18.Text = ConfigRes.SaveLocation;
		label17.Text = ConfigRes.LoginURL;
		label15.Text = Translation.Zoom;
		FormBrowser_UseGadgetRedirect.Text = Translation.FormBrowser_UseGadgetRedirect;
		ToolTipInfo.SetToolTip(FormBrowser_UseGadgetRedirect, Translation.FormBrowser_UseGadgetRedirectToolTip);
		FormBrowser_IsContextMenuEnabled.Text = Translation.FormBrowser_ShowContextMenu;
		ToolTipInfo.SetToolTip(FormBrowser_IsContextMenuEnabled, Translation.FormBrowser_ShowContextMenuToolTip);
		FormBrowser_UseVulkanWorkaround.Text = Translation.FormBrowser_UseVulkanWorkaround;
		ToolTipInfo.SetToolTip(FormBrowser_UseVulkanWorkaround, Translation.FormBrowser_UseVulkanWorkaroundToolTip);

		tabPage21.Text = Translation.AB;
		FormBaseAirCorps_ShowEventMapOnly.Text = Translation.FormBaseAirCorps_ShowEventMapOnly;
		ToolTipInfo.SetToolTip(FormBaseAirCorps_ShowEventMapOnly, Translation.FormBaseAirCorps_ShowEventMapOnlyToolTip);
		FormJson_AutoUpdate.Text = Translation.FormJson_AutoUpdate;
		ToolTipInfo.SetToolTip(FormJson_AutoUpdate, Translation.FormJson_AutoUpdateToolTip);
		label32.Text = Translation.AutoUpdateCouldIncreaseLoad;
		FormJson_UpdatesTree.Text = Translation.FormJson_UpdatesTree;
		ToolTipInfo.SetToolTip(FormJson_UpdatesTree, Translation.FormJson_UpdatesTreeToolTip);
		ToolTipInfo.SetToolTip(FormJson_AutoUpdateFilter, Translation.FormJson_AutoUpdateFilterToolTip);
		label31.Text = Translation.UpdateFilter;
		label33.Text = ConfigRes.HiddenJSON;

		tabPage11.Text = ConfigRes.Notification;
		silenceFullscreen.Text = Translation.silenceFullscreen;
		Notification_Silencio.Text = Translation.Notification_Silencio;
		ToolTipInfo.SetToolTip(Notification_Silencio, Translation.Notification_SilencioToolTip);
		Notification_AnchorageRepair.Text = ConfigRes.AnchorageRepairFinish + ConfigRes.NotificationSetting;
		label10.Text = ConfigRes.ApplyonOK;
		Notification_Damage.Text = ConfigRes.TaihaAdvance + ConfigRes.NotificationSetting;
		Notification_Condition.Text = ConfigRes.FatigueRestore + ConfigRes.NotificationSetting;
		Notification_Repair.Text = ConfigRes.DockEnd + ConfigRes.NotificationSetting;
		Notification_Construction.Text = ConfigRes.ConstructEnd + ConfigRes.NotificationSetting;
		Notification_Expedition.Text = ConfigRes.ExpedReturn + ConfigRes.NotificationSetting;
		Notification_RemodelLevel.Text = Translation.Notification_RemodelLevel;
		Notification_BaseAirCorps.Text = Translation.Notification_BaseAirCorps;
		Notification_BattleEnd.Text = Translation.Notification_BattleEnd;

		BGMPlayer_SyncBrowserMute.Text = Translation.BGMPlayer_SyncBrowserMute;
		ToolTipInfo.SetToolTip(BGMPlayer_SyncBrowserMute, Translation.BGMPlayer_SyncBrowserMuteToolTip);
		BGMPlayer_SetVolumeAll.Text = ConfigRes.BGMPlayer_SetVolumeAll;
		BGMPlayer_Enabled.Text = ConfigRes.BGMPlayer_Enabled;
		BGMPlayer_ColumnContent.HeaderText = ConfigRes.Scene;
		BGMPlayer_ColumnPath.HeaderText = ConfigRes.FileName;
		BGMPlayer_ColumnSetting.HeaderText = ConfigRes.Settings;

		ButtonCancel.Text = ConfigRes.Cancel;
		FolderBrowser.Description = ConfigRes.FolderSearch;
		LayoutFileBrowser.Title = ConfigRes.LayoutSearch;
		APIListBrowser.Title = Translation.APIListBrowser;
		Log_PlayTime.Text = Translation.Log_PlayTime;

		Text = ConfigRes.Settings;
	}

	private void Connection_SaveReceivedData_CheckedChanged(object sender, EventArgs e)
	{

		Connection_PanelSaveData.Enabled = Connection_SaveReceivedData.Checked;

	}


	private void Connection_SaveDataPath_TextChanged(object sender, EventArgs e)
	{

		if (Directory.Exists(Connection_SaveDataPath.Text))
		{
			Connection_SaveDataPath.BackColor = SystemColors.Window;
			ToolTipInfo.SetToolTip(Connection_SaveDataPath, null);
		}
		else
		{
			Connection_SaveDataPath.BackColor = Color.MistyRose;
			ToolTipInfo.SetToolTip(Connection_SaveDataPath, Translation.SaveDataPathDoesNotExist);
		}
	}


	/// <summary>
	/// パラメータの更新をUIに適用します。
	/// </summary>
	internal void UpdateParameter()
	{

		Connection_SaveReceivedData_CheckedChanged(null, new EventArgs());
		Connection_SaveDataPath_TextChanged(null, new EventArgs());
		Debug_EnableDebugMenu_CheckedChanged(null, new EventArgs());
		FormFleet_FixShipNameWidth_CheckedChanged(null, new EventArgs());
	}



	private void Connection_SaveDataPathSearch_Click(object sender, EventArgs e)
	{

		Connection_SaveDataPath.Text = PathHelper.ProcessFolderBrowserDialog(Connection_SaveDataPath.Text, FolderBrowser);

	}


	private void UI_MainFontSelect_Click(object sender, EventArgs e)
	{

		FontSelector.Font = UI_MainFont.Font;

		if (FontSelector.ShowDialog(App.Current.MainWindow) == System.Windows.Forms.DialogResult.OK)
		{

			SerializableFont font = new SerializableFont(FontSelector.Font);

			UI_MainFont.Text = font.SerializeFontAttribute;
			UI_MainFont.BackColor = SystemColors.Window;
			UI_RenderingTest.MainFont = font.FontData;
		}

	}


	private void UI_SubFontSelect_Click(object sender, EventArgs e)
	{

		FontSelector.Font = UI_SubFont.Font;

		if (FontSelector.ShowDialog(App.Current.MainWindow) == System.Windows.Forms.DialogResult.OK)
		{

			SerializableFont font = new SerializableFont(FontSelector.Font);

			UI_SubFont.Text = font.SerializeFontAttribute;
			UI_SubFont.BackColor = SystemColors.Window;
			UI_RenderingTest.SubFont = font.FontData;
		}

	}


	private void DialogConfiguration_Load(object sender, EventArgs e)
	{

		this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)IconContent.FormConfiguration]);

		_UIControl = Owner;

		this.Control_EnableTsunDbSubmission.CheckStateChanged += new System.EventHandler(this.Control_EnableTsunDbSubmission_CheckStateChanged);


	}

	private void DialogConfiguration_FormClosed(object sender, FormClosedEventArgs e)
	{

		ResourceManager.DestroyIcon(Icon);

	}


	private void UI_MainFontApply_Click(object sender, EventArgs e)
	{

		UI_MainFont.Font = SerializableFont.StringToFont(UI_MainFont.Text) ?? UI_MainFont.Font;
	}

	private void UI_SubFontApply_Click(object sender, EventArgs e)
	{

		UI_SubFont.Font = SerializableFont.StringToFont(UI_SubFont.Text) ?? UI_SubFont.Font;
	}




	//ui
	private void Connection_OutputConnectionScript_Click(object sender, EventArgs e)
	{

		string serverAddress = APIObserver.Instance.ServerAddress;
		if (serverAddress == null)
		{
			MessageBox.Show(Translation.PleaseStartKancolle, Translation.DialogCaptionErrorTitle,
				MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}

		using (var dialog = new SaveFileDialog())
		{
			dialog.Filter = "Proxy Script|*.pac|File|*";
			dialog.Title = Translation.SavePacFileAs;
			dialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
			dialog.FileName = System.IO.Directory.GetCurrentDirectory() + "\\proxy.pac";

			if (dialog.ShowDialog(App.Current.MainWindow) == System.Windows.Forms.DialogResult.OK)
			{

				try
				{

					using (StreamWriter sw = new StreamWriter(dialog.FileName))
					{

						sw.WriteLine("function FindProxyForURL(url, host) {");
						sw.WriteLine("  if (/^" + serverAddress.Replace(".", @"\.") + "/.test(host)) {");
						sw.WriteLine("    return \"PROXY localhost:{0}; DIRECT\";", (int)Connection_Port.Value);
						sw.WriteLine("  }");
						sw.WriteLine("  return \"DIRECT\";");
						sw.WriteLine("}");

					}

					Clipboard.SetData(DataFormats.StringFormat, "file:///" + dialog.FileName.Replace('\\', '/'));

					MessageBox.Show(Translation.ProxyAutoConfigSaved, Translation.PacSavedTitle,
						MessageBoxButtons.OK, MessageBoxIcon.Information);


				}
				catch (Exception ex)
				{

					Utility.ErrorReporter.SendErrorReport(ex, Translation.FailedToSavePac);
					MessageBox.Show(Translation.FailedToSavePac + "\r\n" + ex.Message, Translation.DialogCaptionErrorTitle,
						MessageBoxButtons.OK, MessageBoxIcon.Error);

				}

			}
		}

	}



	private void Notification_Expedition_Click(object sender, EventArgs e)
	{

		using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Expedition))
		{
			dialog.ShowDialog(App.Current.MainWindow);
		}
	}

	private void Notification_Construction_Click(object sender, EventArgs e)
	{

		using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Construction))
		{
			dialog.ShowDialog(App.Current.MainWindow);
		}
	}

	private void Notification_Repair_Click(object sender, EventArgs e)
	{

		using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Repair))
		{
			dialog.ShowDialog(App.Current.MainWindow);
		}
	}

	private void Notification_Condition_Click(object sender, EventArgs e)
	{

		using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Condition))
		{
			dialog.ShowDialog(App.Current.MainWindow);
		}
	}

	private void Notification_Damage_Click(object sender, EventArgs e)
	{

		using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.Damage))
		{
			dialog.ShowDialog(App.Current.MainWindow);
		}
	}

	private void Notification_AnchorageRepair_Click(object sender, EventArgs e)
	{

		using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.AnchorageRepair))
		{
			dialog.ShowDialog(App.Current.MainWindow);
		}
	}

	private void Notification_BaseAirCorps_Click(object sender, EventArgs e)
	{
		using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.BaseAirCorps))
		{
			dialog.ShowDialog(App.Current.MainWindow);
		}
	}

	private void Notification_BattleEnd_Click(object sender, EventArgs e)
	{
		using (var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.BattleEnd))
		{
			dialog.ShowDialog(App.Current.MainWindow);
		}
	}

	private void Notification_RemodelLevel_Click(object sender, EventArgs e)
	{
		using var dialog = new DialogConfigurationNotifier(NotifierManager.Instance.RemodelLevel);
		dialog.ShowDialog(App.Current.MainWindow);
	}

	private void Life_LayoutFilePathSearch_Click(object sender, EventArgs e)
	{

		Life_LayoutFilePath.Text = PathHelper.ProcessOpenFileDialog(Life_LayoutFilePath.Text, LayoutFileBrowser);

	}


	private void Debug_APIListPathSearch_Click(object sender, EventArgs e)
	{

		Debug_APIListPath.Text = PathHelper.ProcessOpenFileDialog(Debug_APIListPath.Text, APIListBrowser);

	}


	private void Debug_EnableDebugMenu_CheckedChanged(object sender, EventArgs e)
	{

		Debug_SealingPanel.Visible =
			Connection_UpstreamProxyAddress.Visible =
				Connection_DownstreamProxy.Visible =
					Connection_DownstreamProxyLabel.Visible =
						SubWindow_Json_SealingPanel.Visible =
							Debug_EnableDebugMenu.Checked;

	}


	private void FormBrowser_ScreenShotPathSearch_Click(object sender, EventArgs e)
	{

		FormBrowser_ScreenShotPath.Text = PathHelper.ProcessFolderBrowserDialog(FormBrowser_ScreenShotPath.Text, FolderBrowser);
	}





	/// <summary>
	/// 設定からUIを初期化します。
	/// </summary>
	public void FromConfiguration(Configuration.ConfigurationData config)
	{

		//[通信]
		Connection_Port.Value = config.Connection.Port;
		Connection_SaveReceivedData.Checked = config.Connection.SaveReceivedData;
		Connection_SaveDataPath.Text = config.Connection.SaveDataPath;
		Connection_SaveRequest.Checked = config.Connection.SaveRequest;
		Connection_SaveResponse.Checked = config.Connection.SaveResponse;
		Connection_SaveOtherFile.Checked = config.Connection.SaveOtherFile;
		Connection_ApplyVersion.Checked = config.Connection.ApplyVersion;
		Connection_RegisterAsSystemProxy.Checked = config.Connection.RegisterAsSystemProxy;
		Connection_UseUpstreamProxy.Checked = config.Connection.UseUpstreamProxy;
		Connection_UpstreamProxyPort.Value = config.Connection.UpstreamProxyPort;
		Connection_UpstreamProxyAddress.Text = config.Connection.UpstreamProxyAddress;
		Connection_UseSystemProxy.Checked = config.Connection.UseSystemProxy;
		Connection_DownstreamProxy.Text = config.Connection.DownstreamProxy;

		//[UI]
		UI_MainFont.Text = config.UI.MainFont.SerializeFontAttribute;
		UI_SubFont.Text = config.UI.SubFont.SerializeFontAttribute;
		UI_BarColorMorphing.Checked = config.UI.BarColorMorphing;
		UI_JapaneseShipNames.Checked = config.UI.JapaneseShipName;
		UI_JapaneseShipTypes.Checked = config.UI.JapaneseShipType;
		UI_JapaneseEquipmentNames.Checked = config.UI.JapaneseEquipmentName;
		UI_JapaneseEquipmentTypes.Checked = config.UI.JapaneseEquipmentType;
		UI_DisableOtherTranslations.Checked = config.UI.DisableOtherTranslations;
		UI_NodeNumbering.Checked = !config.UI.UseOriginalNodeId;
		UI_ThemeOptions.SelectedIndex = config.UI.ThemeMode;
		UI_LanguageOptions.SelectedIndex = config.UI.Culture switch
		{
			"ja-JP" => 1,
			_ => 0
		};

		UI_IsLayoutFixed.Checked = config.UI.IsLayoutFixed;
		{
			UI_RenderingTest.MainFont = config.UI.MainFont.FontData;
			UI_RenderingTest.SubFont = config.UI.SubFont.FontData;
			UI_RenderingTest.HPBar.ColorMorphing = config.UI.BarColorMorphing;
			UI_RenderingTest.HPBar.SetBarColorScheme(config.UI.BarColorScheme.Select(c => c.ColorData).ToArray());
			UI_RenderingTestChanger.Maximum = UI_RenderingTest.MaximumValue;
			UI_RenderingTestChanger.Value = UI_RenderingTest.Value;
		}

		//[ログ]
		Log_LogLevel.Value = config.Log.LogLevel;
		Log_SaveLogFlag.Checked = config.Log.SaveLogFlag;
		Log_SaveErrorReport.Checked = config.Log.SaveErrorReport;
		Log_FileEncodingID.SelectedIndex = config.Log.FileEncodingID;
		Log_ShowSpoiler.Checked = config.Log.ShowSpoiler;
		_playTimeCache = config.Log.PlayTime;
		UpdatePlayTime();
		Log_SaveBattleLog.Checked = config.Log.SaveBattleLog;
		Log_SaveLogImmediately.Checked = config.Log.SaveLogImmediately;

		//[動作]
		Control_ConditionBorder.Value = config.Control.ConditionBorder;
		Control_RecordAutoSaving.SelectedIndex = config.Control.RecordAutoSaving;
		Control_UseSystemVolume.Checked = config.Control.UseSystemVolume;
		Control_PowerEngagementForm.SelectedIndex = config.Control.PowerEngagementForm - 1;
		Control_ShowSallyAreaAlertDialog.Checked = config.Control.ShowSallyAreaAlertDialog;
		Control_ShowExpeditionAlertDialog.Checked = config.Control.ShowExpeditionAlertDialog;
		Control_EnableDiscordRPC.Checked = config.Control.EnableDiscordRPC;
		Control_DiscordRPCMessage.Text = config.Control.DiscordRPCMessage;
		Control_DiscordRPCShowFCM.Checked = config.Control.DiscordRPCShowFCM;
		Control_DiscordRPCMessage.ReadOnly = !config.Control.EnableDiscordRPC;
		Control_translationURL.Text = config.Control.UpdateURL.AbsoluteUri;
		Control_ApplicationID.Text = config.Control.DiscordRPCApplicationId;
		Control_EnableTsunDbSubmission.Checked = config.Control.SubmitDataToTsunDb == true;
		checkBoxUseSecretaryIconForRPC.Checked = config.Control.UseFlagshipIconForRPC;

		//[デバッグ]
		Debug_EnableDebugMenu.Checked = config.Debug.EnableDebugMenu;
		Debug_LoadAPIListOnLoad.Checked = config.Debug.LoadAPIListOnLoad;
		Debug_APIListPath.Text = config.Debug.APIListPath;
		Debug_AlertOnError.Checked = config.Debug.AlertOnError;

		//[起動と終了]
		Life_ConfirmOnClosing.Checked = config.Life.ConfirmOnClosing;
		Life_TopMost.Checked = this.TopMost = config.Life.TopMost;      //メインウィンドウに隠れないように
		Life_LayoutFilePath.Text = config.Life.LayoutFilePath;
		Life_CheckUpdateInformation.Checked = config.Life.CheckUpdateInformation;
		Life_ShowStatusBar.Checked = config.Life.ShowStatusBar;
		Life_ClockFormat.SelectedIndex = config.Life.ClockFormat;
		Life_LockLayout.Checked = config.Life.LockLayout;
		Life_CanCloseFloatWindowInLock.Checked = config.Life.CanCloseFloatWindowInLock;

		//[サブウィンドウ]
		FormArsenal_ShowShipName.Checked = config.FormArsenal.ShowShipName;
		FormArsenal_BlinkAtCompletion.Checked = config.FormArsenal.BlinkAtCompletion;
		FormArsenal_MaxShipNameWidth.Value = config.FormArsenal.MaxShipNameWidth;

		FormDock_BlinkAtCompletion.Checked = config.FormDock.BlinkAtCompletion;
		FormDock_MaxShipNameWidth.Value = config.FormDock.MaxShipNameWidth;

		FormFleet_ShowAircraft.Checked = config.FormFleet.ShowAircraft;
		FormFleet_SearchingAbilityMethod.SelectedIndex = config.FormFleet.SearchingAbilityMethod;
		FormFleet_IsScrollable.Checked = config.FormFleet.IsScrollable;
		FormFleet_FixShipNameWidth.Checked = config.FormFleet.FixShipNameWidth;
		FormFleet_ShortenHPBar.Checked = config.FormFleet.ShortenHPBar;
		FormFleet_ShowNextExp.Checked = config.FormFleet.ShowNextExp;
		FormFleet_EquipmentLevelVisibility.SelectedIndex = (int)config.FormFleet.EquipmentLevelVisibility;
		FormFleet_ShowAircraftLevelByNumber.Checked = config.FormFleet.ShowAircraftLevelByNumber;
		FormFleet_AirSuperiorityMethod.SelectedIndex = config.FormFleet.AirSuperiorityMethod;
		FormFleet_ShowAnchorageRepairingTimer.Checked = config.FormFleet.ShowAnchorageRepairingTimer;
		FormFleet_BlinkAtCompletion.Checked = config.FormFleet.BlinkAtCompletion;
		FormFleet_ShowConditionIcon.Checked = config.FormFleet.ShowConditionIcon;
		FormFleet_FixedShipNameWidth.Value = config.FormFleet.FixedShipNameWidth;
		FormFleet_ShowAirSuperiorityRange.Checked = config.FormFleet.ShowAirSuperiorityRange;
		FormFleet_ReflectAnchorageRepairHealing.Checked = config.FormFleet.ReflectAnchorageRepairHealing;
		FormFleet_BlinkAtDamaged.Checked = config.FormFleet.BlinkAtDamaged;
		FormFleet_EmphasizesSubFleetInPort.Checked = config.FormFleet.EmphasizesSubFleetInPort;
		FormFleet_FleetStateDisplayMode.SelectedIndex = config.FormFleet.FleetStateDisplayMode;
		FormFleet_AppliesSallyAreaColor.Checked = config.FormFleet.AppliesSallyAreaColor;

		FormHeadquarters_BlinkAtMaximum.Checked = config.FormHeadquarters.BlinkAtMaximum;
		FormHeadquarters_Visibility.Items.Clear();
		FormHeadquarters_Visibility.Items.AddRange(FormHeadquarters.GetItemNames().ToArray());
		FormHeadquarters.CheckVisibilityConfiguration();
		for (int i = 0; i < FormHeadquarters_Visibility.Items.Count; i++)
		{
			FormHeadquarters_Visibility.SetItemChecked(i, config.FormHeadquarters.Visibility.List[i]);
		}

		{
			FormHeadquarters_DisplayUseItemID.Items.AddRange(
				ElectronicObserver.Data.KCDatabase.Instance.MasterUseItems.Values
					.Where(i => i.Name.Length > 0 && i.Description.Length > 0 && !IgnoredItems.Contains(i.ItemID))
					.Select(i => i.Name).ToArray());
			var item = ElectronicObserver.Data.KCDatabase.Instance.MasterUseItems[config.FormHeadquarters.DisplayUseItemID];

			if (item != null)
			{
				FormHeadquarters_DisplayUseItemID.Text = item.Name;
			}
			else
			{
				FormHeadquarters_DisplayUseItemID.Text = config.FormHeadquarters.DisplayUseItemID.ToString();
			}
		}

		FormQuest_ShowRunningOnly.Checked = config.FormQuest.ShowRunningOnly;
		FormQuest_ShowOnce.Checked = config.FormQuest.ShowOnce;
		FormQuest_ShowDaily.Checked = config.FormQuest.ShowDaily;
		FormQuest_ShowWeekly.Checked = config.FormQuest.ShowWeekly;
		FormQuest_ShowMonthly.Checked = config.FormQuest.ShowMonthly;
		FormQuest_ShowOther.Checked = config.FormQuest.ShowOther;
		FormQuest_ProgressAutoSaving.SelectedIndex = config.FormQuest.ProgressAutoSaving;
		FormQuest_AllowUserToSortRows.Checked = config.FormQuest.AllowUserToSortRows;

		FormShipGroup_AutoUpdate.Checked = config.FormShipGroup.AutoUpdate;
		FormShipGroup_ShowStatusBar.Checked = config.FormShipGroup.ShowStatusBar;
		FormShipGroup_ShipNameSortMethod.SelectedIndex = config.FormShipGroup.ShipNameSortMethod;

		FormBattle_IsScrollable.Checked = config.FormBattle.IsScrollable;
		FormBattle_HideDuringBattle.Checked = config.FormBattle.HideDuringBattle;
		FormBattle_ShowHPBar.Checked = config.FormBattle.ShowHPBar;
		FormBattle_ShowShipTypeInHPBar.Checked = config.FormBattle.ShowShipTypeInHPBar;
		FormBattle_Display7thAsSingleLine.Checked = config.FormBattle.Display7thAsSingleLine;

		FormBrowser_IsEnabled.Checked = config.FormBrowser.IsEnabled;
		FormBrowser_ZoomRate.Value = (decimal)Math.Min(Math.Max(config.FormBrowser.ZoomRate * 100, 10), 1000);
		FormBrowser_ZoomFit.Checked = config.FormBrowser.ZoomFit;
		FormBrowser_LogInPageURL.Text = config.FormBrowser.LogInPageURL;
		FormBrowser_ScreenShotFormat_JPEG.Checked = config.FormBrowser.ScreenShotFormat == 1;
		FormBrowser_ScreenShotFormat_PNG.Checked = config.FormBrowser.ScreenShotFormat == 2;
		FormBrowser_ScreenShotPath.Text = config.FormBrowser.ScreenShotPath;
		FormBrowser_ConfirmAtRefresh.Checked = config.FormBrowser.ConfirmAtRefresh;
		FormBrowser_AppliesStyleSheet.Checked = config.FormBrowser.AppliesStyleSheet;
		FormBrowser_IsDMMreloadDialogDestroyable.Checked = config.FormBrowser.IsDMMreloadDialogDestroyable;
		FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Checked = config.FormBrowser.AvoidTwitterDeterioration;
		FormBrowser_ScreenShotSaveMode.SelectedIndex = config.FormBrowser.ScreenShotSaveMode - 1;
		FormBrowser_HardwareAccelerationEnabled.Checked = config.FormBrowser.HardwareAccelerationEnabled;
		FormBrowser_PreserveDrawingBuffer.Checked = config.FormBrowser.PreserveDrawingBuffer;
		FormBrowser_ForceColorProfile.Checked = config.FormBrowser.ForceColorProfile;
		FormBrowser_SavesBrowserLog.Checked = config.FormBrowser.SavesBrowserLog;
		FormBrowser_UseGadgetRedirect.Checked = config.FormBrowser.UseGadgetRedirect;
		FormBrowser_IsContextMenuEnabled.Checked = config.FormBrowser.IsBrowserContextMenuEnabled;
		FormBrowser_UseVulkanWorkaround.Checked = config.FormBrowser.UseVulkanWorkaround;

		if (!config.FormBrowser.IsToolMenuVisible)
			FormBrowser_ToolMenuDockStyle.SelectedIndex = 4;
		else
			FormBrowser_ToolMenuDockStyle.SelectedIndex = (int)config.FormBrowser.ToolMenuDockStyle - 1;

		FormCompass_CandidateDisplayCount.Value = config.FormCompass.CandidateDisplayCount;
		FormCompass_IsScrollable.Checked = config.FormCompass.IsScrollable;
		FormCompass_MaxShipNameWidth.Value = config.FormCompass.MaxShipNameWidth;

		FormJson_AutoUpdate.Checked = config.FormJson.AutoUpdate;
		FormJson_UpdatesTree.Checked = config.FormJson.UpdatesTree;
		FormJson_AutoUpdateFilter.Text = config.FormJson.AutoUpdateFilter;

		FormBaseAirCorps_ShowEventMapOnly.Checked = config.FormBaseAirCorps.ShowEventMapOnly;


		//[通知]
		{
			bool issilenced = NotifierManager.Instance.GetNotifiers().All(no => no.IsSilenced);
			Notification_Silencio.Checked = issilenced;
			setSilencioConfig(issilenced);
		}

		//[BGM]
		BGMPlayer_Enabled.Checked = config.BGMPlayer.Enabled;
		BGMHandles = config.BGMPlayer.Handles.ToDictionary(h => h.HandleID);
		BGMPlayer_SyncBrowserMute.Checked = config.BGMPlayer.SyncBrowserMute;
		UpdateBGMPlayerUI();

		//finalize
		UpdateParameter();
	}



	/// <summary>
	/// UIをもとに設定を適用します。
	/// </summary>
	public void ToConfiguration(Configuration.ConfigurationData config)
	{

		//[通信]
		{
			bool changed = false;

			changed |= config.Connection.Port != (ushort)Connection_Port.Value;
			config.Connection.Port = (ushort)Connection_Port.Value;

			config.Connection.SaveReceivedData = Connection_SaveReceivedData.Checked;
			config.Connection.SaveDataPath = Connection_SaveDataPath.Text.Trim(@"\ """.ToCharArray());
			config.Connection.SaveRequest = Connection_SaveRequest.Checked;
			config.Connection.SaveResponse = Connection_SaveResponse.Checked;
			config.Connection.SaveOtherFile = Connection_SaveOtherFile.Checked;
			config.Connection.ApplyVersion = Connection_ApplyVersion.Checked;

			changed |= config.Connection.RegisterAsSystemProxy != Connection_RegisterAsSystemProxy.Checked;
			config.Connection.RegisterAsSystemProxy = Connection_RegisterAsSystemProxy.Checked;

			changed |= config.Connection.UseUpstreamProxy != Connection_UseUpstreamProxy.Checked;
			config.Connection.UseUpstreamProxy = Connection_UseUpstreamProxy.Checked;
			changed |= config.Connection.UpstreamProxyPort != (ushort)Connection_UpstreamProxyPort.Value;
			config.Connection.UpstreamProxyPort = (ushort)Connection_UpstreamProxyPort.Value;
			changed |= config.Connection.UpstreamProxyAddress != Connection_UpstreamProxyAddress.Text;
			config.Connection.UpstreamProxyAddress = Connection_UpstreamProxyAddress.Text;

			changed |= config.Connection.UseSystemProxy != Connection_UseSystemProxy.Checked;
			config.Connection.UseSystemProxy = Connection_UseSystemProxy.Checked;

			changed |= config.Connection.DownstreamProxy != Connection_DownstreamProxy.Text;
			config.Connection.DownstreamProxy = Connection_DownstreamProxy.Text;

			if (changed)
			{
				APIObserver.Instance.Start(config.Connection.Port, _UIControl);
			}

		}

		//[UI]
		{
			var newfont = SerializableFont.StringToFont(UI_MainFont.Text, true);
			if (newfont != null)
				config.UI.MainFont = newfont;
		}
		{
			var newfont = SerializableFont.StringToFont(UI_SubFont.Text, true);
			if (newfont != null)
				config.UI.SubFont = newfont;
		}
		config.UI.BarColorMorphing = UI_BarColorMorphing.Checked;
		config.UI.JapaneseShipName = UI_JapaneseShipNames.Checked;
		config.UI.JapaneseShipType = UI_JapaneseShipTypes.Checked;
		config.UI.JapaneseEquipmentName = UI_JapaneseEquipmentNames.Checked;
		config.UI.JapaneseEquipmentType = UI_JapaneseEquipmentTypes.Checked;
		config.UI.DisableOtherTranslations = UI_DisableOtherTranslations.Checked;
		config.UI.UseOriginalNodeId = !UI_NodeNumbering.Checked;
		config.UI.ThemeMode = UI_ThemeOptions.SelectedIndex;
		config.UI.IsLayoutFixed = UI_IsLayoutFixed.Checked;
		config.UI.Culture = UI_LanguageOptions.SelectedItem switch
		{
			string s when s == Translation.Language_Japanese => "ja-JP",
			_ => "en-US"
		};

		//[ログ]
		config.Log.LogLevel = (int)Log_LogLevel.Value;
		config.Log.SaveLogFlag = Log_SaveLogFlag.Checked;
		config.Log.SaveErrorReport = Log_SaveErrorReport.Checked;
		config.Log.FileEncodingID = Log_FileEncodingID.SelectedIndex;
		config.Log.ShowSpoiler = Log_ShowSpoiler.Checked;
		config.Log.SaveBattleLog = Log_SaveBattleLog.Checked;
		config.Log.SaveLogImmediately = Log_SaveLogImmediately.Checked;

		//[動作]
		config.Control.ConditionBorder = (int)Control_ConditionBorder.Value;
		config.Control.RecordAutoSaving = Control_RecordAutoSaving.SelectedIndex;
		config.Control.UseSystemVolume = Control_UseSystemVolume.Checked;
		config.Control.PowerEngagementForm = Control_PowerEngagementForm.SelectedIndex + 1;
		config.Control.ShowSallyAreaAlertDialog = Control_ShowSallyAreaAlertDialog.Checked;
		config.Control.ShowExpeditionAlertDialog = Control_ShowExpeditionAlertDialog.Checked;
		config.Control.EnableDiscordRPC = Control_EnableDiscordRPC.Checked;
		config.Control.DiscordRPCMessage = Control_DiscordRPCMessage.Text;
		config.Control.DiscordRPCShowFCM = Control_DiscordRPCShowFCM.Checked;
		config.Control.UpdateURL = new Uri(Control_translationURL.Text);
		config.Control.DiscordRPCApplicationId = Control_ApplicationID.Text;
		config.Control.SubmitDataToTsunDb = Control_EnableTsunDbSubmission.Checked;
		config.Control.UseFlagshipIconForRPC = checkBoxUseSecretaryIconForRPC.Checked;

		//[デバッグ]
		config.Debug.EnableDebugMenu = Debug_EnableDebugMenu.Checked;
		config.Debug.LoadAPIListOnLoad = Debug_LoadAPIListOnLoad.Checked;
		config.Debug.APIListPath = Debug_APIListPath.Text;
		config.Debug.AlertOnError = Debug_AlertOnError.Checked;

		//[起動と終了]
		config.Life.ConfirmOnClosing = Life_ConfirmOnClosing.Checked;
		config.Life.TopMost = Life_TopMost.Checked;
		config.Life.LayoutFilePath = Life_LayoutFilePath.Text;
		config.Life.CheckUpdateInformation = Life_CheckUpdateInformation.Checked;
		config.Life.ShowStatusBar = Life_ShowStatusBar.Checked;
		config.Life.ClockFormat = Life_ClockFormat.SelectedIndex;
		config.Life.LockLayout = Life_LockLayout.Checked;
		config.Life.CanCloseFloatWindowInLock = Life_CanCloseFloatWindowInLock.Checked;

		//[サブウィンドウ]
		config.FormArsenal.ShowShipName = FormArsenal_ShowShipName.Checked;
		config.FormArsenal.BlinkAtCompletion = FormArsenal_BlinkAtCompletion.Checked;
		config.FormArsenal.MaxShipNameWidth = (int)FormArsenal_MaxShipNameWidth.Value;

		config.FormDock.BlinkAtCompletion = FormDock_BlinkAtCompletion.Checked;
		config.FormDock.MaxShipNameWidth = (int)FormDock_MaxShipNameWidth.Value;

		config.FormFleet.ShowAircraft = FormFleet_ShowAircraft.Checked;
		config.FormFleet.SearchingAbilityMethod = FormFleet_SearchingAbilityMethod.SelectedIndex;
		config.FormFleet.IsScrollable = FormFleet_IsScrollable.Checked;
		config.FormFleet.FixShipNameWidth = FormFleet_FixShipNameWidth.Checked;
		config.FormFleet.ShortenHPBar = FormFleet_ShortenHPBar.Checked;
		config.FormFleet.ShowNextExp = FormFleet_ShowNextExp.Checked;
		config.FormFleet.EquipmentLevelVisibility = (LevelVisibilityFlag)FormFleet_EquipmentLevelVisibility.SelectedIndex;
		config.FormFleet.ShowAircraftLevelByNumber = FormFleet_ShowAircraftLevelByNumber.Checked;
		config.FormFleet.AirSuperiorityMethod = FormFleet_AirSuperiorityMethod.SelectedIndex;
		config.FormFleet.ShowAnchorageRepairingTimer = FormFleet_ShowAnchorageRepairingTimer.Checked;
		config.FormFleet.BlinkAtCompletion = FormFleet_BlinkAtCompletion.Checked;
		config.FormFleet.ShowConditionIcon = FormFleet_ShowConditionIcon.Checked;
		config.FormFleet.FixedShipNameWidth = (int)FormFleet_FixedShipNameWidth.Value;
		config.FormFleet.ShowAirSuperiorityRange = FormFleet_ShowAirSuperiorityRange.Checked;
		config.FormFleet.ReflectAnchorageRepairHealing = FormFleet_ReflectAnchorageRepairHealing.Checked;
		config.FormFleet.BlinkAtDamaged = FormFleet_BlinkAtDamaged.Checked;
		config.FormFleet.EmphasizesSubFleetInPort = FormFleet_EmphasizesSubFleetInPort.Checked;
		config.FormFleet.FleetStateDisplayMode = FormFleet_FleetStateDisplayMode.SelectedIndex;
		config.FormFleet.AppliesSallyAreaColor = FormFleet_AppliesSallyAreaColor.Checked;

		config.FormHeadquarters.BlinkAtMaximum = FormHeadquarters_BlinkAtMaximum.Checked;
		{
			var list = new List<bool>();
			for (int i = 0; i < FormHeadquarters_Visibility.Items.Count; i++)
				list.Add(FormHeadquarters_Visibility.GetItemChecked(i));
			config.FormHeadquarters.Visibility.List = list;
		}
		{
			string name = FormHeadquarters_DisplayUseItemID.Text;
			if (string.IsNullOrEmpty(name))
			{
				config.FormHeadquarters.DisplayUseItemID = -1;

			}
			else
			{
				var item = ElectronicObserver.Data.KCDatabase.Instance.MasterUseItems.Values.FirstOrDefault(p => p.Name == name);

				if (item != null)
				{
					config.FormHeadquarters.DisplayUseItemID = item.ItemID;

				}
				else
				{
					if (int.TryParse(name, out int val))
						config.FormHeadquarters.DisplayUseItemID = val;
					else
						config.FormHeadquarters.DisplayUseItemID = -1;
				}
			}
		}

		config.FormQuest.ShowRunningOnly = FormQuest_ShowRunningOnly.Checked;
		config.FormQuest.ShowOnce = FormQuest_ShowOnce.Checked;
		config.FormQuest.ShowDaily = FormQuest_ShowDaily.Checked;
		config.FormQuest.ShowWeekly = FormQuest_ShowWeekly.Checked;
		config.FormQuest.ShowMonthly = FormQuest_ShowMonthly.Checked;
		config.FormQuest.ShowOther = FormQuest_ShowOther.Checked;
		config.FormQuest.ProgressAutoSaving = FormQuest_ProgressAutoSaving.SelectedIndex;
		config.FormQuest.AllowUserToSortRows = FormQuest_AllowUserToSortRows.Checked;

		config.FormShipGroup.AutoUpdate = FormShipGroup_AutoUpdate.Checked;
		config.FormShipGroup.ShowStatusBar = FormShipGroup_ShowStatusBar.Checked;
		config.FormShipGroup.ShipNameSortMethod = FormShipGroup_ShipNameSortMethod.SelectedIndex;

		config.FormBattle.IsScrollable = FormBattle_IsScrollable.Checked;
		config.FormBattle.HideDuringBattle = FormBattle_HideDuringBattle.Checked;
		config.FormBattle.ShowHPBar = FormBattle_ShowHPBar.Checked;
		config.FormBattle.ShowShipTypeInHPBar = FormBattle_ShowShipTypeInHPBar.Checked;
		config.FormBattle.Display7thAsSingleLine = FormBattle_Display7thAsSingleLine.Checked;

		config.FormBrowser.IsEnabled = FormBrowser_IsEnabled.Checked;
		config.FormBrowser.ZoomRate = (double)FormBrowser_ZoomRate.Value / 100;
		config.FormBrowser.ZoomFit = FormBrowser_ZoomFit.Checked;
		config.FormBrowser.LogInPageURL = FormBrowser_LogInPageURL.Text;
		if (FormBrowser_ScreenShotFormat_JPEG.Checked)
			config.FormBrowser.ScreenShotFormat = 1;
		else
			config.FormBrowser.ScreenShotFormat = 2;
		config.FormBrowser.ScreenShotPath = FormBrowser_ScreenShotPath.Text;
		config.FormBrowser.ConfirmAtRefresh = FormBrowser_ConfirmAtRefresh.Checked;
		config.FormBrowser.AppliesStyleSheet = FormBrowser_AppliesStyleSheet.Checked;
		config.FormBrowser.IsDMMreloadDialogDestroyable = FormBrowser_IsDMMreloadDialogDestroyable.Checked;
		config.FormBrowser.AvoidTwitterDeterioration = FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Checked;
		config.FormBrowser.HardwareAccelerationEnabled = FormBrowser_HardwareAccelerationEnabled.Checked;
		config.FormBrowser.PreserveDrawingBuffer = FormBrowser_PreserveDrawingBuffer.Checked;
		config.FormBrowser.ForceColorProfile = FormBrowser_ForceColorProfile.Checked;
		config.FormBrowser.SavesBrowserLog = FormBrowser_SavesBrowserLog.Checked;
		if (FormBrowser_ToolMenuDockStyle.SelectedIndex == 4)
		{
			config.FormBrowser.IsToolMenuVisible = false;
		}
		else
		{
			config.FormBrowser.IsToolMenuVisible = true;
			config.FormBrowser.ToolMenuDockStyle = (DockStyle)(FormBrowser_ToolMenuDockStyle.SelectedIndex + 1);
		}
		config.FormBrowser.ScreenShotSaveMode = FormBrowser_ScreenShotSaveMode.SelectedIndex + 1;
		config.FormBrowser.UseGadgetRedirect = FormBrowser_UseGadgetRedirect.Checked;
		config.FormBrowser.IsBrowserContextMenuEnabled = FormBrowser_IsContextMenuEnabled.Checked;
		config.FormBrowser.UseVulkanWorkaround = FormBrowser_UseVulkanWorkaround.Checked;

		config.FormCompass.CandidateDisplayCount = (int)FormCompass_CandidateDisplayCount.Value;
		config.FormCompass.IsScrollable = FormCompass_IsScrollable.Checked;
		config.FormCompass.MaxShipNameWidth = (int)FormCompass_MaxShipNameWidth.Value;

		config.FormJson.AutoUpdate = FormJson_AutoUpdate.Checked;
		config.FormJson.UpdatesTree = FormJson_UpdatesTree.Checked;
		config.FormJson.AutoUpdateFilter = FormJson_AutoUpdateFilter.Text;

		config.FormBaseAirCorps.ShowEventMapOnly = FormBaseAirCorps_ShowEventMapOnly.Checked;


		//[通知]
		setSilencioConfig(Notification_Silencio.Checked);

		//[BGM]
		config.BGMPlayer.Enabled = BGMPlayer_Enabled.Checked;
		for (int i = 0; i < BGMPlayer_ControlGrid.Rows.Count; i++)
		{
			BGMHandles[(SyncBGMPlayer.SoundHandleID)BGMPlayer_ControlGrid[BGMPlayer_ColumnContent.Index, i].Value].Enabled = (bool)BGMPlayer_ControlGrid[BGMPlayer_ColumnEnabled.Index, i].Value;
		}
		config.BGMPlayer.Handles = new List<SyncBGMPlayer.SoundHandle>(BGMHandles.Values.ToList());
		config.BGMPlayer.SyncBrowserMute = BGMPlayer_SyncBrowserMute.Checked;
	}


	private void UpdateBGMPlayerUI()
	{

		BGMPlayer_ControlGrid.Rows.Clear();

		var rows = new DataGridViewRow[BGMHandles.Count];

		int i = 0;
		foreach (var h in BGMHandles.Values)
		{
			var row = new DataGridViewRow();
			row.CreateCells(BGMPlayer_ControlGrid);
			row.SetValues(h.Enabled, h.HandleID, h.Path);
			rows[i] = row;
			i++;
		}

		BGMPlayer_ControlGrid.Rows.AddRange(rows);

		BGMPlayer_VolumeAll.Value = (int)BGMHandles.Values.Average(h => h.Volume);
	}


	// BGMPlayer
	private void BGMPlayer_ControlGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
	{
		if (e.ColumnIndex == BGMPlayer_ColumnSetting.Index)
		{

			var handleID = (SyncBGMPlayer.SoundHandleID)BGMPlayer_ControlGrid[BGMPlayer_ColumnContent.Index, e.RowIndex].Value;

			using (var dialog = new DialogConfigurationBGMPlayer(BGMHandles[handleID]))
			{
				if (dialog.ShowDialog(App.Current.MainWindow) == System.Windows.Forms.DialogResult.OK)
				{
					BGMHandles[handleID] = dialog.ResultHandle;
				}
			}

			UpdateBGMPlayerUI();
		}
	}

	private void BGMPlayer_ControlGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{

		if (e.ColumnIndex == BGMPlayer_ColumnContent.Index)
		{
			e.Value = SyncBGMPlayer.SoundHandleIDToString((SyncBGMPlayer.SoundHandleID)e.Value);
			e.FormattingApplied = true;
		}

	}

	//for checkbox
	private void BGMPlayer_ControlGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
	{
		if (BGMPlayer_ControlGrid.Columns[BGMPlayer_ControlGrid.CurrentCellAddress.X] is DataGridViewCheckBoxColumn)
		{
			if (BGMPlayer_ControlGrid.IsCurrentCellDirty)
			{
				BGMPlayer_ControlGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}
	}

	private void BGMPlayer_SetVolumeAll_Click(object sender, EventArgs e)
	{

		if (MessageBox.Show(string.Format(Translation.PlayerVolumeWillBeSet + "\r\n", (int)BGMPlayer_VolumeAll.Value),
			Translation.ConfirmSetting,
			MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
		{

			foreach (var h in BGMHandles.Values)
			{
				h.Volume = (int)BGMPlayer_VolumeAll.Value;
			}

			UpdateBGMPlayerUI();
		}

	}


	private void setSilencioConfig(bool silenced)
	{
		foreach (NotifierBase no in NotifierManager.Instance.GetNotifiers())
		{
			no.IsSilenced = silenced;
		}
	}


	private void UpdatePlayTime()
	{
		double elapsed = (DateTime.Now - _shownTime).TotalSeconds;
		Log_PlayTime.Text = $"{Translation.Log_PlayTime}: {Utility.Mathematics.DateTimeHelper.ToTimeElapsedString(TimeSpan.FromSeconds(_playTimeCache + elapsed))}";
	}

	private void PlayTimeTimer_Tick(object sender, EventArgs e)
	{
		UpdatePlayTime();
	}

	private void FormFleet_FixShipNameWidth_CheckedChanged(object sender, EventArgs e)
	{
		FormFleet_FixedShipNameWidth.Enabled = FormFleet_FixShipNameWidth.Checked;
	}

	private void FormBrowser_ScreenShotFormat_PNG_CheckedChanged(object sender, EventArgs e)
	{
		FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Enabled = true;
	}

	private void FormBrowser_ScreenShotFormat_JPEG_CheckedChanged(object sender, EventArgs e)
	{
		FormBrowser_ScreenShotFormat_AvoidTwitterDeterioration.Enabled = false;
	}


	private void UI_MainFont_Validating(object sender, CancelEventArgs e)
	{

		var newfont = SerializableFont.StringToFont(UI_MainFont.Text, true);

		if (newfont != null)
		{
			UI_RenderingTest.MainFont = newfont;
			UI_MainFont.BackColor = SystemColors.Window;
		}
		else
		{
			UI_MainFont.BackColor = Color.MistyRose;
		}

	}

	private void UI_SubFont_Validating(object sender, CancelEventArgs e)
	{

		var newfont = SerializableFont.StringToFont(UI_SubFont.Text, true);

		if (newfont != null)
		{
			UI_RenderingTest.SubFont = newfont;
			UI_SubFont.BackColor = SystemColors.Window;
		}
		else
		{
			UI_SubFont.BackColor = Color.MistyRose;
		}
	}

	private void UI_BarColorMorphing_CheckedChanged(object sender, EventArgs e)
	{
		UI_RenderingTest.HPBar.ColorMorphing = UI_BarColorMorphing.Checked;
		UI_RenderingTest.Refresh();
	}

	private void UI_MainFont_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Enter)
		{
			e.SuppressKeyPress = true;
			e.Handled = true;
			UI_MainFont_Validating(sender, new CancelEventArgs());
		}
	}

	private void UI_MainFont_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if (e.KeyCode == Keys.Enter)
		{
			e.IsInputKey = true;        // AcceptButton の影響を回避する
		}
	}

	private void UI_SubFont_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Enter)
		{
			e.SuppressKeyPress = true;
			e.Handled = true;
			UI_SubFont_Validating(sender, new CancelEventArgs());
		}
	}


	private void UI_SubFont_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		if (e.KeyCode == Keys.Enter)
		{
			e.IsInputKey = true;        // AcceptButton の影響を回避する
		}
	}

	private void UI_RenderingTestChanger_Scroll(object sender, EventArgs e)
	{
		UI_RenderingTest.Value = UI_RenderingTestChanger.Value;
	}

	private void Control_EnableDiscordRPC_CheckStateChanged(object sender, EventArgs e)
	{
		Control_DiscordRPCMessage.ReadOnly = !Control_EnableDiscordRPC.Checked;
	}

	private void Control_ForceUpdate_Click(object sender, EventArgs e)
	{
		SoftwareUpdater.CheckUpdateAsync();
	}

	private void RefreshTsunDbParameters(object sender, FormClosedEventArgs e)
	{
		if (!(sender is DialogTsunDb dialog)) return;

		Control_EnableTsunDbSubmission.Checked = dialog.DialogResult == DialogResult.Yes;
	}

	private void Control_EnableTsunDbSubmission_CheckStateChanged(object sender, EventArgs e)
	{
		if (Control_EnableTsunDbSubmission.Checked == false) return;

		// --- ask for confirmation
		DialogTsunDb dialogTsunDb = new DialogTsunDb();
		dialogTsunDb.FormClosed += RefreshTsunDbParameters;
		dialogTsunDb.ShowDialog(App.Current.MainWindow);
	}
}
