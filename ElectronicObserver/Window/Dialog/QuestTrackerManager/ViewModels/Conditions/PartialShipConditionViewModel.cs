﻿using System.Collections.Generic;
using System.Linq;
using ElectronicObserver.Window.Dialog.QuestTrackerManager.Models.Conditions;
using ElectronicObserverTypes;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace ElectronicObserver.Window.Dialog.QuestTrackerManager.ViewModels.Conditions;

public partial class PartialShipConditionViewModel : ObservableObject, IConditionViewModel
{
	public PartialShipConditionModel Model { get; }

	public IEnumerable<ShipConditionViewModel> Conditions { get; set; }

	public string Display => $"({Ships}) >= {Model.Count}";

	private string Ships => string.Join("+", Conditions.Select(c => c.Display));

	public PartialShipConditionViewModel(PartialShipConditionModel model)
	{
		Model = model;

		Conditions = CreateViewModels(Model);

		Model.PropertyChanged += (sender, args) =>
		{
			OnPropertyChanged(nameof(Display));
		};

		Model.Conditions.CollectionChanged += (_, e) =>
		{
			Conditions = CreateViewModels(Model);
		};
	}

	private IEnumerable<ShipConditionViewModel> CreateViewModels(PartialShipConditionModel model)
	{
		List<ShipConditionViewModel> conditions = model.Conditions
			.Select(s => new ShipConditionViewModel(s))
			.ToList();

		foreach (IConditionViewModel condition in conditions)
		{
			condition.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName is not nameof(IConditionViewModel.Display)) return;

				OnPropertyChanged(nameof(Display));
			};
		}

		return conditions;
	}

	[ICommand]
	private void AddCondition()
	{
		Model.Conditions.Add(new ShipConditionModel());
	}

	[ICommand]
	private void RemoveCondition(ShipConditionModel? condition)
	{
		if (condition is null) return;

		Model.Conditions.Remove(condition);
	}

	public bool ConditionMet(IFleetData fleet)
	{
		return Conditions.Count(c => c.ConditionMet(fleet)) >= Model.Count;
	}
}
