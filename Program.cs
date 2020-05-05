using Terminal.Gui;
using System;
using Mono.Terminal;

using htopsharp;

class Demo {
	static void Main()
	{
		//Application.UseSystemConsole = true;
		Application.Init();

		var top = Application.Top;
		var tframe = top.Frame;

		var win = new Window("HtopSharp")
		{
			X = 0,
			Y = 1,
			Width = Dim.Fill(),
			Height = Dim.Fill() - 2
		};

		win.ColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Black);

		var menu = new MenuBar(new MenuBarItem[] {
			new MenuBarItem ("_File", new MenuItem [] {
				new MenuItem ("_New", "Creates new file", NewFile),
				new MenuItem ("Text _Editor Demo", "", () => { CreateEditor (top); }),
				new MenuItem ("_Close", "", () => Close ()),
				new MenuItem ("_Quit", "", () => { if (Quit ()) top.Running = false; })
			})
		});

		ShowEntries(win);

		top.Add(win, menu);
		top.Add(menu);
		Application.Run();
	}

	static void ShowEntries(View container)
	{
		var scrollView = new ScrollView(new Rect(50, 10, 20, 8))
		{
			ContentSize = new Size(100, 100),
			ContentOffset = new Point(-1, -1),
			ShowVerticalScrollIndicator = true,
			ShowHorizontalScrollIndicator = true
		};

		scrollView.Add(new Box10x(0, 0));
		//scrollView.Add (new Filler (new Rect (0, 0, 40, 40)));

		var progress = new HtopProgressBar(new Rect(68, 1, 10, 1));

		var cc = new ColorScheme();
		cc.Normal = Terminal.Gui.Attribute.Make(Color.Red, Color.Black);
		cc.Focus = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray);

		progress.ColorScheme = cc;

		float fraction = 0;

		bool timer(MainLoop caller)
		{
			fraction += 0.05f;

			if (fraction > 1)
				fraction = 0;

			progress.Fraction = fraction;
			return true;
		}

		Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(300), timer);

		// A little convoluted, this is because I am using this to test the
		// layout based on referencing elements of another view:

		var login = new Label("Login: ") { X = 3, Y = 6 };
		var password = new Label("Password: ")
		{
			X = Pos.Left(login),
			Y = Pos.Bottom(login) + 1
		};
		var loginText = new TextField("")
		{
			X = Pos.Right(password),
			Y = Pos.Top(login),
			Width = 40
		};
		var passText = new TextField("")
		{
			Secret = true,
			X = Pos.Left(loginText),
			Y = Pos.Top(password),
			Width = Dim.Width(loginText)
		};
		var c = new ColorScheme();
		c.Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Gray);
		c.Focus = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray);

		loginText.ColorScheme = c;

		// This is just to debug the visuals of the scrollview when small
		var scrollView2 = new ScrollView(new Rect(0, 0, 50, 4))
		{
			ContentSize = new Size(120, 10),
			ShowVerticalScrollIndicator = true,
			ShowHorizontalScrollIndicator = true
		};

		var listView = new ListView(new string[] {
"Name                                      Request IP              Time      Thread   Status   Protocol  Sec  User",
"Unknown-plasticproto                           22 127.0.0.1       00:00:00  26       Proto    Plastic   None",
"CalculateMerge                                 21 127.0.0.1       00:00:00  24       Read     Plastic   None pablo"
			});

		listView.Width = Dim.Fill();
		listView.Height = Dim.Fill();

		scrollView2.Add(listView);

		// Add some content
		container.Add(
			login,
			loginText,
			password,
			passText,
			new FrameView(new Rect(3, 10, 25, 6), "Options"){
				new CheckBox (1, 0, "Remember me"),
				new RadioGroup (1, 2, new [] { "_Personal", "_Company" }),
			},
			//scrollView,
			scrollView2,
			new Button("Ok") { X = 3, Y = 19 },
			new Button("Cancel") { X = 10, Y = 19 },
			progress,
			new Label("Press F9 (on Unix ESC+9 is an alias) to activate the menubar") { X = 3, Y = 22 }
		);

	}

	class Box10x : View {
		public Box10x (int x, int y) : base (new Rect (x, y, 10, 10))
		{
		}

		public override void Redraw (Rect region)
		{
			Driver.SetAttribute (ColorScheme.Focus);

			for (int y = 0; y < 10; y++) {
				Move (0, y);
				for (int x = 0; x < 10; x++) {

					Driver.AddRune ((Rune)('0' + (x + y) % 10));
				}
			}

		}
	}

	public static Label ml2;

	static void NewFile ()
	{
		var d = new Dialog (
			"New File", 50, 20,
			new Button ("Ok", is_default: true) { Clicked = () => { Application.RequestStop (); } },
			new Button ("Cancel") { Clicked = () => { Application.RequestStop (); } });
		ml2 = new Label (1, 1, "Mouse Debug Line");
		d.Add (ml2);
		Application.Run (d);
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

	public static Label ml;

	static void CreateEditor(Toplevel top)
	{
		var tframe = top.Frame;
		var ntop = new Toplevel(tframe);
		var menu = new MenuBar(new MenuBarItem[] {
			new MenuBarItem ("_File", new MenuItem [] {
				new MenuItem ("_Close", "", () => {Application.RequestStop ();}),
			}),
			new MenuBarItem ("_Edit", new MenuItem [] {
				new MenuItem ("_Copy", "", null),
				new MenuItem ("C_ut", "", null),
				new MenuItem ("_Paste", "", null)
			}),
		});
		ntop.Add(menu);

		string fname = null;
		foreach (var s in new[] { "/etc/passwd", @"c:\Users\pablo\plastic\server\loader.log.txt" })
			if (System.IO.File.Exists(s))
			{
				fname = s;
				break;
			}

		var win = new Window(fname ?? "Untitled")
		{
			X = 0,
			Y = 1,
			Width = Dim.Fill(),
			Height = Dim.Fill()
		};
		ntop.Add(win);

		var text = new ReadOnlyTextView(new Rect(0, 0, tframe.Width - 2, tframe.Height - 3));

		if (fname != null)
			text.Text = System.IO.File.ReadAllText(fname);

		text.ReadOnly = true;
		win.Add(text);

		Application.Run(ntop);
	}
}
