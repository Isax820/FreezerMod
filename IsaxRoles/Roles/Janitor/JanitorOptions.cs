using MiraAPI.GameOptions;
using System;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace IsaxRoles;

public class JanitorOptions : AbstractOptionGroup<Janitor>
{ 
    public override string GroupName => "Janitor";
    public override Color GroupColor => Palette.ImpostorRed;

    public ModdedToggleOption JanitorCanVent { get; } = new("Can Vent", true);
}