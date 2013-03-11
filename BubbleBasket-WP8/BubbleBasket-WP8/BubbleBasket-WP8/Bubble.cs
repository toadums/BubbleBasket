using SharpDX;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBasket_WP8
{

    public enum BubbleType
    {
        Normal,
    }

    class Bubble
    {
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Vector2 Position;
        public int TTL { get; set; }
        private float dt { get; set; }
        public Boolean Remove = false;//flag for removal
        public static Texture2D CircleTexture;
        public Vector3 BubbleColor;
        public BubbleType MyType = BubbleType.Normal;
        public static int Score;

        public Bubble(Vector2 v, Vector2 a, Vector2 p, int t, float d, Vector3 c)
        {

            Velocity = v;
            Acceleration = a;
            Position = p;
            TTL = t;
            dt = d;
            BubbleColor = c;

            //TODO: Matt generates random color (and type?).
            PickColor();
            PickType();
        }

        private void PickColor()
        {
            Random r = new Random();
            r.Next(/*min,max*/);
        }

        private void PickType()
        {

        }

        public void Update(Basket basket)
        {
            TTL--;

            if (TTL < 1) Remove = true;

            Position.X = Position.X + (Velocity.X) * dt + Acceleration.X * (dt * dt);
            Position.Y = Position.Y + (Velocity.Y) * dt + Acceleration.Y * (dt * dt);

            Velocity.X = Velocity.X + Acceleration.X * dt;
            Velocity.Y = Velocity.Y + Acceleration.Y * dt;

            bool inBasket = InBasket(basket);

            if (Position.X < 0)
            {
                Position.X = 2;
                Velocity.X *= (-1);
            }
            else if (Position.X > ToolkitGame.ScreenBounds.X - CircleTexture.Width)
            {
                Position.X = ToolkitGame.ScreenBounds.X - CircleTexture.Width - 2;
                Velocity.X *= (-1);
            }

            if (inBasket &&
                (Position.Y + CircleTexture.Height >= ToolkitGame.ScreenBounds.Y - Basket.BasketTexture.Height))
            {

                if ((Position.Y >= ToolkitGame.ScreenBounds.Y))
                {
                    Score++;
                    Remove = true;
                }
                else
                {

                    int side = 1;
                    if (Position.X + CircleTexture.Width/2.0f <= basket.Position.X) side = -1;

                    Vector2 E = new Vector2(basket.Position.X + side * basket.Gap / 2.0f, ToolkitGame.ScreenBounds.Y);
                    Vector2 L = new Vector2(basket.Position.X +( basket.Gap / 2.0f + basket.TriangleX)*side,
                        ToolkitGame.ScreenBounds.Y - Basket.BasketTexture.Height);
                    if (intersection(basket, E, L))
                    {
                        Velocity.X *= -1;
                        Position.X += -side*15;
                    }

                }

            }
            else if (Position.Y + CircleTexture.Height > ToolkitGame.ScreenBounds.Y - Basket.BasketTexture.Height + 5 && !inBasket)
            {
                Velocity.Y = Velocity.Y * ((float)(-1.0)) / (float)1.5;
                Position.Y = ToolkitGame.ScreenBounds.Y - CircleTexture.Height - Basket.BasketTexture.Height - 2;
            }
        }

        private bool intersection(Basket basket, Vector2 E, Vector2 L)
        {

            /*
                E is the starting point of the ray,
                L is the end point of the ray,
                C is the center of sphere you're testing against
                r is the radius of that sphere
              Compute:
                d = L - E ( Direction vector of ray, from start to end )
                f = E - C ( Vector from center sphere to ray start )
             */

            Vector2 C = Position + new Vector2(CircleTexture.Width / 2.0f, CircleTexture.Height / 2.0f);
            float r = CircleTexture.Height / 2.0f;

            Vector2 d = L - E;
            Vector2 f = E - C;

            float a = Vector2.Dot(d, d);
            float b = 2 * Vector2.Dot(f, d);
            float c = Vector2.Dot(f,f) - r * r;

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                // no intersection
                return false;
            }
            else
            {
                // ray didn't totally miss sphere,
                // so there is a solution to
                // the equation.

                discriminant = (float)Math.Sqrt(discriminant);

                // either solution may be on or off the ray so need to test both
                // t1 is always the smaller value, because BOTH discriminant and
                // a are nonnegative.
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                // 3x HIT cases:
                //          -o->             --|-->  |            |  --|->
                // Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit), 

                // 3x MISS cases:
                //       ->  o                     o ->              | -> |
                // FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

                if (t1 >= 0 && t1 <= 1)
                {
                    // t1 is an intersection, and if it hits,
                    // it's closer than t2 would be
                    // Impale, Poke
                    return true;
                }

                // here t1 didn't intersect so we are either started
                // inside the sphere or completely past it
                if (t2 >= 0 && t2 <= 1)
                {
                    // ExitWound
                    return true;
                }

                // no intn: FallShort, Past, CompletelyInside
                return false;

            }
        }

        private bool InBasket(Basket basket)
        {
            if ((Position.X >= basket.Position.X - basket.Gap / 2.0f - basket.TriangleX) &&
                (Position.X + CircleTexture.Width <= basket.Position.X + basket.Gap / 2.0f + basket.TriangleX))
            {
                return true;
            }
            else
            {
                return false;
            }

        }


    }
}
