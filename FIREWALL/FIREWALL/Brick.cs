using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FIREWALL
{
    class Brick
    {
        public Rectangle dims;
        private Rectangle borders;
        private Texture2D img;
        private Color c;

        public Brick(Rectangle dimensions, Texture2D design, Color color)
        {
            borders = dimensions;
            dims = new Rectangle(borders.X + 1, borders.Y + 1, borders.Width - 2, borders.Height - 2);
            img = design;
            c = color;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);

            spriteBatch.Draw(img, borders, Color.Black);
            spriteBatch.Draw(img, dims, c);

            spriteBatch.End();
        }
    }
}
