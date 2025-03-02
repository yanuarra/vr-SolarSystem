using UnityEngine;

public static class ConvertToSpriteExtensiton
{
    public static Sprite ConvertToSprite2D(this Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    public static Texture2D ToTexture2D(this Texture texture)
    {
        return Texture2D.CreateExternalTexture(
            texture.width,
            texture.height,
            TextureFormat.RGB24,
            false, false,
            texture.GetNativeTexturePtr());
    }

    public static Sprite ConvertToSprite(this Texture texture)
    {
        Texture2D texture2D = texture.ToTexture2D();
        return Sprite.Create(texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}