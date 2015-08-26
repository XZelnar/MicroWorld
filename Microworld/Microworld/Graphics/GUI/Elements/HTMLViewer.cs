using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Graphics.GUI.Elements
{
    public unsafe class HTMLViewer : Control
    {
        public override Vector2 Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                OnSizeChanged();
            }
        }
        public Vector2 ActualSize
        {
            get { return c == null ? Vector2.Zero : new Vector2(c.ActualSize.Width, c.ActualSize.Height); }
        }
        public String raw = "";
        public Vector2 Offset
        {
            get { return offset; }
            set
            {
                SetOffset(value);
            }
        }

        private Vector2 offset = new Vector2();
        private System.Drawing.Bitmap buf1;
        private Texture2D buf2;
        private bool loaded = false;

        #region Events
        public delegate void PageLoaded();
        public event PageLoaded OnPageLoaded;
        #endregion


        public HTMLViewer(int x, int y, int w, int h)
        {
            loaded = false;
            position = new Vector2(x, y);
            Size = new Vector2(w, h);
            loaded = true;
        }

        public void OnSizeChanged()
        {
            buf1 = new System.Drawing.Bitmap((int)size.X, (int)size.Y, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            buf2 = new Texture2D(Main.renderer.GraphicsDevice, (int)size.X, (int)size.Y);
            if (loaded) Refresh();
        }

        public void LoadHTML(string html)
        {
            raw = html;
            Refresh();
        }

        public void Refresh()
        {
            loaded = false;
            System.Threading.Thread lt = new System.Threading.Thread(new System.Threading.ThreadStart(_load));
            lt.SetApartmentState(System.Threading.ApartmentState.STA);
            lt.Start();
        }

        public void SetOffset(Vector2 offset)
        {
            this.offset = offset;
            if (!isUpdating && loaded && c != null)
            {
                loaded = false;
                System.Threading.Thread lt = new System.Threading.Thread(new System.Threading.ThreadStart(_update));
                lt.SetApartmentState(System.Threading.ApartmentState.STA);
                lt.Start();
            }
        }

        HtmlRenderer.HtmlContainer c;
        private void _load()
        {
            if (raw == "")
            {
                loaded = true;
                return;
            }
            offset = new Vector2();
            lock (buf1)
            {
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(buf1);
                g.Clear(System.Drawing.Color.Transparent);
                //CopyBitmapToTexture();
                try
                {
                    //HtmlRenderer.HtmlRender.Render(g, raw, new System.Drawing.PointF(), new System.Drawing.SizeF(size.X, size.Y));
                    c = new HtmlRenderer.HtmlContainer();
                    c.AvoidImagesLateLoading = false;
                    c.Location = new System.Drawing.PointF();
                    c.MaxSize = new System.Drawing.SizeF(size.X, size.Y);
                    c.ImageLoad += new EventHandler<HtmlRenderer.Entities.HtmlImageLoadEventArgs>(c_ImageLoad);
                    c.SetHtml(raw);
                    c.PerformLayout(g);
                    c.PerformPaint(g);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot display HTML page. Aborting...");

                    g.Clear(System.Drawing.Color.Transparent);
                    CopyBitmapToTexture();

                    loaded = true;
                    return;
                }
                //System.Threading.Thread.Sleep(100);
                lock (buf2)
                {
                    CopyBitmapToTexture();
                    //SetBackground();
                }
            }
            if (OnPageLoaded != null)
                OnPageLoaded.Invoke();
            loaded = true;
        }

        bool isUpdating = false;
        private void _update()
        {
            isUpdating = true;
            lock (buf1)
            {
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(buf1);
                g.Clear(System.Drawing.Color.Transparent);
                try
                {
                    c.ScrollOffset = new System.Drawing.PointF(offset.X, offset.Y);
                    c.PerformLayout(g);
                    c.PerformPaint(g);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot display HTML page. Aborting...");

                    g.Clear(System.Drawing.Color.Transparent);
                    CopyBitmapToTexture();

                    loaded = true;
                    return;
                }
                lock (buf2)
                {
                    CopyBitmapToTexture();
                }
            }
            loaded = true;
            isUpdating = false;
        }

        void c_ImageLoad(object sender, HtmlRenderer.Entities.HtmlImageLoadEventArgs e)
        {
            System.Drawing.Image a;
            try
            {
                a = System.Drawing.Bitmap.FromFile(e.Src);
            }
            catch
            {
                throw new Exception("Trying to load external or unexisting image in HTML. Only local images are allowed");
            }
            e.Callback(a);
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
        }

        private void CopyBitmapToTexture()
        {
            byte[] textureData = new byte[4 * (int)(size.X * size.Y)];

            System.Drawing.Imaging.BitmapData bmpData = buf1.LockBits(new System.Drawing.Rectangle(0, 0, (int)size.X, (int)size.Y), 
                System.Drawing.Imaging.ImageLockMode.ReadOnly, buf1.PixelFormat);
            IntPtr safePtr = bmpData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(safePtr, textureData, 0, textureData.Length);
            buf1.UnlockBits(bmpData);

            byte t = 0;
            for (int i = 0; i < textureData.Length; i += 4)
            {
                t = textureData[i];
                textureData[i] = textureData[i + 2];
                textureData[i + 2] = t;
            }

            buf2.SetData<byte>(textureData);
        }

        public void SetBackground()
        {
            Color[] c = new Color[(int)(size.X * size.Y)];
            buf2.GetData<Color>(c);
            Color t;
            for (int i = 0; i < c.Length; i++)
            {
                t = c[i];
                //if (t.R == bg2replace.R && t.G == bg2replace.G && t.B == bg2replace.B) 
                //    c[i] = background;
            }
            buf2.SetData<Color>(c);
        }

        public override void Draw(Renderer renderer)
        {
            lock (buf2)
                renderer.Draw(buf2, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.White);
        }

    }
}
