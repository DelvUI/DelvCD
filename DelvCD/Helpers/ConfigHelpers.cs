using Dalamud.Interface.Internal.Notifications;
using Dalamud.Plugin.Services;
using DelvCD.Config;
using DelvCD.Helpers.DataSources;
using DelvCD.UIElements;
using ImGuiNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DelvCD.Helpers
{
    public static class ConfigHelpers
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            TypeNameHandling = TypeNameHandling.Objects,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            SerializationBinder = new DelvCDSerializationBinder()
        };

        public static T? SerializedClone<T>(T? obj) where T : class?
        {
            if (obj == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj, Formatting.None, _serializerSettings), _serializerSettings);
        }

        public static void ExportToClipboard<T>(T toExport)
        {
            string? exportString = ConfigHelpers.GetExportString(toExport);

            if (exportString is not null)
            {
                ImGui.SetClipboardText(exportString);
                DrawHelpers.DrawNotification("Export string copied to clipboard.");
            }
            else
            {
                DrawHelpers.DrawNotification("Failed to Export!", NotificationType.Error);
            }
        }

        public static string? GetExportString<T>(T toExport)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(toExport, Formatting.None, _serializerSettings);
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (DeflateStream compressionStream = new DeflateStream(outputStream, CompressionLevel.Optimal))
                    {
                        using (StreamWriter writer = new StreamWriter(compressionStream, Encoding.UTF8))
                        {
                            writer.Write(jsonString);
                        }
                    }

                    return Convert.ToBase64String(outputStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Error(ex.ToString());
            }

            return null;
        }

        public static T? GetFromImportString<T>(string importString)
        {
            if (string.IsNullOrEmpty(importString)) { return default; }

            try
            {
                byte[] bytes = Convert.FromBase64String(importString);

                string decodedJsonString;
                using (MemoryStream inputStream = new MemoryStream(bytes))
                {
                    using (DeflateStream compressionStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(compressionStream, Encoding.UTF8))
                        {
                            decodedJsonString = reader.ReadToEnd();
                        }
                    }
                }

                decodedJsonString = SanitizedJsonString(decodedJsonString);

                T? importedObj = JsonConvert.DeserializeObject<T>(decodedJsonString, _serializerSettings);

                if (!decodedJsonString.Contains("\"Version\":"))
                {
                    if (importedObj is UIElement element)
                    {
                        MigrateStyleConditions(element);
                    }
                }

                return importedObj;
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Error(ex.ToString());
            }

            return default;
        }

        // backwards compatibility
        private static string SanitizedJsonString(string json)
        {
            return json
                .Replace("XIVAuras", "DelvCD")
                .Replace("AuraList", "ElementList")
                .Replace("AuraGroup", "Group")
                .Replace("AuraBar", "Bar")
                .Replace("AuraIcon", "Icon")
                .Replace("AuraLabel", "Label")
                .Replace("Auras", "UIElements");
        }

        public static DelvCDConfig LoadConfig(string path)
        {
            DelvCDConfig? config = null;

            try
            {
                if (File.Exists(path))
                {
                    string jsonString = File.ReadAllText(path);
                    config = JsonConvert.DeserializeObject<DelvCDConfig>(jsonString, _serializerSettings);

                    // TODO: Eventualy remove this:
                    // special migration needed for 0.3.0.0 -> 0.4.0.0
                    // ugly, but it is what it is...
                    if (config != null && jsonString.Contains("\"Version\": \"0.3.0.0\""))
                    {
                        MigrateStyleConditions(config.ElementList.UIElements);
                        SaveConfig(config);
                    }
                }
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Error(ex.ToString());

                string backupPath = $"{path}.bak";
                if (File.Exists(path))
                {
                    try
                    {
                        File.Copy(path, backupPath);
                        Singletons.Get<IPluginLog>().Information($"Backed up DelvCD config to '{backupPath}'.");
                    }
                    catch
                    {
                        Singletons.Get<IPluginLog>().Warning($"Unable to back up DelvCD config.");
                    }
                }
            }

            return config ?? new DelvCDConfig();
        }

        public static void MigrateStyleConditions(List<UIElement> elements)
        {
            foreach (UIElement element in elements)
            {
                MigrateStyleConditions(element);
            }
        }

        public static void MigrateStyleConditions(UIElement element)
        {
            if (element is Group group)
            {
                MigrateStyleConditions(group.ElementList.UIElements);
            }
            else if (element is Icon icon)
            {
                DataSource[] dataSources = icon.TriggerConfig.TriggerOptions.Select(x => x.DataSource).ToArray();
                icon.IconStyleConfig.UpdateDataSources(dataSources, true);

                foreach (StyleCondition<IconStyleConfig> condition in icon.StyleConditions.Conditions)
                {
                    condition.TriggerDataSourceIndex = Math.Max(0, condition.TriggerDataSourceIndex - 1);

                    if (condition.Source >= 3)
                    {
                        condition.Source -= 3;
                    }
                }

                icon.StyleConditions.UpdateDataSources(dataSources, true);

                foreach (Label label in icon.LabelListConfig.Labels)
                {
                    MigrateTextFormats(label, dataSources);

                    foreach (StyleCondition<LabelStyleConfig> condition in label.StyleConditions.Conditions)
                    {
                        condition.TriggerDataSourceIndex = Math.Max(0, condition.TriggerDataSourceIndex - 1);

                        if (condition.Source >= 3)
                        {
                            condition.Source -= 3;
                        }
                    }
                }
            }
            else if (element is Bar bar)
            {
                foreach (StyleCondition<BarStyleConfig> condition in bar.StyleConditions.Conditions)
                {
                    condition.TriggerDataSourceIndex = Math.Max(0, condition.TriggerDataSourceIndex - 1);

                    if (condition.Source >= 3)
                    {
                        condition.Source -= 3;
                    }
                }
            }
            else if (element is Label label)
            {
                foreach (StyleCondition<LabelStyleConfig> condition in label.StyleConditions.Conditions)
                {
                    condition.TriggerDataSourceIndex = Math.Max(0, condition.TriggerDataSourceIndex - 1);

                    if (condition.Source >= 3)
                    {
                        condition.Source -= 3;
                    }
                }
            }
        }

        private static List<Dictionary<string, string>> _migrationTextFormatMap = new()
        {
            new Dictionary<string, string>()
            {
                ["[value"] = "[cooldown_timer",
                ["[maxvalue"] = "[max_cooldown_timer",
                ["[stacks"] = "[cooldown_stacks",
                ["[maxstacks"] = "[max_cooldown_stacks",
            },

            new Dictionary<string, string>()
            {
                ["[value"] = "[status_timer",
                ["[maxvalue"] = "[max_status_timer",
                ["[stacks"] = "[status_stacks",
                ["[maxstacks"] = "[max_status_stacks",
            },

            new Dictionary<string, string>()
            {
                ["[value"] = "[item_cooldown_timer",
                ["[maxvalue"] = "[max_item_cooldown_timer",
                ["[stacks"] = "[item_cooldown_stacks",
                ["[maxstacks"] = "[max_item_cooldown_stacks",
            },
        };

        private static void MigrateTextFormats(Label label, DataSource[] dataSources)
        {
            bool hasCooldownTrigger = dataSources.Any(x => x is CooldownDataSource);
            bool hasStatusTrigger = dataSources.Any(x => x is StatusDataSource);
            bool hasItemCooldownTrigger = dataSources.Any(x => x is ItemCooldownDataSource);

            int index = hasCooldownTrigger ? 0 : (hasStatusTrigger ? 1 : (hasItemCooldownTrigger ? 2 : -1));
            if (index == -1) { return; }

            MigrateTextFormat(label.LabelStyleConfig, _migrationTextFormatMap[index]);

            foreach (StyleCondition<LabelStyleConfig> condition in label.StyleConditions.Conditions)
            {
                MigrateTextFormat(condition.Style, _migrationTextFormatMap[index]);
            }
        }

        private static void MigrateTextFormat(LabelStyleConfig labelStyle, Dictionary<string, string> map)
        {
            foreach (KeyValuePair<string, string> item in map)
            {
                labelStyle.TextFormat = labelStyle.TextFormat.Replace(item.Key, item.Value);
            }
        }

        public static void SaveConfig()
        {
            ConfigHelpers.SaveConfig(Singletons.Get<DelvCDConfig>());
        }

        public static void SaveConfig(DelvCDConfig config)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(config, Formatting.Indented, _serializerSettings);
                File.WriteAllText(Plugin.ConfigFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Error(ex.ToString());
            }
        }

        public static void CheckVersion()
        {
            string path = Path.Combine(Plugin.ConfigFileDir, "version");
            string? previousVersion = null;

            try
            {
                bool needsWrite = false;
                bool needsBackup = false;

                if (!File.Exists(path))
                {
                    needsWrite = true;
                    needsBackup = true;
                    previousVersion = "0.3.0.0";
                }
                else
                {
                    previousVersion = File.ReadAllText(path);
                    if (previousVersion != Plugin.Version)
                    {
                        needsWrite = true;
                        needsBackup = true;
                    }
                }

                if (needsWrite)
                {
                    File.WriteAllText(path, Plugin.Version);
                }

                if (needsBackup && previousVersion != null)
                {
                    BackupPreviousVersion(previousVersion);
                }
            }
            catch (Exception e)
            {
                Singletons.Get<IPluginLog>().Error("Error checking version: " + e.Message);
            }
        }

        private static void BackupPreviousVersion(string version)
        {
            string backupsRoot = Path.Combine(Plugin.ConfigFileDir, "Backups");
            if (!Directory.Exists(backupsRoot))
            {
                Directory.CreateDirectory(backupsRoot);
            }

            string versionFolder = Path.Combine(backupsRoot, version);
            if (!Directory.Exists(versionFolder))
            {
                Directory.CreateDirectory(versionFolder);
            }

            try
            {
                string destination = Path.Combine(versionFolder, "DelvCD.json");
                File.Copy(Plugin.ConfigFilePath, destination);
            }
            catch { }
        }
    }

    public class LabelConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Write not supported.");
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(LabelStyleConfig))
            {
                return serializer.Deserialize(reader, objectType);
            }

            LabelStyleConfig? label = null;
            try
            {
                label = serializer.Deserialize(reader, objectType) as LabelStyleConfig;
                if (label is not null &&
                    Singletons.IsRegistered<DelvCDConfig>() &&
                    Singletons.IsRegistered<FontsManager>())
                {
                    string[] fontOptions = FontsManager.GetFontList();
                    if (!FontsManager.ValidateFont(fontOptions, label.FontID, label.FontKey))
                    {
                        string[] parts = label.FontKey.Split('_');
                        if (parts.Length < 2)
                        {
                            return label;
                        }

                        string fontName = parts[0];
                        string[] installedFonts = FontsManager.GetFontNamesFromPath(FontsManager.GetUserFontPath());
                        foreach (string installedFont in installedFonts)
                        {
                            if (fontName.ToLower().Equals(installedFont.ToLower()))
                            {
                                int fontSize = int.TryParse(parts[1], out int size) ? size : 16;
                                bool cnjp = label.FontKey.Contains("_cnjp");
                                bool kr = label.FontKey.Contains("_kr");
                                FontData fontData = new FontData(fontName, fontSize, cnjp, kr);
                                Singletons.Get<DelvCDConfig>().FontConfig.AddFont(fontData);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Warning(ex.ToString());
            }

            return label;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(LabelStyleConfig) == objectType;
        }
    }

    /// <summary>
    /// Because the game blocks the json serializer from loading assemblies at runtime, we define
    /// a custom SerializationBinder to ignore the assembly name for the types defined by this plugin.
    /// </summary>
    public class DelvCDSerializationBinder : ISerializationBinder
    {
        // FIXME: Make this automatic somehow
        private static List<Type> _configTypes = new List<Type>()
        {
            typeof(Bar),
            typeof(Group),
            typeof(Icon),
            typeof(Label),
            typeof(UIElement),
            typeof(ElementListConfig),
            typeof(BarStyleConfig),
            typeof(CooldownTrigger),
            typeof(ConfigColor),
            typeof(FontConfig),
            typeof(FontData),
            typeof(IconStyleConfig),
            typeof(ItemCooldownTrigger),
            typeof(LabelStyleConfig),
            typeof(StatusTrigger),
            typeof(TriggerConfig),
            typeof(TriggerData),
            typeof(TriggerOptions),
            typeof(VisibilityConfig),
            typeof(DelvCDConfig)
        };

        private readonly Dictionary<Type, string> typeToName = new Dictionary<Type, string>();
        private readonly Dictionary<string, Type> nameToType = new Dictionary<string, Type>();

        public DelvCDSerializationBinder()
        {
            foreach (Type type in _configTypes)
            {
                if (type.FullName is not null)
                {
                    typeToName.Add(type, type.FullName);
                    nameToType.Add(type.FullName, type);
                }
            }
        }

        public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            if (typeToName.TryGetValue(serializedType, out string? name))
            {
                assemblyName = null;
                typeName = name;
            }
            else
            {
                assemblyName = serializedType.Assembly.FullName;
                typeName = serializedType.FullName;
            }
        }

        public Type BindToType(string? assemblyName, string? typeName)
        {
            if (typeName is not null &&
                nameToType.TryGetValue(typeName, out Type? type))
            {
                return type;
            }

            return Type.GetType($"{typeName}, {assemblyName}", true) ??
                throw new TypeLoadException($"Unable to load type '{typeName}' from assembly '{assemblyName}'");
        }
    }
}
