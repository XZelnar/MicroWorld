using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MicroWorld.Graphics.GUI;

namespace MicroWorld.Debug
{
    public partial class ComponentsList : Form
    {
        public ComponentsList()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lv.Items.Clear();
            if (MicroWorld.Graphics.GUI.GUIEngine.CurScene == MicroWorld.Graphics.GUI.GUIEngine.s_game)
            {
                Components.Component c;
                for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
                {
                    c = Components.ComponentsManager.Components[i];
                    lv.Items.Add(new ListViewItem(new string[] {c.ID.ToString(), c.ToString(), c.Graphics.Position.ToString(), c.Graphics.GetSize().ToString(),
                        c.Graphics.Visible.ToString(), 
                        (GUIEngine.s_subComponentButtons.isVisible && GUIEngine.s_subComponentButtons.SelectedComponent == c).ToString()}));
                }
            }
        }

        private void ComponentsList_Shown(object sender, EventArgs e)
        {
            GlobalEvents.onComponentPlacedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentPlacedByPlayer);
            GlobalEvents.onComponentRemovedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);
            button1_Click(null, null);
        }

        void GlobalEvents_onComponentRemovedByPlayer(Components.Component sender)
        {
            button1_Click(null, null);
        }

        void GlobalEvents_onComponentPlacedByPlayer(Components.Component sender)
        {
            button1_Click(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv.SelectedIndices.Count > 0)
            {
                GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_subComponentButtons);
                GUIEngine.s_subComponentButtons.SelectedComponent =
                    Components.ComponentsManager.Components[lv.SelectedIndices[0]];
                GUIEngine.s_subComponentButtons.isVisible = true;
                GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_subComponentButtons);
            }
        }

        private void ComponentsList_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalEvents.onComponentPlacedByPlayer -= new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentPlacedByPlayer);
            GlobalEvents.onComponentRemovedByPlayer -= new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);
        }
    }
}
