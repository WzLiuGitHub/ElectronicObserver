﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Storage;
using ElectronicObserver.ViewModels;
using MessagePack;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window.Integrate;

/// <summary>
/// 取り込むウィンドウのベースとなるフォーム
/// </summary>
[INotifyPropertyChanged]
public partial class FormIntegrate : DockContent
{

	public readonly static String PREFIX = "FormIntegrated_";

	public static string Prefix => "Integrate_";

	public string PersistString => MakePersistString();

	private string MakePersistString()
	{
		using SHA256 sha256 = SHA256.Create();

		byte[] bytes = Encoding.UTF8.GetBytes(titleTextBox.Text + classNameTextBox.Text + fileNameTextBox.Text);
		string hash = BitConverter.ToString(sha256.ComputeHash(bytes));

		return Prefix + hash;
	}

	[DataContract(Name = "MatchControl")]
	public enum MatchControl
	{
		[EnumMember]
		Exact = 0,
		[EnumMember]
		Contains,
		[EnumMember]
		StartEnd,
		[EnumMember]
		Ignore
	}

	[MessagePackObject(true)]
	[DataContract(Name = "MatchString")]
	public class MatchString
	{

		[DataMember]
		public String Name { get; set; }

		[DataMember]
		public MatchControl MatchControl { get; set; }

		// needed for MessagePack, it doesn't recognize the other constructor for some reason
		public MatchString()
		{

		}

		public MatchString(String name, MatchControl match)
		{
			Name = name;
			MatchControl = match;
		}

		public bool Match(String name)
		{
			switch (MatchControl)
			{
				case FormIntegrate.MatchControl.Exact:
					return (name == Name);
				case FormIntegrate.MatchControl.Contains:
					return name.Contains(Name);
				case FormIntegrate.MatchControl.StartEnd:
					return name.StartsWith(Name) || name.EndsWith(Name);
				case FormIntegrate.MatchControl.Ignore:
					return true;
			}
			throw new NotImplementedException("サポートされていないMatchControl");
		}
	}

	[MessagePackObject(true)]
	[DataContract(Name = "WindowInfo")]
	public sealed class WindowInfo : DataStorage
	{

		[DataMember]
		public String CurrentTitle { get; set; }

		[DataMember]
		public MatchString Title { get; set; }

		[DataMember]
		public MatchString ClassName { get; set; }

		[DataMember]
		public MatchString ProcessFilePath { get; set; }


		public WindowInfo()
		{
			Initialize();
		}

		public WindowInfo(string currentTitle, MatchString title, MatchString className,
			MatchString processFilePath)
		{
			Initialize();

			CurrentTitle = currentTitle;
			Title = title;
			ClassName = className;
			ProcessFilePath = processFilePath;
		}

		public override void Initialize()
		{
		}

		public bool Match(String title, String className, String filePath)
		{
			return Title.Match(title) &&
				   ClassName.Match(className) &&
				   ProcessFilePath.Match(filePath);
		}
	}

	private static String[] MATCH_COMBO_ITEMS = new String[] {
		"完全一致",
		"含む",
		"前方後方一致",
		"条件を無視"
	};

	private object parent;

	/// <summary>
	/// 次のウィンドウキャプチャ時に必要な情報
	/// </summary>
	public WindowInfo WindowData
	{
		get
		{
			WindowInfo info = new WindowInfo
			{
				CurrentTitle = Text,
				Title = new MatchString(titleTextBox.Text,
					(MatchControl)titleComboBox.SelectedIndex),
				ClassName = new MatchString(classNameTextBox.Text,
					(MatchControl)classNameComboBox.SelectedIndex),
				ProcessFilePath = new MatchString(fileNameTextBox.Text,
					(MatchControl)fileNameComboBox.SelectedIndex)
			};

			return info;
		}
		set
		{
			Text = value.CurrentTitle;
			//TabText = value.CurrentTitle.Length > 16 ? value.CurrentTitle.Substring( 0, 16 ) + "..." : value.CurrentTitle;
			titleTextBox.Text = value.Title.Name;
			titleComboBox.SelectedIndex = (int)value.Title.MatchControl;
			classNameTextBox.Text = value.ClassName.Name;
			classNameComboBox.SelectedIndex = (int)value.ClassName.MatchControl;
			fileNameTextBox.Text = value.ProcessFilePath.Name;
			fileNameComboBox.SelectedIndex = (int)value.ProcessFilePath.MatchControl;

			OnPropertyChanged(nameof(Text));
		}
	}

	private IntPtr attachingWindow;

	// 戻すときに必要になる情報
	private uint origStyle;
	private IntPtr origOwner;
	private WinAPI.RECT origWindowRect;
	private IntPtr origMenu;

	public FormIntegrate(object parent, WindowInfo? info = null)
	{
		InitializeComponent();

		this.parent = parent;

		windowCaptureButton.Image = ResourceManager.Instance.Icons.Images[(int)IconContent.FormWindowCapture];

		titleComboBox.Items.AddRange(MATCH_COMBO_ITEMS);
		classNameComboBox.Items.AddRange(MATCH_COMBO_ITEMS);
		fileNameComboBox.Items.AddRange(MATCH_COMBO_ITEMS);

		TabPageContextMenuStrip = tabContextMenu;

		Utility.Configuration.Instance.ConfigurationChanged += ConfigurationChanged;
		ConfigurationChanged();

		switch (parent)
		{
			case FormMain main:
				main.WindowCapture.AddCapturedWindow(this);
				break;

			case FormMainViewModel { WindowCapture: { WinformsControl: FormWindowCapture fwc } }:
				if (info is not null)
				{
					WindowData = info;
				}
				fwc.AddCapturedWindow(this);
				break;
		}
	}

	void ConfigurationChanged()
	{
		Font = Utility.Configuration.Config.UI.MainFont;
	}

	/// <summary>
	/// PersistStringから復元
	/// </summary>
	public static FormIntegrate FromPersistString(FormMain parent, String str)
	{
		WindowInfo info = new WindowInfo();
		info = (WindowInfo)info.Load(new StringReader(str.Substring(PREFIX.Length)));
		FormIntegrate form = new FormIntegrate(parent)
		{
			WindowData = info
		};
		return form;
	}

	private static string GetMainModuleFilepath(int processId)
	{
		// System.Diagnostics.Processからだと64bit/32bitの壁を超えられないのでWMIで取得
		string wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE ProcessId = " + processId;
		using (var searcher = new ManagementObjectSearcher(wmiQueryString))
		{
			using (var results = searcher.Get())
			{
				foreach (ManagementObject mo in results)
				{
					return (string)mo["ExecutablePath"];
				}
			}
		}
		return null;
	}

	/// <summary>
	/// WindowDataにマッチするウィンドウを探す
	/// </summary>
	private IntPtr FindWindow()
	{
		StringBuilder className = new StringBuilder(256);
		StringBuilder windowText = new StringBuilder(256);
		IntPtr result = IntPtr.Zero;
		int currentProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
		WindowInfo info = WindowData;

		WinAPI.EnumWindows((WinAPI.EnumWindowsDelegate)((hWnd, lparam) =>
		{
			WinAPI.GetClassName(hWnd, className, className.Capacity);
			WinAPI.GetWindowText(hWnd, windowText, windowText.Capacity);
			WinAPI.GetWindowThreadProcessId(hWnd, out uint processId);
			if (info.ClassName.Match(className.ToString()) &&
				info.Title.Match(windowText.ToString()) &&
				WinAPI.IsWindowVisible(hWnd) &&
				processId != currentProcessId)
			{
				String fileName = GetMainModuleFilepath((int)processId);
				if (info.ProcessFilePath.Match(fileName))
				{
					result = hWnd;
					return false;
				}
			}
			return true;
		}), IntPtr.Zero);

		return result;
	}

	/// <summary>
	/// ウィンドウを取り込む
	/// </summary>
	public bool Grab()
	{
		if (attachingWindow != IntPtr.Zero)
		{
			// 既にアタッチ済み
			return true;
		}
		IntPtr hWnd = FindWindow();
		if (hWnd != IntPtr.Zero)
		{
			Attach(hWnd, false);
			return true;
		}
		infoLabel.Text = "ウィンドウが見つかりませんでした";
		return false;
	}

	private static WindowInfo WindowInfoFromHandle(IntPtr hWnd)
	{
		WindowInfo info = new WindowInfo();
		StringBuilder sb = new StringBuilder(256);

		WinAPI.GetClassName(hWnd, sb, sb.Capacity);
		info.ClassName = new MatchString(sb.ToString(), MatchControl.Exact);

		WinAPI.GetWindowText(hWnd, sb, sb.Capacity);
		info.Title = new MatchString(sb.ToString(), MatchControl.Exact);

		WinAPI.GetWindowThreadProcessId(hWnd, out uint processId);
		String fileName = GetMainModuleFilepath((int)processId);
		info.ProcessFilePath = new MatchString(fileName, MatchControl.Exact);

		info.CurrentTitle = info.Title.Name;

		return info;
	}

	private void Attach(IntPtr hWnd, bool showFloating)
	{

		if (attachingWindow != IntPtr.Zero)
		{
			if (attachingWindow == hWnd)
			{
				// 既にアタッチ済み
				return;
			}
			Detach();
		}
		else
		{
			settingPanel.Visible = false;
			StripMenu_Detach.Enabled = true;
		}

		origStyle = unchecked((uint)(long)WinAPI.GetWindowLong(hWnd, WinAPI.GWL_STYLE));
		origOwner = WinAPI.GetWindowLong(hWnd, WinAPI.GWL_HWNDPARENT);

		// ターゲットが最大化されていたら戻す
		if ((origStyle & WinAPI.WS_MAXIMIZE) != 0)
		{
			origStyle &= unchecked((uint)~WinAPI.WS_MAXIMIZE);
			WinAPI.SetWindowLong(hWnd, WinAPI.GWL_STYLE, new IntPtr(unchecked((int)origStyle)));
		}

		// キャプションを設定
		StringBuilder stringBuilder = new StringBuilder(32);
		WinAPI.GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);
		Text = stringBuilder.ToString();

		// アイコンを設定
		IntPtr hicon = WinAPI.SendMessage(hWnd, WinAPI.WM_GETICON, (IntPtr)WinAPI.ICON_SMALL, IntPtr.Zero);
		if (hicon == IntPtr.Zero)
		{
			hicon = WinAPI.GeClassLong(hWnd, WinAPI.GCLP_HICON);
		}
		if (hicon != IntPtr.Zero)
		{
			this.Icon = Icon.FromHandle(hicon);
		}

		// メニューを取得
		origMenu = WinAPI.GetMenu(hWnd);

		WinAPI.GetWindowRect(hWnd, out origWindowRect);

		if (showFloating)
		{
			// このウィンドウの大きさ・位置を設定
			/*
			 Show(parent.MainPanel, new Rectangle(
				origWindowRect.left,
				origWindowRect.top,
				origWindowRect.right - origWindowRect.left,
				origWindowRect.bottom - origWindowRect.top));
			*/
		}

		// ターゲットを子ウィンドウに設定
		uint newStyle = origStyle;
		newStyle &= unchecked((uint)~(WinAPI.WS_POPUP |
									  WinAPI.WS_CAPTION |
									  WinAPI.WS_BORDER |
									  WinAPI.WS_THICKFRAME |
									  WinAPI.WS_MINIMIZEBOX |
									  WinAPI.WS_MAXIMIZEBOX));
		newStyle |= WinAPI.WS_CHILD;
		WinAPI.SetWindowLong(hWnd, WinAPI.GWL_STYLE, new IntPtr(unchecked((int)newStyle)));
		WinAPI.SetParent(hWnd, this.Handle);
		WinAPI.MoveWindow(hWnd, 0, 0, this.Width, this.Height, true);

		this.attachingWindow = hWnd;

		OnPropertyChanged(nameof(Icon));
	}

	private void InternalDetach()
	{
		if (attachingWindow != IntPtr.Zero)
		{
			WinAPI.SetParent(attachingWindow, IntPtr.Zero);
			WinAPI.SetWindowLong(attachingWindow, WinAPI.GWL_STYLE, new IntPtr(unchecked((int)origStyle)));
			WinAPI.SetWindowLong(attachingWindow, WinAPI.GWL_HWNDPARENT, origOwner);
			WinAPI.SetMenu(attachingWindow, origMenu);
			WinAPI.MoveWindow(attachingWindow, origWindowRect.left, origWindowRect.top,
				origWindowRect.right - origWindowRect.left,
				origWindowRect.bottom - origWindowRect.top, true);
			attachingWindow = IntPtr.Zero;

		}
	}

	/// <summary>
	/// ウィンドウを開放する
	/// </summary>
	public void Detach()
	{
		if (attachingWindow != IntPtr.Zero)
		{
			InternalDetach();
			settingPanel.Visible = true;
			StripMenu_Detach.Enabled = false;
			infoLabel.Text = "ウィンドウを開放しました";
		}
	}

	/// <summary>
	/// ウィンドウを元の場所を維持しながら取り込む
	/// </summary>
	public void Show(IntPtr hWnd)
	{
		Attach(hWnd, true);
		WindowData = WindowInfoFromHandle(hWnd);
	}

	private void FormIntegrated_FormClosing(object sender, FormClosingEventArgs e)
	{
		InternalDetach();
		Utility.Configuration.Instance.ConfigurationChanged -= ConfigurationChanged;
	}

	private void FormIntegrated_Resize(object sender, EventArgs e)
	{
		if (attachingWindow != IntPtr.Zero)
		{
			Size size = ClientSize;
			WinAPI.MoveWindow(attachingWindow, 0, 0, size.Width, size.Height, true);
		}
	}

	protected override string GetPersistString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		WindowData.Save(stringBuilder);
		return PREFIX + stringBuilder.ToString();
	}

	private void integrateButton_Click(object sender, EventArgs e)
	{
		Grab();
	}

	private void windowCaptureButton_WindowCaptured(IntPtr hWnd)
	{

		int capacity = WinAPI.GetWindowTextLength(hWnd) * 2;
		StringBuilder stringBuilder = new StringBuilder(capacity);
		WinAPI.GetWindowText(hWnd, stringBuilder, stringBuilder.Capacity);

		if (MessageBox.Show(stringBuilder.ToString() + "\r\n" + FormWindowCapture.WarningMessage,
				"ウィンドウキャプチャの確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
			== System.Windows.Forms.DialogResult.Yes)
		{

			Attach(hWnd, false);
			WindowData = WindowInfoFromHandle(hWnd);
		}
	}

	private void StripMenu_Detach_Click(object sender, EventArgs e)
	{
		Detach();
	}


}
