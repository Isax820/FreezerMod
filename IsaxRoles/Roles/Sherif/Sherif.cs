using MiraAPI.Roles;
using UnityEngine;

namespace IsaxRoles;

public class Sheriff : CrewmateRole, ICustomRole
{
    public string RoleName => "Sheriff";
    public string RoleDescription => "Shoot the Impostor";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => Color.yellow;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        CanUseVent = false,
        OptionsScreenshot = SheriffAssets.SheriffBanner,
    };
}