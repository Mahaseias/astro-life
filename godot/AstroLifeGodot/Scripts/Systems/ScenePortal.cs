using Godot;
using System.IO;

public partial class ScenePortal : Area2D
{
    [Export(PropertyHint.File, "*.tscn")] public string NextScenePath = "";

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        Sprite2D visual = GetNodeOrNull<Sprite2D>("Visual");
        Label label = GetNodeOrNull<Label>("Label");

        if (visual != null)
        {
            if (AssetRegistry.Instance != null)
            {
                visual.Texture = AssetRegistry.Instance.GetTexture(AssetRegistry.Portal, visual.Texture);
                AssetRegistry.Instance.ApplyNearest(visual);
            }
            else
            {
                visual.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
            }

            if (IsSolidWhiteCard(visual.Texture))
            {
                visual.Texture = CreatePortalFallbackTexture();
            }
        }

        if (label != null && !string.IsNullOrWhiteSpace(NextScenePath))
        {
            label.Text = Path.GetFileNameWithoutExtension(NextScenePath);
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not PlayerController)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(NextScenePath))
        {
            GD.PrintErr("ScenePortal sem NextScenePath definido.");
            return;
        }

        GetTree().ChangeSceneToFile(NextScenePath);
    }

    private static bool IsSolidWhiteCard(Texture2D texture)
    {
        if (texture == null)
        {
            return true;
        }

        Image image = texture.GetImage();
        if (image == null)
        {
            return false;
        }

        int w = image.GetWidth();
        int h = image.GetHeight();
        if (w <= 4 || h <= 4)
        {
            return false;
        }

        Color[] corners =
        {
            image.GetPixel(0, 0),
            image.GetPixel(w - 1, 0),
            image.GetPixel(0, h - 1),
            image.GetPixel(w - 1, h - 1),
        };

        int whiteCorners = 0;
        foreach (Color c in corners)
        {
            if (c.A > 0.95f && c.R > 0.92f && c.G > 0.92f && c.B > 0.92f)
            {
                whiteCorners++;
            }
        }

        return whiteCorners >= 3;
    }

    private static Texture2D CreatePortalFallbackTexture()
    {
        const int w = 48;
        const int h = 64;
        Image image = Image.CreateEmpty(w, h, false, Image.Format.Rgba8);
        image.Fill(new Color(0, 0, 0, 0));

        Vector2 center = new(w * 0.5f, h * 0.5f);
        float outerRadius = 18f;
        float innerRadius = 12f;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = x - center.X;
                float dy = y - center.Y;
                float d = Mathf.Sqrt((dx * dx) + (dy * dy));

                if (d <= outerRadius && d >= innerRadius)
                {
                    image.SetPixel(x, y, new Color(0.29f, 0.64f, 1.0f, 1.0f));
                }
                else if (d < innerRadius - 2f)
                {
                    image.SetPixel(x, y, new Color(0.10f, 0.20f, 0.45f, 0.82f));
                }
            }
        }

        ImageTexture tex = ImageTexture.CreateFromImage(image);
        return tex;
    }
}
