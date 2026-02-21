# Astro Life (Godot Vertical Slice)

## Versao recomendada
- Godot 4.x com suporte C#/.NET (recomendado: Godot 4.2+ .NET)
- Projeto: `godot/AstroLifeGodot/`

## Como abrir
1. Abra o Godot .NET.
2. Clique em **Import**.
3. Selecione `godot/AstroLifeGodot/project.godot`.

## Como rodar
1. Abra o projeto.
2. Clique em **Play** (F5).
3. Cena inicial: `MainMenu.tscn`.
4. No menu, escolha entre:
   - `Jogar - Astronauta`
   - `Jogar - Astronauta Feminina`

## Controles
- Movimento: `A/D` ou `Setas`
- Pulo: `Space`
- Debug:
  - `F1` -> W1_1
  - `F2` -> W1_2
  - `F3` -> Boss
  - `R` -> Respawn

## Fluxo das cenas
- `MainMenu` -> `W1_1` -> `W1_2` -> `Boss` -> `Victory`

## Sistemas implementados
- Player side-scroller 2.5D (CharacterBody2D)
- Dois personagens principais em pixel art (masculino e feminino)
- Animacoes de estado: `idle`, `walk`, `jump`, `fall`
- Oxigenio (drain + pickup + empty => respawn)
- Checkpoint por fase
- RespawnManager global (autoload)
- Portal entre fases
- Boss com loop `INVULNERAVEL <-> VULNERAVEL`

## Padrao visual Astro-Life
- Style guide: `godot/AstroLifeGodot/Art/Style/ASTRO_LIFE_STYLE_GUIDE.md`
- Paleta oficial: `godot/AstroLifeGodot/Art/Style/palette_astro_life.gpl`
- Spritesheets 64x64:
  - `godot/AstroLifeGodot/Art/Characters/male_spritesheet_64x64.png`
  - `godot/AstroLifeGodot/Art/Characters/female_spritesheet_64x64.png`
- Tiles base 32x32: `godot/AstroLifeGodot/Art/Tiles/tileset_base_32.png`
- UI 16px grid: `godot/AstroLifeGodot/Art/UI/*`
- Boss concept render: `godot/AstroLifeGodot/Art/Enemies/collector_boss_body_160x96.png`

## Onde ajustar balanceamento
- Dreno base de oxigenio:
  - `godot/AstroLifeGodot/Scripts/Systems/OxygenSystem.cs`
  - propriedade `DrainPerSecond`
- Valor do pickup:
  - `godot/AstroLifeGodot/Scripts/Systems/OxygenPickup.cs`
  - propriedade `OxygenAmount`
- Respawn/Checkpoint:
  - `godot/AstroLifeGodot/Scripts/Core/RespawnManager.cs`
- Boss (HP, tempos, multiplicador de drain):
  - `godot/AstroLifeGodot/Scripts/Enemies/CollectorBoss2D.cs`

## Autoloads
Este projeto ja vem com autoload no `project.godot`:
- `GameSession` -> `res://Scripts/Core/GameSession.cs`
- `RespawnManager` -> `res://Scripts/Core/RespawnManager.cs`
- `GlobalDebug` -> `res://Scripts/Core/GlobalDebug.cs`

Se o Godot nao carregar autoload automaticamente, configure em:
`Project > Project Settings > Autoload`.
