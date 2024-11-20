namespace MyGui.net
{
    using MyGui.net.Properties;
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    public class NineSlicePictureBox : PictureBox
    {
        public MyGuiResource? Resource { get; set; }

        public NineSlicePictureBox() : base()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false); // Disable default buffering
            this.SetStyle(ControlStyles.Opaque, true);
        }

        public NineSlicePictureBox(
            MyGuiResource newRes = null,
            Image initialImage = null,
            PictureBoxSizeMode sizeMode = PictureBoxSizeMode.Normal,
            DockStyle dockStyle = DockStyle.None,
            int width = 100,
            int height = 100
        ) : base()
        {
            Resource = newRes;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false); // Disable default buffering
            this.SetStyle(ControlStyles.Opaque, true);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null || Resource == null || Resource.basisSkins == null || Resource.tileSize == null)
            {
                base.OnPaint(e);
                return;
            }

            Debug.WriteLine($"Drawing widget with skin {Resource.name} and tilesize {Resource.tileSize}.");

            Graphics g = e.Graphics;

            // Parse tile size
            //var tileSize = Util.GetWidgetPos(false, Resource.tileSize, new(1, 1));

            //foreach (var skin in Resource.basisSkins)
            for (int i = Resource.basisSkins.Count - 1; i > 0; i--)
            {
                var skin = Resource.basisSkins[i];
                var tileOffset = Util.GetWidgetPos(false, skin.offset, new(1, 1));
                if (skin.type == "SimpleText")
                {
                    continue;
                }
                MyGuiBasisSkinState normalState = new MyGuiBasisSkinState();
                foreach (var item in skin.states)
                {
                    Debug.WriteLine(item.name);
                    if (item.name == "normal")
                    {
                        normalState = item;
                        break;
                    }
                }
                Debug.WriteLine($"drawing skin with align {skin.align}");
                if (skin.offset == null) continue;

                var posSize = Util.GetWidgetPosAndSize(false, normalState.offset, new(1, 1));
                //var tileRect = new Rectangle(posSize.Item1 + (Size)tileOffset, (Size)posSize.Item2);
                var tileRect = new Rectangle(posSize.Item1, (Size)posSize.Item2);

                var clientPos = this.ClientRectangle;
                //clientPos.Offset(tileOffset);
                clientPos.X += tileOffset.X;
                clientPos.Y += tileOffset.Y;

                // Calculate the destination rectangle based on alignment
                //var destRect = GetAlignedRectangle(skin.align, this.ClientRectangle, tileRect.Size);
                var destRect = GetAlignedRectangle(skin.align, clientPos, tileRect.Size);

                // Draw the image
                //g.FillRectangle(new SolidBrush(this.BackColor), destRect);
                g.DrawImage(Image, destRect, tileRect, GraphicsUnit.Pixel);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing here to suppress background rendering
        }

        private Rectangle GetAlignedRectangle(string? align, Rectangle container, Size tileSize)
        {
            if (string.IsNullOrEmpty(align) || align == "[DEFAULT]" || align == "Default")
            {
                // Default: no scaling or positioning adjustment
                return new Rectangle(container.Location, tileSize);
            }

            int x = container.Left, y = container.Top, width = tileSize.Width, height = tileSize.Height;

            switch (align)
            {
                case "Stretch":
                    width = container.Width;
                    height = container.Height;
                    break;

                case "Center":
                    x += (container.Width - tileSize.Width) / 2;
                    y += (container.Height - tileSize.Height) / 2;
                    break;

                case "Left Top":
                    // Already default
                    break;

                case "Left Bottom":
                    y = container.Bottom - tileSize.Height;
                    break;

                case "Left VStretch":
                    height = container.Height;
                    break;

                case "Left VCenter":
                    y += (container.Height - tileSize.Height) / 2;
                    break;

                case "Right Top":
                    x = container.Right - tileSize.Width;
                    break;

                case "Right Bottom":
                    x = container.Right - tileSize.Width;
                    y = container.Bottom - tileSize.Height;
                    break;

                case "Right VStretch":
                    x = container.Right - tileSize.Width;
                    height = container.Height;
                    break;

                case "Right VCenter":
                    x = container.Right - tileSize.Width;
                    y += (container.Height - tileSize.Height) / 2;
                    break;

                case "HStretch Top":
                    width = container.Width;
                    break;

                case "HStretch Bottom":
                    width = container.Width;
                    y = container.Bottom - tileSize.Height;
                    break;

                case "HStretch VStretch":
                    width = container.Width;
                    height = container.Height;
                    break;

                case "HStretch VCenter":
                    width = container.Width;
                    y += (container.Height - tileSize.Height) / 2;
                    break;

                case "HCenter Top":
                    x += (container.Width - tileSize.Width) / 2;
                    break;

                case "HCenter Bottom":
                    x += (container.Width - tileSize.Width) / 2;
                    y = container.Bottom - tileSize.Height;
                    break;

                case "HCenter VStretch":
                    x += (container.Width - tileSize.Width) / 2;
                    height = container.Height;
                    break;

                case "HCenter VCenter":
                    x += (container.Width - tileSize.Width) / 2;
                    y += (container.Height - tileSize.Height) / 2;
                    break;

                default:
                    Debug.WriteLine($"Unknown align type: {align}");
                    break;
            }

            return new Rectangle(x, y, width, height);
        }
    }
}
