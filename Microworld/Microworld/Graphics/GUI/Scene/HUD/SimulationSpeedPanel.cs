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

namespace MicroWorld.Graphics.GUI.Scene
{
    class SimulationSpeedPanel : HUDScene
    {
        Vector2 size = new Vector2(196, 69);
        Vector2 pos = new Vector2();
        public new void Initialize()
        {
            ShouldBeScaled = false;
            Layer = 500;
            
            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            pos = new Vector2(w, h) - size;

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), 
                Shortcuts.BG_COLOR * 0.75f);

            String s = "File: ";
            if (GUIEngine.s_levelSelection.folder == "")
            {
                if (IO.SaveEngine.LastLoadedFile == "")
                {
                    s += "Untitled_1";
                }
                else
                {
                    if (IO.SaveEngine.LastLoadedFile.Length > 10)
                        s += IO.SaveEngine.LastLoadedFile.Substring(0, 10) + "..";
                    else
                        s += IO.SaveEngine.LastLoadedFile;
                }
            }
            else
            {
                s += GUIEngine.s_levelSelection.folder.Substring(GUIEngine.s_levelSelection.folder.IndexOf('/') + 1);
                s = s.Substring(0, s.Length - 1);
                s += "_" + GUIEngine.s_levelSelection.selectedLevel.ToString();
            }
            s += "\r\n";
            s += "Sacale: 1:";
            String scale = ((float)Math.Round(Settings.GameScale * 100) / 100f).ToString();
            String rs = "";
            for (int i = 0; i < scale.Length; i++)
            {
                if (scale[i] == ',')
                    rs += ".";
                else
                    rs += scale[i];
            }
            s += rs;

            renderer.DrawStringLeft(GUIEngine.font, s, pos + new Vector2(4, 4), Color.White);

            base.Draw(renderer);
        }
    }
}
