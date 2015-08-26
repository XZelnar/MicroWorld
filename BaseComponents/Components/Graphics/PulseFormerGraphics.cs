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

namespace MicroWorld.Components.Graphics
{
    class PulseFormerGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw;
        Texture2D progressbar;
        Color[] fbobuffer;

        static Color bgColor = new Color(0, 0, 128), currentColor = new Color(200,200,200), tickColor = Color.Red;

        public PulseFormerGraphics()
        {
            Size = new Vector2(56, 24);
            Layer = 50;
        }

        public override void Initialize()
        {
            progressbar = new Texture2D(Main.renderer.GraphicsDevice, 136, 1);
            fbobuffer = new Color[progressbar.Width * progressbar.Height];
            
            base.Initialize();
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/PulseFormer/PulseFormer0cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/PulseFormer";
        }

        public override string GetCSToolTip()
        {
            return "Pulse Former";
        }

        public override string GetComponentSelectorPath()
        {
            return "Basic";
        }

        public override string GetHandbookFile()
        {
            return "Components/Pulse Former.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    return new Vector2(24, 8);
                default:
                    return new Vector2();
            }
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0 };
            //return base.GetPossibleRotations();
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (parent.ComponentRotation == Component.Rotation.cw0 || parent.ComponentRotation == Component.Rotation.cw180)
                return new Vector2(56, 24);
            else
                return new Vector2(24, 56);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Logics.PulseFormerLogics l = (Components.Logics.PulseFormerLogics)parent.Logics;
            UpdateProgressBar();

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, Color.White);
                    renderer.Draw(progressbar, new Rectangle((int)Position.X + 11, (int)Position.Y + 8, 34, 8), null, Color.White);
                    break;
                default:
                    break;
            }
        }

        public void UpdateProgressBar()
        {
            Components.Logics.PulseFormerLogics l = (Components.Logics.PulseFormerLogics)parent.Logics;
            if (l.pulsesOld == l.pulses)
                return;
            if (l.pulses.Length == 0)
            {
                for (int i = 0; i < fbobuffer.Length; i++)
                {
                    fbobuffer[i] = Shortcuts.BG_COLOR;
                }
            }
            else
            {
                float v = 0;
                for (int x = 0; x < progressbar.Width; x++)
                {
                    v = l.pulses[x * l.pulses.Length / progressbar.Width];
                    fbobuffer[x] = Color.White * v;
                }
            }
            progressbar.SetData<Color>(fbobuffer);

            l.pulsesOld = l.pulses;
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle(x, y, (int)Size.X, (int)Size.Y), new Color(1f, 1f, 1f, 0.5f));
                    break;
                default:
                    renderer.Draw(texture0cw, new Vector2(x, y) + GetCenter(parent.ComponentRotation), null, 
                        new Color(1f, 1f, 1f, 0.5f), rotation.GetHashCode() * (float)Math.PI / 2f, 
                        GetCenter(parent.ComponentRotation), 1);
                    break;
            }
        }

    }
}
