﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserver.Data.Battle.Phase;

namespace ElectronicObserver.Data.Battle;

/// <summary>
/// 昼戦の基底クラス
/// </summary>
public abstract class BattleDay : BattleData
{

	public PhaseJetBaseAirAttack JetBaseAirAttack { get; protected set; }
	public PhaseJetAirBattle JetAirBattle { get; protected set; }
	public PhaseBaseAirAttack BaseAirAttack { get; protected set; }
	public PhaseFriendlyAirBattle FriendlyAirBattle { get; protected set; }
	public PhaseAirBattle AirBattle { get; protected set; }
	public PhaseOpeningASW OpeningASW { get; protected set; }
	public PhaseTorpedo OpeningTorpedo { get; protected set; }
	public PhaseShelling Shelling1 { get; protected set; }
	public PhaseShelling Shelling2 { get; protected set; }
	public PhaseShelling Shelling3 { get; protected set; }
	public PhaseTorpedo Torpedo { get; protected set; }

}
