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
    class CreditsFrame : MenuFrameScene
    {
        Elements.Label l;

        public override void Initialize()
        {
            l = new Elements.Label((int)Position.X + 30, (int)Position.Y + 30, "");
            l.font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_14");
            l.foreground = Color.White;
            controls.Add(l);

            l.text =
                "Developed\r\n" +
                "  -Glib Boytsun\r\n" +
                "Art by\r\n" +
                "  -Glib Boytsun\r\n" +
                "  -Stanislav Efremov\r\n" +
                "\r\n" +
                "Powered by:\r\n" +
                "  -Lua\r\n" +
                "  -LuaInterface\r\n" +
                "  -HTML Renderer\r\n" +
                "Full copyright notices can be found in Copyrights.txt file\r\n" +
                "\r\n" +
                "Music and sound effects from:\r\n" +
                "  http://www.looperman.com/loops/detail/65672\r\n" +
                "  http://www.looperman.com/loops/detail/51500\r\n" +
                "  http://freesound.org/people/Erokia/sounds/218854/\r\n" +
                "  http://freesound.org/people/junggle/sounds/26777/\r\n" +
                "  http://www.flashkit.com/soundfx/Electronic/Computers/High_Tec-NEO_Soun-8397/index.php"
                ;

            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            l.Position = GetPosForWH(w, h);
            base.OnResolutionChanged(w, h, oldw, oldh);
        }
    }
}
