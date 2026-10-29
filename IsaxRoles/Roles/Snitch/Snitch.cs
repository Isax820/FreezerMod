using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using UnityEngine;


namespace IsaxRoles;

public class Snitch : CrewmateRole, ICustomRole
{
    public string RoleName => "Snitch";
    public string RoleDescription => "Complete your tasks to reveal the Impostor(s).";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => Color.green;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        CanUseVent = false,
    };

    public bool impostorsRevealed = false;
    public bool revealed = false;
    public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();
    public Dictionary<byte, ArrowBehaviour> SnitchArrows = new Dictionary<byte, ArrowBehaviour>();
    private static readonly string[] VoteAreaIdNames = { "TargetPlayerId", "targetPlayerId", "PlayerId", "playerId" };
    private static PropertyInfo voteAreaIdProperty;
    private static FieldInfo voteAreaIdField;
    private static bool voteAreaIdResolved;

    private static void ResolveVoteAreaIdMember()
    {
        if (voteAreaIdResolved) return;
        voteAreaIdResolved = true;

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        foreach (var name in VoteAreaIdNames)
        {
            voteAreaIdProperty = typeof(PlayerVoteArea).GetProperty(name, flags);
            if (voteAreaIdProperty != null) return;

            voteAreaIdField = typeof(PlayerVoteArea).GetField(name, flags);
            if (voteAreaIdField != null) return;
        }
    }

    private static PlayerControl GetPlayerFromVoteArea(PlayerVoteArea voteArea)
    {
        ResolveVoteAreaIdMember();

        object rawId = null;
        if (voteAreaIdProperty != null)
            rawId = voteAreaIdProperty.GetValue(voteArea);
        else if (voteAreaIdField != null)
            rawId = voteAreaIdField.GetValue(voteArea);

        if (rawId != null)
        {
            // unchecked : un sbyte négatif devient simplement une valeur byte
            var id = unchecked((byte)System.Convert.ToInt32(rawId));
            var byId = Utils.PlayerById(id);
            if (byId != null) return byId;
        }

        // Dernier recours : comparer le nom affiché au nom des joueurs
        if (voteArea.NameText == null) return null;
        var displayed = voteArea.NameText.text;

        return PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(p => p.Data != null && p.Data.PlayerName == displayed);
    }

    public void HudUpdate(HudManager hudManager)
    {
        if (MeetingHud.Instance != null && impostorsRevealed)
        {
            UpdateMeeting(MeetingHud.Instance);
        }
    }

    private void UpdateMeeting(MeetingHud __instance)
    {
        foreach (var voteArea in __instance.playerStates)
        {
            var target = GetPlayerFromVoteArea(voteArea);

            if (target != null && target.Data != null && target.Data.Role.IsImpostor)
            {
                voteArea.NameText.color = Palette.ImpostorRed;
            }
        }
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        if (SnitchArrows.TryGetValue(targetPlayerId, out var arrow))
        {
            if (arrow != null)
            {
                // Détruire le gameObject détruit aussi le composant
                Object.Destroy(arrow.gameObject);
            }

            SnitchArrows.Remove(targetPlayerId);
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        SnitchArrows.Values.DestroyAll();
        SnitchArrows.Clear();
        ImpArrows.DestroyAll();
        ImpArrows.Clear();
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
public class CompleteTask
{
    public static LoadableAsset<Sprite> ArrowSprite => SnitchAssets.Arrow;

    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.Data.Role is Snitch snitch)
        {
            if (__instance.Data.IsDead) return;

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var localRole = PlayerControl.LocalPlayer.Data.Role;

            if (tasksLeft == 1)
            {
                if (localRole is Snitch)
                {
                    Coroutines.Start(Utils.FlashCoroutine(snitch.RoleColor));
                }
                else if (localRole.IsImpostor)
                {
                    Coroutines.Start(Utils.FlashCoroutine(snitch.RoleColor));

                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = ArrowSprite.LoadAsset();
                    arrow.image = renderer;
                    gameObj.layer = 5;
                    snitch.ImpArrows.Add(arrow);
                }

                snitch.revealed = true;
            }
            else if (tasksLeft == 0)
            {
                if (localRole is Snitch)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                    snitch.impostorsRevealed = true;

                    var impostors = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.Role.IsImpostor);
                    foreach (var imp in impostors)
                    {
                        // Évite une ArgumentException si la flèche existe déjà
                        if (snitch.SnitchArrows.ContainsKey(imp.PlayerId)) continue;

                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = ArrowSprite.LoadAsset();
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitch.SnitchArrows.Add(imp.PlayerId, arrow);
                    }
                }
                else if (localRole.IsImpostor)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
            }
        }
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public class UpdateArrows
{
    public static void Postfix(HudManager __instance)
    {
        if (PlayerControl.AllPlayerControls.Count <= 1) return;
        if (PlayerControl.LocalPlayer == null) return;
        if (PlayerControl.LocalPlayer.Data == null) return;

        foreach (var pc in PlayerControl.AllPlayerControls)
        {
            if (pc.Data == null) continue;

            if (pc.Data.Role is Snitch snitch)
            {
                if (PlayerControl.LocalPlayer.Data.IsDead || pc.Data.IsDead)
                {
                    snitch.SnitchArrows.Values.DestroyAll();
                    snitch.SnitchArrows.Clear();
                    snitch.ImpArrows.DestroyAll();
                    snitch.ImpArrows.Clear();
                }

                foreach (var arrow in snitch.ImpArrows)
                {
                    if (arrow != null)
                        arrow.target = pc.transform.position;
                }

                // ToList() : on itère sur une copie car DestroyArrow modifie le dictionnaire
                foreach (var arrow in snitch.SnitchArrows.ToList())
                {
                    var target = Utils.PlayerById(arrow.Key);
                    if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected)
                    {
                        snitch.DestroyArrow(arrow.Key);
                        continue;
                    }

                    if (arrow.Value != null)
                        arrow.Value.target = target.transform.position;
                }
            }
        }
    }
}