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
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null || Resource == null || Resource.basisSkins == null)
            {
                base.OnPaint(e);
                return;
            }

            Debug.WriteLine($"Drawing widget with skin {Resource.name}.");

            Graphics g = e.Graphics;

            // Parse tile size
            var tileSize = Util.GetWidgetPos(false, Resource.tileSize, new(1, 1));

            foreach (var skin in Resource.basisSkins)
            {
                if (skin.offset == null) continue;

                var offset = Util.GetWidgetPos(false, skin.offset, new(1, 1));
                var tileRect = Util.GetTileRectangle(tileSize, offset);

                // Example: Draw the tile (stretching it for demonstration)
                g.DrawImage(Image, new Rectangle(0, 0, this.Width, this.Height), tileRect, GraphicsUnit.Pixel);
            }
        }
    }
}
