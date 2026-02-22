using Godot;

public partial class AutoTileFill : Node
{
	[Export] public NodePath TileMapLayerPath = "TileMap"; // ajuste se seu layer tiver outro nome

	public override void _Ready()
	{
		var layer = GetNodeOrNull<TileMapLayer>(TileMapLayerPath);
		if (layer == null)
		{
			GD.PrintErr($"AutoTileFill: TileMapLayer não encontrado em '{TileMapLayerPath}'.");
			return;
		}

		layer.Clear();

		// Pega o primeiro Source do TileSet automaticamente (geralmente 0)
		int sourceId = 0;

		// Escolhe UM tile existente do atlas (0,0). Se não existir, troque para outro (ex: 1,0)
		Vector2I atlas = new Vector2I(0, 0);

		// chão
		for (int x = -12; x <= 12; x++)
			layer.SetCell(new Vector2I(x, 6), sourceId, atlas);

		// plataforma
		for (int x = -6; x <= 6; x++)
			layer.SetCell(new Vector2I(x, 2), sourceId, atlas);

		GD.Print("AutoTileFill: preenchido.");
	}
}
