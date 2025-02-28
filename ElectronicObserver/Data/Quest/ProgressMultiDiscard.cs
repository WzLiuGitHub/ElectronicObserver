﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Data.Quest;

[DataContract(Name = "ProgressMultiDiscard")]
public class ProgressMultiDiscard : ProgressData
{

	[DataMember]
	private ProgressDiscard[] ProgressList;

	public ProgressMultiDiscard(QuestData quest, IEnumerable<ProgressDiscard> progressList)
		: base(quest, 1)
	{
		ProgressList = progressList.ToArray();
		foreach (var p in ProgressList)
			p.IgnoreCheckProgress = true;

		ProgressMax = progressList.Sum(p => p.ProgressMax);
	}


	public void Increment(IEnumerable<int> equipments)
	{
		foreach (var p in ProgressList)
			p.Increment(equipments);

		Progress = ProgressList.Sum(p => p.Progress);
	}


	public override void Increment()
	{
		throw new NotSupportedException();
	}

	public override void Decrement()
	{
		throw new NotSupportedException();
	}


	public override void CheckProgress(QuestData q)
	{
		foreach (var p in ProgressList)
			p.ApplyTemporaryProgress(q);

		Progress = ProgressList.Sum(p => p.Progress);
	}


	public override string ToString()
	{
		if (ProgressList.All(p => p.IsCleared))
			return "100%";
		else
			return string.Join(" \n", ProgressList.Where(p => !p.IsCleared).Select(p => p.ToString() + " " + p.GetClearCondition()));
	}


	public override string GetClearCondition()
	{
		return string.Join("\n", ProgressList.Select(p => p.GetClearCondition()));
	}
}
