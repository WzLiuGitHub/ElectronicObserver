﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ElectronicObserver.Utility.Data;
using ElectronicObserverTypes;
using ElectronicObserverTypes.Mocks;
using Moq;
using Xunit;
using FleetDataCustom = ElectronicObserver.Data.FleetDataCustom;

namespace ElectronicObserverCoreTests;

[Collection(DatabaseCollection.Name)]
public class AswAttackTest
{
	private DatabaseFixture Db { get; }

	public AswAttackTest(DatabaseFixture db)
	{
		Db = db;
	}

	[Fact]
	public void DayAttackTest1()
	{
		IFleetData fleet = new FleetDataCustom();

		var mock = new Mock<IShipData>();

		mock.Setup(s => s.ASWBase).Returns(58);
		mock.Setup(s => s.HPRate).Returns(1);
		mock.Setup(s => s.AllSlotInstance).Returns(new ReadOnlyCollection<IEquipmentData?>(new List<IEquipmentData?>
		{
			Equipment.HFDF(),
			Equipment.AswTorpedo(),
			Equipment.Type2DepthCharge()
		}));

		IShipData akebono = mock.Object;

		Assert.Equal(104, akebono.GetAswAttackPower(DayAttackKind.DepthCharge, fleet));
	}

	[Fact]
	public void DayAttackTest1TestData()
	{
		IFleetData fleet = new FleetDataCustom();

		IShipData akebono = new ShipDataMock(Db.MasterShips[ShipId.AkebonoKai])
		{
			Level = 98,
			AllSlotInstance = new List<IEquipmentData>
			{
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.Sonar_HFDF_Type144147ASDIC]),
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.DepthCharge_LightweightASWTorpedo_InitialTestModel]),
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.DepthCharge_Type2DepthCharge]),
			}
		};

		Assert.Equal(104, akebono.GetAswAttackPower(DayAttackKind.DepthCharge, fleet));
	}
}
