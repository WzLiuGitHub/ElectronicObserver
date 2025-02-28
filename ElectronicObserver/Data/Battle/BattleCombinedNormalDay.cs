﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Data.Battle.Phase;

namespace ElectronicObserver.Data.Battle;

/// <summary>
/// 機動部隊 vs 通常艦隊 昼戦
/// </summary>
public class BattleCombinedNormalDay : BattleDay
{

	public override void LoadFromResponse(string apiname, dynamic data)
	{
		base.LoadFromResponse(apiname, (object)data);

		JetBaseAirAttack = new PhaseJetBaseAirAttack(this, "噴式基地航空隊攻撃");
		JetAirBattle = new PhaseJetAirBattle(this, "噴式航空戦");
		BaseAirAttack = new PhaseBaseAirAttack(this, "基地航空隊攻撃");
		FriendlySupportInfo = new PhaseFriendlySupportInfo(this, "友軍艦隊");
		FriendlyAirBattle = new PhaseFriendlyAirBattle(this, "友軍支援航空攻撃");
		AirBattle = new PhaseAirBattle(this, "航空戦");
		Support = new PhaseSupport(this, "支援攻撃");
		OpeningASW = new PhaseOpeningASW(this, "先制対潜");
		OpeningTorpedo = new PhaseTorpedo(this, "先制雷撃", 0);
		Shelling1 = new PhaseShelling(this, "第一次砲撃戦", 1, "1");
		Torpedo = new PhaseTorpedo(this, "雷撃戦", 2);
		Shelling2 = new PhaseShelling(this, "第二次砲撃戦", 3, "2");
		Shelling3 = new PhaseShelling(this, "第三次砲撃戦", 4, "3");

		foreach (var phase in GetPhases())
			phase.EmulateBattle(_resultHPs, _attackDamages);

	}


	public override string APIName => "api_req_combined_battle/battle";

	public override string BattleName => ConstantsRes.Title_CombinedNormalDay;



	public override IEnumerable<PhaseBase> GetPhases()
	{
		yield return Initial;
		yield return Searching;
		yield return JetBaseAirAttack;
		yield return JetAirBattle;
		yield return BaseAirAttack;
		yield return FriendlySupportInfo;
		yield return FriendlyAirBattle;
		yield return AirBattle;
		yield return Support;
		yield return OpeningASW;
		yield return OpeningTorpedo;
		yield return Shelling1;
		yield return Torpedo;
		yield return Shelling2;
		yield return Shelling3;
	}

}
