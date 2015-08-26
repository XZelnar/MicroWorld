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

namespace MicroWorld.Graphics.GUI.Background
{
    unsafe class MainMenu : Background
    {
        static Color PATH_COLOR = new Color(0, 150, 150);
        static Color CHARGE_COLOR = new Color(255, 255, 0);
        static Color JUNCTION_COLOR = new Color(1, 1, 1);
        static Color ACTIVE_JUNCTION_COLOR = new Color(255, 1, 1);
        static Color WIRE_INSULATION = new Color(50, 50, 50);
        static Color JUNCTION_INSULATION_COLOR = new Color(200, 200, 200);

        #region Seeds
        static int[] Seeds = new int[]{
            1,
218,
569,
1378,
2330,
2504,
3094,
3447,
3514,
4284,
5368,
6752,
8010,
8388,
8735,
8941,
9639,
11313,
11559,
11999,
12703,
13255,
13437,
13763,
14255,
14381,
14521,
14811,
15716,
15744,
16156,
16626,
16651,
17209,
17512,
17590,
17825,
18044,
18524,
19032,
19412,
19688,
20210,
20403,
20678,
21263,
22863,
23449,
23750,
23883,
25274,
27626,
28387,
28724,
29374,
29418,
29528,
29713,
29818,
29893,
30088,
30351,
31402,
32159,
32448,
33411,
33825,
35304,
35524,
35666,
35697,
35715,
36421,
36740,
36884,
36885,
37229,
37333,
37968,
38361,
38382,
38870,
38957,
38986,
39324,
40397,
40564,
40578,
40663,
40996,
41802,
42315,
42489,
42595,
42678,
42744,
43007,
43063,
43074,
44154,
44158,
44462,
45238,
46531,
46567,
46851,
48171,
48184,
48415,
48490,
48942,
48970,
49068,
49140,
49624,
50874,
50962,
51386,
51471,
52423,
52834,
52940,
52968,
53963,
54041,
54871,
54939,
54983,
55527,
56560,
56827,
57260,
57459,
58119,
58333,
58537,
58626,
58674,
58917,
60121,
60624,
61186,
61451,
62442,
62975,
63698,
64143,
64250,
64849,
65145,
65231,
65708,
65831,
66392,
66400,
67073,
67677,
67912,
67962,
68007,
68325,
68354,
68448,
68477,
69198,
69227,
69518,
69642,
69644,
70044,
70437,
71448,
71630,
72016,
72796,
72987,
73170,
74091,
74112,
75199,
75804,
76186,
76326,
76563,
76718,
77056,
77112,
77334,
77852,
78293,
78601,
79314,
80706,
81930,
81946,
82541,
83720,
84153,
84442,
86199,
86409,
86943,
88070,
88690,
90926,
91202,
91820,
93330,
94059,
94109,
94345,
94828,
95744,
95987,
96117,
96275,
96867,
97298,
97937,
98053,
98230,
98430,
99086,
99405,
99564,
99651
        };
        #endregion

        enum Side
        {
            Left = 0,
            LeftTop = 1,
            TopLeft = 1,
            Top = 2,
            TopRight = 3,
            RightTop = 3,
            Right = 4,
            RightDown = 5,
            DownRight = 5,
            Down = 6,
            DownLeft = 7,
            LeftDown = 7,
            None = 8
        }
        class Charge
        {
            public Vector2 position;
            public Side from;
            public short depth;

            public Charge(Vector2 pos, Side f, byte d)
            {
                from = f;
                position = pos;
                depth = d;
            }
        }

        public Texture2D bg, background, wires, chargesTexture, joints, overlay;//in order of drawing
        public Texture2D[] decs = new Texture2D[10];

        Color[] ab, abg, aw, ac, aj, ao;

        private List<Vector2> startPoints = new List<Vector2>();
        private List<Charge>[] charges = new List<Charge>[10];

        public int ticks = 0;
        private const int LINES_COUNT = 3;
        public int seed = 0;
        //public List<int> aaaa = new List<int>();

        public override void Initialize()
        {
            width = Main.WindowWidth / 2;
            height = Main.WindowHeight / 2;
            width = 400;
            height = 240;
            bg = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            wires = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            chargesTexture = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            joints = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            overlay = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            background = new Texture2D(Main.graphics.GraphicsDevice, width, height);

            //for (int i = 0; i < Seeds.Length; i++)
            //{
            //    aaaa.Add(Seeds[i]);
            //}

            Generate();

            //System.IO.StreamWriter sw = new System.IO.StreamWriter("aaa.txt");
            //for (int i = 0; i < 100000; i++)
            //{
            //    q = true;
            //    Generate();
            //    if (q) sw.WriteLine(i);
            //}
            //sw.Close();
            //seed = 23;
            //Generate();
        }

        public void Generate()
        {
            aw = new Color[width * height];
            ac = new Color[width * height];
            aj = new Color[width * height];
            ab = new Color[width * height];
            ao = new Color[width * height];
            abg = new Color[width * height];
            charges[0] = new List<Charge>();
            startPoints.Clear();

            seed = Seeds[new Random().Next(Seeds.Length * 100) % Seeds.Length];
            //seed++;
            //seed = 95987;
            //Main.LoadingDetails = seed.ToString();
            //cs++;
            //if (cs > aaaa.Count - 1) cs = aaaa.Count - 1;
            //seed = aaaa[cs];

            Random r = new Random(Main.StartMS);
            int t = 0;
            for (int i = 0; i < ab.Length; i++)
            {
                t = r.Next(-50, 51);
                if (t < 0)
                {
                    ab[i] = new Color(-t, -t, 255);
                }
                else
                {
                    ab[i] = new Color(0, 0, 255 - t);
                }
            }
            bg.SetData<Color>(ab);

            GeneratePath();

            wires.SetData<Color>(aw);

            PostCheck();
            ClearDoubleStartPoints();
            GenerateOverlay();

            for (int i = 0; i < 150; i++)
            {
                Update();
            }

            //LoadContent();
            //GenerateBackground();//lol
        }

        public void GenerateOverlay()
        {
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (aw[x + y * width] != PATH_COLOR && (
                        //aw[(x + 1) + (y + 1) * width] == PATH_COLOR ||
                        aw[(x) + (y + 1) * width] == PATH_COLOR ||
                        //aw[(x - 1) + (y + 1) * width] == PATH_COLOR ||
                        aw[(x + 1) + (y) * width] == PATH_COLOR ||
                        aw[(x - 1) + (y) * width] == PATH_COLOR ||
                        //aw[(x + 1) + (y - 1) * width] == PATH_COLOR ||
                        aw[(x) + (y - 1) * width] == PATH_COLOR))// ||
                        //aw[(x - 1) + (y - 1) * width] == PATH_COLOR))
                    {
                        ao[x + y * width] = WIRE_INSULATION;
                    }
                    if (aw[x + y * width] != PATH_COLOR && (
                        //aw[(x + 1) + (y + 1) * width] == PATH_COLOR ||
                        aj[(x) + (y + 1) * width] == JUNCTION_COLOR ||
                        //aw[(x - 1) + (y + 1) * width] == PATH_COLOR ||
                        aj[(x + 1) + (y) * width] == JUNCTION_COLOR ||
                        aj[(x - 1) + (y) * width] == JUNCTION_COLOR ||
                        //aw[(x + 1) + (y - 1) * width] == PATH_COLOR ||
                        aj[(x) + (y - 1) * width] == JUNCTION_COLOR))// ||
                    //aw[(x - 1) + (y - 1) * width] == PATH_COLOR))
                    {
                        ao[x + y * width] = JUNCTION_INSULATION_COLOR;
                    }
                }
            }
            overlay.SetData<Color>(ao);
        }

        public void GenerateBackground()
        {
            Random r = new Random();
            int x, y, t;
            bool canplace = true;
            Color[][] c = new Color[decs.Length][];
            for (int i = 0; i < decs.Length; i++)
            {
                c[i] = new Color[decs[i].Width * decs[i].Height];
                decs[i].GetData<Color>(c[i]);
            }
            for (int i = 0; i < 1000; i++)
            {
                canplace = true;
                t = r.Next(decs.Length * 100) % decs.Length;
                x = r.Next(width);
                y = r.Next(height);
                for (int tx = 0; tx < decs[t].Width; tx++)
                {
                    for (int ty = 0; ty < decs[t].Height; ty++)
                    {
                        if (x + tx < width - 1 &&
                            y + ty < height - 1)
                            if (c[t][tx + ty * decs[t].Width].A != 0 &&
                                (aw[x + tx + (y + ty) * width] == PATH_COLOR || 
                                 abg[x + tx + (y + ty) * width].A != 0))
                            {
                                canplace = false;
                                break;
                            }
                    }
                    if (!canplace) break;
                }

                if (canplace)
                {
                    for (int tx = 0; tx < decs[t].Width; tx++)
                    {
                        for (int ty = 0; ty < decs[t].Height; ty++)
                        {
                            if (x + tx < width - 1 && y + ty < height - 1)
                                abg[x + tx + (y + ty) * width] = c[t][tx + ty * decs[t].Width];
                        }
                    }
                }
            }
            background.SetData<Color>(abg);
        }

        public void LoadContent()
        {
            for (int i = 0; i < decs.Length; i++)
            {
                decs[i] = ResourceManager.Load<Texture2D>("GUI/MMDec/dec" + (i + 1).ToString());
            }
            background = ResourceManager.Load<Texture2D>("GUI/MMDec/dec");
            Main.LoadingDetails = "Initializing scenes...";
        }

        public void GeneratePath()
        {
            Random r = new Random(seed);
            for (int i = 0; i < LINES_COUNT+1; i++)//up
            {
                int x = r.Next(100, 300), y = 1;
                Side s = Side.Down;
                while (x >= 1 && y >= 1 && x < width - 2 && y < height - 2)
                {
                    DrawLine(ref x, ref y, r.Next(50, 150), s);
                    s = GetAdjacentSide(s, ref r);
                }
            }
            for (int i = 0; i < LINES_COUNT+1; i++)//down
            {
                int x = r.Next(100, 300), y = height - 3;
                Side s = Side.Top;
                while (x >= 1 && y >= 1 && x < width - 2 && y < height - 2)
                {
                    DrawLine(ref x, ref y, r.Next(50, 150), s);
                    s = GetAdjacentSide(s, ref r);
                }
            }
            for (int i = 0; i < LINES_COUNT; i++)//left
            {
                int y = r.Next(50, 150), x = 1;
                Side s = Side.Right;
                while (x >= 1 && y >= 1 && x < width - 2 && y < height - 2)
                {
                    DrawLine(ref x, ref y, r.Next(50, 150), s);
                    s = GetAdjacentSide(s, ref r);
                }
            }
            for (int i = 0; i < LINES_COUNT; i++)//right
            {
                int y = r.Next(50, 150), x = width - 3;
                Side s = Side.Left;
                while (x >= 1 && y >= 1 && x < width - 2 && y < height - 2)
                {
                    DrawLine(ref x, ref y, r.Next(50, 150), s);
                    s = GetAdjacentSide(s, ref r);
                }
            }
        }

        public void ClearDoubleStartPoints()
        {
            for (int i = 0; i < startPoints.Count - 1; i++)
            {
                for (int j = i + 1; j < startPoints.Count; j++)
                {
                    if (startPoints[i].X == startPoints[j].X && startPoints[i].Y == startPoints[j].Y)
                    {
                        startPoints.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        private Side GetAdjacentSide(Side s, ref Random r)
        {
            byte t = (byte)s;
            t = (byte)(r.Next(3) - 1 + t);
            return (Side)t;
        }

        private void DrawLine(ref int x, ref int y, int length, Side s)
        {
            if (s == Side.Left)
            {
                int t = x - length;
                if (t < 0) t = 0;
                for (; x > t; x--)
                {
                    SetPathColor(x, y, ref aw);
                }
                x++;
                return;
            }
            if (s == Side.LeftTop)
            {
                int tx = x - length;
                int ty = y - length;
                if (tx < 0)
                {
                    ty -= tx;
                    tx = 0;
                }
                if (ty < 0)
                {
                    tx -= ty;
                    ty = 0;
                }
                for (; x > tx && y > ty; x--)
                {
                    y--;
                    SetPathColor(x, y, ref aw);
                }
                x++;
                return;
            }
            if (s == Side.Top)
            {
                int t = y - length;
                if (t < 0) t = 0;
                for (; y > t; y--)
                {
                    SetPathColor(x, y, ref aw);
                }
                y++;
                return;
            }
            if (s == Side.RightTop)
            {
                int tx = x + length;
                int ty = y - length;
                if (tx > width - 1)
                {
                    ty -= (width - 1 - tx);
                    tx = width - 1;
                }
                if (ty < 0)
                {
                    tx -= ty;
                    ty = 0;
                }
                for (; x < tx && y > ty; x++)
                {
                    y--;
                    SetPathColor(x, y, ref aw);
                }
                x--;
                return;
            }
            if (s == Side.Right)
            {
                int t = x + length;
                if (t > width - 1) t = width - 1;
                for (; x < t; x++)
                {
                    SetPathColor(x, y, ref aw);
                }
                x--;
                return;
            }
            if (s == Side.RightDown)
            {
                int tx = x + length;
                int ty = y + length;
                if (tx > width - 1)
                {
                    ty -= (width - 1 - tx);
                    tx = width - 1;
                }
                if (ty > height - 1)
                {
                    tx -= (height - 1 - ty);
                    ty = height - 1;
                }
                for (; x < tx && y < ty; x++)
                {
                    y++;
                    SetPathColor(x, y, ref aw);
                }
                x--;
                return;
            }
            if (s == Side.Down)
            {
                int t = y + length;
                if (t > height - 1) t = height - 1;
                for (; y < t; y++)
                {
                    SetPathColor(x, y, ref aw);
                }
                y--;
                return;
            }
            if (s == Side.LeftDown)
            {
                int tx = x - length;
                int ty = y + length;
                if (tx < 0)
                {
                    ty -= tx;
                    tx = 0;
                }
                if (ty > height - 1)
                {
                    tx -= (height - 1 - ty);
                    ty = height - 1;
                }
                for (; x > tx && y < ty; x--)
                {
                    y++;
                    SetPathColor(x, y, ref aw);
                }
                x++;
                return;
            }
        }

        public void PostCheck()
        {
            if (Utilities.Tools.IsRunningOnMono())
                return;
            #region JunctionCheck
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (aw[x + y * width] != PATH_COLOR) continue;

                    int c = 0;
                    if (aw[x - 1 + y * width] == PATH_COLOR) c++;
                    if (aw[x + 1 + y * width] == PATH_COLOR) c++;
                    if (aw[x + (y - 1) * width] == PATH_COLOR) c++;
                    if (aw[x + (y + 1) * width] == PATH_COLOR) c++;
                    if (aw[x + 1 + (y + 1) * width] == PATH_COLOR) c++;
                    if (aw[x - 1 + (y + 1) * width] == PATH_COLOR) c++;
                    if (aw[x + 1 + (y - 1) * width] == PATH_COLOR) c++;
                    if (aw[x - 1 + (y - 1) * width] == PATH_COLOR) c++;
                    if (c > 3)
                    {
                        //ao[x - 1 + (y - 1) * width] = Color.Black;
                        //ao[x + (y - 1) * width] = Color.Black;
                        //ao[x + 1 + (y - 1) * width] = Color.Black;
                        //ao[x - 1 + (y) * width] = Color.Black;
                        aj[x + (y) * width] = JUNCTION_COLOR;
                        startPoints.Add(new Vector2(x, y));
                        continue;
                        //ao[x + 1 + (y) * width] = Color.Black;
                        //ao[x - 1 + (y + 1) * width] = Color.Black;
                        //ao[x + (y + 1) * width] = Color.Black;
                        //ao[x + 1 + (y + 1) * width] = Color.Black;
                    }
                    if (GetAColor(x - 1, y - 1) != GetAColor(x + 1, y + 1))
                    {
                        aj[x + (y) * width] = JUNCTION_COLOR;
                        startPoints.Add(new Vector2(x, y));
                        continue;
                    }
                    if (GetAColor(x, y - 1) != GetAColor(x, y + 1))
                    {
                        aj[x + (y) * width] = JUNCTION_COLOR;
                        startPoints.Add(new Vector2(x, y));
                        continue;
                    }
                    if (GetAColor(x + 1, y - 1) != GetAColor(x - 1, y + 1))
                    {
                        aj[x + (y) * width] = JUNCTION_COLOR;
                        startPoints.Add(new Vector2(x, y));
                        continue;
                    }
                    if (GetAColor(x - 1, y) != GetAColor(x + 1, y))
                    {
                        aj[x + (y) * width] = JUNCTION_COLOR;
                        startPoints.Add(new Vector2(x, y));
                        continue;
                    }
                }
            }
            #endregion



            #region ContourCheck
            for (int x = 0; x < width - 1; x++)
            {
                if (aw[x + width * 2] == PATH_COLOR)
                {
                    aj[x + width * 2] = JUNCTION_COLOR;
                    startPoints.Add(new Vector2(x, 2));
                }
                if (aw[x + (height - 3) * width] == PATH_COLOR)
                {
                    aj[x + (height - 3) * width] = JUNCTION_COLOR;
                    startPoints.Add(new Vector2(x, height - 3));
                }
            }

            for (int y = 0; y < height - 1; y++)
            {
                if (aw[y * width + 2] == PATH_COLOR)
                {
                    aj[y * width + 2] = JUNCTION_COLOR;
                    startPoints.Add(new Vector2(2, y));
                }
                if (aw[y * width + (width - 3)] == PATH_COLOR)
                {
                    aj[y * width + (width - 3)] = JUNCTION_COLOR;
                    startPoints.Add(new Vector2(width - 3, y));
                }
            }
            #endregion


            #region ReGenCheck
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int c = 0;
                    if (aj[x - 1 + y * width] == JUNCTION_COLOR) c++;
                    if (aj[x + 1 + y * width] == JUNCTION_COLOR) c++;
                    if (aj[x + (y - 1) * width] == JUNCTION_COLOR) c++;
                    if (aj[x + (y + 1) * width] == JUNCTION_COLOR) c++;
                    if (aj[x + 1 + (y + 1) * width] == JUNCTION_COLOR) c++;
                    if (aj[x - 1 + (y + 1) * width] == JUNCTION_COLOR) c++;
                    if (aj[x + 1 + (y - 1) * width] == JUNCTION_COLOR) c++;
                    if (aj[x - 1 + (y - 1) * width] == JUNCTION_COLOR) c++;
                    if (c >= 5)
                    {
                        //q = false;
                        //Generate();
                        return;
                    }
                }
            }
            #endregion



            joints.SetData<Color>(aj);
        }

        private Color GetAColor(int x, int y)
        {
            return aw[x + y * width];
        }

        public void SetPathColor(int x, int y, ref Color[] a)
        {
            if (x < 0 || y < 0 || x > width - 1 || y > height - 1) return;
            a[x + y * width] = PATH_COLOR;
            if (x <= 1 || y <= 1 || x >= width - 2 || y >= height - 2)
            {
                startPoints.Add(new Vector2(x, y));
            }
        }

        public override void Update()
        {
            //a = new Color[width * height];
            //texture.GetData<Color>(a);
            //ao = new Color[width * height];
            //overlay.GetData<Color>(ao);

            if (charges[4] != null)
            {
                for (int j = 0; j < charges[4].Count; j++)
                {
                    ac[(int)(charges[4][j].position.Y * width + charges[4][j].position.X)] =
                        PATH_COLOR;
                }
            }

            for (int i = charges.Length - 1; i > 0; i--)
            {
                charges[i] = charges[i - 1];
            }
            charges[0] = new List<Charge>();



            ticks++;
            if (ticks > 30)
            {
                ticks -= 30;
                Random r = new Random();
                charges[0].Add(new Charge(startPoints[r.Next(startPoints.Count)], Side.None, 0));
            }



            for (int i = 0; i < charges[1].Count; i++)
            {
                //*
                if (aj[(int)(charges[1][i].position.X + charges[1][i].position.Y * width)] != JUNCTION_COLOR &&
                    charges[1][i].from != Side.None)
                {
                    Vector2 v = SideToVector(charges[1][i].from);
                    InvertVector(ref v);
                    Vector2 pos = charges[1][i].position + v;
                    if (pos.X < 0 || pos.Y < 0 || pos.X > width - 1 || pos.Y > height - 1) continue;
                    if (aw[(int)((pos.Y) * width + pos.X)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + v, charges[1][i].from,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                    continue;
                }//*/
                int start = charges[0].Count;
                //byte mul = 0;

                if (charges[1][i].depth > 7) continue;
                #region SideChecks
                //Left
                if (charges[1][i].from != Side.Left && charges[1][i].position.X > 0)
                {
                    if (aw[(int)(charges[1][i].position.Y * width + charges[1][i].position.X - 1)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + new Vector2(-1, 0), Side.Right,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                }
                //TopLeft
                if (charges[1][i].from != Side.TopLeft && charges[1][i].position.X > 0 && charges[1][i].position.Y > 0)
                {
                    if (aw[(int)((charges[1][i].position.Y - 1) * width + charges[1][i].position.X - 1)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + new Vector2(-1, -1), Side.RightDown,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                }
                //Top
                if (charges[1][i].from != Side.Top && charges[1][i].position.Y > 0)
                {
                    if (aw[(int)((charges[1][i].position.Y - 1) * width + charges[1][i].position.X)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + new Vector2(0, -1), Side.Down,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                }
                //TopRight
                if (charges[1][i].from != Side.TopRight && charges[1][i].position.X < width - 1 && charges[1][i].position.Y > 0)
                {
                    if (aw[(int)((charges[1][i].position.Y - 1) * width + charges[1][i].position.X + 1)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + new Vector2(1, -1), Side.LeftDown,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                }
                //Right
                if (charges[1][i].from != Side.Right && charges[1][i].position.X < width - 1)
                {
                    if (aw[(int)(charges[1][i].position.Y * width + charges[1][i].position.X + 1)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + new Vector2(1, 0), Side.Left,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                }
                //BottomRight
                if (charges[1][i].from != Side.DownRight && charges[1][i].position.X < width - 1 && charges[1][i].position.Y < height - 1)
                {
                    if (aw[(int)((charges[1][i].position.Y + 1) * width + charges[1][i].position.X + 1)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + new Vector2(1, 1), Side.LeftTop,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                }
                //Bottom
                if (charges[1][i].from != Side.Down && charges[1][i].position.Y < height - 1)
                {
                    if (aw[(int)((charges[1][i].position.Y + 1) * width + charges[1][i].position.X)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + new Vector2(0, 1), Side.Top,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                }
                //BottomLeft
                if (charges[1][i].from != Side.DownLeft && charges[1][i].position.X > 0 && charges[1][i].position.Y < height - 1)
                {
                    if (aw[(int)((charges[1][i].position.Y + 1) * width + charges[1][i].position.X - 1)] == PATH_COLOR)
                    {
                        charges[0].Add(new Charge(charges[1][i].position + new Vector2(-1, 1), Side.RightTop,
                            (byte)(charges[1][i].depth)));
                        //mul++;
                    }
                }
                #endregion

                if (aj[(int)(charges[1][i].position.X + charges[1][i].position.Y * width)] == JUNCTION_COLOR)
                {
                    for (int j = start; j < charges[0].Count; j++)
                    {
                        charges[0][j].depth++;
                    }
                }
            }


            for (int i = 0; i < charges.Length; i++)
            {
                if (charges[i] != null)
                {
                    float opacity = (1 - ((float)i / (charges.Length - 1)));
                    for (int j = 0; j < charges[i].Count; j++)
                    {
                        ac[(int)(charges[i][j].position.Y * width + charges[i][j].position.X)] =
                            CHARGE_COLOR * opacity;
                    }
                }
            }

            chargesTexture.SetData<Color>(ac);

            JointsOverlayUpdate();
        }

        public void JointsOverlayUpdate()
        {
            for (int i = 0; i < startPoints.Count; i++)
            {
                Color c = ac[(int)(startPoints[i].X + startPoints[i].Y * width)];
                if (c != Color.Black * 0f)
                {
                    ao[(int)(startPoints[i].X + startPoints[i].Y * width)] = ACTIVE_JUNCTION_COLOR * ((float)c.A / 255f);
                }
                else
                {
                    ao[(int)(startPoints[i].X + startPoints[i].Y * width)] = JUNCTION_COLOR;
                }
            }
            overlay.SetData<Color>(ao);
        }

        private void InvertVector(ref Vector2 v)
        {
            v.X *= -1;
            v.Y *= -1;
        }

        private Vector2 SideToVector(Side s)
        {
            if (s == Side.Left) return new Vector2(-1, 0);
            if (s == Side.LeftTop) return new Vector2(-1, -1);
            if (s == Side.Top) return new Vector2(0, -1);
            if (s == Side.TopRight) return new Vector2(1, -1);
            if (s == Side.Right) return new Vector2(1, 0);
            if (s == Side.RightDown) return new Vector2(1, 1);
            if (s == Side.Down) return new Vector2(0, 1);
            if (s == Side.DownLeft) return new Vector2(-1, 1);
            if (s == Side.None) return new Vector2(0, 0);
            return new Vector2();
        }

        public override void Draw(Renderer renderer)
        {
            Main.renderer.Draw(bg,
                new Rectangle(0, 0, Main.window.ClientBounds.Width, Main.window.ClientBounds.Height),
                new Rectangle(2, 2, width - 4, height - 4), Color.White);
            //Main.renderer.Draw(background,
            //    new Rectangle(0, 0, Main.window.ClientBounds.Width, Main.window.ClientBounds.Height),
            //    new Rectangle(2, 2, width - 4, height - 4), new Color(0,150,0) * 0.9f);
            Main.renderer.Draw(wires,
                new Rectangle(0, 0, Main.window.ClientBounds.Width, Main.window.ClientBounds.Height),
                new Rectangle(2, 2, width - 4, height - 4), Color.White);
            Main.renderer.Draw(chargesTexture,
                new Rectangle(0, 0, Main.window.ClientBounds.Width, Main.window.ClientBounds.Height),
                new Rectangle(2, 2, width - 4, height - 4), Color.White);
            if (!Utilities.Tools.IsRunningOnMono())
                Main.renderer.Draw(joints,
                    new Rectangle(0, 0, Main.window.ClientBounds.Width, Main.window.ClientBounds.Height),
                    new Rectangle(2, 2, width - 4, height - 4), Color.White);
            Main.renderer.Draw(overlay,
                new Rectangle(0, 0, Main.window.ClientBounds.Width, Main.window.ClientBounds.Height),
                new Rectangle(2, 2, width - 4, height - 4), Color.White);
        }

    }
}
