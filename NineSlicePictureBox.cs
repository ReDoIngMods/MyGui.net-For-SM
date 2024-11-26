namespace MyGui.net
{
    using MyGui.net.Properties;
    using SkiaSharp;
    using SkiaSharp.Views.Desktop;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    public class NineSlicePictureBox : PictureBox
    {
        public MyGuiResource? Resource;

        private SKControl _skControl;

        public NineSlicePictureBox() : base()
        {
            // Initialize custom SkiaSharp control for rendering
            _skControl = new SKControl
            {
                Dock = DockStyle.Fill
            };
            _skControl.Enabled = false;
            _skControl.PaintSurface += OnPaintSurface;
            this.Controls.Add(_skControl);

            // Configure control styles for custom rendering
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
            _skControl = new SKControl
            {
                Dock = DockStyle.Fill
            };
            _skControl.PaintSurface += OnPaintSurface;
            this.Controls.Add(_skControl);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false); // Disable default buffering
            this.SetStyle(ControlStyles.Opaque, true);
        }

        // Handle SkiaSharp rendering on the surface
        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (Image == null || Resource == null || Resource.basisSkins == null || Resource.tileSize == null)
            {
                return; // If no resource or image, don't do anything
            }

            Debug.WriteLine($"Drawing widget with skin {Resource.name} and tilesize {Resource.tileSize}.");

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);

            // Iterate through skins (reverse order to match original logic)
            for (int i = Resource.basisSkins.Count - 1; i > 0; i--)
            {
                var skin = Resource.basisSkins[i];
                var tileOffset = Util.GetWidgetPos(false, skin.offset, new(1, 1));
                if (skin.type == "SimpleText")
                {
                    continue; // Skip if it's a text type skin
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
                Debug.WriteLine($"Drawing skin with align {skin.align}");
                if (skin.offset == null) continue;

                var posSize = Util.GetWidgetPosAndSize(false, normalState.offset, new(1, 1));
                var tileRect = new SKRect(posSize.Item1.X, posSize.Item1.Y, posSize.Item2.X, posSize.Item2.Y);

                var clientPos = new SKRect(this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Width, this.ClientRectangle.Height);
                clientPos.Offset(tileOffset.X, tileOffset.Y);

                // Calculate the destination rectangle based on alignment
                var destRect = GetAlignedRectangle(skin.align, clientPos, tileRect.Size);

                // Draw the image using SkiaSharp
                var skImage = SKImage.FromBitmap(SKBitmap.Decode(Resource.path));
                canvas.DrawImage(skImage, new SKPoint(destRect.Left, destRect.Right));//destRect, tileRect, SKFilterQuality.Medium);
            }
        }

        // Suppress background painting (leave it transparent)
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing here to suppress background rendering
        }

        // Get aligned rectangle for drawing
        private SKRect GetAlignedRectangle(string? align, SKRect container,  SKSize tileSize)
        {
            if (string.IsNullOrEmpty(align) || align == "[DEFAULT]" || align == "Default")
            {
                // Default: no scaling or positioning adjustment
                return new SKRect(container.Location.X, container.Location.Y, tileSize.Width, tileSize.Height);
            }

            int x = (int)container.Left, y = (int)container.Top, width = (int)tileSize.Width, height = (int)tileSize.Height;

            switch (align)
            {
                case "Stretch":
                    width = (int)container.Width;
                    height = (int)container.Height;
                    break;

                case "Center":
                    x += ((int)container.Width - (int)tileSize.Width) / 2;
                    y += ((int)container.Height - (int)tileSize.Height) / 2;
                    break;

                case "Left Top":
                    // Already default
                    break;

                case "Left Bottom":
                    y = (int)container.Bottom - (int)tileSize.Height;
                    break;

                case "Left VStretch":
                    height = (int)container.Height;
                    break;

                case "Left VCenter":
                    y += ((int)container.Height - (int)tileSize.Height) / 2;
                    break;

                case "Right Top":
                    x = (int)container.Right - (int)tileSize.Width;
                    break;

                case "Right Bottom":
                    x = (int)container.Right - (int)tileSize.Width;
                    y = (int)container.Bottom - (int)tileSize.Height;
                    break;

                case "Right VStretch":
                    x = (int)container.Right - (int)tileSize.Width;
                    height = (int)container.Height;
                    break;

                case "Right VCenter":
                    x = (int)container.Right - (int)tileSize.Width;
                    y += ((int)container.Height - (int)tileSize.Height) / 2;
                    break;

                default:
                    Debug.WriteLine($"Unknown align type: {align}");
                    break;
            }

            return new SKRect(x, y, width, height);
        }
    }
}
