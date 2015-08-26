using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components
{
    public class EmptyComponent : Component
    {

        private void constructor()
        {
            //ID = ComponentsManager.GetFreeID();
            Logics = new Logics.EmptyLogics();
            Graphics = new Graphics.EmptyGraphics();
            Graphics.parent = this;
            Logics.parent = this;
            Graphics.Visible = false;

            ToolTip = null;
        }

        public EmptyComponent()
        {
            constructor();
        }

        public EmptyComponent(int id)
        {
            ID = id;
            constructor();
        }

        public override bool isIn(int x, int y)
        {
            return false;
        }


        //================================================================IO===========================================================

        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);
        }

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);
        }

        public override void PostLoad()
        {
            base.PostLoad();
        }
    }
}
