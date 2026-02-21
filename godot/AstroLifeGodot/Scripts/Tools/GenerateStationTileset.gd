extends SceneTree

const TILE_SIZE := 32
const TEXTURE_PATH := "res://Art/Tiles/StationTiles.png"
const TILESET_PATH := "res://Art/Tiles/StationTiles.tres"
const SCENE_PATH := "res://Scenes/TestTiles.tscn"

func _init() -> void:
	var texture: Texture2D = load(TEXTURE_PATH)
	if texture == null:
		push_error("Texture not found: %s" % TEXTURE_PATH)
		quit(1)
		return

	_apply_pixel_project_settings()
	_configure_import_texture_flags()

	var tileset := TileSet.new()
	tileset.tile_size = Vector2i(TILE_SIZE, TILE_SIZE)

	var atlas := TileSetAtlasSource.new()
	atlas.texture = texture
	atlas.texture_region_size = Vector2i(TILE_SIZE, TILE_SIZE)

	var source_id := tileset.add_source(atlas)
	var used_tiles := _create_tiles_from_texture(atlas, texture)

	var save_tileset_result := ResourceSaver.save(tileset, TILESET_PATH)
	if save_tileset_result != OK:
		push_error("Failed to save TileSet: %s" % TILESET_PATH)
		quit(2)
		return

	var scene_save_result := _create_test_scene(tileset, source_id, used_tiles)
	if scene_save_result != OK:
		push_error("Failed to save test scene: %s" % SCENE_PATH)
		quit(3)
		return

	print("TileSet generated: %s" % TILESET_PATH)
	print("Scene generated: %s" % SCENE_PATH)
	quit(0)

func _apply_pixel_project_settings() -> void:
	ProjectSettings.set_setting("rendering/2d/snap/snap_2d_transforms_to_pixel", true)
	ProjectSettings.set_setting("rendering/2d/snap/snap_2d_vertices_to_pixel", true)
	ProjectSettings.set_setting("rendering/textures/canvas_textures/default_texture_filter", 0)
	ProjectSettings.set_setting("rendering/textures/canvas_textures/default_texture_repeat", 0)
	ProjectSettings.save()

func _configure_import_texture_flags() -> void:
	var import_path := ProjectSettings.globalize_path(TEXTURE_PATH + ".import")
	if not FileAccess.file_exists(import_path):
		return

	var text := FileAccess.get_file_as_string(import_path)
	if text.is_empty():
		return

	if not text.contains("mipmaps/generate=false"):
		text += "\nmipmaps/generate=false\n"

	var write := FileAccess.open(import_path, FileAccess.WRITE)
	if write != null:
		write.store_string(text)

func _create_tiles_from_texture(atlas: TileSetAtlasSource, texture: Texture2D) -> Array[Vector2i]:
	var image := texture.get_image()
	if image == null:
		return []

	var width := texture.get_width() / TILE_SIZE
	var height := texture.get_height() / TILE_SIZE
	var used: Array[Vector2i] = []

	for y in range(height):
		for x in range(width):
			var atlas_coords := Vector2i(x, y)
			if _is_cell_fully_transparent(image, x, y):
				continue
			atlas.create_tile(atlas_coords)
			used.append(atlas_coords)

	return used

func _is_cell_fully_transparent(image: Image, grid_x: int, grid_y: int) -> bool:
	var start_x := grid_x * TILE_SIZE
	var start_y := grid_y * TILE_SIZE

	for py in range(start_y, start_y + TILE_SIZE):
		for px in range(start_x, start_x + TILE_SIZE):
			if image.get_pixel(px, py).a > 0.01:
				return false

	return true

func _create_test_scene(tileset: TileSet, source_id: int, used_tiles: Array[Vector2i]) -> int:
	var root := Node2D.new()
	root.name = "TestTiles"

	var tilemap := TileMapLayer.new()
	tilemap.name = "TileMap"
	tilemap.tile_set = tileset
	tilemap.texture_filter = CanvasItem.TEXTURE_FILTER_NEAREST
	tilemap.texture_repeat = CanvasItem.TEXTURE_REPEAT_DISABLED
	tilemap.position = Vector2.ZERO

	root.add_child(tilemap)
	tilemap.owner = root

	_paint_demo_tiles(tilemap, source_id, used_tiles)

	var packed := PackedScene.new()
	var pack_result := packed.pack(root)
	if pack_result != OK:
		return pack_result

	return ResourceSaver.save(packed, SCENE_PATH)

func _paint_demo_tiles(tilemap: TileMapLayer, source_id: int, used_tiles: Array[Vector2i]) -> void:
	if used_tiles.is_empty():
		return

	var max_tiles := mini(used_tiles.size(), 8)
	for i in range(max_tiles):
		tilemap.set_cell(Vector2i(i, 0), source_id, used_tiles[i], 0)

	for x in range(20):
		var atlas_coords := used_tiles[x % max_tiles]
		tilemap.set_cell(Vector2i(x, 1), source_id, atlas_coords, 0)

	for x in range(5):
		for y in range(2):
			var idx := (x + y) % max_tiles
			tilemap.set_cell(Vector2i(6 + x, 2 + y), source_id, used_tiles[idx], 0)