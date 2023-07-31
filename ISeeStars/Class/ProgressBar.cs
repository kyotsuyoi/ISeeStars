﻿using ISS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class ProgressBar
{
    protected readonly Texture2D background;
    protected readonly Texture2D foreground;
    protected readonly Vector2 position;
    protected readonly float maxValue;
    protected float currentValue;
    protected Rectangle part;

    public ProgressBar(Texture2D bg, Texture2D fg, float max, Vector2 pos)
    {
        background = bg;
        foreground = fg;
        maxValue = max;
        currentValue = max;
        position = pos;
        part = new Rectangle(0, 0, foreground.Width, foreground.Height);
    }

    public virtual void Update(float value)
    {
        currentValue = value;
        part.Width = (int)(currentValue / maxValue * foreground.Width);
    }

    public virtual void Draw()
    {
        Globals.SpriteBatch.Draw(background, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        Globals.SpriteBatch.Draw(foreground, position, part, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
    }
}
