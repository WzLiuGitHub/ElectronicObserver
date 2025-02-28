﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Data.Battle.Phase;

namespace ElectronicObserver.Data.Battle;

/// <summary>
/// 通常艦隊 vs 通常艦隊 夜戦
/// </summary>
public class BattleNormalNight : BattleNight
{

	public override void LoadFromResponse(string apiname, dynamic data)
	{
		base.LoadFromResponse(apiname, (object)data);

		NightInitial = new PhaseNightInitial(this, "夜戦開始", false);
		FriendlySupportInfo = new PhaseFriendlySupportInfo(this, "友軍艦隊");
		FriendlyShelling = new PhaseFriendlyShelling(this, "友軍艦隊援護");
		// 支援なし?
		NightBattle = new PhaseNightBattle(this, "夜戦", 0);

		foreach (var phase in GetPhases())
			phase.EmulateBattle(_resultHPs, _attackDamages);
	}


	public override string APIName => "api_req_battle_midnight/battle";

	public override string BattleName => ConstantsRes.Title_NormalNight;



	public override IEnumerable<PhaseBase> GetPhases()
	{
		yield return Initial;
		yield return NightInitial;
		yield return FriendlySupportInfo;
		yield return FriendlyShelling;
		yield return NightBattle;
	}
}
