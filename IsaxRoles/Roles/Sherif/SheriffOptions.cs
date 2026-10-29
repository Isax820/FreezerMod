using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using System;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace IsaxRoles;

public class SheriffOptions : AbstractOptionGroup<Sheriff>
{ 
    public override string GroupName => "Sheriff";
    public override Color GroupColor => Color.yellow;

    [ModdedNumberOption("Shoot Cooldown", 5, 60, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ShootCooldown { get; set; } = 30;
}