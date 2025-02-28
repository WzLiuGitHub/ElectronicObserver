﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Browser.ExtraBrowser.Converter;

public class TitleConverter : IValueConverter
{
	object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return "WPF Webview2 Example - " + (value ?? "No Title Specified");
	}

	object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return Binding.DoNothing;
	}
}
