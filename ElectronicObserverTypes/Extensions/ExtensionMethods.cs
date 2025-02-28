﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ElectronicObserverTypes.Extensions;

// I have no better name right now
public static class ExtensionMethods
{
	public static string Display(this Enum enumValue) =>
		enumValue.GetType()
			.GetMember(enumValue.ToString())
			.First()?
			.GetCustomAttribute<DisplayAttribute>()?
			.GetName() ?? enumValue.ToString();
}
