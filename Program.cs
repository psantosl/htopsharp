using Terminal.Gui;
using System;
using Mono.Terminal;

using htopsharp;

class Demo {
	static void Main()
	{
		// Application.UseSystemConsole = true;
		Application.Init();

		var top = Application.Top;
		var tframe = top.Frame;

		top.ColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Black);

		var menu = new MenuBar(new MenuBarItem[] {
			new MenuBarItem ("_File", new MenuItem [] {
				new MenuItem ("_Close", "", () => Close ()),
				new MenuItem ("_Quit", "", () => { if (Quit ()) top.Running = false; })
			})
		});

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

		top.Add(cpuArea, processArea, menu);

		Application.Run();
	}

	static View CreateCpuArea()
	{
		var progressColorScheme = new ColorScheme();
		progressColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.BrightGreen, Color.Black);
		progressColorScheme.Focus = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray);

		var labelColorScheme = new ColorScheme();
		labelColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.BrighCyan, Color.Black);

		FrameView frame = new FrameView("CPU");

		var labelCore0 = new Label("0:")
		{
			X = 0,
			Y = 0,
			ColorScheme = labelColorScheme
		};

		var core0 = new HtopProgressBar()
		{
			X = Pos.Right(labelCore0) + 1,
			Height = 1,
			Width = Dim.Fill()
		};

		var labelCore1 = new Label("1:")
		{
			X = 0,
			Y = 1,
			ColorScheme = labelColorScheme
		};

		var core1 = new HtopProgressBar()
		{
			X = Pos.Right(labelCore1) + 1,
			Height = 1,
			Y = 1,
			Width = Dim.Fill()
		};

		core0.ColorScheme = progressColorScheme;
		core1.ColorScheme = progressColorScheme;

		Random rnd = new Random();

		bool timer(MainLoop caller)
		{
			core0.Fraction = (float) rnd.NextDouble();
			core1.Fraction = (float) rnd.NextDouble();
			return true;
		}

		Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(600), timer);


		frame.Add(labelCore0, core0, labelCore1, core1);

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
		var n = MessageBox.Query (50, 7, "Quit Demo", "Are you sure you want to quit this demo?", "Yes", "No");
		return n == 0;
	}

	static void Close ()
	{
		MessageBox.ErrorQuery (50, 5, "Error", "There is nothing to close", "Ok");
	}
}
