using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBasket_WP8
{
    class Basket
    {
        public static Texture2D BasketTexture;
        public Vector2 Position;
        public float Gap;
        public float Angle;
        public float TriangleX;
        public float TriangleY;
        public float ReflectionAngle;

        public Basket(Vector2 p, float g)
        {

            Position = p;
            Gap = g;

        }

        public void Update()
        {

        }


    }
}
