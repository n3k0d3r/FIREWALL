using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FIREWALL
{
    class Paddle
    {
        List<List<Keys>> controls = new List<List<Keys>> {
            new List<Keys> { Keys.W, Keys.S },
            new List<Keys> { Keys.Up, Keys.Down } };
        public Rectangle dims;
        private int contI;
        private Texture2D img;
        private Color c;
        public int score;

        public Paddle(Rectangle dimensions, Texture2D design, Color color, int controlScheme)
        {
            dims = dimensions;
            img = design;
            c = color;
            contI = controlScheme;
        }

        public void Update(GameTime gameTime, Vector2Int res)
        {
            KeyboardState keyPress = Keyboard.GetState();

            if (keyPress.IsKeyDown(controls[contI][0]))
                dims.Y -= res.pixelHeight(.0046875);
            if (keyPress.IsKeyDown(controls[contI][1]))
                dims.Y += res.pixelHeight(.0046875);
            if (dims.Y <= 0)
                dims.Y = 0;
            if (dims.Y >= res.Y - dims.Height)
                dims.Y = res.Y - dims.Height;
        }

        public void Update(GameTime gameTime, Vector2Int res, List<Ball> balls)
        {
            List<int> d = new List<int>();
            foreach(Ball ball in balls)
            {
                d.Add(Math.Abs(dims.X - ball.position().X));
            }
            int i = d.IndexOf(d.Min());
            if (Math.Abs(balls[i].position().Y - (dims.Y + dims.Height / 2)) <= res.pixelHeight(.0046875))
                dims.Y = balls[i].position().Y - dims.Height / 2;
            else if (balls[i].position().Y < dims.Y + dims.Height / 2)
                dims.Y -= res.pixelHeight(.0046875);
            else if (balls[i].position().Y > dims.Y + dims.Height / 2)
                dims.Y += res.pixelHeight(.0046875);
            if (dims.Y <= 0)
                dims.Y = 0;
            if (dims.Y >= res.Y - dims.Height)
                dims.Y = res.Y - dims.Height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);

            spriteBatch.Draw(img, dims, c);

            spriteBatch.End();
        }
    }
}