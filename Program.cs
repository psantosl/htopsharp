using Terminal.Gui;
using System;
using Mono.Terminal;

using htopsharp;
using System.Collections.Generic;

class Demo {
	static void Main()
	{
		// Application.UseSystemConsole = true;
		Application.Init();

		var top = Application.Top;
		var tframe = top.Frame;

		top.ColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Black);

		View cpuArea = CreateCpuArea();

		View processArea = CreateProcessArea();

		int margin = 1;

		cpuArea.X = 0 + margin;
		cpuArea.Y = 0 + margin;
		cpuArea.Width = Dim.Percent(50) - margin;
		cpuArea.Height = Dim.Percent(30) - margin;

		processArea.X = 0 + margin;
		processArea.Y = Pos.Bottom(cpuArea) + margin;
		processArea.Width = Dim.Fill(margin);
		processArea.Height = Dim.Fill(margin);

		top.Add(cpuArea, processArea);

		var statusBar = new StatusBar(new StatusItem[] {
			new StatusItem(Key.Esc, "ESC Quit", () => { if (Quit ()) top.Running = false; }),
		});
		statusBar.ColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.Green);

		top.Add(statusBar);
		Application.Run();
	}

	static View CreateCpuArea()
	{
		var progressColorScheme = new ColorScheme();
		progressColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.BrightGreen, Color.Black);
		progressColorScheme.Focus = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray);

		var labelColorScheme = new ColorScheme();
		labelColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.BrighCyan, Color.Black);

		var cores = new List<HtopProgressBar>();

		FrameView frame = new FrameView("CPU");

		for (int i = 0; i < 8; ++i)
		{
			var labelCore = new Label(i + ":")
			{
				X = 0,
				Y = i,
				ColorScheme = labelColorScheme
			};

			var core = new HtopProgressBar()
			{
				X = Pos.Right(labelCore) + 1,
				Y = i,
				Height = 1,
				Width = Dim.Fill(),
				ColorScheme = progressColorScheme
			};

			cores.Add(core);

			frame.Add(labelCore, core);
		}

		Random rnd = new Random();

		bool timer(MainLoop caller)
		{
			foreach (var core in cores)
				core.Fraction = (float) rnd.NextDouble();

			return true;
		}

		Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(600), timer);

		return frame;
	}

	static View CreateProcessArea()
	{
		var text = new ReadOnlyTextView()
		{
			Width = Dim.Fill(),
			Height = Dim.Fill()
		};

		text.Text =
@"PID USER      PR  NI    VIRT    RES    SHR S  %CPU %MEM     TIME+ COMMAND
    1 root      20   0    8892    308    272 S   0.0  0.0   0:00.12 init
    5 root      20   0    8908    224    172 S   0.0  0.0   0:00.00 init
    6 pablo     20   0   16276   3792   3520 S   0.0  0.0   0:00.54 bash
   74 root      20   0    8908    224    172 S   0.0  0.0   0:00.01 init
   75 pablo     20   0   16260   3648   3544 S   0.0  0.0   0:00.11 bash
   92 pablo     20   0   14560   1544   1072 R   0.0  0.0   0:00.00 top";

		text.ReadOnly = true;

		FrameView frame = new FrameView("Process list");

		frame.Add(text);

		return frame;
	}

	static bool Quit ()
	{
		var n = MessageBox.Query (50, 7, "Quit", "Are you sure you want to quit htopsharp?", "Yes", "No");
		return n == 0;
	}

	static void Close ()
	{
		MessageBox.ErrorQuery (50, 5, "Error", "There is nothing to close", "Ok");
	}
}
