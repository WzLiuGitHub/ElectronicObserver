﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Data;
using ElectronicObserver.Observer;
using ElectronicObserver.Utility;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Utility.Storage;

namespace ElectronicObserver.Resource.Record;

/// <summary>
/// 資源のレコードを保持します。
/// </summary>
public class ResourceRecord : RecordBase
{

	public sealed class ResourceElement : RecordElementBase
	{

		/// <summary>
		/// 記録日時
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// 燃料
		/// </summary>
		public int Fuel { get; set; }

		/// <summary>
		/// 弾薬
		/// </summary>
		public int Ammo { get; set; }

		/// <summary>
		/// 鋼材
		/// </summary>
		public int Steel { get; set; }

		/// <summary>
		/// ボーキサイト
		/// </summary>
		public int Bauxite { get; set; }


		/// <summary>
		/// 高速建造材
		/// </summary>
		public int InstantConstruction { get; set; }

		/// <summary>
		/// 高速修復材
		/// </summary>
		public int InstantRepair { get; set; }

		/// <summary>
		/// 開発資材
		/// </summary>
		public int DevelopmentMaterial { get; set; }

		/// <summary>
		/// 改修資材
		/// </summary>
		public int ModdingMaterial { get; set; }

		/// <summary>
		/// 艦隊司令部Lv.
		/// </summary>
		public int HQLevel { get; set; }

		/// <summary>
		/// 提督経験値
		/// </summary>
		public int HQExp { get; set; }


		public ResourceElement()
		{
			Date = DateTime.Now;
		}

		public ResourceElement(string line)
			: this()
		{
			LoadLine(line);
		}

		public ResourceElement(int fuel, int ammo, int steel, int bauxite, int instantConstruction, int instantRepair, int developmentMaterial, int moddingMaterial, int hqLevel, int hqExp)
			: this()
		{
			Fuel = fuel;
			Ammo = ammo;
			Steel = steel;
			Bauxite = bauxite;
			InstantConstruction = instantConstruction;
			InstantRepair = instantRepair;
			DevelopmentMaterial = developmentMaterial;
			ModdingMaterial = moddingMaterial;
			HQLevel = hqLevel;
			HQExp = hqExp;
		}

		public override void LoadLine(string line)
		{

			string[] elem = CsvHelper.ParseCsvLine(line).ToArray();
			if (elem.Length < 11) throw new ArgumentException("要素数が少なすぎます。");

			Date = DateTimeHelper.CSVStringToTime(elem[0]);
			Fuel = int.Parse(elem[1]);
			Ammo = int.Parse(elem[2]);
			Steel = int.Parse(elem[3]);
			Bauxite = int.Parse(elem[4]);
			InstantConstruction = int.Parse(elem[5]);
			InstantRepair = int.Parse(elem[6]);
			DevelopmentMaterial = int.Parse(elem[7]);
			ModdingMaterial = int.Parse(elem[8]);
			HQLevel = int.Parse(elem[9]);
			HQExp = int.Parse(elem[10]);

		}

		public override string SaveLine()
		{
			return string.Join(",",
				DateTimeHelper.TimeToCSVString(Date),
				Fuel,
				Ammo,
				Steel,
				Bauxite,
				InstantConstruction,
				InstantRepair,
				DevelopmentMaterial,
				ModdingMaterial,
				HQLevel,
				HQExp);
		}

	}


	public List<ResourceElement> Record { get; private set; }
	private DateTime _prevTime;
	private bool _initialFlag;
	private int LastSavedCount;


	public ResourceRecord()
		: base()
	{

		Record = new List<ResourceElement>();
		_prevTime = DateTime.Now;
		_initialFlag = false;
	}

	public override void RegisterEvents()
	{
		var ao = APIObserver.Instance;

		ao["api_start2/getData"].ResponseReceived += ResourceRecord_Started;
		ao["api_port/port"].ResponseReceived += ResourceRecord_Updated;
	}


	private void ResourceRecord_Started(string apiname, dynamic data)
	{
		_initialFlag = true;
	}


	void ResourceRecord_Updated(string apiname, dynamic data)
	{

		if (_initialFlag || DateTimeHelper.IsCrossedHour(_prevTime))
		{
			_prevTime = DateTime.Now;
			_initialFlag = false;

			var material = KCDatabase.Instance.Material;
			var admiral = KCDatabase.Instance.Admiral;
			Record.Add(new ResourceElement(
				material.Fuel,
				material.Ammo,
				material.Steel,
				material.Bauxite,
				material.InstantConstruction,
				material.InstantRepair,
				material.DevelopmentMaterial,
				material.ModdingMaterial,
				admiral.Level,
				admiral.Exp));
		}
	}


	public ResourceElement this[int i]
	{
		get { return Record[i]; }
		set { Record[i] = value; }
	}


	/// <summary>
	/// 指定した日時以降の最も古い記録を返します。
	/// </summary>
	public ResourceElement GetRecord(DateTime target)
	{

		int i;
		for (i = Record.Count - 1; i >= 0; i--)
		{
			if (Record[i].Date < target)
			{
				i++;
				break;
			}
		}
		// Record内の全ての記録がtarget以降だった
		if (i < 0)
			i = 0;

		if (0 <= i && i < Record.Count)
		{
			return Record[i];
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// 前回の戦果更新以降の最も古い記録を返します。
	/// </summary>
	public ResourceElement GetRecordPrevious()
	{
		DateTime now = DateTime.Now;
		bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(now);

		now = DateTime.UtcNow + new TimeSpan(9, 0, 0);

		DateTime target;
		if (now.TimeOfDay.Hours < 2)
		{
			target = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0).Subtract(TimeSpan.FromDays(1));
		}
		else if (now.TimeOfDay.Hours < 14)
		{
			target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
		}
		else
		{
			target = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
		}

		if (isDst)
			target += new TimeSpan(1, 0, 0);

		return GetRecord(target.Add(DateTimeHelper.GetTimeDifference()));
	}

	/// <summary>
	/// 今日の戦果更新以降の最も古い記録を返します。
	/// </summary>
	public ResourceElement GetRecordDay()
	{
		DateTime now = DateTime.Now;
		bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(now);

		now = DateTime.UtcNow + new TimeSpan(9, 0, 0);

		DateTime target;
		if (now.TimeOfDay.Hours < 2)
		{
			target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0).Subtract(TimeSpan.FromDays(1));
		}
		else
		{
			target = new DateTime(now.Year, now.Month, now.Day, 2, 0, 0);
		}

		if (isDst)
			target += new TimeSpan(1, 0, 0);

		return GetRecord(target.Add(DateTimeHelper.GetTimeDifference()));
	}


	/// <summary>
	/// 今月の戦果更新以降の最も古い記録を返します。
	/// </summary>
	public ResourceElement GetRecordMonth()
	{
		DateTime now = DateTime.UtcNow + new TimeSpan(9, 0, 0);

		return GetRecord(new DateTime(now.Year, now.Month, 1).Add(DateTimeHelper.GetTimeDifference()));
	}




	protected override void LoadLine(string line)
	{
		Record.Add(new ResourceElement(line));
	}

	protected override string SaveLinesAll()
	{
		var sb = new StringBuilder();
		foreach (var elem in Record.OrderBy(r => r.Date))
		{
			sb.AppendLine(elem.SaveLine());
		}
		return sb.ToString();
	}

	protected override string SaveLinesPartial()
	{
		var sb = new StringBuilder();
		foreach (var elem in Record.Skip(LastSavedCount).OrderBy(r => r.Date))
		{
			sb.AppendLine(elem.SaveLine());
		}
		return sb.ToString();
	}

	protected override void UpdateLastSavedIndex()
	{
		LastSavedCount = Record.Count;
	}

	public override bool NeedToSave => LastSavedCount < Record.Count;

	public override bool SupportsPartialSave => true;

	protected override void ClearRecord()
	{
		Record.Clear();
		LastSavedCount = 0;
	}



	public override string RecordHeader => "日時,燃料,弾薬,鋼材,ボーキ,高速建造材,高速修復材,開発資材,改修資材,司令部Lv,提督Exp";

	public override string FileName => "ResourceRecord.csv";
}
