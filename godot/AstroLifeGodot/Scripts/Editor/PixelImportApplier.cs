using Godot;
using System;
using System.Collections.Generic;
using System.IO;

[Tool]
public partial class PixelImportApplier : RefCounted
{
    private static readonly string[] TextureExtensions = { ".png", ".webp", ".jpg", ".jpeg" };
    private const string ArtRootResPath = "res://Art";

    public static bool Apply(EditorInterface editorInterface = null)
    {
        ApplyProjectPixelSettings();

        List<string> changedTextureResPaths = new();
        foreach (string textureResPath in EnumerateArtTextures())
        {
            if (PatchImportFile(textureResPath))
            {
                changedTextureResPaths.Add(textureResPath);
            }
        }

        if (changedTextureResPaths.Count > 0 && editorInterface != null)
        {
            Godot.Collections.Array<string> files = new();
            foreach (string path in changedTextureResPaths)
            {
                files.Add(path);
            }

            EditorFileSystem fs = editorInterface.GetResourceFilesystem();
            fs?.Call("reimport_files", files);
        }

        GD.Print($"[PixelImportApplier] Done. Reimported: {changedTextureResPaths.Count}");
        return true;
    }

    private static void ApplyProjectPixelSettings()
    {
        ProjectSettings.SetSetting("rendering/textures/canvas_textures/default_texture_filter", 0);
        ProjectSettings.SetSetting("rendering/textures/canvas_textures/default_texture_repeat", 0);
        ProjectSettings.SetSetting("rendering/2d/snap/snap_2d_transforms_to_pixel", true);
        ProjectSettings.SetSetting("rendering/2d/snap/snap_2d_vertices_to_pixel", true);
        ProjectSettings.Save();
    }

    private static IEnumerable<string> EnumerateArtTextures()
    {
        string artGlobal = ProjectSettings.GlobalizePath(ArtRootResPath);
        if (!Directory.Exists(artGlobal))
        {
            yield break;
        }

        foreach (string file in Directory.EnumerateFiles(artGlobal, "*.*", SearchOption.AllDirectories))
        {
            string ext = Path.GetExtension(file).ToLowerInvariant();
            bool isTexture = Array.Exists(TextureExtensions, value => value == ext);
            if (!isTexture)
            {
                continue;
            }

            yield return GlobalToResPath(file);
        }
    }

    private static bool PatchImportFile(string textureResPath)
    {
        string importGlobalPath = ProjectSettings.GlobalizePath(textureResPath + ".import");
        if (!File.Exists(importGlobalPath))
        {
            return false;
        }

        List<string> lines = new(File.ReadAllLines(importGlobalPath));
        bool changed = false;

        changed |= ReplaceOrAppend(lines, "compress/mode=", "compress/mode=0");
        changed |= ReplaceOrAppend(lines, "mipmaps/generate=", "mipmaps/generate=false");
        changed |= ReplaceOrAppend(lines, "mipmaps/limit=", "mipmaps/limit=-1");
        changed |= ReplaceOrAppend(lines, "process/fix_alpha_border=", "process/fix_alpha_border=false");
        changed |= ReplaceOrAppend(lines, "detect_3d/compress_to=", "detect_3d/compress_to=1");

        if (!changed)
        {
            return false;
        }

        UTF8EncodingNoBom.WriteAllLines(importGlobalPath, lines);
        return true;
    }

    private static bool ReplaceOrAppend(List<string> lines, string startsWith, string replacement)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith(startsWith, StringComparison.Ordinal))
            {
                if (lines[i] == replacement)
                {
                    return false;
                }

                lines[i] = replacement;
                return true;
            }
        }

        lines.Add(replacement);
        return true;
    }

    private static string GlobalToResPath(string globalPath)
    {
        string projectRoot = ProjectSettings.GlobalizePath("res://");
        string normalized = globalPath.Replace('\\', '/');
        string normalizedRoot = projectRoot.Replace('\\', '/');
        if (!normalizedRoot.EndsWith("/"))
        {
            normalizedRoot += "/";
        }

        if (normalized.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            return "res://" + normalized.Substring(normalizedRoot.Length);
        }

        return globalPath;
    }

    private static class UTF8EncodingNoBom
    {
        public static void WriteAllLines(string path, IEnumerable<string> lines)
        {
            using StreamWriter writer = new(path, false, new System.Text.UTF8Encoding(false));
            foreach (string line in lines)
            {
                writer.WriteLine(line);
            }
        }
    }
}
