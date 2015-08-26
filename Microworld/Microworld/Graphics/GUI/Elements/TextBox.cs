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

namespace MicroWorld.Graphics.GUI.Elements
{
    //V2.0
    public class TextBox : Control
    {
        public enum InputMask
        {
            All = 0,
            Numbers = 1
        }

        public const int HISTORY_LENGTH = 100;

        #region Variables
        private String DrawText = "";
        private String text = "";
        private String[] DrawTextLines = new String[0];
        private InputMask mask = InputMask.All;
        private SpriteFont font = null;
        private Vector2 charSize = new Vector2(), twoCharSize = new Vector2();
        private String[] history = new String[HISTORY_LENGTH];
        private String[] redoHistory = new String[HISTORY_LENGTH];
        private int maxLength = 0;
        private bool multiline = true;
        private int cursor = 0;
        private Vector2 cursorPos = new Vector2();
        private int selectionStart = -1;
        private int selectionLength = 0;
        private Vector2 drawTextSize = new Vector2();
        private Vector2 SizePerOneSymbol = new Vector2();//how much 1 symbol and 1 line add to size
        private bool IsSelecting = false;
        private int SelectionButton = 0;
        private bool isMouseSelection = false;
        private bool wasSymbolInputThisTick = false;

        internal Vector2 TextOffset = new Vector2();

        public Color BackgroundColor = Color.Black;
        public Color ForegroundColor = Color.White;
        public Color SelectionColor = Color.Gray;
        public Color CursorColor = Color.White;
        public ContextMenu contextMenu = new ContextMenu();
        public bool Editable = true;
        public Texture2D Texture = null;
        public bool ShouldOffset = false;//TODO
        public bool BGAsColor = false;

        public String Text
        {
            get { return text; }
            set
            {
                String old = text;
                text = value;
                if (mask == InputMask.Numbers)
                {
                    String old2 = text;
                    MakeANumber();
                    if (old2 != text)
                        return;
                }
                if (maxLength > 0 && text.Length > maxLength)
                    text = text.Substring(0, maxLength);
                UpdateString();
                updateOffset();
                updateCursor();
                if (text != old)
                {
                    if (onTextChanged != null)
                    {
                        onTextChanged.Invoke(this, new TextChangedArgs() { NewText = text, OldText = old });
                    }
                }
            }
        }
        public InputMask Mask
        {
            get { return mask; }
            set
            {
                if (mask == value) return;
                mask = value;
                if (mask == InputMask.Numbers)
                {
                    MakeANumber();
                }
            }
        }
        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                InitCharSizes();
                updateOffset();
                updateCursor();
            }
        }
        public int MaxLength
        {
            get { return maxLength; }
            set
            {
                maxLength = value;
                if (maxLength > 0 && text.Length > maxLength)
                    Text = text.Substring(0, maxLength);
            }
        }
        public bool Multiline
        {
            get { return multiline; }
            set
            {
                if (multiline == value) return;
                multiline = value;
                if (!multiline)
                {
                    String r = "";
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] != '\r' && text[i] != '\n')
                            r += text[i];
                    }
                }
            }
        }
        public String SelectedText
        {
            get
            {
                if (SelectionStart != -1)
                {
                    return text.Substring(Math.Min(selectionStart + selectionLength, selectionStart), Math.Abs(selectionLength));
                }
                return "";
            }
        }
        public int Cursor
        {
            get { return cursor; }
            set
            {
                int oldcur = cursor;
                cursor = value;
                CheckCursorBounds();
                cursorPos = getCoordForCursor(cursor);
                OffsetDrawToCursor();
                ProcessKBSelection(oldcur);
            }
        }
        public int SelectionStart
        {
            get { return selectionStart; }
            set
            {
                selectionStart = value;
                CheckSelectionBounds();
            }
        }
        public int SelectionLength
        {
            get { return selectionLength; }
            set
            {
                selectionLength = value;
                CheckSelectionBounds();
            }
        }
        public override bool isFocused
        {
            get
            { return base.isFocused; }
            set
            {
                bool old = isFocused;
                base.isFocused = value;
                if (!old && value)
                {
                    if (onFocused != null)
                        onFocused.Invoke();
                }
                if (old && !value)
                {
                    if (onLooseFocus != null)
                        onLooseFocus.Invoke();
                }
            }
        }
        #endregion



        #region Events
        public class TextChangedArgs
        {
            public String NewText, OldText;
        }
        public delegate void TextChangedEventHandler(object sender, TextChangedArgs e);
        public event TextChangedEventHandler onTextChanged;
        public delegate void FocusEventHandler();
        public event FocusEventHandler onFocused, onLooseFocus;
        #endregion



        #region Utils
        private void ProcessKBSelection(int oldcursor)
        {
            if (isMouseSelection) return;
            if (wasSymbolInputThisTick) return;
            if (InputEngine.Shift)
            {
                if (oldcursor != Cursor)
                {
                    if (selectionStart == -1)
                    {
                        selectionStart = Math.Min(Cursor, oldcursor);
                        selectionLength = Math.Max(Cursor, oldcursor) - selectionStart;
                    }
                    else
                    {
                        int selectionEnd = selectionStart+selectionLength;
                        if (Cursor < selectionStart)
                        {
                            selectionStart = Cursor;
                            selectionLength = selectionEnd - selectionStart;
                            return;
                        }
                        else if (Cursor > selectionStart)
                        {
                            if (Cursor >= selectionEnd)
                            {
                                selectionLength = Cursor - selectionStart;
                                return;
                            }
                            else
                            {
                                if (oldcursor > Cursor)//left
                                {
                                    selectionLength = Cursor - selectionStart;
                                }
                                else//right
                                {
                                    selectionStart = Cursor;
                                    selectionLength = selectionEnd - selectionStart;
                                }
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                selectionStart = -1;
                selectionLength = 0;
            }
        }

        private void InitCharSizes()
        {
            charSize = font.MeasureString("w");
            twoCharSize = font.MeasureString("ww\r\nww");
            SizePerOneSymbol = twoCharSize - charSize;
        }

        private void UpdateString()
        {
            CheckMultiline();
            DrawText = text;//TODO colors and stuff???
            if (font != null)
                drawTextSize = font.MeasureString(DrawText);
            DrawTextLines = DrawText.Split('\n');
        }

        private void CheckMultiline()
        {
            if (!multiline)
            {
                var a = text.Split('\n');
                if (a.Length > 1)
                {
                    text = "";
                    for (int i = 0; i < a.Length - 1; i++)
                    {
                        text += a[i].Substring(0, a[i].Length - 1);
                    }
                    text += a[a.Length - 1];
                }
            }
        }

        private void CheckCursorBounds()
        {
            if (cursor < 0) cursor = 0;
            if (cursor > text.Length) cursor = text.Length;
        }

        public void CheckSelectionBounds()
        {
            if (selectionStart == -1)
            {
                selectionLength = 0;
                return;
            }
            //if (selectionLength < 0)
            //{
            //    selectionStart += selectionLength;
            //    selectionLength *= -1;
            //}
            if (selectionLength + selectionStart > text.Length)
                selectionLength = text.Length - selectionStart;
        }

        private int GetIndexForCoords(float x, float y)
        {
            x += TextOffset.X;
            y += TextOffset.Y;
            x += SizePerOneSymbol.X / 2;
            int i = (int)(y / SizePerOneSymbol.Y);
            int j = (int)(x / SizePerOneSymbol.X);
            if (DrawTextLines.Length == 0)
                return 0;
            if (i >= DrawTextLines.Length)
                i = DrawTextLines.Length - 1;
            if (i < 0)
                i = 0;
            if (j > DrawTextLines[i].Length)
                j = DrawTextLines[i].Length;
            if (j > 0 && DrawTextLines[i][j - 1] == '\r')
                j--;
            int res = 0;
            for (int ti = 0; ti < i; ti++)
            {
                res += DrawTextLines[ti].Length + 1;
            }
            res += j;
            return res;
        }

        private void OffsetDrawToCursor()
        {
            bool changed = false;
            if (cursor == 0)
            {
                if (cursorPos.X != 0 || cursorPos.Y != 0)
                {
                    TextOffset = new Vector2();
                    cursorPos = new Vector2();
                    Cursor = Cursor;
                }
                return;
            }
            if (cursorPos.X >= size.X)
            {
                TextOffset.X += cursorPos.X - size.X + 1;
                changed = true;
            }
            if (cursorPos.Y + SizePerOneSymbol.Y >= size.Y && SizePerOneSymbol.Y <= size.Y)
            {
                TextOffset.Y += cursorPos.Y + SizePerOneSymbol.Y - size.Y + 1;
                changed = true;
            }

            if (cursorPos.X < 0)
            {
                TextOffset.X += cursorPos.X;
                changed = true;
            }
            if (cursorPos.Y < 0)
            {
                TextOffset.Y += cursorPos.Y;
                changed = true;
            }
            if (changed)
                Cursor = Cursor;
        }

        #region History
        private void AddHistory()
        {
            for (int i = HISTORY_LENGTH - 1; i > 0; i--)
            {
                history[i] = history[i - 1];
            }
            history[0] = Text;
        }

        private void RecoverHistory()
        {
            if (history[1] == null) return;
            Text = history[1];
            for (int i = 0; i < HISTORY_LENGTH - 2; i++)
            {
                history[i] = history[i + 1];
            }
            history[HISTORY_LENGTH - 1] = null;
        }

        private void AddRedoHistory()
        {
            for (int i = HISTORY_LENGTH - 1; i > 0; i--)
            {
                redoHistory[i] = redoHistory[i - 1];
            }
            redoHistory[0] = Text;
        }

        private void RecoverRedoHistory()
        {
            if (redoHistory[0] == null) return;
            Text = redoHistory[0];
            for (int i = 0; i < HISTORY_LENGTH - 2; i++)
            {
                redoHistory[i] = redoHistory[i + 1];
            }
            redoHistory[HISTORY_LENGTH - 1] = null;
        }

        private void ClearRedo()
        {
            for (int i = 0; i < HISTORY_LENGTH - 1; i++)
            {
                redoHistory[i] = null;
            }
        }
        #endregion
        //2
        private void MakeANumber()
        {
            String t = Text;
            String r = "";
            bool wasDotFound = false;
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == '-' && i == 0)
                    r += t[i];
                if (t[i] >= '0' && t[i] <= '9')
                    r += t[i];
                if (t[i] == '.' || t[i] == ',')
                {
                    if (!wasDotFound)
                    {
                        r += System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                        wasDotFound = true;
                    }
                }
            }
            if (r != Text)
                Text = r;
        }

        //Replacement for String.Insert to fix some bugs
        private void Insert(int pos, String s)
        {
            ClearRedo();
            DeleteSelection();
            if (pos == 0) Text = s + Text;
            else if (pos == Text.Length) Text = Text + s;
            else Text = Text.Substring(0, pos) + s + Text.Substring(pos, Text.Length - pos);
            //text.Insert(pos, s);
        }

        private void DeleteSelection()
        {
            if (SelectionStart != -1)
            {
                int min = Math.Min(SelectionStart + SelectionLength, selectionStart);
                int max = min + Math.Abs(SelectionLength);
                Text = Text.Substring(0, min) + Text.Substring(max, text.Length - max);
                SelectionStart = -1;
                SelectionLength = 0;
                Cursor = min;
            }
        }


        private void updateOffset()
        {
            if (cursorPos.X < TextOffset.X) TextOffset.X = cursorPos.X;
            if (cursorPos.Y < TextOffset.Y) TextOffset.Y = cursorPos.Y;

            if (cursorPos.X > TextOffset.X + size.X) TextOffset.X = cursorPos.X - size.X;
            if (cursorPos.Y > TextOffset.Y + size.Y - SizePerOneSymbol.Y) TextOffset.Y = cursorPos.Y - size.Y + SizePerOneSymbol.Y;
        }

        private void updateCursor()
        {
            if (Cursor < 0) Cursor = 0;
            if (Cursor > text.Length) Cursor = text.Length;
            cursorPos = getCoordForCursor(Cursor);
            updateOffset();
        }

        private void UpdateSelection()
        {
            if (SelectionStart != -1)
            {
                if (SelectionStart + SelectionLength > text.Length)
                {
                    SelectionStart = -1;
                    SelectionLength = 0;
                }
            }
        }

        private Vector2 getCoordForCursor(int cur)
        {
            int x = 0, y = 0, c = 0, i = 0;

            for (i = 0; i < DrawTextLines.Length - 1; i++)
            {
                if (c + DrawTextLines[i].Length >= cur)
                {
                    x = (cur - c) * (int)charSize.X;
                    break;
                }
                else
                {
                    c += DrawTextLines[i].Length + 1;
                    y += (int)SizePerOneSymbol.Y;
                }
            }

            if (i == DrawTextLines.Length - 1)
                x = (cur - c) * (int)charSize.X;

            return new Vector2(x, y) - TextOffset;
        }

        private void RemoveUndrawableSymbols(ref String s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < 9 || s[i] > 126)
                {
                    s = s.Remove(i, 1);
                    i--;
                }
            }
        }

        #region BasicCommands
        private void copy()
        {
            if (SelectionStart == -1 || SelectionLength == 0)
                return;
            String s = SelectedText;
            InputEngine.SetClipboardText(s);
        }

        private void cut()
        {
            if (SelectionLength == 0 || SelectionStart == -1)
                return;
            String s = SelectedText;
            InputEngine.SetClipboardText(s);
            DeleteSelection();
        }

        private void paste()
        {
            String s = InputEngine.GetClipboardText();
            if (s == null || s == "")
                return;
            RemoveUndrawableSymbols(ref s);
            Insert(Cursor, s);
            Cursor += s.Length;
        }

        private void undo()
        {
            SelectionStart = -1;
            SelectionLength = 0;
            AddRedoHistory();
            RecoverHistory();
        }

        private void redo()
        {
            SelectionStart = -1;
            SelectionLength = 0;
            RecoverRedoHistory();
        }
        #endregion

        #endregion



        #region ControlFuncs
        protected TextBox() { }

        public TextBox(int x, int y, int w, int h)
        {
            Position = new Vector2(x, y);
            Size = new Vector2(w, h);
        }

        public TextBox(int x, int y, int w, int h, String txt)
            : this(x, y, w, h)
        {
            Text = txt;
        }

        public override void Initialize()
        {
            if (Font == null)
            {
                Font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_11");
                Text = Text;
            }
            if (Texture == null)
                Texture = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");

            contextMenu.Add("Undo");
            contextMenu.Add("Redo");
            contextMenu.Add(null);
            contextMenu.Add("Copy");
            contextMenu.Add("Cut");
            contextMenu.Add("Paste");
            contextMenu.onElementSelected += new ContextMenu.ElementSelectedHandler(contextMenu_onElementSelected);

            base.Initialize();
        }

        public override void Update()
        {
            wasSymbolInputThisTick = false;
            if (contextMenu.isVisible)
            {
                if (history[1] == null)
                {
                    contextMenu.Elements[0].isEnabled = false;
                }
                else
                {
                    contextMenu.Elements[0].isEnabled = true;
                }
                if (redoHistory[0] == null)
                {
                    contextMenu.Elements[1].isEnabled = false;
                }
                else
                {
                    contextMenu.Elements[1].isEnabled = true;
                }
            }
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            Vector2 posold = position;
            if (ShouldOffset)
                Position = Position + Settings.GameOffset * Settings.GameScale;
            //bg
            if (BGAsColor)
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y), BackgroundColor);
            else
                RenderHelper.SmartDrawRectangle(Texture, 5, (int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y, BackgroundColor,
                    renderer);

            #region ScissorsInit
            float x = position.X < 0 ? 0 : position.X;
            float y = position.Y < 0 ? 0 : position.Y;
            float w = size.X < 0 ? 0 : size.X;
            float h = size.Y < 0 ? 0 : size.Y;
            if (x > Main.WindowWidth)
                x = Main.WindowWidth;
            if (y > Main.WindowHeight)
                y = Main.WindowHeight;
            if (x + w > Main.WindowWidth)
                w = Main.WindowWidth - x;
            if (y + h > Main.WindowHeight)
                h = Main.WindowHeight - y;
            #endregion
            renderer.SetScissorRectangle(x, y, w, h, false);
            //selection
            if (SelectionStart != -1 && SelectionLength != 0)
            {
                int min = Math.Min(SelectionStart + SelectionLength, selectionStart);
                int max = min + Math.Abs(SelectionLength);
                var sc = getCoordForCursor(min);
                int sci = (int)((sc.Y + TextOffset.Y) / SizePerOneSymbol.Y);
                int scj = (int)((sc.X + TextOffset.X) / SizePerOneSymbol.X);
                if (scj < 0) scj = 0;
                var sc2 = getCoordForCursor(max);
                int sc2i = (int)((sc2.Y + TextOffset.Y) / SizePerOneSymbol.Y);
                int sc2j = (int)((sc2.X + TextOffset.X) / SizePerOneSymbol.X);
                if (sc2j < 0) sc2j = 0;
                if (sci == sc2i)//same line
                {
                    renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)(position.X + sc.X), (int)(position.Y + sc.Y),
                        (int)(sc2.X - sc.X), (int)charSize.Y), SelectionColor);
                }
                else//multiline
                {
                    renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)(position.X + sc.X), (int)(position.Y + sc.Y),
                        (int)(Font.MeasureString(DrawTextLines[sci].Substring(scj)).X - sc.X), (int)charSize.Y), SelectionColor);
                    float c = sc.Y + SizePerOneSymbol.Y;
                    for (int i = sci + 1; i < sc2i; i++)
                    {
                        renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)(position.X), (int)(position.Y + c),
                            (int)(Font.MeasureString(DrawTextLines[i]).X), (int)charSize.Y), SelectionColor);
                        c += SizePerOneSymbol.Y;
                    }
                    renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)(position.X), (int)(position.Y + sc2.Y),
                        (int)(sc2.X), (int)charSize.Y), SelectionColor);
                }
            }
            //text
            renderer.DrawString(font, DrawText, position - TextOffset, ForegroundColor, 
                Renderer.TextAlignment.Left);
            //cursor
            if (Editable && isFocused && Main.Ticks % 60 < 30)
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)(position.X + cursorPos.X), (int)(position.Y + cursorPos.Y), 
                    1, (int)charSize.Y), CursorColor);

            renderer.ResetScissorRectangle();

            base.Draw(renderer);

            if (ShouldOffset)
                position = posold;
        }

        void contextMenu_onElementSelected(ContextMenu.ElementSelectedArgs e)
        {
            switch (e.index)
            {
                case 0://undo
                    undo();
                    break;
                case 1://redo
                    redo();
                    break;
                case 2://-----
                    break;
                case 3://copy
                    copy();
                    break;
                case 4://cut
                    cut();
                    break;
                case 5://paste
                    paste();
                    break;
                default:
                    break;
            }
        }

        public override bool IsIn(int x, int y)
        {
            return base.IsIn(x, y) || (contextMenu.isVisible && contextMenu.IsIn(x, y));
        }
        #endregion


        
        #region IO
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                selectionStart = -1;
                selectionLength = 0;
                isFocused = true;
                Cursor = GetIndexForCoords(e.curState.X - position.X, e.curState.Y - position.Y);
                if (e.button == 1 && !contextMenu.isVisible)
                {
                    contextMenu.Show();
                    return;
                }
                if (contextMenu.isVisible && contextMenu.IsIn(e.curState.X, e.curState.Y))
                {
                    contextMenu.onButtonClick(e);
                    return;
                }
                else
                {
                    contextMenu.Close();
                }
            }
            else
            {
                isFocused = false;
            }
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (IsSelecting && SelectionButton != e.button)
            {
                IsSelecting = false;
                SelectionStart = -1;
                SelectionLength = 0;
                isMouseSelection = true;
                Cursor = GetIndexForCoords(e.curState.X - position.X, e.curState.Y - position.Y);
                isMouseSelection = false;
                return;
            }
            if (!contextMenu.isVisible && IsIn(e.curState.X, e.curState.Y))
            {
                IsSelecting = true;
                SelectionStart = GetIndexForCoords(e.curState.X - position.X, e.curState.Y - position.Y);
                isMouseSelection = true;
                Cursor = SelectionStart;
                isMouseSelection = false;
                SelectionLength = 0;
                SelectionButton = e.button;
            }
            else
            {
                base.onButtonDown(e);
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (IsSelecting)
            {
                SelectionLength = GetIndexForCoords(e.curState.X - position.X, e.curState.Y - position.Y) - SelectionStart;
                IsSelecting = false;
                if (SelectionButton == 1)//right
                {
                    contextMenu.position = new Vector2(e.curState.X, e.curState.Y);
                    contextMenu.Show();
                }
                return;
            }
            base.onButtonUp(e);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (IsSelecting)
            {
                SelectionLength = GetIndexForCoords(e.curState.X - position.X, e.curState.Y - position.Y) - SelectionStart;
                isMouseSelection = true;
                Cursor = SelectionLength + SelectionStart;
                isMouseSelection = false;
            }
            base.onMouseMove(e);
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            if (isFocused && !contextMenu.isVisible)
            {
                TextOffset.Y -= SizePerOneSymbol.Y * e.delta / 120f;
                if (TextOffset.Y > DrawTextLines.Length * SizePerOneSymbol.Y - size.Y)
                    TextOffset.Y = DrawTextLines.Length * SizePerOneSymbol.Y - size.Y;
                if (TextOffset.Y < 0) TextOffset.Y = 0;
                Cursor = Cursor;
            }
            base.onMouseWheelMove(e);
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (!isFocused || contextMenu.isVisible || !Editable)
                return;

            e.Handled = true;

            #region Navigation
            if (e.key == Keys.Home.GetHashCode())
            {
                if (InputEngine.Control)
                    Cursor = 0;
                else
                    Cursor = GetIndexForCoords(0, cursorPos.Y);
                return;
            }
            if (e.key == Keys.End.GetHashCode())
            {
                if (InputEngine.Control)
                    Cursor = text.Length;
                else
                {
                    var sc = getCoordForCursor(Cursor);
                    int sci = (int)((sc.Y + TextOffset.Y) / SizePerOneSymbol.Y);
                    int scj = (int)((sc.X + TextOffset.X) / SizePerOneSymbol.X);
                    if (DrawTextLines[sci].EndsWith("\r"))
                        Cursor = Cursor - scj + DrawTextLines[sci].Length - 1;
                    else
                        Cursor = Cursor - scj + DrawTextLines[sci].Length;
                }
                return;
            }
            if (e.key == Keys.PageUp.GetHashCode())
            {
                Cursor = GetIndexForCoords(cursorPos.X, cursorPos.Y - (size.Y - size.Y % SizePerOneSymbol.Y));
                return;
            }
            if (e.key == Keys.PageDown.GetHashCode())
            {
                Cursor = GetIndexForCoords(cursorPos.X, cursorPos.Y + (size.Y - size.Y % SizePerOneSymbol.Y));
                return;
            }

            if (e.key == Keys.Left.GetHashCode())
            {
                if (cursorPos.X == 0)
                    Cursor = Cursor - 2;
                else
                    Cursor = Cursor - 1;
                return;
            }
            if (e.key == Keys.Right.GetHashCode())
            {
                if (cursor < DrawText.Length)
                {
                    if (DrawText[cursor] == '\r')
                    {
                        Cursor = Cursor + 2;
                    }
                    else
                    {
                        Cursor = Cursor + 1;
                    }
                }
                return;
            }
            if (e.key == Keys.Up.GetHashCode())
            {
                Cursor = GetIndexForCoords(cursorPos.X, cursorPos.Y - SizePerOneSymbol.Y);
                return;
            }
            if (e.key == Keys.Down.GetHashCode())
            {
                Cursor = GetIndexForCoords(cursorPos.X, cursorPos.Y + SizePerOneSymbol.Y);
                return;
            }
            #endregion

            #region Hotkeys
            if (InputEngine.Control)
            {
                //Ctrl+X
                if (e.key == Keys.X.GetHashCode())
                {
                    cut();
                    return;
                }
                //Ctrl+C
                if (e.key == Keys.C.GetHashCode())
                {
                    copy();
                    return;
                }
                //Ctrl+V
                if (e.key == Keys.V.GetHashCode())
                {
                    paste();
                    return;
                }
                //Ctrl+A
                if (e.key == Keys.A.GetHashCode())
                {
                    SelectionStart = 0;
                    SelectionLength = text.Length;
                    return;
                }
            }
            #endregion
            
            #region Delete
            if (e.key == Keys.Back.GetHashCode())
            {
                if (Cursor > 0)
                {
                    if (DrawText[Cursor - 1] == '\n')
                    {
                        SelectionStart = Cursor - 2;
                        selectionLength = 2;
                    }
                    else
                    {
                        SelectionStart = Cursor - 1;
                        selectionLength = 1;
                    }
                    DeleteSelection();
                }
            }
            if (e.key == Keys.Delete.GetHashCode())
            {
                if (Cursor <= text.Length - 1)
                {
                    SelectionStart = Cursor;
                    if (Cursor + 1 < DrawText.Length && DrawText[Cursor + 1] == '\n')
                    {
                        selectionLength = 2;
                    }
                    else
                    {
                        selectionLength = 1;
                    }
                    DeleteSelection();
                }
            }
            #endregion

            #region Chars
            String a = GetStringForKey((Keys)e.key).ToString();
            if (a != "")
            {
                wasSymbolInputThisTick = true;
                Insert(cursor, a);
                Cursor = Cursor + a.Length;
                wasSymbolInputThisTick = false;
            }
            #endregion
            base.onKeyPressed(e);
        }

        private String GetStringForKey(Keys k)
        {
            bool ShouldShift = InputEngine.Shift ^ InputEngine.CapsLock;
            int khc = k.GetHashCode();

            #region A-Z
            //a-z,A-Z
            if (khc >= Keys.A.GetHashCode() && khc <= Keys.Z.GetHashCode())
                return ShouldShift ? ((char)k).ToString() : ((char)k).ToString().ToLower();
            #endregion

            #region D Keys
            //`
            if (k == Keys.OemTilde)
                return ShouldShift ? "~" : "`";
            //D0-D9
            if (khc >= Keys.D0.GetHashCode() && khc <= Keys.D9.GetHashCode())
                if (InputEngine.Shift)
                {
                    if (k == Keys.D0)
                        return ")";
                    if (k == Keys.D1)
                        return "!";
                    if (k == Keys.D2)
                        return "@";
                    if (k == Keys.D3)
                        return "#";
                    if (k == Keys.D4)
                        return "$";
                    if (k == Keys.D5)
                        return "%";
                    if (k == Keys.D6)
                        return "^";
                    if (k == Keys.D7)
                        return "&";
                    if (k == Keys.D8)
                        return "*";
                    if (k == Keys.D9)
                        return "(";
                }
                else
                    return ((char)k).ToString();
            //-
            if (k == Keys.OemMinus)
                return ShouldShift ? "_" : "-";
            //+
            if (k == Keys.OemPlus)
                return ShouldShift ? "+" : "=";
            #endregion

            #region NumKeys
            //N0-N9 w/ numlock
            if (InputEngine.NumLock && khc >= Keys.NumPad0.GetHashCode() && khc <= Keys.NumPad9.GetHashCode())
                return k.ToString().Substring(k.ToString().Length - 1);
            //N/
            if (k == Keys.Divide)
                return "/";
            //N*
            if (k == Keys.Multiply)
                return "*";
            //N-
            if (k == Keys.Subtract)
                return "-";
            //N+
            if (k == Keys.Add)
                return "+";
            //N.
            if (!ShouldShift && k == Keys.Decimal)
                return ".";
            #endregion

            #region OEM
            // \
            if (k == Keys.OemPipe)
                return ShouldShift ? "|" : "\\";
            //[
            if (k == Keys.OemOpenBrackets)
                return ShouldShift ? "{" : "[";
            //]
            if (k == Keys.OemCloseBrackets)
                return ShouldShift ? "}" : "]";
            //;
            if (k == Keys.OemSemicolon)
                return ShouldShift ? ":" : ";";
            //'
            if (k == Keys.OemQuotes)
                return ShouldShift ? "\"" : "'";
            //,
            if (k == Keys.OemComma)
                return ShouldShift ? "<" : ",";
            //.
            if (k == Keys.OemPeriod)
                return ShouldShift ? ">" : ".";
            //?
            if (k == Keys.OemQuestion)
                return ShouldShift ? "?" : "/";
            #endregion

            #region Enter
            if (k == Keys.Enter)
                return "\r\n";
            #endregion

            #region Space
            if (k == Keys.Space)
                return " ";
            #endregion

            return "";
        }
        #endregion
    }
}
