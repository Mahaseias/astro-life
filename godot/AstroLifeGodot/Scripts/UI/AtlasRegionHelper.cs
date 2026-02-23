using Godot;

[Tool]
public partial class AtlasRegionHelper : Control
{
    [Export] public NodePath SheetViewPath = "Center/SheetView";
    [Export] public string SheetPath = "res://Art/Tiles/StationTiles.png";

    private TextureRect _sheetView;
    private Vector2 _dragStart;
    private bool _dragging;

    public override void _Ready()
    {
        _sheetView = GetNodeOrNull<TextureRect>(SheetViewPath);
        if (_sheetView == null)
        {
            GD.PushWarning("[AtlasRegionHelper] SheetView not found.");
            return;
        }

        _sheetView.Texture = ResourceLoader.Load<Texture2D>(SheetPath);
        _sheetView.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_sheetView == null || _sheetView.Texture == null)
        {
            return;
        }

        if (@event is InputEventKey key && key.Pressed && !key.Echo && key.Keycode == Key.F12)
        {
            Vector2I? pixel = MouseToTexturePixel();
            if (pixel.HasValue)
            {
                GD.Print($"[AtlasRegionHelper] Mouse pixel: {pixel.Value.X}, {pixel.Value.Y}");
            }
        }

        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
        {
            if (mb.Pressed)
            {
                Vector2I? pixel = MouseToTexturePixel();
                if (pixel.HasValue)
                {
                    _dragging = true;
                    _dragStart = pixel.Value;
                }
            }
            else if (_dragging)
            {
                _dragging = false;
                Vector2I? end = MouseToTexturePixel();
                if (end.HasValue)
                {
                    Rect2I rect = BuildRect(_dragStart, end.Value);
                    GD.Print($"[AtlasRegionHelper] Selected Rect2i({rect.Position.X}, {rect.Position.Y}, {rect.Size.X}, {rect.Size.Y})");
                }
            }
        }
    }

    private Vector2I? MouseToTexturePixel()
    {
        Vector2 mouse = GetGlobalMousePosition();
        Vector2 local = _sheetView.GetGlobalTransformWithCanvas().AffineInverse() * mouse;
        Vector2 texSize = _sheetView.Texture.GetSize();
        Vector2 viewSize = _sheetView.Size;

        if (viewSize.X <= 0 || viewSize.Y <= 0)
        {
            return null;
        }

        float u = local.X / viewSize.X;
        float v = local.Y / viewSize.Y;
        if (u < 0f || u > 1f || v < 0f || v > 1f)
        {
            return null;
        }

        int px = Mathf.Clamp(Mathf.FloorToInt(u * texSize.X), 0, (int)texSize.X - 1);
        int py = Mathf.Clamp(Mathf.FloorToInt(v * texSize.Y), 0, (int)texSize.Y - 1);
        return new Vector2I(px, py);
    }

    private static Rect2I BuildRect(Vector2 start, Vector2 end)
    {
        int x = Mathf.Min((int)start.X, (int)end.X);
        int y = Mathf.Min((int)start.Y, (int)end.Y);
        int w = Mathf.Abs((int)end.X - (int)start.X) + 1;
        int h = Mathf.Abs((int)end.Y - (int)start.Y) + 1;
        return new Rect2I(x, y, w, h);
    }
}
