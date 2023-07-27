using ISS;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

public class Game : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GameManager _gm;

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Globals.ScreenX = 1024;
        Globals.ScreenY = 768;

        _graphics.PreferredBackBufferWidth = Globals.ScreenX;
        _graphics.PreferredBackBufferHeight = Globals.ScreenY;
        _graphics.ApplyChanges();

        Globals.Content = Content;
        Globals.Gravity = 10;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Globals.SpriteBatch = _spriteBatch;
        _gm = new GameManager(new Vector2(_graphics.PreferredBackBufferWidth / 2, /*(_graphics.PreferredBackBufferHeight / 2) + 100*/0));
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        Globals.Update(gameTime);
        _gm.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);
        _gm.Draw();
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

