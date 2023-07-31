using ISS;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

public class Game : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GameManager _gameManager;

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        //IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        //_graphics.IsFullScreen = true;
        Globals.ScreenSize = new Vector2(1024, 768);

        _graphics.PreferredBackBufferWidth = (int)Globals.ScreenSize.X;
        _graphics.PreferredBackBufferHeight = (int)Globals.ScreenSize.Y;
        _graphics.ApplyChanges();

        Globals.Content = Content;
        Globals.Gravity = 10;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;
        _gameManager = new GameManager(new Vector2(_graphics.PreferredBackBufferWidth / 2, 0));
    }    

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        Globals.Update(gameTime, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
        _gameManager.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);
        _gameManager.Draw();
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

