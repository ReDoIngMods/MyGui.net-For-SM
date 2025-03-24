namespace MyGui.net
{
	public class CustomTabControl : TabControl
	{
		public CustomTabControl()
		{
			// Enable custom painting and optimized double buffering.
			this.SetStyle(ControlStyles.UserPaint |
						  ControlStyles.AllPaintingInWmPaint |
						  ControlStyles.OptimizedDoubleBuffer, true);
			this.DrawMode = TabDrawMode.OwnerDrawFixed;
			this.DoubleBuffered = true;
			this.ItemSize = new Size(120, 25);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			// Fill the entire control's background.
			using (SolidBrush backgroundBrush = new SolidBrush(this.Parent?.BackColor ?? Color.FromKnownColor(KnownColor.Control)))
			{
				e.Graphics.FillRectangle(backgroundBrush, this.ClientRectangle);
			}

			// Calculate the inner frame rectangle (where the tab pages are shown).
			Rectangle frameRect = new Rectangle(3, this.ItemSize.Height + 3, this.Width - 7, this.Height - this.ItemSize.Height - 7);

			// Draw the border around the inner frame.
			using (Pen framePen = new Pen(Color.FromKnownColor(KnownColor.ControlDark), 1))
			{
				e.Graphics.DrawRectangle(framePen, frameRect);
			}

			// Draw each tab.
			for (int i = 0; i < this.TabCount; i++)
			{
				DrawTab(e.Graphics, i);
			}
		}

		private void DrawTab(Graphics g, int index)
		{
			Rectangle tabRect = this.GetTabRect(index);
			bool isSelected = this.SelectedIndex == index;

			tabRect.Offset(new(1, 0));
			if (!isSelected)
			{
				tabRect.Offset(new(0, 2));
				tabRect.Inflate(new(0, -2));
			}

			// Colors
			Color tabColor = Color.FromKnownColor(KnownColor.ControlLight);       // Dark tab background
			Color selectedTabColor = Color.FromKnownColor(KnownColor.ControlLightLight); // Slightly lighter for selected tab
			Color borderColor = Color.FromKnownColor(KnownColor.ControlDark);   // Highlight color for the selected tab
			Color textColor = Color.FromKnownColor(KnownColor.ControlText);

			// Draw the background of the tab.
			using (SolidBrush tabBrush = new SolidBrush(isSelected ? selectedTabColor : tabColor))
			{
				tabRect.Height += 5;
				g.FillRectangle(tabBrush, tabRect);
			}

			// Draw border for the selected tab (omit the bottom edge).
			using (Pen borderPen = new Pen(borderColor, 1))
			{
				g.DrawLine(borderPen, tabRect.Left, tabRect.Top, tabRect.Right - 1, tabRect.Top);   // Top
				g.DrawLine(borderPen, tabRect.Left, tabRect.Top, tabRect.Left, tabRect.Bottom); // Left
				g.DrawLine(borderPen, tabRect.Right - 1, tabRect.Top, tabRect.Right - 1, tabRect.Bottom); // Right
				if (!isSelected)
				{
					g.DrawLine(borderPen, tabRect.Left, tabRect.Bottom - 4, tabRect.Right, tabRect.Bottom - 4);   // Top
				}
			}

			// Draw the tab text centered.
			tabRect.Height -= 3;
			TextRenderer.DrawText(g, this.TabPages[index].Text, this.Font, tabRect, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// Do nothing to prevent flicker, full painting is done in OnPaint.
		}
	}
}
