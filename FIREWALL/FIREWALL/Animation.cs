using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FIREWALL
{
    class Animation
    {
        private List<Texture2D> frames;
        private Rectangle dims;
        private Timer time;
        private int msDelay;
        private int cursor;

        public Animation(List<Texture2D> framesList, Rectangle dimensions, int delay)
        {
            frames = framesList;
            dims = dimensions;
            msDelay = delay;
            cursor = 0;

            time = new Timer();
            time.Interval = msDelay;
            time.Elapsed += new ElapsedEventHandler(moveCursor);
            time.AutoReset = true;
        }

        public void toggle()
        {
            time.Enabled = !time.Enabled;
        }

        public void reset()
        {
            time.Enabled = false;
            cursor = 0;
        }

        private void moveCursor(object sender, ElapsedEventArgs e)
        {
            cursor++;
            if (cursor >= frames.Count)
                cursor = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(frames[cursor], dims, Color.White);

            spriteBatch.End();
        }
    }
}
