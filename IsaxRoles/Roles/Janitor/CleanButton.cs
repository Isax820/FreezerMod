using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace IsaxRoles;

public class CleanButton : CustomActionButton<DeadBody>
{
    private const string JanitorRoleName = "Janitor";

    public override string Name => "";
    public override float Cooldown => 30f;
    public override float EffectDuration => 0f;
    public override int MaxUses => 0;
    public override MiraKeybind Keybind => MiraGlobalKeybinds.PrimaryAbility;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override LoadableAsset<Sprite> Sprite => JanitorAssets.CleanButton;

    public override void FixedUpdateHandler(PlayerControl playerControl)
    {
        if (Timer >= 0)
        {
            // Le cooldown du bouton suit le cooldown de kill de l'Impostor
            Timer = PlayerControl.LocalPlayer.killTimer;
        }
        else if (HasEffect && EffectActive)
        {
            EffectActive = false;
            Timer = Cooldown;
            OnEffectEnd();
        }

        if (CanUse())
        {
            Button?.SetEnabled();
        }
        else
        {
            Button?.SetDisabled();
        }

        Button?.SetCoolDown(Timer, EffectActive ? EffectDuration : Cooldown);

        FixedUpdate(playerControl);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
        PlayerControl.LocalPlayer.RpcCleanBody(Target.ParentId);
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestDeadBody(1f);
    }

    public override void SetOutline(bool active)
    {
        if (Target == null || Target.bodyRenderers == null)
        {
            return;
        }

        // Un cadavre peut avoir plusieurs renderers : on applique le contour à tous
        foreach (var renderer in Target.bodyRenderers)
        {
            if (renderer == null) continue;

            renderer.material.SetFloat("_Outline", active ? 1f : 0f);
            renderer.material.SetColor("_OutlineColor", Palette.ImpostorRed);
        }
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return target != null;
    }

    // Affiche le bouton uniquement pour le rôle Janitor.
    // Comparaison par nom : elle compile quel que soit le namespace de la classe Janitor.
    public override bool Enabled(RoleBehaviour? role)
    {
        return role is ICustomRole customRole && customRole.RoleName == JanitorRoleName;
    }
}