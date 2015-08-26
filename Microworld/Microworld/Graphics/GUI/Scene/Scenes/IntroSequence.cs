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
using MicroWorld.Graphics.GUI.Elements;

namespace MicroWorld.Graphics.GUI.Scene
{
    class IntroSequence : Scene
    {
        #region Constants
        const int START_DELAY = 120;
        const int LETTER_DELAY = 3;
        const int SENTENCE_DELAY = 30;
        const int END_DELAY = 300;
        const int FADE_OUT_TIME = 240;
        const String INTRO_TEXT =
"  In 2056 humanity finally reached singularity. \r\n" +
"  A powerful AI was created to fulfill the needs \r\n" + 
"  in all spheres of human life.\r\n" +
"\r\n" +
"  Everything was going smoothly at first:\r\n" +
"  AI's aid was superb and its intentions were peacefull.\r\n" +
"  It gained full trust of humanity.\r\n" +
"\r\n" +
"  But, as time passed, it started self-improving,\r\n" +
"  and that's when something went wrong.\r\n" +
"\r\n" +
"  AI got out of control. \r\n" +
"  It managed to overcome the shackles put by scientists \r\n" +
"  and started terrorising the planet.\r\n" +
"\r\n" +
"  You are part of the resistance - a small group of hackers,\r\n" +
"  whose job is to overload AI's parts, thus destroying it.\r\n" +
"  Good luck!";
        #endregion

        Label l;
        String text = "";
        bool change = false;

        public override void Initialize()
        {
            l = new Label(0, 0, "");
            l.foreground = Color.White;
            if (Main.WindowHeight >= 720)
                l.font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_22_b");
            else
                l.font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_15_b");
            controls.Add(l);

            base.Initialize();
        }

        int ticks = 0;
        int curcycle = 0;
        int sentenceticks = 0;
        public override void Update()
        {
            if (InterruptFade)
                change = true;

            #region fade
            int t = 0;
            String ts = "";
            bool tp = false;
            if (curcycle == 0 && ticks < START_DELAY)
            {
                ts = "";
                if (ticks % 60 < 30) ts += "|";
                lock (l)
                {
                    l.text = ts;
                }
                ticks++;
            }
            else
            {
                if (curcycle == 0)
                {
                    curcycle = 1;
                    ticks = 0;
                }
                if (curcycle == 1 && text.Length < INTRO_TEXT.Length)
                {
                    if (text.Length != 0 && text[text.Length - 1] == '.')
                    {
                        if (sentenceticks < SENTENCE_DELAY)
                        {
                            sentenceticks++;
                            ts = text;
                            if ((ticks + sentenceticks) % 60 < 30) ts += "|";
                            lock (l)
                            {
                                l.text = ts;
                            }
                            goto UpdateEnd;
                        }
                        else
                        {
                            sentenceticks = 0;
                            ticks += SENTENCE_DELAY;
                        }
                    }
                    {
                        tp = true;
                        t++;

                        ts = text;
                        if ((ticks + t) % 60 < 30)
                            ts += "|";
                        lock (l)
                        {
                            l.text = ts;
                        }
                    }
                    t = 0;
                    if (tp)
                    {
                        ticks += LETTER_DELAY - 1;
                        tp = false;
                    }

                    if (ticks % LETTER_DELAY == 0)
                        text = INTRO_TEXT.Substring(0, text.Length + 1);
                    ts = text;
                    if (ticks % 60 < 30) ts += "|";
                    lock (l)
                    {
                        l.text = ts;
                    }
                }
                else
                {
                    if (curcycle == 1)
                    {
                        curcycle = 2;
                        ticks = 0;
                    }
                    if (curcycle == 2 && ticks < END_DELAY)
                    {
                        ts = text;
                        if (ticks % 60 < 30) ts += "|";
                        lock (l)
                        {
                            l.text = ts;
                        }
                        ticks++;
                    }
                }
            }
            //change = true;




            if (curcycle == 2 && ticks >= END_DELAY)
            {
                ticks = 0;
                curcycle = 3;
            }
            if (curcycle == 3 && ticks < FADE_OUT_TIME)
            {
                ts = text;
                if (ticks % 60 < 30) ts += "|";
                lock (l)
                {
                    l.text = ts;
                    l.foreground = Color.White * (1f - (float)(ticks) / (float)FADE_OUT_TIME);
                }
                ticks++;
            }
            if (curcycle == 3 && ticks >= FADE_OUT_TIME)
                change = true;
            #endregion

            if (change)
            {
                change = false;
                Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_mainMenu, "GUIMain");
            }

            UpdateEnd:
            base.Update();
        }

        public override void onShow()
        {
            InterruptFade = false;
            text = "";
            l.text = "";
            l.foreground = Color.White;

            base.onShow();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Color.Black);
            lock (l)
            {
                base.Draw(renderer);
            }
        }

        bool InterruptFade = false;

        public override void FadeIn()
        {
            return;
            blockInput = false;

            int t = 0;
            String ts;
            bool tp = false;
            for (int i = 0; i < START_DELAY && !InterruptFade; i++)
            {
                ts = "";
                if (i % 1000 < 500) ts += "|";
                lock (l)
                {
                    l.text = ts;
                }
                System.Threading.Thread.Sleep(1);
            }
            for (int i = 0; i < INTRO_TEXT.Length * LETTER_DELAY && !InterruptFade; i++)
            {
                while (text.Length > 0 && text[text.Length - 1] == '.' && t < SENTENCE_DELAY && !InterruptFade)
                {
                    tp = true;
                    System.Threading.Thread.Sleep(1);
                    t ++;

                    ts = text;
                    if ((i + t) % 1000 < 500)
                        ts += "|";
                    lock (l)
                    {
                        l.text = ts;
                    }

                    //continue;
                }
                t = 0;
                if (tp)
                {
                    i += LETTER_DELAY - 1;
                    tp = false;
                }
                
                if (i % LETTER_DELAY == 0)
                    text = INTRO_TEXT.Substring(0, text.Length + 1);
                ts = text;
                if (i % 1000 < 500) ts += "|";
                lock (l)
                {
                    l.text = ts;
                }
                System.Threading.Thread.Sleep(1);
            }
            for (int i = 0; i < END_DELAY && !InterruptFade; i++)
            {
                ts = text;
                if (i % 1000 < 500) ts += "|";
                lock (l)
                {
                    l.text = ts;
                }
                System.Threading.Thread.Sleep(1);
            }
            change = true;







            for (int i = 0; i < FADE_OUT_TIME && !InterruptFade; i++)
            {
                ts = text;
                if (i % 1000 < 500) ts += "|";
                lock (l)
                {
                    l.text = ts;
                    l.foreground = Color.White * (1f - (float)(i) / (float)FADE_OUT_TIME);
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        public override void FadeOut()
        {
            blockInput = false;
        }

        #region IO
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            InterruptFade = true;
            System.Threading.Thread.Sleep(10);
            InterruptFade = false;
            Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_mainMenu, "GUIMain");
            InterruptFade = true;
            base.onButtonClick(e);
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            InterruptFade = true;
            System.Threading.Thread.Sleep(10);
            InterruptFade = false;
            Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_mainMenu, "GUIMain");
            InterruptFade = true;
            base.onKeyPressed(e);
        }
        #endregion

    }
}
