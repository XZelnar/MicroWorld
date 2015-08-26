using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MicroWorld.Graphics
{
    public unsafe class LiquidSystem
    {
        public static readonly Vector2 TileSize = new Vector2(4, 4);
        public static readonly int MaxLevel = 4;

        internal unsafe struct TileMapElement
        {

            public int level;
            public bool Free;
            internal bool updated;
            internal int x, y;

            public TileMapElement(int x, int y, bool free = true, int level = 0)
            {
                this.x = x;
                this.y = y;
                Free = free;
                this.level = level;
                updated = false;
            }

            public override string ToString()
            {
                return "Level = " + level.ToString();
            }
        }



        private int W = 0, H = 0;
        private TileMapElement[,] tileMap = new TileMapElement[0, 0];

        public LiquidSystem(int w, int h)
        {
            W = w;
            H = h;

            tileMap = new TileMapElement[W, H];

            fixed (TileMapElement* tptr = tileMap)
            {
                for (int x = 0; x < W; x++)
                    for (int y = 0; y < H; y++)
                    {
                        l = tptr + y + x * H;
                        (*l).x = x;
                        (*l).y = y;
                        (*l).Free = true;
                    }
            }
        }

        public void AddLiquid(int amount, Vector2 pos)
        {
            if (amount <= 0)
                return;

            if (pos.X < 0)
                pos.X = 0;
            if (pos.Y < 0)
                pos.Y = 0;
            if (pos.X >= W)
                pos.X = W - 1;
            if (pos.Y >= H)
                pos.Y = H - 1;

            fixed (TileMapElement* tptr = tileMap)
                for (int i = 0; i < W * H; i++)
                    (*(tptr + i)).updated = false;

            List<long> open = new List<long>();
            int curind = 0;
            fixed (TileMapElement* ptrmap = tileMap)
            {
                open.Add((long)(ptrmap + (int)pos.X * H + (int)pos.Y));
                while (open.Count > curind && amount > 0)
                {
                    a = (TileMapElement*)((long)open[curind]);
                    curind++;
                    //(*a).updated = true;

                    if ((*a).Free && (*a).level < MaxLevel)
                    {
                        if ((*a).level + amount >= MaxLevel)
                        {
                            wasChanged = true;
                            amount -= (MaxLevel - (*a).level);
                            (*a).level = MaxLevel;
                        }
                        else
                        {
                            wasChanged = true;
                            (*a).level += amount;
                            amount = 0;
                        }
                    }

                    if ((*a).x > 0 && !(*(l = a - H)).updated)//left
                    {
                        open.Add((long)l);
                        (*l).updated = true;
                    }
                    if ((*a).x < W - 1 && !(*(l = a + H)).updated)//right
                    {
                        open.Add((long)l);
                        (*l).updated = true;
                    }
                    if ((*a).y > 0 && !(*(l = a - 1)).updated)//up
                    {
                        open.Add((long)l);
                        (*l).updated = true;
                    }
                    if ((*a).y < H - 1 && !(*(l = a + 1)).updated)//down
                    {
                        open.Add((long)l);
                        (*l).updated = true;
                    }
                }
            }
        }

        public void RemoveFromBottom(int amount)
        {
            if (amount <= 0)
                return;
            wasChanged = true;

            int t = amount > W * 4 ? 4 : amount / 4 / W + 1;

            fixed (TileMapElement* ptrmap = tileMap)
            {
                while (true)//for if t < 4 and not enough after all passes
                {
                    for (int y = H - 1; y >= 0; y--)
                        for (int x = 0; x < W; x++)
                        {
                            if ((*(a = ptrmap + y + x * H)).Free && (*a).level > 0)
                            {
                                if (t >= amount && (*a).level >= amount)
                                {
                                    (*a).level -= amount;
                                    return;
                                }
                                else
                                {
                                    amount -= t;
                                    (*a).level -= t;
                                }
                            }
                        }
                }
            }
        }

        public void RemoveFromTop(int amount)
        {
            if (amount <= 0)
                return;
            wasChanged = true;

            fixed (TileMapElement* ptrmap = tileMap)
            {
                for (int y = 0; y < H; y++)
                    for (int x = 0; x < W; x++)
                    {
                        if ((*(a = ptrmap + y + x * H)).Free && (*a).level > 0)
                        {
                            if ((*a).level > amount)
                            {
                                (*a).level -= amount;
                                return;
                            }
                            else
                            {
                                amount -= (*a).level;
                                (*a).level = 0;
                            }
                        }
                    }
            }
        }

        public void SetAmount(int amount)
        {
            if (amount < 0)
                return;

            if (amount == 0)
            {
                fixed (TileMapElement* ptrmap = tileMap)
                {
                    for (int i = 0; i < W*H; i++)
                    {
                        (*(ptrmap + i)).level = 0;
                    }
                    return;
                }
            }

            wasChanged = true;

            fixed (TileMapElement* ptrmap = tileMap)
            {
                for (int y = H - 1; y >= 0 && amount > 0; y--)
                    for (int x = 0; x < W && amount > 0; x++)
                    {
                        if ((*(a = ptrmap + y + x * H)).Free)
                        {
                            if (amount > MaxLevel)
                            {
                                (*a).level = MaxLevel;
                                amount -= MaxLevel;
                            }
                            else
                            {
                                (*a).level = amount;
                                amount = 0;
                            }
                        }
                    }
            }
        }

        public int GetHeightAt(int x)
        {
            if (x < 0)
                x = 0;
            if (x >= W)
                x = W - 1;

            fixed (TileMapElement* ptrmap = tileMap)
            {
                int h = 0;
                for (int y = H - 1; y >= 0; y--)
                {
                    if ((*(a = ptrmap + y + x * H)).Free && (*a).level == 4)
                        h++;
                    else
                        return h;
                }
                return h;
            }
        }

        public void Update()
        {
            _updatePass();
            _updatePass();
            _updatePass();
        }

        TileMapElement* a, l, r, d;
        bool wasChanged = false;
        int updateSideSwapper = 0;
        //TODO optimize
        private unsafe void _updatePass()
        {
            if (!wasChanged)
                return;
            wasChanged = false;

            int x, y;
            updateSideSwapper++;
            if (updateSideSwapper >= 3)
                updateSideSwapper = 0;
            bool ussb = updateSideSwapper > 0;
            int inc = ussb ? 1 : -1;
            int start = ussb ? 0 : W - 1;
            fixed (TileMapElement* ptrmap = tileMap)
            {
                for (y = H - 1; y >= 0; y--)
                {
                    //swapps between starting from left and starting from right
                    for (x = start; (ussb && x < W) || (!ussb && x >= 0); x += inc)
                    {
                        a = ptrmap + y + x * H;
                        if ((*a).level != 0)
                        {
                            l = x > 0 ? ptrmap + y + (x - 1) * H : null;
                            r = x < W - 1 ? ptrmap + y + (x + 1) * H : null;
                            d = y < H - 1 ? ptrmap + (y + 1) + x * H : null;

                            l = (l != null && (*l).Free) ? l : null;
                            r = (r != null && (*r).Free) ? r : null;
                            d = (d != null && (*d).Free) ? d : null;

                            if (d != null && (*d).level != MaxLevel)//drop down
                            {
                                wasChanged = true;
                                if ((*d).level + (*a).level <= MaxLevel)
                                {
                                    (*d).level += (*a).level;
                                    (*a).level = 0;
                                    continue;
                                }
                                else
                                {
                                    (*a).level -= MaxLevel - (*d).level;
                                    (*d).level = MaxLevel;
                                }
                            }

                            //split left and right

                            if (r != null)
                            {
                                if (l == null)
                                {
                                    if ((*a).level < 2 && (*r).level < 2)
                                        continue;
                                    int lev = (*r).level + (*a).level;
                                    if ((*a).level != lev / 2)
                                        wasChanged = true;
                                    (*r).level = lev / 2;
                                    (*a).level = (*r).level;
                                    if (lev % 2 == 1)
                                        (*r).level++;
                                    continue;
                                }
                                else
                                {
                                    if ((*a).level < 2 && (*r).level < 2 && (*l).level < 2)
                                        continue;
                                    int lev = (*l).level + (*a).level + (*r).level;
                                    if ((*a).level != lev / 3)
                                        wasChanged = true;
                                    (*l).level = lev / 3;
                                    (*r).level = (*l).level;
                                    (*a).level = (*l).level;
                                    if (lev % 3 >= 1)
                                        (*r).level++;
                                    if (lev % 3 == 2)
                                        (*l).level++;
                                    continue;
                                }
                            }

                            if (l != null)
                            {
                                if ((*a).level < 2 && (*l).level < 2)
                                    continue;
                                int lev = (*l).level + (*a).level;
                                if ((*a).level != lev / 2)
                                    wasChanged = true;
                                (*l).level = lev / 2;
                                (*a).level = (*l).level;
                                if (lev % 2 == 1)
                                    (*l).level++;
                                continue;
                            }
                        }
                    }
                }
            }
        }

        //TODO optimize
        public void Render(RenderTarget2D fbo, RenderTarget2D bufferFBO, Renderer renderer, Texture2D distortionTexture = null)
        {
            if (fbo == null || bufferFBO == null || fbo.Width != W || fbo.Height != H || bufferFBO.Width != W || bufferFBO.Height != H)
                return;
            if (distortionTexture == null)
                distortionTexture = MicroWorld.Graphics.Effects.Effects.liquidDistortionTexture;

            bool b = renderer.IsDrawing;
            bool s = renderer.IsScaeld;
            var cfbo = renderer.CurFBO;
            if (b)
                renderer.End();
            if (cfbo == bufferFBO)
                renderer.DisableFBO();
            renderer.GraphicsDevice.Textures[0] = null;

            Color[] fboarr = new Color[W * H];

            fixed(TileMapElement* ptrmap = tileMap)
            fixed (Color* ptrfbo = fboarr)
            {
                for (int x = 0; x < W; x++)
                {
                    for (int y = 0; y < H; y++)
                    {
                        *(ptrfbo + x + y * W) = (*(ptrmap + y + x * H)).level <= 1 ? Color.Transparent : Color.White;
                    }
                }
            }
            bufferFBO.SetData<Color>(fboarr);

            renderer.EnableFBO(fbo);
            renderer.GraphicsDevice.Clear(Color.Transparent);
            _setUpShader(distortionTexture);
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
                MicroWorld.Graphics.Effects.Effects.liquid);
            renderer.Draw(bufferFBO, new Vector2(), new Color(0, 108, 255));
            renderer.End();

            if (cfbo == fbo)
                renderer.EnableFBO(bufferFBO);
            if (b)
                renderer.Begin(s);
        }

        private void _setUpShader(Texture2D dist)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                W, H, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            MicroWorld.Graphics.Effects.Effects.liquid.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            MicroWorld.Graphics.Effects.Effects.liquid.Parameters["dist"].SetValue(dist);
            MicroWorld.Graphics.Effects.Effects.liquid.Parameters["pixel"].SetValue(new float[] { 1f / W, 1f / H });
            MicroWorld.Graphics.Effects.Effects.liquid.Parameters["phase"].SetValue((float)(Main.Ticks % 1200) / 600f * (float)Math.PI);
            MicroWorld.Graphics.Effects.Effects.liquid.Parameters["pi"].SetValue((float)Math.PI);
        }
    }
}