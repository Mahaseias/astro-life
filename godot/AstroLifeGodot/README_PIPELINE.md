# Astro-Life Asset Pipeline (Godot 4 + C#)

## 1) Onde colocar o spritesheet
- Coloque o sheet principal em:
  - `res://Art/Tiles/StationTiles.png`
- (Opcional futuro) personagens:
  - `res://Art/Spritesheets/Characters.png`

## 2) Como rodar o pipeline
1. Abra `Project > Project Settings > Plugins`.
2. Ative `AstroLife Pipeline`.
3. No menu superior, clique:
   - `Tools -> AstroLife -> Build Pipeline`

O pipeline faz:
- aplica import/pixel settings para `res://Art/**`
- gera/atualiza `res://Tilesets/StationTileset.tres`
- gera/atualiza `res://Scenes/TestLevel.tscn`

## 3) Ajustar regiões (tiles/props/UI)
### Tiles (região do atlas usada para TileSet)
- Arquivo: `res://Scripts/Editor/BuildStationTileset.cs`
- Constante:
```csharp
public static readonly Rect2I TILE_ATLAS_REGION_XYWH = new(0, 0, 1024, 512);
```

### Props/UI por AtlasTexture
- Arquivo: `res://Scripts/Core/AtlasRegistry.cs`
- Constantes:
```csharp
public static readonly Rect2I REGION_OXYGEN_PICKUP = new(1030, 540, 64, 64);
public static readonly Rect2I REGION_CHECKPOINT = new(1180, 520, 96, 192);
public static readonly Rect2I REGION_UI_OXYGEN_BAR = new(1200, 820, 320, 80);
```

## 4) Debug de coordenadas de atlas
- Abra: `res://Scenes/AtlasRegionHelper.tscn`
- `F12`: imprime pixel do mouse na textura
- Arraste com botão esquerdo: imprime `Rect2i(x, y, w, h)`

## 5) Checklist Pixel Perfect
- `rendering/textures/canvas_textures/default_texture_filter = Nearest`
- `rendering/textures/canvas_textures/default_texture_repeat = Disabled`
- `rendering/2d/snap/snap_2d_transforms_to_pixel = true`
- `rendering/2d/snap/snap_2d_vertices_to_pixel = true`
- Import de texturas (`res://Art/**`):
  - mipmaps OFF
  - compress mode sem perda para pixel art (ajustado pelo pipeline)

## 6) Se o import não aplicar 100% automático
No Inspector da textura (2-3 cliques):
1. Selecione `StationSheet.png`
2. Aba Import: desative `Mipmaps`, mantenha compressão sem blur
3. Clique `Reimport`
