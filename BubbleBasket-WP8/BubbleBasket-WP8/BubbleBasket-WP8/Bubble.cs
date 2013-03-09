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
        public Boolean OneBounce = false;//prevents a particle from getting stuck on the bottom
        public Boolean Remove = false;//flag for removal
        public static Texture2D CircleTexture;
        public Color CircleColor { get; set; }
        public BubbleType MyType = BubbleType.Normal;


        public Bubble(Vector2 v, Vector2 a, Vector2 p, int t,float d)
        {

            Velocity = v;
            Acceleration = a;
            Position = p;
            TTL = t;
            dt = d;

            //TODO: Matt generates random color (and type?).
            PickColor();
            PickType();
        }

        private void PickColor(){
            Random r = new Random();
            r.Next(/*min,max*/);
        }

        private void PickType(){

        }

        public void Update()
        {
            TTL--;

            if (TTL < 1) Remove = true;

            Position.X = Position.X + (Velocity.X) * dt + Acceleration.X * (dt * dt);
            Position.Y = Position.Y + (Velocity.Y) * dt + Acceleration.Y * (dt * dt);

            Velocity.X = Velocity.X + Acceleration.X * dt;
            Velocity.Y = Velocity.Y + Acceleration.Y * dt;

            if (Position.X < 0 || Position.X > ToolkitGame.ScreenBounds.X - CircleTexture.Width)
            {
                Velocity.X *= (-1);
            }

            if (Position.Y > ToolkitGame.ScreenBounds.Y - CircleTexture.Height && !OneBounce)
            {
                Velocity.Y = Velocity.Y * ((float)(-1.0)) / (float)1.5;
                OneBounce = true;
                //reset onebounce
            }
            else if (OneBounce)
            {
                OneBounce = false;
            }




        }
        


    }
}
