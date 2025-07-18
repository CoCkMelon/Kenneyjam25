using UnityEngine;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class DialogueLoaderEnhanced
{
    public static DialogueScene Load(string path)
    {
        string fullPath = GetFullPath(path);
        
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Dialogue file not found: {fullPath}");
            return null;
        }
        
        try
        {
            var yaml = File.ReadAllText(fullPath, System.Text.Encoding.UTF8);
            Debug.Log($"Loaded YAML content length: {yaml.Length}");
            Debug.Log($"First 100 chars of YAML: '{yaml.Substring(0, Mathf.Min(100, yaml.Length))}'");
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
            var result = deserializer.Deserialize<DialogueScene>(yaml);
            
            // Debug the parsed result
            if (result != null && result.lines != null)
            {
                Debug.Log($"Successfully parsed {result.lines.Count} dialogue lines");
                for (int i = 0; i < Mathf.Min(3, result.lines.Count); i++)
                {
                    var line = result.lines[i];
                    Debug.Log($"Line {i}: Speaker='{line.speaker}', Text length={line.text?.Length ?? 0}");
                    if (!string.IsNullOrEmpty(line.text))
                    {
                        Debug.Log($"Line {i} text preview: '{line.text.Substring(0, Mathf.Min(50, line.text.Length))}'");
                    }
                }
            }
            
            return result;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading dialogue from {fullPath}: {e.Message}");
            return null;
        }
    }
    
    private static string GetFullPath(string path)
    {
        // If it's already an absolute path, use it as is
        if (Path.IsPathRooted(path))
        {
            return path;
        }
        
        // Try different relative path combinations
        string[] possiblePaths = {
            // Direct path from Assets folder
            Path.Combine(Application.dataPath, path),
            
            // Remove "Assets/" prefix if present
            Path.Combine(Application.dataPath, path.StartsWith("Assets/") ? path.Substring(7) : path),
            
            // Try StreamingAssets folder
            Path.Combine(Application.streamingAssetsPath, path),
            Path.Combine(Application.streamingAssetsPath, Path.GetFileName(path)),
            
            // Try persistent data path
            Path.Combine(Application.persistentDataPath, path),
            Path.Combine(Application.persistentDataPath, Path.GetFileName(path)),
            
            // Try relative to project root
            Path.Combine(Directory.GetCurrentDirectory(), path),
            
            // Try just the filename in various locations
            Path.Combine(Application.dataPath, "DialogueData", Path.GetFileName(path)),
            Path.Combine(Application.dataPath, "Resources", Path.GetFileName(path)),
        };
        
        foreach (string possiblePath in possiblePaths)
        {
            if (File.Exists(possiblePath))
            {
                Debug.Log($"Found dialogue file at: {possiblePath}");
                return possiblePath;
            }
        }
        
        // If none found, return the original path and let it fail with a clear error
        Debug.LogWarning($"Dialogue file not found in any of the expected locations. Tried: {string.Join(", ", possiblePaths)}");
        return path;
    }
    
    /// <summary>
    /// Load dialogue from Resources folder (no file extension needed)
    /// </summary>
    public static DialogueScene LoadFromResources(string resourcePath)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        if (textAsset == null)
        {
            Debug.LogError($"Dialogue resource not found: {resourcePath}");
            return null;
        }
        
        try
        {
            Debug.Log($"Loaded resource text length: {textAsset.text.Length}");
            Debug.Log($"First 100 chars of resource text: '{textAsset.text.Substring(0, Mathf.Min(100, textAsset.text.Length))}'");
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
            var result = deserializer.Deserialize<DialogueScene>(textAsset.text);
            
            // Debug the parsed result
            if (result != null && result.lines != null)
            {
                Debug.Log($"Successfully parsed {result.lines.Count} dialogue lines from Resources");
                for (int i = 0; i < Mathf.Min(3, result.lines.Count); i++)
                {
                    var line = result.lines[i];
                    Debug.Log($"Line {i}: Speaker='{line.speaker}', Text length={line.text?.Length ?? 0}");
                    if (!string.IsNullOrEmpty(line.text))
                    {
                        Debug.Log($"Line {i} text preview: '{line.text.Substring(0, Mathf.Min(50, line.text.Length))}'");
                    }
                }
            }
            
            return result;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading dialogue from Resources/{resourcePath}: {e.Message}");
            return null;
        }
    }
}
