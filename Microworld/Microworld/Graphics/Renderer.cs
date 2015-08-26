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

namespace MicroWorld.Graphics
{
    public unsafe class Renderer
    {
        public enum TextAlignment
        {
            Left = 0,
            Center = 1,
            Right = 2
        }

        public Color Overlay = Color.White;
        //internal Effect ForcedEffect = null;

        private SpriteBatch sb;
        bool isScaeld = false;
        bool isDrawing = false;

        public readonly BlendState Multiply = new BlendState
        {
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.InverseSourceAlpha,
            ColorBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.SourceAlpha,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        }; 

        internal Renderer(SpriteBatch spriteBatch)
        {
            sb = spriteBatch;
            MaskCreate();
        }

        internal void OnResolutionChanged()
        {
            MaskCreate();
        }

        public bool IsDrawing
        {
            get { return isDrawing; }
        }

        public bool IsScaeld
        {
            get { return isScaeld; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return sb.GraphicsDevice;
            }
        }

        public Matrix GetTransformMatrix()
        {
            return Matrix.CreateTranslation(Settings.GameOffset.X, Settings.GameOffset.Y, 0) * Matrix.CreateScale(Settings.GameScale);
        }

        #region BeginMethods
        public void BeginUnscaled()
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
            isScaeld = false;
            isDrawing = true;
        }

        public void BeginUnscaled(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthState, RasterizerState rasterizerState)
        {
            sb.Begin(sortMode, blendState, samplerState, depthState, rasterizerState);
            isScaeld = false;
            isDrawing = true;
        }

        public void BeginUnscaled(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthState, RasterizerState rasterizerState, Effect e)
        {
            sb.Begin(sortMode, blendState, samplerState, depthState, rasterizerState, e);
            isScaeld = false;
            isDrawing = true;
        }

        public void BeginUnscaled(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthState, RasterizerState rasterizerState, Effect e, Matrix m)
        {
            sb.Begin(sortMode, blendState, samplerState, depthState, rasterizerState, e, m);
            isScaeld = false;
            isDrawing = true;
        }

        public void Begin()
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                DepthStencilState.Default, RasterizerState.CullNone, null,// Matrix.CreateScale(Settings.GameScale));
                Matrix.CreateTranslation(Settings.GameOffset.X, Settings.GameOffset.Y, 0) * Matrix.CreateScale(Settings.GameScale));
            isScaeld = true;
            isDrawing = true;
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthState, RasterizerState rasterizerState)
        {
            sb.Begin(sortMode, blendState, samplerState, depthState, rasterizerState, null,// Matrix.CreateScale(Settings.GameScale));
                Matrix.CreateTranslation(Settings.GameOffset.X, Settings.GameOffset.Y, 0) * Matrix.CreateScale(Settings.GameScale));
            isScaeld = true;
            isDrawing = true;
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthState, RasterizerState rasterizerState, Effect e)
        {
            sb.Begin(sortMode, blendState, samplerState, depthState, rasterizerState, e,// Matrix.CreateScale(Settings.GameScale));
                Matrix.CreateTranslation(Settings.GameOffset.X, Settings.GameOffset.Y, 0) * Matrix.CreateScale(Settings.GameScale));
            isScaeld = true;
            isDrawing = true;
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthState, RasterizerState rasterizerState, Effect e, Matrix m, bool LeftMatrix)
        {
            if (LeftMatrix)
            {
                sb.Begin(sortMode, blendState, samplerState, depthState, rasterizerState, e,
                    m * Matrix.CreateTranslation(Settings.GameOffset.X, Settings.GameOffset.Y, 0) * Matrix.CreateScale(Settings.GameScale));
            }
            else
            {
                sb.Begin(sortMode, blendState, samplerState, depthState, rasterizerState, e,
                    Matrix.CreateTranslation(Settings.GameOffset.X, Settings.GameOffset.Y, 0) * Matrix.CreateScale(Settings.GameScale) * m);
            }
            isScaeld = true;
            isDrawing = true;
        }

        public void Begin(bool scaled)
        {
            if (scaled) Begin();
            else BeginUnscaled();
            isDrawing = true;
        }
        #endregion

        public void Clear(Color c)
        {
            GraphicsDevice.Clear(c);
        }

        private Color MulColorByOverlay(Color c)
        {
            return Overlay == Color.White ? c : new Color(c.ToVector4() * Overlay.ToVector4());
        }

        #region DrawMethods
        public void Draw(Rectangle dest, Color c)
        {
            sb.Draw(GraphicsEngine.pixel, dest, MulColorByOverlay(c));
        }

        public void Draw(Texture2D texture, Rectangle dest, Color c)
        {
            sb.Draw(texture, dest, MulColorByOverlay(c));
        }

        public void Draw(Texture2D texture, float x, float y, float w, float h, Color c)
        {
            if (w == h && texture.Width == texture.Height)
                sb.Draw(texture, new Vector2(x, y), null, c, 0, new Vector2(), w / texture.Width, SpriteEffects.None, 0);
            else
                sb.Draw(texture, new Rectangle((int)x, (int)y, (int)w, (int)h), MulColorByOverlay(c));
        }

        public void Draw(Texture2D texture, Vector2 pos, Color c)
        {
            if (texture != null && !texture.IsDisposed)
                sb.Draw(texture, pos, MulColorByOverlay(c));
        }

        public void Draw(Texture2D texture, Vector2 pos, Vector2 size, Color c)
        {
            sb.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), MulColorByOverlay(c));
        }

        public void Draw(Texture2D texture, Rectangle dest, Rectangle? src, Color c)
        {
            sb.Draw(texture, dest, src, MulColorByOverlay(c));
        }

        public void Draw(Texture2D texture, Vector2 pos, Rectangle? src, Color c)
        {
            sb.Draw(texture, pos, src, MulColorByOverlay(c));
        }

        public void Draw(Texture2D texture, Rectangle dest, Rectangle? src, Color c,
            float rotation, Vector2 origin)
        {
            sb.Draw(texture, dest, src, MulColorByOverlay(c), rotation, origin, SpriteEffects.None, 0);
        }

        public void Draw(Texture2D texture, Rectangle dest, Rectangle? src, Color c, 
            float rotation, Vector2 origin, SpriteEffects effects, float layer)
        {
            sb.Draw(texture, dest, src, MulColorByOverlay(c), rotation, origin, effects, layer);
        }

        public void Draw(Texture2D texture, Vector2 pos, Rectangle? src, Color c,
            float rotation, Vector2 origin, float scale)
        {
            sb.Draw(texture, pos, src, MulColorByOverlay(c), rotation, origin, scale, SpriteEffects.None, 0);
        }

        public void Draw(Texture2D texture, Vector2 pos, Rectangle? src, Color c,
            float rotation, Vector2 origin, float scale, SpriteEffects effects, float layer)
        {
            sb.Draw(texture, pos, src, MulColorByOverlay(c), rotation, origin, scale, effects, layer);
        }

        public void Draw(Texture2D texture, Vector2 pos, Rectangle? src, Color c,
            float rotation, Vector2 origin, Vector2 scale)
        {
            sb.Draw(texture, pos, src, MulColorByOverlay(c), rotation, origin, scale, SpriteEffects.None, 0);
        }

        public void Draw(Texture2D texture, Vector2 pos, Rectangle? src, Color c,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layer)
        {
            sb.Draw(texture, pos, src, MulColorByOverlay(c), rotation, origin, scale, effects, layer);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color c1, Color c2)
        {
            bool a = isDrawing;
            bool b = isScaeld;
            if (isDrawing) End();

            GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.LineStrip,
                new VertexPositionColorTexture[]{
                    new VertexPositionColorTexture(new Vector3(x1, y1, 0),
                        c1, new Vector2()),
                    new VertexPositionColorTexture(new Vector3(x2, y2, 0),
                        c2, new Vector2())},
                    0, 1);

            if (a) Begin(b);
        }

        public void DrawLinesList(VertexPositionColorTexture[] v)
        {
            bool a = isDrawing;
            bool b = isScaeld;
            if (isDrawing) End();

            GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList,
                v, 0, v.Length / 2);

            if (a) Begin(b);
        }
        #endregion

        #region Scissors
        private bool ScissorsWasScaled = false;
        private Rectangle? PrevScissors = null;

        public void SetScissorRectangle(float x, float y, float w, float h, bool ShouldBeScaled)
        {
            ScissorsWasScaled = Main.renderer.IsScaeld;
            bool d = IsDrawing;
            if(isDrawing)
                Main.renderer.End();
            PrevScissors = Main.graphics.GraphicsDevice.ScissorRectangle;
            Vector2 p = new Vector2(
                ShouldBeScaled ? (int)(x * Settings.GameScale) : (int)x,
                ShouldBeScaled ? (int)(y * Settings.GameScale) : (int)y);
            Vector2 s = new Vector2(
                ShouldBeScaled ? (int)(w * Settings.GameScale) : (int)w,
                ShouldBeScaled ? (int)(h * Settings.GameScale) : (int)h);
            if (s.X + p.X > Main.graphics.PreferredBackBufferWidth)
            {
                s.X = Main.graphics.PreferredBackBufferWidth - p.X;
            }
            if (s.Y + p.Y > Main.graphics.PreferredBackBufferHeight)
            {
                s.Y = Main.graphics.PreferredBackBufferHeight - p.Y;
            }
            Main.graphics.GraphicsDevice.ScissorRectangle = new Rectangle((int)p.X, (int)p.Y, (int)s.X, (int)s.Y);
            if (d)
            {
                if (!ShouldBeScaled)
                {
                    Main.renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                        null, Graphics.GraphicsEngine.s_ScissorsOn);
                }
                else
                {
                    Main.renderer.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                        null, Graphics.GraphicsEngine.s_ScissorsOn);
                }
            }
        }

        public void ResetScissorRectangle()
        {
            if (PrevScissors == null) return;

            bool d = IsDrawing;
            if (d)
                Main.renderer.End();
            Main.graphics.GraphicsDevice.ScissorRectangle = (Rectangle)PrevScissors;
            if (d)
                Main.renderer.Begin(ScissorsWasScaled);

            PrevScissors = null;
        }
        #endregion

        #region DrawStringMethods
        //TODO overrides
        public void DrawStringLeft(SpriteFont sf, String s, Vector2 pos, Color c)
        {
            try
            {
                sb.DrawString(sf, s, pos, c);
            }
            catch { }
        }

        public unsafe void DrawStringCentered(SpriteFont sf, String s, Rectangle rec, Color c)
        {
            var a = s.Split('\n');
            Vector2 r;
            int cy = (int)rec.Y;
            int i;
            for (i = 0; i < a.Length - 1; i++)
            {
                a[i] = a[i].Substring(0, a[i].Length - 1);
                r = sf.MeasureString(a[i]);
                DrawStringLeft(sf, a[i], new Vector2((int)(rec.X + (rec.Width - r.X) / 2), cy), c);
                cy += (int)r.Y;
            }
            r = sf.MeasureString(a[i]);
            DrawStringLeft(sf, a[i], new Vector2((int)(rec.X + (rec.Width - r.X) / 2), cy), c);
        }

        public unsafe void DrawStringRight(SpriteFont sf, String s, Rectangle rec, Color c)
        {
            var a = s.Split('\n');
            Vector2 r;
            int cy = (int)rec.Y;
            int i;
            for (i = 0; i < a.Length - 1; i++)
            {
                a[i] = a[i].Substring(0, a[i].Length - 1);
                r = sf.MeasureString(a[i]);
                DrawStringLeft(sf, a[i], new Vector2(rec.X + rec.Width - r.X, cy), c);
                cy += (int)r.Y;
            }
            r = sf.MeasureString(a[i]);
            DrawStringLeft(sf, a[i], new Vector2(rec.X + rec.Width - r.X, cy), c);
        }

        public unsafe void DrawString(SpriteFont sf, String s, Rectangle rec, Color c, TextAlignment a)
        {
            if (a == TextAlignment.Center)
            {
                DrawStringCentered(sf, s, rec, c);
                return;
            }
            if (a == TextAlignment.Left)
            {
                DrawStringLeft(sf, s, new Vector2(rec.X, rec.Y), c);
                return;
            }
            if (a == TextAlignment.Right)
            {
                DrawStringRight(sf, s, rec, c);
                return;
            }
        }

        public unsafe void DrawString(SpriteFont sf, String s, Vector2 pos, Color c, TextAlignment a)
        {
            if (a == TextAlignment.Center)
            {
                var t = sf.MeasureString(s);
                DrawStringCentered(sf, s, new Rectangle((int)pos.X, (int)pos.Y, (int)t.X, (int)t.Y), c);
                return;
            }
            if (a == TextAlignment.Left)
            {
                DrawStringLeft(sf, s, pos, c);
                return;
            }
            if (a == TextAlignment.Right)
            {
                var t = sf.MeasureString(s);
                DrawStringRight(sf, s, new Rectangle((int)pos.X, (int)pos.Y, (int)t.X, (int)t.Y), c);
                return;
            }
        }
        #endregion

        #region FBO
        RenderTarget2D curFBO;
        public RenderTarget2D CurFBO
        {
            get { return curFBO; }
        }
        public RenderTarget2D CreateFBO(int w, int h)
        {
            RenderTarget2D FBOTexture;
            FBOTexture = new RenderTarget2D(
                Main.renderer.GraphicsDevice,
                (int)w,
                (int)h,
                false,
                Main.renderer.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None);
            return FBOTexture;
        }

        public RenderTarget2D CreateFBOWStencil(int w, int h)
        {
            RenderTarget2D FBOTexture;
            FBOTexture = new RenderTarget2D(
                Main.renderer.GraphicsDevice,
                (int)w,
                (int)h,
                false,
                Main.renderer.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24Stencil8);
            return FBOTexture;
        }

        public void EnableFBO(RenderTarget2D FBOTexture)
        {
            if (isDrawing) End();
            Main.renderer.GraphicsDevice.SetRenderTarget(FBOTexture);
            Main.renderer.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };
            curFBO = FBOTexture;
        }

        public void DisableFBO()
        {
            if (isDrawing) End();
            Main.renderer.GraphicsDevice.SetRenderTarget(null);
            curFBO = null;
        }
        #endregion

        #region Masking
        DepthStencilState s1, s2;
        AlphaTestEffect a;
        bool IsMaskBeingUsed = false;
        bool IsMaskBeingDrawnOn = false;
        public void MaskCreate()
        {
            MaskClear();

            var m = Matrix.CreateOrthographicOffCenter(0,
                sb.GraphicsDevice.PresentationParameters.BackBufferWidth,
                sb.GraphicsDevice.PresentationParameters.BackBufferHeight,
                0, 0, 1
            );

            a = new AlphaTestEffect(sb.GraphicsDevice)
            {
                Projection = m
            };

            s1 = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };

            s2 = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Equal,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 1,
                DepthBufferEnable = false
            };
        }

        public void MaskClear()
        {
            GraphicsDevice.Clear(ClearOptions.Stencil, Color.Transparent, 0, 0);
        }

        public void MaskBeginDrawOn()
        {
            MaskBeginDrawOnScaled();
        }

        public void MaskBeginDrawOnScaled()
        {
            if (isDrawing) End();
            Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, s1, null, a);
            IsMaskBeingDrawnOn = true;
        }

        public void MaskBeginDrawOnUnScaled()
        {
            if (isDrawing) End();
            BeginUnscaled(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, s1, null, a);
            IsMaskBeingDrawnOn = true;
        }

        public void MaskUse()
        {
            if (isDrawing) End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, s2, null, a);
            IsMaskBeingUsed = true;
        }

        public void MaskUseScaled()
        {
            if (isDrawing) End();
            Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, s2, null, a);
            IsMaskBeingUsed = true;
        }

        public void MaskStopUse()
        {
            if (isDrawing) End();
        }
        #endregion

        #region PushPop
        //rasterizers and stuff
        struct RendererState
        {
            //state
            public bool IsDrawing;
            public bool IsScaled;
            //stencil
            public DepthStencilState s1, s2;
            public AlphaTestEffect a;
            public bool MaskBeingUsed;
            public bool MaskBeingDrawnOn;
            //FBO
            public RenderTarget2D FBO;
            //overlay
            public Color Overlay;
        }

        List<RendererState> states = new List<RendererState>();
        public void Push()
        {
            if (states.Count >= 10)
            {
                throw new Exception("Maximum amount (10) of saved states was reached.");
            }
            RendererState s = new RendererState();
            //states
            s.IsDrawing = IsDrawing;
            s.IsScaled = IsScaeld;
            //stencil
            s.s1 = s1;
            s.s2 = s2;
            s.a = a;
            s.MaskBeingDrawnOn = IsMaskBeingDrawnOn;
            s.MaskBeingUsed = IsMaskBeingUsed;
            //FBO
            s.FBO = curFBO;
            //overlay
            s.Overlay = Overlay;

            states.Add(s);
        }

        public void Pop()
        {
            if (states.Count == 0)
                return;
            if (IsDrawing)
                End();

            var s = states[states.Count - 1];
            states.RemoveAt(states.Count - 1);

            s1 = s.s1;
            s2 = s.s2;
            a = s.a;

            Overlay = s.Overlay;

            if (s.IsDrawing)
            {
                if (s.MaskBeingUsed)
                {
                    if (s.IsScaled)
                        MaskUseScaled();
                    else
                        MaskUse();
                }
                else
                {
                    if (s.MaskBeingDrawnOn)
                    {
                        if (s.IsScaled)
                            MaskBeginDrawOnScaled();
                        else
                            MaskBeginDrawOn();
                    }
                    else
                        if (s.FBO != null)
                            EnableFBO(s.FBO);
                    Begin(s.IsScaled);
                }
            }
        }
        #endregion

        public void End()
        {
            sb.End();
            isDrawing = false;
            IsMaskBeingDrawnOn = false;
            IsMaskBeingUsed = false;
        }
    }
}
