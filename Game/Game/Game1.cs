#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using MyGame.IO;
using MyGame.GameStateObjects;
using MyGame.DrawingUtils;
using MyGame.Networking;

#endregion

namespace MyGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private static Boolean isServer;
        private static int playerID;
        public static int PlayerID
        {
            get { return playerID; }
        }
        public static Boolean IsServer
        {
            get { return isServer; }
        }
        public static void AsserIsServer()
        {
            if (!isServer)
            {
                throw new Exception("AsserIsServer Failed");
            }
        }
        public static void AssertIsNotServer()
        {
            if (isServer)
            {
                throw new Exception("AssertIsNotServer Failed");
            }
        }
        private static Boolean isActive = true;
        public static Boolean IsInstanceActive
        {
            get { return isActive; }
        }

        public static ThreadSafeQueue<TCPMessage> outgoingQue;
        public static ThreadSafeQueue<TCPMessage> inCommingQue;
        private static GameTime currentGameTime = new GameTime();

        public static GameTime CurrentGameTime
        {
            get { return currentGameTime; }
            private set { currentGameTime = value; }
        }

        private GraphicsDeviceManager graphics;
        private MyGraphicsClass myGraphicsObject;
        private Camera camera;
        private InputManager inputManager;
        private ServerLogic serverLogic = null;
        private ClientLogic clientLogic = null;
        private BackGround backGround;
        private GameStateObjects.GameState gameState;

        public Game1(ThreadSafeQueue<TCPMessage> outgoingQue, ThreadSafeQueue<TCPMessage> inCommingQue, int playerID)
            : base()
        {
            Game1.playerID = playerID;
            Game1.isServer = playerID == 0;
            Game1.outgoingQue = outgoingQue;
            Game1.inCommingQue = inCommingQue;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            //graphics.IsFullScreen = false;

            this.Window.AllowUserResizing = true;
            this.InactiveSleepTime = new TimeSpan(0);
            this.IsFixedTimeStep = false;
            IsMouseVisible = true;
            graphics.ApplyChanges();
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
            base.Initialize();
            TextureLoader.Initialize(Content);
            //Window.IsBorderless = true;
            //Window.AllowUserResizing = true;

            //TODO: this should not be hard coded forever
            Vector2 worldSize = new Vector2(20000);
            backGround = new BackGround(worldSize);
            StaticGameObjectCollection.Initialize(worldSize);

            inputManager = new InputManager(graphics);
            gameState = new GameStateObjects.GameState(worldSize, camera);
            camera.SetGameState(gameState);

            if (isServer)
            {
                serverLogic = new ServerLogic(worldSize, inputManager);
            }
            else
            {
                clientLogic = new ClientLogic(inputManager, camera);
            }

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Texture2D line = Content.Load<Texture2D>("line");
            Textures.LoadContent(Content);

            camera = new Camera(new Vector2(0), 1f, 0, graphics);

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            myGraphicsObject = new DrawingUtils.MyGraphicsClass(graphics, spriteBatch, camera);
            DrawingUtils.MyGraphicsClass.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            isActive = this.IsActive;
            Game1.CurrentGameTime = gameTime;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);

            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (true)
            {
                while (!inCommingQue.IsEmpty)
                {
                    TCPMessage m = inCommingQue.Dequeue();
                    if (m is GameUpdate)
                    {
                        ((GameUpdate)m).Apply(this);
                    }
                }
            }
            
            if (this.IsActive)
            {
                inputManager.Update();
            }

            if (isServer)
            {
                serverLogic.Update(secondsElapsed);
            }
            else
            {
                clientLogic.Update(secondsElapsed);
            }

            gameState.Update(gameTime);
            camera.Update(secondsElapsed);
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Wheat);

            myGraphicsObject.BeginWorld();
            backGround.Draw(gameTime, myGraphicsObject);
            myGraphicsObject.End(); 
            gameState.Draw(gameTime, myGraphicsObject);
            myGraphicsObject.Begin(Matrix.Identity);

            Ship focus;
            if (Game1.IsServer)
            {
                focus = PlayerControllers.StaticControllerFocus.GetFocus(1);
            }
            else
            {
                focus = PlayerControllers.StaticControllerFocus.GetFocus(Game1.PlayerID);
            }
            if (focus != null)
            {
                myGraphicsObject.DrawDebugFont("Health: "+focus.Health.ToString(), new Vector2(0), 1);
                myGraphicsObject.DrawDebugFont("Kills: " + focus.Kills().ToString(), new Vector2(0, 30), 1);
            }

            /*
            myGraphicsObject.DrawDebugFont(StaticGameObjectCollection.Collection.Tree.CompleteList().Count.ToString(), new Vector2(0), 1);
            myGraphicsObject.DrawDebugFont(StaticGameObjectCollection.Collection.GetMasterList().GetMaster().Count.ToString(), new Vector2(0, 30), 1);
            myGraphicsObject.DrawDebugFont(StaticGameObjectCollection.Collection.GetMasterList().Count().ToString(), new Vector2(0, 60), 1);
            myGraphicsObject.DrawDebugFont(StaticGameObjectCollection.Collection.Count().ToString(), new Vector2(0, 90), 1);
            */
            myGraphicsObject.End();

        }


    }
}
