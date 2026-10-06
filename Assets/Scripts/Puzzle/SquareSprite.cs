using UnityEngine;

public static class SquareSprite
{
    private static Sprite sprite;

    public static Sprite Get()
    {
        if (sprite == null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        return sprite;
    }
}
