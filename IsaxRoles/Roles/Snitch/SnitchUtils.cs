using System.Collections;
using System.Collections.Generic;
using MiraAPI.Roles;
using UnityEngine;

namespace IsaxRoles;

public static class Utils
{
    public static PlayerControl PlayerById(byte id)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.PlayerId == id)
                return player;
        }

        return null;
    }

    public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
    {
        color.a = alpha;

        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            fullscreen.enabled = true;
            fullscreen.gameObject.SetActive(true);
            fullscreen.color = color;
        }

        yield return new WaitForSeconds(waitfor);

        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            if (fullscreen.color.Equals(color))
            {
                fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                fullscreen.enabled = false;
            }
        }
    }

    public static void DestroyAll(this IEnumerable<Component> listie)
    {
        foreach (var item in listie)
        {
            if (item == null) continue;

            var go = item.gameObject;
            if (go != null)
                Object.Destroy(go);
            else
                Object.Destroy(item);
        }
    }

    private static readonly Dictionary<byte, List<RoleBehaviour>> playerRolesHistory = new();

    public static void SavePlayerRole(byte playerId, RoleBehaviour role)
    {
        if (!playerRolesHistory.TryGetValue(playerId, out var history))
        {
            history = new List<RoleBehaviour>();
            playerRolesHistory[playerId] = history;
        }

        history.Add(role);
    }

    public static List<RoleBehaviour> GetPlayerRolesHistory(byte playerId)
    {
        return playerRolesHistory.TryGetValue(playerId, out var history)
            ? history
            : new List<RoleBehaviour>();
    }

    public static RoleBehaviour GetPlayerLastRole(byte playerId)
    {
        if (playerRolesHistory.TryGetValue(playerId, out var history) && history.Count > 0)
            return history[history.Count - 1];

        return null;
    }

    public static void ClearPlayerRolesHistory() => playerRolesHistory.Clear();

    public static SpriteRenderer myRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;
}

public static class RoleExtensions
{
    private static readonly Dictionary<ICustomRole, bool> roleCanDoTasks = new();

    public static void SetCanDoTasks(this ICustomRole role, bool value)
    {
        roleCanDoTasks[role] = value;
    }

    public static bool GetCanDoTasks(this ICustomRole role)
    {
        return roleCanDoTasks.TryGetValue(role, out var value) ? value : true;
    }
}