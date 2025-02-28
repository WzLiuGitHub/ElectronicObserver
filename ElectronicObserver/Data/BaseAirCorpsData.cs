﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data;

/// <summary>
/// 基地航空隊のデータを扱います。
/// </summary>
public class BaseAirCorpsData : APIWrapper, IIdentifiable
{


	/// <summary>
	/// 飛行場が存在する海域ID
	/// </summary>
	public int MapAreaID => RawData.api_area_id() ? (int)RawData.api_area_id : -1;


	/// <summary>
	/// 航空隊ID
	/// </summary>
	public int AirCorpsID => (int)RawData.api_rid;


	/// <summary>
	/// 航空隊名
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	/// 戦闘行動半径 base distance
	/// </summary>
	public int Distance { get; private set; }

	///<summary>
	///LBAS bonus distance
	///</summary>
	public int Bonus_Distance { get; private set; }

	///<summary>
	///LBAS base distance
	///</summary>
	public int Base_Distance { get; private set; }
	/// <summary>
	/// 行動指示
	/// 0=待機, 1=出撃, 2=防空, 3=退避, 4=休息
	/// </summary>
	public int ActionKind { get; private set; }


	/// <summary>
	/// 航空中隊情報
	/// </summary>
	public IDDictionary<BaseAirCorpsSquadron> Squadrons { get; private set; }

	public BaseAirCorpsSquadron this[int i] => Squadrons[i];




	public BaseAirCorpsData()
	{
		Squadrons = new IDDictionary<BaseAirCorpsSquadron>();
	}


	public override void LoadFromRequest(string apiname, Dictionary<string, string> data)
	{
		base.LoadFromRequest(apiname, data);

		switch (apiname)
		{
			case "api_req_air_corps/change_name":
				Name = data["api_name"];
				break;

			case "api_req_air_corps/set_action":
			{

				int[] ids = data["api_base_id"].Split(",".ToCharArray()).Select(s => int.Parse(s)).ToArray();
				int[] actions = data["api_action_kind"].Split(",".ToCharArray()).Select(s => int.Parse(s)).ToArray();

				int index = Array.IndexOf(ids, AirCorpsID);

				if (index >= 0)
				{
					ActionKind = actions[index];
				}

			}
			break;
		}
	}

	public override void LoadFromResponse(string apiname, dynamic data)
	{

		switch (apiname)
		{
			case "api_get_member/base_air_corps":
			default:
				base.LoadFromResponse(apiname, (object)data);

				Name = (string)data.api_name;
				Distance = (int)data.api_distance.api_base + (int)data.api_distance.api_bonus;
				ActionKind = (int)data.api_action_kind;
				Base_Distance = (int)data.api_distance.api_base;
				Bonus_Distance = (int)data.api_distance.api_bonus;
				SetSquadrons(apiname, data.api_plane_info);
				break;

			case "api_req_air_corps/change_deployment_base":
				Distance = (int)data.api_distance.api_base + (int)data.api_distance.api_bonus;
				Base_Distance = (int)data.api_distance.api_base;
				Bonus_Distance = (int)data.api_distance.api_bonus;
				SetSquadrons(apiname, data.api_plane_info);
				break;

			case "api_req_air_corps/set_plane":
			{
				var prev = Squadrons.Values.Select(sq => sq != null && sq.State == 1 ? sq.EquipmentMasterID : 0).ToArray();
				SetSquadrons(apiname, data.api_plane_info);

				foreach (var deleted in prev.Except(Squadrons.Values.Select(sq => sq != null && sq.State == 1 ? sq.EquipmentMasterID : 0)))
				{
					var eq = KCDatabase.Instance.Equipments[deleted];

					if (eq != null)
					{
						KCDatabase.Instance.RelocatedEquipments.Add(new RelocationData(deleted, DateTime.Now));
					}
				}

				Distance = (int)data.api_distance.api_base + (int)data.api_distance.api_bonus;
				Base_Distance = (int)data.api_distance.api_base;
				Bonus_Distance = (int)data.api_distance.api_bonus;
			}
			break;

			case "api_req_air_corps/supply":
				SetSquadrons(apiname, data.api_plane_info);
				break;
		}
	}

	private void SetSquadrons(string apiname, dynamic data)
	{

		foreach (var elem in data)
		{

			int id = (int)elem.api_squadron_id;

			if (!Squadrons.ContainsKey(id))
			{
				var a = new BaseAirCorpsSquadron();
				a.LoadFromResponse(apiname, elem);
				Squadrons.Add(a);

			}
			else
			{
				Squadrons[id].LoadFromResponse(apiname, elem);
			}
		}
	}



	public override string ToString() => $"[{MapAreaID}:{AirCorpsID}] {Name}";



	public int ID => GetID(RawData);


	public static int GetID(int mapAreaID, int airCorpsID) => mapAreaID * 10 + airCorpsID;

	public static int GetID(Dictionary<string, string> request)
		=> GetID(int.Parse(request["api_area_id"]), int.Parse(request["api_base_id"]));

	public static int GetID(dynamic response)
		=> GetID(response.api_area_id() ? (int)response.api_area_id : -1, (int)response.api_rid);


}
