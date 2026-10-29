using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Translation;
using Reactor;
using Reactor.Utilities;
using Reactor.Networking;
using Reactor.Networking.Attributes;

namespace FreezerRole;

[BepInAutoPlugin("com.isax.freezer", "FrezerRole")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class FreezerRole : BasePlugin, IMiraPlugin
{
    public const string DisplayVersion = "1.0.0";
    public const string Author = "Isax820";
    public FreezerRole()
    {
        MiraLocaleManager.Register("com.isax.freezer");
    }

    public Harmony Harmony { get; } = new(Id);
    public string OptionsTitleText { get; } = "Mira API\nFreezerRole";
    public string CustomOptionMenuNameTwo { get; } = "Example Options";

    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    public override void Load()
    {
        ReactorCredits.Register(
            "FreezerRole",
            DisplayVersion,
            false,
            ReactorCredits.AlwaysShow
        );

        Harmony.PatchAll();

        Log.LogMessage("FreezerRole initialized.");
    }
}