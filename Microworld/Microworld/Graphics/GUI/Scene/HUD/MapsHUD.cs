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
    class MapsHUD : HUDScene
    {
        Vector2 position = new Vector2(), size = new Vector2(200, 57);

        internal Elements.CheckBox luminosity, magnetic;
        public new void Initialize()
        {
            ShouldBeScaled = false;
            Layer = 500;

            position = new Vector2(Main.WindowWidth - size.X, 0);

            luminosity = new Elements.CheckBox((int)(position.X - size.X + 6), (int)(position.Y + 6), (int)size.X - 12, 20, "Luminosity overlay", false);
            luminosity.onCheckedChanged += new Elements.CheckBox.CheckBoxCheckedHandler(luminosity_onCheckedChanged);
            luminosity.foreground = Color.White;
            controls.Add(luminosity);

            magnetic = new Elements.CheckBox((int)(position.X - size.X + 6), (int)(position.Y + 31), (int)size.X - 12, 20, "Magnetic overlay", false);
            magnetic.onCheckedChanged += new Elements.CheckBox.CheckBoxCheckedHandler(magnetic_onCheckedChanged);
            magnetic.foreground = Color.White;
            controls.Add(magnetic);

            base.Initialize();
        }

        void magnetic_onCheckedChanged(object sender, bool IsChecked)
        {
            GUIEngine.s_magneticOverlay.IsActive = IsChecked;
        }

        void luminosity_onCheckedChanged(object sender, bool IsChecked)
        {
            GUIEngine.s_lightOverlay.IsActive = IsChecked;
        }

        public override void onShow()
        {
            magnetic.Checked = GUIEngine.s_magneticOverlay.IsActive;
            luminosity.Checked = GUIEngine.s_lightOverlay.IsActive;

            base.onShow();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);

            position = new Vector2(Main.WindowWidth - size.X, position.Y);
            luminosity.Position = new Vector2(position.X + 5, position.Y + 5);
            magnetic.Position = new Vector2(position.X + 5, position.Y + 30);
        }

        public override void Draw(Renderer renderer)
        {
            RenderHelper.SmartDrawRectangle(GraphicsEngine.bg, 5, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y, Color.White, renderer);
            base.Draw(renderer);
        }

    }
}
