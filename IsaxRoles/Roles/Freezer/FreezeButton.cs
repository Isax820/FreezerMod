using System;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace FreezerRole;

[RegisterCustomButton]
public class FreezeButton : CustomActionButton<PlayerControl>
{
    public override string Name => "button.freeze";

    public override float Cooldown => OptionGroupSingleton<FreezerRoleSettings>.Instance.FreezeDuration;
    public override bool PauseTimerInVent => true;

    public static class FreezerAssets
    {
    public static LoadableAsset<Sprite> FreezeButton { get; } =
        new LoadableResourceAsset("FreezerRole.Resources.FreezeIcon.png");
    }

    public override LoadableAsset<Sprite> Sprite => FreezerAssets.FreezeButton;
    public override MiraKeybind? Keybind => MiraGlobalKeybinds.PrimaryAbility;

    protected override void OnClick()
    {
        Target?.RpcAddModifier<FreezeModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(Palette.Blue));
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return true;
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is FreezerRole;
    }
}

internal class RegisterCustomButtonAttribute : Attribute
{
}