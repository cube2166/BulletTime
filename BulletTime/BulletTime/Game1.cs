using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BulletTime
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    ///
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture_blank;


        public const int window_width = 800;
        public const int window_height = 600;
        Vector2 circle_center;
        int circle_radius;

        MyObjList ObjectCollect;
        Player player;
        bool gameOver;
        SpriteFont stateFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = window_width;
            graphics.PreferredBackBufferHeight = window_height;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            circle_center = new Vector2(window_width / 2 - 1, window_height / 2 - 1);
            circle_radius = window_height / 2 - 50;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture_blank = Content.Load<Texture2D>("blank");
            stateFont = Content.Load<SpriteFont>("GameState");

            ObjectCollect = new MyObjList();

            player = new Player(texture_blank, window_width / 2, window_height / 2, 10, 10, Color.Yellow, 5);
            player.showHandler += prepareShow;
            ObjectCollect.Add(player);

            for (int ii = 0; ii < 10; ii++)
            {
                Launchpad temp = new Launchpad(texture_blank, circle_center, circle_radius, 10, prepareShow, ii);
                temp.showHandler += prepareShow;
                ObjectCollect.Add(temp);
            }
            // TODO: use this.Content to load your game content here
        }

        public void prepareShow(object obj1, object obj2)
        {
            Texture2D temp = obj1 as Texture2D;
            MyObject temp2 = obj2 as MyObject;
            spriteBatch.Draw(temp, new Rectangle((int)temp2.X, (int)temp2.Y, temp2.Width, temp2.Height), temp2.thisClolor);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameOver == false)
            {
                for (int ii = 0; ii < ObjectCollect.Count; ii++)
                {
                    ObjectCollect[ii].Update(elapsedTime);
                }
                if (player.OnCheck() == true)
                {
                    gameOver = true;
                }
            }

            //float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;


            //for (int ii = 0; ii < ObjectCollect.Count; ii++)
            //{
            //    ObjectCollect[ii].Update(elapsedTime);
            //}
            //if(player.OnCheck() == true)
            //{
            //    gameOver = true;
            //}

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            {
                player.MoveLeft();
            }
            else if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            {
                player.MoveRight();
            }
            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            {
                player.MoveUP();
            }
            else if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            {
                player.MoveDown();
            }

            if (keyboard.IsKeyDown(Keys.R) && gameOver)
            {
                gameOver = false;
                StartGame();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        public void StartGame()
        {
            player.X = window_width / 2;
            player.Y = window_height / 2;
            int count = ObjectCollect.Count;
            for (int ii = 0; ii < count; ii++)
            {
                bool clear = false;
                foreach (var item in ObjectCollect)
                {
                    Bullet temp = item as Bullet;
                    if (temp != null)
                    {
                        ObjectCollect.Remove(item);
                        clear = false;
                        break;
                    }
                    clear = true;
                }
                if (clear == true)
                    break;
            }
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            if (gameOver == true)
            {
                // Fill the screen with black before the game starts

                String title = "BulletTime";
                string pressSpace = "Press Space to start";
                String press = "Press";
                String RR = " R";
                String to_ReStart = " to ReStart";

                // Measure the size of text in the given font
                Vector2 titleSize = stateFont.MeasureString(title);
                Vector2 pressSpaceSize = stateFont.MeasureString(pressSpace);
                Vector2 pressSize = stateFont.MeasureString(press);
                Vector2 RRSize = stateFont.MeasureString(RR);
                Vector2 to_ReStartSize = stateFont.MeasureString(to_ReStart);

                // Draw the text horizontally centered
                spriteBatch.DrawString(stateFont, title,
                new Vector2(window_width / 2 - titleSize.X / 2, window_height / 3),
                Color.ForestGreen);
                //spriteBatch.DrawString(stateFont, pressSpace,
                //new Vector2(window_width / 2 - pressSpaceSize.X / 2,
                //window_height / 2), Color.White);
                spriteBatch.DrawString(stateFont, press,
                new Vector2(window_width / 2 - pressSpaceSize.X / 2,
                window_height / 2), Color.White);
                spriteBatch.DrawString(stateFont, RR,
                new Vector2(window_width / 2 - pressSpaceSize.X / 2 + pressSize.X,
                window_height / 2), Color.Red);
                spriteBatch.DrawString(stateFont, to_ReStart,
                new Vector2(window_width / 2 - pressSpaceSize.X / 2 + pressSize.X + RRSize.X,
                window_height / 2), Color.White);


            }
            else
            {
                for (int ii = 0; ii < ObjectCollect.Count; ii++)
                {
                    ObjectCollect[ii].Show();
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


        private double radians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }
}
