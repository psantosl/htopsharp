using Terminal.Gui;
using System;
using Mono.Terminal;

class Demo {
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

	class Filler : View {
		public Filler (Rect rect) : base (rect)
		{
		}

		public override void Redraw (Rect region)
		{
			Driver.SetAttribute (ColorScheme.Focus);
			var f = Frame;

			for (int y = 0; y < f.Width; y++) {
				Move (0, y);
				for (int x = 0; x < f.Height; x++) {
					Rune r;
					switch (x % 3) {
					case 0:
						r = '.';
						break;
					case 1:
						r = 'o';
						break;
					default:
						r = 'O';
						break;
					}
					Driver.AddRune (r);
				}
			}
		}
	}


	static void ShowTextAlignments (View container)
	{
		container.Add (
			new Label (new Rect (0, 0, 40, 3), "1-Hello world, how are you doing today") { TextAlignment = TextAlignment.Left },
			new Label (new Rect (0, 4, 40, 3), "2-Hello world, how are you doing today") { TextAlignment = TextAlignment.Right },
			new Label (new Rect (0, 8, 40, 3), "3-Hello world, how are you doing today") { TextAlignment = TextAlignment.Centered },
			new Label (new Rect (0, 12, 40, 3), "4-Hello world, how are you doing today") { TextAlignment = TextAlignment.Justified });
	}

	static void ShowEntries (View container)
	{
		var scrollView = new ScrollView (new Rect (50, 10, 20, 8)) {
			ContentSize = new Size (100, 100),
			ContentOffset = new Point (-1, -1),
			ShowVerticalScrollIndicator = true,
			ShowHorizontalScrollIndicator = true
		};

		scrollView.Add (new Box10x (0, 0));
		//scrollView.Add (new Filler (new Rect (0, 0, 40, 40)));

	
		
		var progress = new HtopProgressBar (new Rect (68, 1, 10, 1));

		var cc = new ColorScheme();
		cc.Normal = Terminal.Gui.Attribute.Make(Color.Red, Color.Black);
		cc.Focus = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray);

		progress.ColorScheme = cc;

		float fraction = 0;

		bool timer (MainLoop caller)
		{
            fraction += 0.05f;

			if (fraction > 1)
				fraction = 0;

			progress.Fraction = fraction;
			return true;
		}

		Application.MainLoop.AddTimeout (TimeSpan.FromMilliseconds (300), timer);

		// A little convoluted, this is because I am using this to test the
		// layout based on referencing elements of another view:

		var login = new Label ("Login: ") { X = 3, Y = 6 };
		var password = new Label ("Password: ") {
			X = Pos.Left (login),
			Y = Pos.Bottom (login) + 1
		};
		var loginText = new TextField ("") {
			X = Pos.Right (password),
			Y = Pos.Top (login),
			Width = 40
		};
		var passText = new TextField ("") {
			Secret = true,
			X = Pos.Left (loginText),
			Y = Pos.Top (password),
			Width = Dim.Width (loginText)
		};
		var c = new ColorScheme();
		c.Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Gray);
		c.Focus = Terminal.Gui.Attribute.Make(Color.White, Color.DarkGray);

		loginText.ColorScheme = c;

		// This is just to debug the visuals of the scrollview when small
		var scrollView2 = new ScrollView (new Rect (0, 0, 50, 4)) {
			ContentSize = new Size (120, 10),
			ShowVerticalScrollIndicator = true,
			ShowHorizontalScrollIndicator = true
		};

		var listView = new ListView(new string [] {
"Name                                      Request IP              Time      Thread   Status   Protocol  Sec  User",
"Unknown-plasticproto                           22 127.0.0.1       00:00:00  26       Proto    Plastic   None",
"CalculateMerge                                 21 127.0.0.1       00:00:00  24       Read     Plastic   None pablo"
			});

		listView.Width = Dim.Fill();
		listView.Height = Dim.Fill();

		scrollView2.Add(listView);

		// Add some content
		container.Add (
			login,
			loginText,
			password,
			passText,
			new FrameView (new Rect (3, 10, 25, 6), "Options"){
				new CheckBox (1, 0, "Remember me"),
				new RadioGroup (1, 2, new [] { "_Personal", "_Company" }),
			},
			//scrollView,
			scrollView2,
			new Button ("Ok") { X = 3, Y = 19 },
			new Button ("Cancel") { X = 10, Y = 19 },
			progress,
			new Label ("Press F9 (on Unix ESC+9 is an alias) to activate the menubar") { X = 3, Y = 22 }
		);

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
	static void Main ()
	{
		//Application.UseSystemConsole = true;
		Application.Init ();

		var top = Application.Top;
		var tframe = top.Frame;

		var win = new Window ("Hello"){
			X = 0,
			Y = 1,
			Width = Dim.Fill (),
			Height = Dim.Fill () - 2
		};					

		win.ColorScheme.Normal = Terminal.Gui.Attribute.Make(Color.White, Color.Black);

		var menu = new MenuBar (new MenuBarItem [] {
			new MenuBarItem ("_File", new MenuItem [] {
				new MenuItem ("_New", "Creates new file", NewFile),
				new MenuItem ("_Open", "", null),
				new MenuItem ("_Close", "", () => Close ()),
				new MenuItem ("_Quit", "", () => { if (Quit ()) top.Running = false; })
			}),
			new MenuBarItem ("_Edit", new MenuItem [] {
				new MenuItem ("_Copy", "", null),
				new MenuItem ("C_ut", "", null),
				new MenuItem ("_Paste", "", null)
			})
		});

		ShowEntries (win);
		/*int count = 0;
		ml = new Label (new Rect (3, 17, 47, 1), "Mouse: ");
		Application.RootMouseEvent += delegate (MouseEvent me) {
			ml.Text = $"Mouse: ({me.X},{me.Y}) - {me.Flags} {count++}";
		};

		win.Add (ml);*/

		top.Add (win, menu);
		top.Add (menu);
		Application.Run ();
	}


	public class HtopProgressBar : View {
		bool isActivity;
		int activityPos, delta;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Terminal.Gui.ProgressBar"/> class, starts in percentage mode with an absolute position and size.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public HtopProgressBar (Rect rect) : base (rect)
		{
			CanFocus = false;
			fraction = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Terminal.Gui.ProgressBar"/> class, starts in percentage mode and uses relative layout.
		/// </summary>
		public HtopProgressBar () : base ()
		{
			CanFocus = false;
			fraction = 0;
		}

		float fraction;

		/// <summary>
		/// Gets or sets the progress indicator fraction to display, must be a value between 0 and 1.
		/// </summary>
		/// <value>The fraction representing the progress.</value>
		public float Fraction {
			get => fraction;
			set {
				fraction = value;
				isActivity = false;
				SetNeedsDisplay ();
			}
		}

		/// <summary>
		/// Notifies the progress bar that some progress has taken place.
		/// </summary>
		/// <remarks>
		/// If the ProgressBar is is percentage mode, it switches to activity
		/// mode.   If is in activity mode, the marker is moved.
		/// </remarks>
		public void Pulse ()
		{
			if (!isActivity) {
				isActivity = true;
				activityPos = 0;
				delta = 1;
			} else {
				activityPos += delta;
				if (activityPos < 0) {
					activityPos = 1;
					delta = 1;
				} else if (activityPos >= Frame.Width) {
					activityPos = Frame.Width - 2;
					delta = -1;
				}
			}

			SetNeedsDisplay ();
		}

		public override void Redraw(Rect region)
		{
			Driver.SetAttribute (ColorScheme.Normal);

			int top = Frame.Width;
			if (isActivity) {
				Move (0, 0);
				Driver.AddRune ('|');
				for (int i = 0; i < top; i++)
					if (i == activityPos)
						Driver.AddRune ('|');
					else
						Driver.AddRune (' ');
			} else {
				Move (0, 0);
				int mid = (int)(fraction * top);
				int i;
				for (i = 0; i < mid; i++)
					Driver.AddRune ('|');
				for (; i < top; i++)
					Driver.AddRune (' ');
			}
		}
	}
}
