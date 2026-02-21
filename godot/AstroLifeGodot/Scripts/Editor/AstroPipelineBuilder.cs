using Godot;
using System.IO;

[Tool]
public partial class AstroPipelineBuilder : RefCounted
{
    public static bool BuildAll(EditorInterface editorInterface)
    {
        EnsureFolderStructure();
        EnsureDefaultSheet();

        bool importsOk = PixelImportApplier.Apply(editorInterface);
        bool tilesOk = BuildStationTileset.Build();
        bool levelOk = TestLevelBuilder.Build();

        editorInterface?.GetResourceFilesystem()?.Scan();

        bool success = importsOk && tilesOk && levelOk;
        if (success)
        {
            GD.Print("[AstroPipelineBuilder] Pipeline build completed successfully.");
        }
        else
        {
            GD.PushWarning("[AstroPipelineBuilder] Pipeline build finished with warnings/errors.");
        }

        return success;
    }

    private static void EnsureFolderStructure()
    {
        string[] dirs =
        {
            "res://Art/Spritesheets",
            "res://Art/Tiles",
            "res://Art/Props",
            "res://Art/UI",
            "res://Tilesets",
            "res://Scenes",
            "res://Scripts/Editor",
            "res://Scripts/Core",
            "res://Scripts/UI",
        };

        foreach (string dir in dirs)
        {
            DirAccess.MakeDirRecursiveAbsolute(dir);
        }
    }

    private static void EnsureDefaultSheet()
    {
        string sheetRes = BuildStationTileset.SHEET_PATH;
        string fallbackRes = "res://Art/Spritesheets/StationSheet.png";

        string sheetGlobal = ProjectSettings.GlobalizePath(sheetRes);
        if (File.Exists(sheetGlobal))
        {
            return;
        }

        string fallbackGlobal = ProjectSettings.GlobalizePath(fallbackRes);
        if (!File.Exists(fallbackGlobal))
        {
            GD.PushWarning($"[AstroPipelineBuilder] Missing spritesheet at {sheetRes} and fallback at {fallbackRes}.");
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(sheetGlobal));
        File.Copy(fallbackGlobal, sheetGlobal, true);
        GD.Print($"[AstroPipelineBuilder] Created default sheet from fallback: {sheetRes}");
    }
}
