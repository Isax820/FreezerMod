using System;
using System.Diagnostics;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace IsaxRoles;

[RegisterCustomRole]
public class Freezer : ImpostorRole, ICustomRole
{
    public string RoleName => "Freezer";
    public string RoleDescription => "Freeze players";
    public string RoleLongDescription => "Gèle un joueur pendant quelques secondes.";
    public Color RoleColor => Color.blue;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = MiraAssets.ImpostorFile,
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(MiraAssets.ImpostorFile.LoadAsset(), "ApiExample.Role.Impostor.Freezer"),
        MaxRoleCount = 2,
    };

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}

internal class RegisterCustomRoleAttribute : Attribute
{
}