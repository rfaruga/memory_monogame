using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1 {

    public class Game1 : Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D background;
        private Texture2D question;
        private Texture2D numbers;

        private int selectedFirst = -1;
        private int selectedSecond = -1;

        private Point gameStart = new Point(50, 50);

        private Dictionary<int, Rectangle> numbersPosition = new Dictionary<int, Rectangle>();
        private List<GameField> gameFields = new List<GameField>();

        private double elapsedTime = 0;
        private double visibleTime = 2000;

        private int moves = 0;
        private int win = 0;

        private string windowTitle => win == 12 ? $"You won in {moves} moves" : $"Moves: {moves}";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("background");
            question = Content.Load<Texture2D>("question");
            numbers = Content.Load<Texture2D>("numbers");

            var id = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    numbersPosition.Add(id, new Rectangle(j * 100, i * 100, 100, 100));
                    id++;
                }
            }

            List<int> randNumbers = new List<int>();

            for (int i = 0; i < 12; i++)
            {
                randNumbers.Add(i);
                randNumbers.Add(i);
            }

            var rand = new Random();

            id = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var index = rand.Next(0, randNumbers.Count - 1);
                    var num = randNumbers[index];
                    randNumbers.RemoveAt(index);

                    gameFields.Add(new GameField
                    {
                        id = id++,
                        hidden = true,
                        position = new Rectangle(gameStart.X + j * 100, gameStart.Y + i * 100, 100, 100),
                        numberId = num,
                        win = false
                    });
                }
            }

            // TODO: use this.Content to load your game content here
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
            if (selectedFirst != -1)
            {
                elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedTime > visibleTime)
                {
                    elapsedTime = 0;

                    var result = gameFields.Where(item => item.id == selectedFirst || item.id == selectedSecond)
                        .ToArray();

                    if (result.Length == 2)
                    {
                        if (result.First().numberId == result.Last().numberId)
                        {
                            foreach (var item in gameFields)
                            {
                                if (item.id == selectedFirst || item.id == selectedSecond)
                                {
                                    item.hidden = false;
                                    item.win = true;
                                }
                            }
                            win++;
                        }
                    }

                    selectedFirst = -1;
                    selectedSecond = -1;

                    foreach (var item in gameFields)
                    {
                        if (!item.win && !item.hidden)
                        {
                            item.hidden = true;
                        }
                    }
                }
            }

            var mouseState = Mouse.GetState();

            foreach (var item in gameFields)
            {
                if ((mouseState.LeftButton == ButtonState.Pressed) && item.position.Contains(mouseState.Position))
                {
                    if (selectedFirst == -1 && selectedSecond == -1)
                    {
                        item.hidden = false;
                        selectedFirst = item.id;
                    }
                    else if (selectedFirst != -1 && selectedSecond == -1 && selectedFirst != item.id)
                    {
                        item.hidden = false;
                        selectedSecond = item.id;
                        moves++;
                    }
                }
            }

            Window.Title = windowTitle;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // background
            spriteBatch.Draw(background, new Rectangle(0, 0, 1000, 800), Color.White);

            foreach (var item in gameFields)
            {
                if (item.hidden)
                {
                    spriteBatch.Draw(question, item.position, null, Color.White);
                }
                else if (item.win || !item.hidden)
                {
                    spriteBatch.Draw(numbers, item.position, numbersPosition[item.numberId], Color.White);
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
