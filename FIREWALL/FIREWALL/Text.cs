using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FIREWALL
{
    enum Justify { Top, Center };
    enum Align { Left, Center };
    class Text
    {
        private SpriteFont font;
        private Rectangle dims;
        private string t;
        private string tfx;
        private Color c;
        private string format;
        private bool isTypeEffect;
        private float scale;
        private int msDelay;
        private Timer time;
        public bool isTyping;
        public bool isTyped;

        public Text(SpriteFont spriteFont, Rectangle dimensions, string text, Color color, bool typeEffect, int millisecondDelay)
        {
            font = spriteFont;
            dims = dimensions;
            t = text;
            tfx = "";
            c = color;
            isTypeEffect = typeEffect;
            scale = 1f;
            msDelay = millisecondDelay;
            time = new Timer();
            time.Interval = msDelay;
            time.Elapsed += new ElapsedEventHandler(addLetter);
            time.AutoReset = true;
            isTyped = !isTypeEffect;
        }

        public void Update(GameTime gameTime)
        {
            if (isTyping && !time.Enabled)
                time.Enabled = true;
            else if (!isTyping && time.Enabled)
                time.Enabled = false;
        }

        private void addLetter(object sender, ElapsedEventArgs e)
        {
            if (isTypeEffect && tfx.Length < format.Length)
                tfx += format.Substring(tfx.Length, 1);
            else
                isTyped = true;
        }

        public void optimizeLine()
        {
            float prevScale = 0f, newScale = 0f;
            int height = 1;
            bool optimized = false;
            while (!optimized)
            {
                prevScale = newScale;
                newScale = (1f / ((float)font.LineSpacing / (float)height));

                if (font.MeasureString(t).X * newScale > dims.Width || font.MeasureString(t).Y * newScale > dims.Height)
                    optimized = true;
                height++;
            }
            scale = prevScale;
            format = t;
        }

        public void optimize()
        {
            float prevScale = 0f, newScale = 0f;
            string prevText = "", newText = "";
            int height = 1;
            bool optimized = false;
            while (!optimized)
            {
                prevScale = newScale;
                newScale = (1f / ((float)font.LineSpacing / (float)height));
                prevText = newText;
                newText = "";

                int c = 0;
                while (t.Substring(c, 1) == " ")
                {
                    newText += " "; c++;
                }
                string[] words = t.Substring(c, t.Length - c).Split(' ');
                words[0] = newText + words[0];
                newText = "";
                for (int i = 0; i < words.Length; i++)
                    words[i] = " " + words[i];

                string line = "";
                for(int i = 0; i < words.Length; i++)
                {
                    if (line == "")
                        words[i] = words[i].Substring(1, words[i].Length - 1);
                    if (font.MeasureString(words[i]).X * newScale > dims.Width)
                    {
                        line += words[i];
                        optimized = true;
                    }
                    else if (font.MeasureString(line + words[i]).X * newScale <= dims.Width)
                        line += words[i];
                    else
                    {
                        if (newText == "" && font.MeasureString(line).Y * newScale > dims.Height)
                        {
                            newText += line;
                            optimized = true;
                        }
                        else
                        {
                            if (font.MeasureString(newText + "\n" + line).Y * newScale > dims.Height)
                                optimized = true;
                            if (newText != "")
                                newText += "\n";
                            newText += line;
                        }
                        line = words[i].Substring(1, words[i].Length - 1);
                    }
                    if (i + 1 == words.Length)
                    {
                        if (newText != "")
                            newText += "\n";
                        newText += line;
                    }
                }
                if (font.MeasureString(newText).Y * newScale > dims.Height)
                    optimized = true;
                height++;
            }
            scale = prevScale;
            format = prevText;
        }

        public void Draw(SpriteBatch spriteBatch, Justify j, Align a)
        {
            spriteBatch.Begin();
            Vector2 origin = Vector2.Zero;
            if (j == Justify.Top)
                origin.Y = dims.Y;
            if (j == Justify.Center)
                origin.Y = dims.Y + dims.Height / 2 - font.MeasureString(format).Y * scale / 2;
            if (a == Align.Left)
                origin.X = dims.X;
            if (a == Align.Center)
                origin.X = dims.X + dims.Width / 2 - font.MeasureString(format).X * scale / 2;
            if (isTypeEffect)
                spriteBatch.DrawString(font, tfx, origin, c, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            else
                spriteBatch.DrawString(font, format, origin, c, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
