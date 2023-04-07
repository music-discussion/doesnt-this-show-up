using System.Collections.Generic;
using System.Linq;
using Hazel;
using TOHTOR.Utilities;
using UnityEngine;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace TOHTOR.API;

public class VentApi
{
    public static void ForceNoVenting(PlayerControl player, int ventId = -1)
    {
        Vector2 originalPos = player.GetTruePosition();
        if (ventId == -1)
        {
            List<Vent> vents = Object.FindObjectsOfType<Vent>().ToList();
            if (vents.Count == 0) return;
            ventId = vents.Sorted(v => Vector2.Distance(originalPos, v.transform.position)).First().Id;
        }

        MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(player.MyPhysics.NetId, (byte)RpcCalls.EnterVent, SendOption.None);
        messageWriter.WritePacked(ventId);
        messageWriter.EndMessage();

        Async.Schedule(() => player.MyPhysics.RpcBootFromVent(ventId), 0.01f);
        Async.Schedule(() => Utils.Teleport(player.NetTransform, originalPos), 1f);
    }
}