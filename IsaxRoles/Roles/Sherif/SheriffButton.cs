using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace IsaxRoles;

public class SheriffButton : CustomActionButton<PlayerControl>
{
    private const string SheriffRoleName = "Sheriff";

    // Couleur du contour autour de la cible.
    private static readonly Color OutlineColor = Color.yellow;

    public override string Name => "Shoot";
    public override float Cooldown => OptionGroupSingleton<SheriffOptions>.Instance.ShootCooldown;
    public override float EffectDuration => 0f;
    public override int MaxUses => 0; // 0 = utilisations illimitées
    public override MiraKeybind Keybind => MiraGlobalKeybinds.PrimaryAbility;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override LoadableAsset<Sprite> Sprite => SheriffAssets.FreezeIcon;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is ICustomRole customRole && customRole.RoleName == SheriffRoleName;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        // TODO : logique du tir (tuer la cible si Impostor, sinon le Sheriff meurt, etc.)
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance);
    }

    public override void SetOutline(bool active)
    {
        Target?.cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(OutlineColor));
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return target != null && target.Data != null && !target.Data.IsDead;
    }
}