# Project: Astronauta 2.5D (Unity)

## Goal
Build a 2.5D retro hybrid (3D world + 2D UI/FX). Prioritize "playable > pretty".

## Unity Version
Use Unity LTS (prefer 2022.3.x). Avoid features that require newer versions unless necessary.

## Code Style
- C# .NET standard for Unity
- One public class per file, filename matches class
- Keep scripts small; prefer composition (components) over inheritance
- No third-party packages unless asked

## Gameplay Pillars
- 2.5D platformer: 3D physics, lock Z axis for player (movement on X/Y only, but world is 3D)
- Gravity is a first-class system: per-world GravityProfile (ScriptableObject)
- Oxygen is a core progression resource: collect oxygen; upgrades unlock after boss

## Definition of Done
- Compiles with zero errors
- No new Console exceptions during play
- Each task includes a small demo scene or updates an existing scene
- Provide short manual test steps

## Folder Conventions
All project content must go under Assets/_Project/...
Scenes in Assets/_Project/Scenes/...
Scripts in Assets/_Project/Scripts/...
ScriptableObjects in Assets/_Project/ScriptableObjects/...
