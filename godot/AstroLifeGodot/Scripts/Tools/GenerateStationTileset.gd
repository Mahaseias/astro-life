extends SceneTree

const TILE_SIZE := 32
const TEXTURE_PATH := "res://Art/Spritesheets/StationSheet.png"
const TILESET_PATH := "res://Tilesets/StationTileset.tres"
const SCENE_PATH := "res://Scenes/TestLevel.tscn"
const TILE_ATLAS_REGION := Rect2i(0, 0, 1024, 512) # top-left tile area only

func _init() -> void:
	_apply_pixel_project_settings()
	_configure_import_texture_flags()

	var texture: Texture2D = load(TEXTURE_PATH)
	if texture == null:
		push_error("[FixLego] Texture not found: %s" % TEXTURE_PATH)
		quit(1)
		return

	var tileset := _build_tileset(texture)
	if tileset == null:
		push_error("[FixLego] Failed to build tileset.")
		quit(2)
		return

	var save_tileset_result := ResourceSaver.save(tileset, TILESET_PATH)
	if save_tileset_result != OK:
		push_error("[FixLego] Failed to save TileSet: %s (%s)" % [TILESET_PATH, save_tileset_result])
		quit(3)
		return

	var scene_save_result := _upsert_test_scene(tileset)
	if scene_save_result != OK:
		push_error("[FixLego] Failed to save test scene: %s (%s)" % [SCENE_PATH, scene_save_result])
		quit(4)
		return

	print("[FixLego] TileSet generated: %s" % TILESET_PATH)
	print("[FixLego] Scene updated: %s" % SCENE_PATH)
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

	text = _replace_or_append_line(text, "mipmaps/generate=", "mipmaps/generate=false")
	text = _replace_or_append_line(text, "compress/mode=", "compress/mode=0")

	var write := FileAccess.open(import_path, FileAccess.WRITE)
	if write != null:
		write.store_string(text)

func _replace_or_append_line(text: String, key_prefix: String, value_line: String) -> String:
	var lines := text.split("\n")
	var replaced := false
	for i in range(lines.size()):
		if lines[i].begins_with(key_prefix):
			lines[i] = value_line
			replaced = true
	if not replaced:
		lines.append(value_line)
	return "\n".join(lines)

func _build_tileset(texture: Texture2D) -> TileSet:
	var image := texture.get_image()
	if image == null:
		return null

	var sheet_size := Vector2i(texture.get_width(), texture.get_height())
	var clamped := _clamp_region(TILE_ATLAS_REGION, sheet_size)
	var start_cell_x := clamped.position.x / TILE_SIZE
	var start_cell_y := clamped.position.y / TILE_SIZE
	var cells_x := clamped.size.x / TILE_SIZE
	var cells_y := clamped.size.y / TILE_SIZE

	var tileset := TileSet.new()
	tileset.tile_size = Vector2i(TILE_SIZE, TILE_SIZE)

	var atlas := TileSetAtlasSource.new()
	atlas.texture = texture
	atlas.texture_region_size = Vector2i(TILE_SIZE, TILE_SIZE)
	atlas.margins = Vector2i.ZERO
	atlas.separation = Vector2i.ZERO

	tileset.add_source(atlas)

	var created := 0
	for y in range(cells_y):
		for x in range(cells_x):
			var px := clamped.position.x + (x * TILE_SIZE)
			var py := clamped.position.y + (y * TILE_SIZE)
			if _is_cell_fully_transparent(image, px, py):
				continue

			var atlas_coords := Vector2i(start_cell_x + x, start_cell_y + y)
			atlas.create_tile(atlas_coords)
			created += 1

	print("[FixLego] Tiles created: %d" % created)
	return tileset

func _clamp_region(region: Rect2i, max_size: Vector2i) -> Rect2i:
	var x := clampi(region.position.x, 0, max_size.x)
	var y := clampi(region.position.y, 0, max_size.y)
	var w := clampi(region.size.x, 0, max_size.x - x)
	var h := clampi(region.size.y, 0, max_size.y - y)
	w -= w % TILE_SIZE
	h -= h % TILE_SIZE
	return Rect2i(x, y, w, h)

func _is_cell_fully_transparent(image: Image, start_x: int, start_y: int) -> bool:
	for py in range(start_y, start_y + TILE_SIZE):
		for px in range(start_x, start_x + TILE_SIZE):
			if image.get_pixel(px, py).a > 0.01:
				return false
	return true

func _upsert_test_scene(tileset: TileSet) -> int:
	var packed_existing := load(SCENE_PATH) as PackedScene
	var root: Node2D
	if packed_existing != null:
		root = packed_existing.instantiate() as Node2D
	if root == null:
		root = Node2D.new()
		root.name = "TestLevel"

	var tilemap := _find_first_tilemap(root)
	if tilemap == null:
		tilemap = TileMapLayer.new()
		tilemap.name = "TileMap"
		root.add_child(tilemap)
		tilemap.owner = root

	tilemap.tile_set = tileset
	tilemap.texture_filter = CanvasItem.TEXTURE_FILTER_NEAREST
	tilemap.texture_repeat = CanvasItem.TEXTURE_REPEAT_DISABLED

	if tilemap.get_used_cells().is_empty():
		_paint_demo_tiles(tilemap)

	var packed := PackedScene.new()
	var pack_result := packed.pack(root)
	if pack_result != OK:
		root.free()
		return pack_result
	var save_result := ResourceSaver.save(packed, SCENE_PATH)
	root.free()
	return save_result

func _find_first_tilemap(node: Node) -> TileMapLayer:
	if node is TileMapLayer:
		return node as TileMapLayer
	for child in node.get_children():
		var found := _find_first_tilemap(child)
		if found != null:
			return found
	return null

func _paint_demo_tiles(tilemap: TileMapLayer) -> void:
	var atlas_coord := Vector2i.ZERO
	for x in range(30):
		tilemap.set_cell(Vector2i(x, 14), 0, atlas_coord, 0)
	for x in range(6, 12):
		tilemap.set_cell(Vector2i(x, 10), 0, atlas_coord, 0)
	for x in range(17, 24):
		tilemap.set_cell(Vector2i(x, 8), 0, atlas_coord, 0)
