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
using System.IO;

namespace MicroWorld
{
    public static class ResourceManager
    {
        struct NamedObject
        {
            public Object obj;
            public String name;

            public NamedObject(String n, Object o)
            {
                name = n;
                obj = o;
            }
        }

        private static List<NamedObject> textures = new List<NamedObject>();
        private static List<NamedObject> fonts = new List<NamedObject>();

        private static ContentManager content;

        private static Dictionary<String, String> overrides = new Dictionary<string, string>();

        internal static void Initialize(ContentManager c)
        {
            content = c;

            if (System.IO.File.Exists("overrides.txt"))//TODO disable
            {
                System.IO.StreamReader sr = new System.IO.StreamReader("overrides.txt");
                while (sr.Peek() > -1)
                {
                    String s = sr.ReadLine();
                    if (s.Length > 3)
                    {
                        var a = s.Split(';');
                        if (a.Length == 2)
                        {
                            a[0].Trim();
                            a[1].Trim();
                            overrides.Add(a[0], a[1]);
                            IO.Log.Write("    Adding override from " + a[0] + " to " + a[1]);
                        }
                    }
                }
                sr.Close();
            }
        }

        public static T Load<T>(String name)
        {
            IO.Log.Write("                Loading resource " + name + " of type " + typeof(T).ToString());
            if (typeof(T) == typeof(SpriteFont))
            {
                if (Main.curState == "GUIGlobalLoad")
                {
                    Main.LoadingDetails = "Loading font: " + name + "...";
                }
                var a = GetFont(name);
                if (a == null)
                {
                    a = content.Load<SpriteFont>(name);
                    fonts.Add(new NamedObject(name, a));
                }
                return (T)((object)a);
            }
            if (typeof(T) == typeof(Texture2D))
            {
                //override
                if (overrides.ContainsKey(name))
                {
                    String s = overrides[name];
                    IO.Log.Write("    Detected " + name + ". Overriding to " + s);
                    name = s;
                }
                if (Main.curState == "GUIGlobalLoad")
                {
                    Main.LoadingDetails = "Loading texture: " + name + "...";
                }
                var a = GetTexture2D(name);
                if (a == null)
                {
                    if (overrides.ContainsValue(name))//TODO disable
                    {
                        //a = content.Load<Texture2D>(name);
                        System.IO.StreamReader sr = new System.IO.StreamReader(name);
                        a = Texture2D.FromStream(Main.renderer.GraphicsDevice, sr.BaseStream);
                        sr.Close();
                    }
                    else
                    {
                        a = content.Load<Texture2D>(name);
                    }
                    textures.Add(new NamedObject(name, a));
                }
                return (T)((object)a);
            }
            if (typeof(T) == typeof(Effect))
                if (Utilities.Tools.IsRunningOnMono())
                {
                    BinaryReader Reader = new BinaryReader(File.Open("Content\\" + name + ".mgfxo", FileMode.Open));
                    Effect e = new Effect(Main.renderer.GraphicsDevice, Reader.ReadBytes((int)Reader.BaseStream.Length));
                    return (T)((object)e);
                }
                else
                {
                    return content.Load<T>(name);
                }

            return content.Load<T>(name);
        }

        public static Texture2D LoadTexture2D(String name)
        {
            return Load<Texture2D>(name);
        }

        public static SpriteFont LoadSpriteFont(String name)
        {
            return Load<SpriteFont>(name);
        }

        public static SoundEffect LoadSoundEffect(String name)
        {
            return Load<SoundEffect>(name);
        }

        private static SpriteFont GetFont(String name)
        {
            for (int i = 0; i < fonts.Count; i++)
            {
                if (fonts[i].name == name) return fonts[i].obj as SpriteFont;
            }
            return null;
        }

        private static Texture2D GetTexture2D(String name)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                if (textures[i].name == name) return textures[i].obj as Texture2D;
            }
            return null;
        }
    }
}
