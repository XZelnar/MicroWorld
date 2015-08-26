using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace MicroWorld.Encyclopedia
{
    public class Article
    {
        public Vector2 position = new Vector2();
        private Vector2 size = new Vector2(200, 200);

        public Vector2 Size
        {
            get { return size; }
            set
            {
                Vector2 delta = value - size;
                size = value;
                view.Size += delta;
                view.Refresh();
            }
        }

        public string Link = "";
        public bool HasLink
        {
            get { return Link != null && Link.Length != 0 && IsURL(Link); }
        }

        internal Graphics.GUI.Elements.HTMLViewer view = new Graphics.GUI.Elements.HTMLViewer(0, 0, 200, 200);

        public bool IsURL(String uriName)
        {
            Uri uriResult;
            return Uri.TryCreate(uriName, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        }

        public void GoToLink()
        {
            System.Diagnostics.Process.Start(Link);
        }

        public void OnResolutionChanged(int w, int h, int ow, int oh)
        {
            Size = new Vector2(w - 20, h - 50);
            //view.Refresh();
        }

        public void Draw(Graphics.Renderer renderer)
        {
            /*
            if (texture == null)
                texture = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");

            Main.renderer.Draw(Graphics.GraphicsEngine.pixel,
                new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                new Rectangle(0, 0, (int)size.X, (int)size.Y),
                Color.Black * 0.5f);
            Main.renderer.Draw(texture,
                new Rectangle((int)position.X, (int)position.Y, (int)4, (int)4),
                new Rectangle(0, 0, (int)4, (int)4),
                Color.White);
            Main.renderer.Draw(texture,
                new Rectangle((int)(position.X + size.X - 4), (int)position.Y, (int)4, (int)4),
                new Rectangle(252, 0, (int)4, (int)4),
                Color.White);
            Main.renderer.Draw(texture,
                new Rectangle((int)position.X, (int)(position.Y + size.Y - 4), (int)4, (int)4),
                new Rectangle(0, 252, (int)4, (int)4),
                Color.White);
            Main.renderer.Draw(texture,
                new Rectangle((int)(position.X + size.X - 4), (int)(position.Y + size.Y - 4), 4, 4),
                new Rectangle(252, 252, 4, 4),
                Color.White);//*/
            view.Draw(renderer);
        }

        public void Clear()
        {
            Link = "";
            view.position = position;
            view.size = size;
            view.LoadHTML("<body> </body>");
        }

        public void LoadHTML(String path)
        {
            if (!File.Exists(path)) return;
            StreamReader br = new StreamReader(path);
            String text = br.ReadToEnd();
            br.Close();
            view.LoadHTML(text);
        }

        public void Load(String path)
        {
            if (!File.Exists(path)) return;
            Clear();
            BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open));
            Link = br.ReadString();
            String text = br.ReadString();
            br.Close();
            DoCode(ref text);
            view.LoadHTML(text);
        }

        public void DoCode(ref String s)
        {
            String r = "";
            for (int i = 0; i < s.Length; i++)
            {
                r += ((char)((int)s[i] ^ 12145)).ToString();
            }
            s = r;
        }

        public Texture2D ReadTexture(BinaryReader br, int w, int h)
        {
            Texture2D tex = new Texture2D(Main.renderer.GraphicsDevice, w, h);
            Color[] d = new Color[w * h];
            Color c;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    c = new Color(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
                    d[x + y * w] = c;
                }
            }
            tex.SetData<Color>(d);
            return tex;
        }

        public void Save(String path)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate));
            String text = Link;
            bw.Write(Link);
            text = view.raw;
            DoCode(ref text);
            bw.Write(text);
            bw.Close();
        }

        public void SaveTexture(BinaryWriter bw, Texture2D tex)
        {
            Color[] d = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(d);
            Color c;
            for (int x = 0; x < tex.Width; x++)
            {
                for (int y = 0; y < tex.Height; y++)
                {
                    c = d[y * tex.Width + x];
                    bw.Write(c.R);
                    bw.Write(c.G);
                    bw.Write(c.B);
                    bw.Write(c.A);
                }
            }
        }

        private String CharArrToStr(char[] c)
        {
            String r = "";
            for (int i = 0; i < c.Length; i++)
            {
                r += c[i];
            }
            return r;
        }
    }
}
