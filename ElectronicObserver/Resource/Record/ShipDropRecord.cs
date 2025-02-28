﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.Utility.Storage;

namespace ElectronicObserver.Resource.Record;

[DebuggerDisplay("{Record.Count} Records")]
public class ShipDropRecord : RecordBase
{

	[DebuggerDisplay("[{Date}] : {ShipName} / {ItemName}")]
	public sealed class ShipDropElement : RecordElementBase
	{

		/// <summary>
		/// ドロップした艦のID　-1=なし
		/// </summary>
		public int ShipID { get; set; }

		/// <summary>
		/// ドロップした艦の名前
		/// </summary>
		public string ShipName { get; set; }

		/// <summary>
		/// ドロップしたアイテムのID　-1=なし
		/// </summary>
		public int ItemID { get; set; }

		/// <summary>
		/// ドロップしたアイテムの名前
		/// </summary>
		public string ItemName { get; set; }

		/// <summary>
		/// ドロップした装備のID　-1=なし
		/// </summary>
		public int EquipmentID { get; set; }

		/// <summary>
		/// ドロップした装備の名前
		/// </summary>
		public string EquipmentName { get; set; }

		/// <summary>
		/// ドロップした日時
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// 海域カテゴリID
		/// </summary>
		public int MapAreaID { get; set; }

		/// <summary>
		/// 海域カテゴリ内番号
		/// </summary>
		public int MapInfoID { get; set; }

		/// <summary>
		/// 海域セルID
		/// </summary>
		public int CellID { get; set; }

		/// <summary>
		/// 難易度(甲乙丙)
		/// </summary>
		public int Difficulty { get; set; }

		/// <summary>
		/// ボスかどうか
		/// </summary>
		public bool IsBossNode { get; set; }

		/// <summary>
		/// 敵編成ID
		/// </summary>
		public ulong EnemyFleetID { get; set; }

		/// <summary>
		/// 勝利ランク
		/// </summary>
		public string Rank { get; set; }

		/// <summary>
		/// 司令部Lv.
		/// </summary>
		public int HQLevel { get; set; }


		public ShipDropElement()
		{
			ShipID = -1;
			Date = DateTime.Now;
		}

		public ShipDropElement(string line)
			: this()
		{
			LoadLine(line);
		}

		public ShipDropElement(int shipID, int itemID, int equipmentID, int mapAreaID, int mapInfoID, int cellID, int difficulty, bool isBossNode, ulong enemyFleetID, string rank, int hqLevel)
		{
			ShipID = shipID;
			if (shipID == -1)
				ShipName = "(なし)";
			else if (shipID == -2)
				ShipName = "(満員)";
			else
			{
				var ship = KCDatabase.Instance.MasterShips[shipID];
				if (ship != null)
					ShipName = ship.NameWithClass;
				else
					ShipName = "???";
			}

			ItemID = itemID;
			if (itemID == -1)
				ItemName = "(なし)";
			else
			{
				var item = KCDatabase.Instance.MasterUseItems[itemID];
				if (item != null)
					ItemName = item.Name;
				else
					ItemName = "???";
			}

			EquipmentID = equipmentID;
			if (equipmentID == -1)
				EquipmentName = "(なし)";
			else
			{
				var eq = KCDatabase.Instance.MasterEquipments[equipmentID];
				if (eq != null)
					EquipmentName = eq.NameEN;
				else
					EquipmentName = "???";
			}

			Date = DateTime.Now;
			MapAreaID = mapAreaID;
			MapInfoID = mapInfoID;
			CellID = cellID;
			Difficulty = difficulty;
			IsBossNode = isBossNode;
			EnemyFleetID = enemyFleetID;
			Rank = rank;
			HQLevel = hqLevel;
		}


		public override void LoadLine(string line)
		{

			string[] elem = CsvHelper.ParseCsvLine(line).ToArray();
			if (elem.Length < 15) throw new ArgumentException("要素数が少なすぎます。");

			ShipID = int.Parse(elem[0]);
			ShipName = elem[1];
			ItemID = int.Parse(elem[2]);
			ItemName = elem[3];
			EquipmentID = int.Parse(elem[4]);
			EquipmentName = elem[5];
			Date = DateTimeHelper.CSVStringToTime(elem[6]);
			MapAreaID = int.Parse(elem[7]);
			MapInfoID = int.Parse(elem[8]);
			CellID = int.Parse(elem[9]);
			Difficulty = Constants.GetDifficulty(elem[10]);
			IsBossNode = string.Compare(elem[11], "ボス") == 0;
			EnemyFleetID = Convert.ToUInt64(elem[12], 16);
			Rank = elem[13];
			HQLevel = int.Parse(elem[14]);

		}

		public override string SaveLine()
		{

			return string.Join(",",
				ShipID,
				CsvHelper.EscapeCsvCell(ShipName),
				ItemID,
				CsvHelper.EscapeCsvCell(ItemName),
				EquipmentID,
				CsvHelper.EscapeCsvCell(EquipmentName),
				DateTimeHelper.TimeToCSVString(Date),
				MapAreaID,
				MapInfoID,
				CellID,
				Constants.GetDifficulty(Difficulty),
				IsBossNode ? "ボス" : "-",
				EnemyFleetID.ToString("x16"),
				Rank,
				HQLevel);

		}
	}



	public List<ShipDropElement> Record { get; private set; }
	private int LastSavedCount;


	public ShipDropRecord()
		: base()
	{
		Record = new List<ShipDropElement>();
	}

	public override void RegisterEvents()
	{
		// nop
	}


	public ShipDropElement this[int i]
	{
		get { return Record[i]; }
		set { Record[i] = value; }
	}

	public void Add(int shipID, int itemID, int equipmentID, int mapAreaID, int mapInfoID, int cellID, int difficulty, bool isBossNode, ulong enemyFleetID, string rank, int hqLevel)
	{

		Record.Add(new ShipDropElement(shipID, itemID, equipmentID, mapAreaID, mapInfoID, cellID, difficulty, isBossNode, enemyFleetID, rank, hqLevel));
	}


	protected override void LoadLine(string line)
	{
		Record.Add(new ShipDropElement(line));
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


	public override string RecordHeader => "艦船ID,艦名,アイテムID,アイテム名,装備ID,装備名,入手日時,海域,海域,セル,難易度,ボス,敵編成ID,ランク,司令部Lv";

	public override string FileName => "ShipDropRecord.csv";
}
