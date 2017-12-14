using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FIREWALL
{
    class Ball
    {
        private Rectangle dims;
        private Texture2D img;
        private Color c;
        public Vector2Int vel;

        public Ball(Rectangle dimensions, Texture2D design, Color color)
        {
            dims = dimensions;
            img = design;
            c = color;
            vel = Vector2Int.Zero;
        }

        public Point position()
        {
            return new Point(dims.X + dims.Width / 2, dims.Y + dims.Height / 2);
        }

        public void sendOff(Vector2Int velocity)
        {
            vel = velocity;
        }

        public bool collide(Paddle paddle)
        {
            bool result = false;
            if (paddle.dims.Intersects(dims))
            {
                result = true;
                Rectangle colRect = Rectangle.Intersect(paddle.dims, dims);
                if (colRect.Width <= colRect.Height)
                {
                    if (vel.X < 0)
                        dims.X = colRect.X + colRect.Width;
                    else
                        dims.X = colRect.X - dims.Width;
                    vel.X *= -1;
                }
                else
                {
                    if (vel.Y < 0)
                        dims.Y = colRect.Y + colRect.Height;
                    else
                        dims.Y = colRect.Y - dims.Height;
                    vel.Y *= -1;
                }
            }
            return result;
        }

        public bool collide(Brick brick)
        {
            bool result = false;
            if (brick.dims.Intersects(dims))
            {
                result = true;
                Rectangle colRect = Rectangle.Intersect(brick.dims, dims);
                if (colRect.Width <= colRect.Height)
                {
                    if (vel.X < 0)
                        dims.X = colRect.X + colRect.Width;
                    else
                        dims.X = colRect.X - dims.Width;
                    vel.X *= -1;
                }
                else
                {
                    if (vel.Y < 0)
                        dims.Y = colRect.Y + colRect.Height;
                    else
                        dims.Y = colRect.Y - dims.Height;
                    vel.Y *= -1;
                }
            }
            return result;
        }

        public int Update(GameTime gameTime, Vector2Int res)
        {
            int result = -1;
            dims.X += vel.X;
            dims.Y += vel.Y;

            if(dims.X <= 0)
            {
                dims.X = 0;
                result = 0;
            }
            if(dims.X >= res.X - dims.Width)
            {
                dims.X = res.X - dims.Width;
                result = 1;
            }
            if(dims.Y <= 0)
            {
                dims.Y = 0;
                vel.Y *= -1;
            }
            if(dims.Y >= res.Y - dims.Width)
            {
                dims.Y = res.Y - dims.Height;
                vel.Y *= -1;
            }
            return result;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);

            spriteBatch.Draw(img, dims, c);

            spriteBatch.End();
        }
    }
}
