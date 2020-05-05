using Terminal.Gui;

namespace htopsharp
{
	public class HtopProgressBar : View
	{
		bool isActivity;
		int activityPos, delta;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Terminal.Gui.ProgressBar"/> class, starts in percentage mode with an absolute position and size.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public HtopProgressBar(Rect rect) : base(rect)
		{
			CanFocus = false;
			fraction = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Terminal.Gui.ProgressBar"/> class, starts in percentage mode and uses relative layout.
		/// </summary>
		public HtopProgressBar() : base()
		{
			CanFocus = false;
			fraction = 0;
		}

		float fraction;

		/// <summary>
		/// Gets or sets the progress indicator fraction to display, must be a value between 0 and 1.
		/// </summary>
		/// <value>The fraction representing the progress.</value>
		public float Fraction
		{
			get => fraction;
			set
			{
				fraction = value;
				isActivity = false;
				SetNeedsDisplay();
			}
		}

		/// <summary>
		/// Notifies the progress bar that some progress has taken place.
		/// </summary>
		/// <remarks>
		/// If the ProgressBar is is percentage mode, it switches to activity
		/// mode.   If is in activity mode, the marker is moved.
		/// </remarks>
		public void Pulse()
		{
			if (!isActivity)
			{
				isActivity = true;
				activityPos = 0;
				delta = 1;
			}
			else
			{
				activityPos += delta;
				if (activityPos < 0)
				{
					activityPos = 1;
					delta = 1;
				}
				else if (activityPos >= Frame.Width)
				{
					activityPos = Frame.Width - 2;
					delta = -1;
				}
			}

			SetNeedsDisplay();
		}

		public override void Redraw(Rect region)
		{
			Driver.SetAttribute(ColorScheme.Normal);

			int top = Frame.Width;
			if (isActivity)
			{
				Move(0, 0);
				Driver.AddRune('|');
				for (int i = 0; i < top; i++)
					if (i == activityPos)
						Driver.AddRune('|');
					else
						Driver.AddRune(' ');
			}
			else
			{
				Move(0, 0);
				int mid = (int)(fraction * top);
				int i;
				for (i = 0; i < mid; i++)
					Driver.AddRune('|');
				for (; i < top; i++)
					Driver.AddRune(' ');
			}
		}
	}
}
