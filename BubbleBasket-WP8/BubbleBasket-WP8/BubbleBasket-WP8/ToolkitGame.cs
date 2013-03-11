// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBasket_WP8
{

    class ToolkitGame : Game
    {

        /* =============================== Please Read ===============================
         * This project defaults to run on the Windows Phone 8 Emulator. 
         * But first you must do the following:
         *   o In the configuration manager (DEBUG > Configuration Manager) select x86 as the ACTIVE SOLUTION PLATFORM
         * 
         * If you are compiling for a WP8 device do the following:
         *   o Remove all SharpDX.* References
         *   o Add new references from SharpDX/Bin/standard-wp8-ARM
         *   o In the configuration manager (DEBUG > Configuration Manager) select ARM as the ACTIVE SOLUTION PLATFORM
         *   o Run your app
         * ===========================================================================*/

        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        Texture2D BallTexture;
        Texture2D Background;

        private readonly IPointerService pointerService;
        private readonly PointerState pointerState = new PointerState();

        public static Vector2 ScreenBounds;

        private Texture2D BasketLeft;
        List<Bubble> ToRemove;
        private Basket basket;
        
        public enum BubbleColor : long
        {
            Red = 0xED2024FF,
            Purple = 0x4C4EA1FF,
            Yellow = 0xFFEE00FF,
            Green = 0x6ABD45FF,
            Blue = 0x74E0FFFF,
            White = 0xFFFFFFFF,
        }

        Vector3 Red = new Vector3((float)0xED / 256.0f, (float)0x20 / 256.0f, (float)0x24 / 256.0f);
        Vector3 Purple = new Vector3((float)0x4C / 256.0f, (float)0x4E / 256.0f, (float)0xE1 / 256.0f);
        Vector3 Yellow = new Vector3((float)0xFF / 256.0f, (float)0xEE / 256.0f, (float)0x00 / 256.0f);
        Vector3 Green = new Vector3((float)0x6A / 256.0f, (float)0xBD / 256.0f, (float)0x45 / 256.0f);
        Vector3 Blue = new Vector3((float)0x74 / 256.0f, (float)0xE0 / 256.0f, (float)0xFF / 256.0f);
        Vector3 White = new Vector3((float)0xFF / 256.0f, (float)0xFF / 256.0f, (float)0xFF / 256.0f);

        List<Bubble> Bubbles;

        public ToolkitGame()
        {

            pointerService = new PointerManager(this);
            Bubbles = new List<Bubble>();
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            ToRemove = new List<Bubble>();

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

        }

        protected override void Initialize()
        {
            Window.Title = "Toolkit WinRT";

            base.Initialize();
        }

        protected override void LoadContent()
        {

            BasketLeft = Content.Load<Texture2D>("bb_basket.dds");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            BallTexture = Content.Load<Texture2D>("WhiteBubble75x75.dds");
            //Background texture from subtlepatterns.com
            Background = Content.Load<Texture2D>("bb_bg768x1280.dds");

            Bubble.CircleTexture = Content.Load<Texture2D>("WhiteBubble75x75.dds");
            Basket.BasketTexture = Content.Load<Texture2D>("bb_basket.dds");
            Bubble.Score = 0;
            ScreenBounds = new Vector2(GraphicsDevice.BackBuffer.Width, GraphicsDevice.BackBuffer.Height);

            basket = new Basket(new Vector2(ScreenBounds.X / 2.0f, ScreenBounds.Y - Basket.BasketTexture.Height), 150)
            {
                TriangleX = 38.0f,
                TriangleY = Basket.BasketTexture.Height,
                Angle = (float)Math.Atan(68.0f/38.0f)
            };

            basket.ReflectionAngle = (float)((90.0f - (basket.Angle * 180.0f / Math.PI)) * Math.PI / 180.0f);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        private void AddBubble(DrawingPointF loc)
        {
            Random rand = new Random();//create a new random instance

            float sign = (rand.Next(2) == 0) ? (-1) : 1;//get a random number that determines whether particles X acceleration + velocity is +/-
            Vector2 velocity = new Vector2((1 + rand.Next(2)) * sign, (-1) * (80 + 10 + rand.Next(50)));//randomize x and y velocity
            Vector2 acc = new Vector2(((float)2.3 * sign), (float)19.0);//intialize acceleration

            Vector3 color = Red;

            switch (rand.Next(0, 5))
            {
                case 1: color = Red; break;
                case 2: color = Green; break;
                case 3: color = Blue; break;
                case 4: color = Purple; break;
                case 0: color = Yellow; break;
            }

            Bubbles.Add(new Bubble(velocity, acc, loc / new Vector2(480, 800) * ScreenBounds, 400 + rand.Next(200), (float)0.09, color));

        }

        private void RemoveDeadBubbles()
        {
            foreach (Bubble Bub in ToRemove)
            {
                Bubbles.Remove(Bub);
            }

            ToRemove.Clear();
        }

        protected override void Update(GameTime gameTime)
        {

            pointerService.GetState(pointerState);

            foreach (PointerPoint p in pointerState.Points)
            {
                if (p.EventType != PointerEventType.Pressed) continue;
                AddBubble(p.Position);
            }

            foreach (Bubble Bub in Bubbles)
            {
                Bub.Update(basket);
                
                if (Bub.Remove) ToRemove.Add(Bub);
            }

            RemoveDeadBubbles();

            //System.Diagnostics.Debug.WriteLine(Bubble.Score);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(Background, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, GraphicsDevice.BlendStates.NonPremultiplied);

            spriteBatch.Draw(Basket.BasketTexture, 
                new Vector2(basket.Position.X - Basket.BasketTexture.Width - basket.Gap/2.0f, basket.Position.Y), 
                Color.White);
            
            spriteBatch.Draw(Basket.BasketTexture, new Vector2(basket.Position.X + basket.Gap/2.0f, basket.Position.Y),
                null, 
                Color.White, 0,new Vector2(0,0), new Vector2(1,1), SpriteEffects.FlipHorizontally,0.0f );


            foreach (Bubble Bub in Bubbles)
            {
                spriteBatch.Draw(BallTexture, Bub.Position, new Color(Bub.BubbleColor, 0xFF));
            }
            spriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }

    }
}