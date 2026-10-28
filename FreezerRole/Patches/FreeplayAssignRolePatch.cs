using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Roles;

namespace FreezerRole;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class FreeplayAssignRolePatch
{
    private static PlayerControl? _lastAssigned;

    public static void Postfix(PlayerControl __instance)
    {
        if (AmongUsClient.Instance == null ||
            AmongUsClient.Instance.NetworkMode != NetworkModes.FreePlay) return;

        if (__instance != PlayerControl.LocalPlayer) return;
        if (__instance.Data == null || __instance.Data.Role == null) return;
        if (_lastAssigned == __instance) return;

        _lastAssigned = __instance;
        __instance.RpcSetRole((RoleTypes)RoleId.Get<Freezer>(), true);
    }
}