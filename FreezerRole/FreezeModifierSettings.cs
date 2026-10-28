using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;

namespace FreezerRole;

public class FreezeModifierSettings : AbstractOptionGroup<Freezer>
{
    public override string GroupName => "options.freezeModifierSettings";

    [ModdedToggleOption("options.freezeModifierSettings.useColor")]
    public bool UseColor { get; set; } = true;
}