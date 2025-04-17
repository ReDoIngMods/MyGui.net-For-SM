using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyGui.net
{
	public class AccessibleDescriptionTooltipFilter : IMessageFilter
	{
		[DllImport("user32.dll")]
		private static extern IntPtr WindowFromPoint(Point pt);

		private readonly Dictionary<Form, ToolTip> toolTips = new();
		private Control lastControl = null;

		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == 0x200) // WM_MOUSEMOVE
			{
				Point mousePos = Cursor.Position;
				IntPtr hWnd = WindowFromPoint(mousePos);

				if (hWnd == IntPtr.Zero)
					return false;

				Control ctrl = Control.FromHandle(hWnd) ?? Control.FromChildHandle(hWnd);

				if (ctrl != null && ctrl != lastControl)
				{
					lastControl = ctrl;

					if (!string.IsNullOrEmpty(ctrl.AccessibleDescription))
					{
						ToolTip tip = GetToolTipFor(ctrl);
						if (tip != null)
							tip.SetToolTip(ctrl, ctrl.AccessibleDescription);
					}
					else
					{
						ToolTip tip = GetToolTipFor(ctrl);
						if (tip != null)
							tip.SetToolTip(ctrl, null);
					}
				}
			}

			return false;
		}

		private ToolTip GetToolTipFor(Control ctrl)
		{
			Form form = ctrl.FindForm();
			if (form == null)
				return null;

			if (!toolTips.TryGetValue(form, out var tip))
			{
				tip = new ToolTip
				{
					ShowAlways = true
				};

				toolTips[form] = tip;

				// Optional: clean up when the form is closed
				form.FormClosed += (s, e) => toolTips.Remove(form);
			}

			return tip;
		}
	}
}
