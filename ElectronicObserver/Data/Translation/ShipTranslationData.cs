﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynaJson;
using ElectronicObserver.Utility;

namespace ElectronicObserver.Data.Translation;

public class ShipTranslationData : TranslationBase
{
	private string FilePath = TranslationManager.WorkingFolder + @"\ship.json";

	private Dictionary<string, string> ShipList;
	private Dictionary<string, string> TypeList;
	private Dictionary<string, string> SuffixList;

	private bool isShipLoaded => Configuration.Config.UI.JapaneseShipName == false && ShipList != null && SuffixList != null;
	private bool isTypeLoaded => Configuration.Config.UI.JapaneseShipType == false && TypeList != null;

	public string Name(string rawData)
	{
		if (isShipLoaded == false) return rawData;

		// save current ship name to prevent suffix replacements that can show up in names
		// tre suffix can be found in Intrepid which gets you In Trepid
		string currentShipName = "";
		foreach (var s in ShipList.OrderByDescending(s => s.Key.Length))
		{
			if (rawData.Equals(s.Key)) return s.Value;

			if (rawData.StartsWith(s.Key))
			{
				var pos = rawData.IndexOf(s.Key);
				rawData = rawData.Remove(pos, s.Key.Length).Insert(pos, s.Value);
				currentShipName = s.Key;
			}
		}

		var name = rawData; // prevent suffix from being replaced twice.
		foreach (var sf in SuffixList.OrderByDescending(sf => sf.Key.Length))
		{
			if (rawData.Contains(sf.Key))
			{
				var pos = rawData.IndexOf(sf.Key);

				if (pos < currentShipName.Length) continue;

				rawData = rawData.Remove(pos, sf.Key.Length).Insert(pos, new String('0', sf.Value.Length));
				name = name.Remove(pos, sf.Key.Length).Insert(pos, sf.Value);

				if (rawData.Substring(pos - 1, 1).Contains(" ") == false)
				{
					rawData = rawData.Insert(pos, " ");
					name = name.Insert(pos, " ");
				}
			}
		}
		return name;
	}

	public string TypeName(string rawData) => isTypeLoaded && TypeList.ContainsKey(rawData) ? TypeList[rawData] : rawData;
	public ShipTranslationData()
	{
		Initialize();
	}
	public override void Initialize()
	{
		ShipList = new Dictionary<string, string>();
		TypeList = new Dictionary<string, string>();
		SuffixList = new Dictionary<string, string>();
		LoadDictionary(FilePath);
	}

	public void LoadDictionary(string path)
	{
		var json = Load(path);
		if (json == null) return;

		foreach (KeyValuePair<string, object> category in json)
		{
			if (category.Key == "version") continue;

			var entries = JsonObject.Parse(category.Value.ToString());
			foreach (KeyValuePair<string, dynamic> entry in entries)
			{
				if (category.Key == "ship")
				{
					ShipList.Add(entry.Key, entries[entry.Key]);
				}
				if (category.Key == "stype")
				{
					TypeList.Add(entry.Key, entries[entry.Key]);
				}
				if (category.Key == "suffix")
				{
					SuffixList.Add(entry.Key, entries[entry.Key]);
				}
			}
		}
	}
}
