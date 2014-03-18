﻿using System;
using Sparrow.Textures;
using Sparrow.Core;
using Sparrow.Samples.Desktop;
using Sparrow;

namespace SparrowSharp.Samples.Desktop
{
	class MainClass
	{
		[STAThread]
		public static void Main()
		{
			new DesktopViewController ((width, height) => SparrowSharpApp.Start(width, height, new Benchmark()));
		}
	}
}