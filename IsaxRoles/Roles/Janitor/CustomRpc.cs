using System;
using System.Linq;
using AmongUs.GameOptions;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace IsaxRoles;
public static class CustomRpc
{

    [MethodRpc((uint) CustomRpcCalls.CleanBody)]
    public static void RpcCleanBody(this PlayerControl source, Byte target)
    {
        var coroutineInstance = new Coroutine();
        Coroutines.Start(coroutineInstance.CleanBodyCoroutine(target));
    }
}