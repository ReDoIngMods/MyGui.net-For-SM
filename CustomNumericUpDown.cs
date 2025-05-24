namespace MyGui.NET
{
    using System;
    using System.Windows.Forms;

    public class CustomNumericUpDown : NumericUpDown
    {
        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x020A;
            const int WHEEL_DELTA = 120;

            if (m.Msg == WM_MOUSEWHEEL)
            {
                int delta = (int)m.WParam >> 16;
                // Convert the scroll delta into an increment amount
                decimal incrementValue = this.Increment;
                int scrollDelta = delta / WHEEL_DELTA;
                decimal valueChange = scrollDelta * incrementValue;

                // Adjust the value by the increment
                decimal newValue = this.Value + valueChange;
                if (newValue <= this.Maximum && newValue >= this.Minimum)
                {
                    this.Value = newValue;
                }

                m.Result = IntPtr.Zero; // Indicate that the message has been handled
                return;
            }

            base.WndProc(ref m);
        }
    }
}
