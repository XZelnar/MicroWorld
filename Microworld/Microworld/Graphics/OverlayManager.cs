using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicroWorld.Graphics.Overlays;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Graphics
{
    static class OverlayManager
    {
        private static List<Overlay> overlays = new List<Overlay>();

        public static void Add(Overlay p)
        {
            overlays.Add(p);
        }

        public static void Remove(Overlay p)
        {
            overlays.Remove(p);
        }

        public static void Clear()
        {
            Dispose();
            overlays.Clear();
            _id = 0;
        }

        static int _id = 0;
        public static int GetID()
        {
            return _id++;
        }

        public static Overlay GetByID(int id)
        {
            for (int i = 0; i < overlays.Count; i++)
                if (overlays[i].ID == id)
                    return overlays[i];
            return null;
        }






        public static void Initialize()
        {
        }

        public static void LoadContent()
        {
            StopHighlight.texture = ResourceManager.Load<Texture2D>("Particles/StopHighlight");
            Overlays.Arrow.LoadArrows();
        }

        public static void Update()
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].Update();
                if (overlays[i].IsDead)
                {
                    overlays.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].Draw(Main.renderer);
            }
        }

        public static void Dispose()
        {
            for (int i = 0; i < overlays.Count; i++)
            {
                overlays[i].Dispose();
            }
        }


        #region Functions
        public static void HighlightStop()
        {
            Add(new StopHighlight());
        }
        #endregion
    }
}
