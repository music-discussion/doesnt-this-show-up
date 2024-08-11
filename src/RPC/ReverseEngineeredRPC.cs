using Hazel;
using InnerNet;
using Lotus.API.Odyssey;
using Lotus.API.Player;
using Lotus.Extensions;
using Lotus.GUI.Name.Interfaces;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using VentLib.Networking.RPC;
using VentLib.Networking.RPC.Interfaces;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using Object = UnityEngine.Object;

namespace Lotus.RPC;

public static class ReverseEngineeredRPC
{
    public static void RpcChangeSkin(PlayerControl player, NetworkedPlayerInfo.PlayerOutfit newOutift, int targetClientId = -1)
    {
        MassRpc massRpc = RpcV3.Mass();

        player.SetName(newOutift.PlayerName);
        massRpc.Start(player.NetId, RpcCalls.SetName).Write(player.Data.NetId).Write(newOutift.PlayerName).End();

        player.SetColor(newOutift.ColorId);
        massRpc.Start(player.NetId, RpcCalls.SetColor).Write(player.Data.NetId).Write((byte)newOutift.ColorId).End();

        player.SetHat(newOutift.HatId, newOutift.ColorId);
        massRpc.Start(player.NetId, RpcCalls.SetHatStr).Write(newOutift.HatId).Write(player.GetNextRpcSequenceId(RpcCalls.SetHatStr)).End();

        player.SetSkin(newOutift.SkinId, newOutift.ColorId);
        massRpc.Start(player.NetId, RpcCalls.SetSkinStr).Write(newOutift.SkinId).Write(player.GetNextRpcSequenceId(RpcCalls.SetSkinStr)).End();

        player.SetVisor(newOutift.VisorId, newOutift.ColorId);
        massRpc.Start(player.NetId, RpcCalls.SetVisorStr).Write(newOutift.VisorId).Write(player.GetNextRpcSequenceId(RpcCalls.SetVisorStr)).End();

        player.SetPet(newOutift.PetId);
        massRpc.Start(player.NetId, RpcCalls.SetPetStr).Write(newOutift.PetId).Write(player.GetNextRpcSequenceId(RpcCalls.SetPetStr)).End();

        player.SetNamePlate(newOutift.NamePlateId);
        massRpc.Start(player.NetId, RpcCalls.SetNamePlateStr).Write(newOutift.NamePlateId).Write(player.GetNextRpcSequenceId(RpcCalls.SetNamePlateStr)).End();

        massRpc.Send(targetClientId);
    }

    public static IEnumerator UnshfitButtonTrigger(PlayerControl p)
    {
        if (!p.PrimaryRole().RoleAbilityFlags.HasFlag(Roles.RoleAbilityFlag.UsesUnshiftTrigger)) yield break;
        PlayerControl target = Players.GetAlivePlayers().Where(t => t.PlayerId != p.PlayerId).ToList().GetRandom();
        p.RpcRejectShapeshift();
        var outfit = p.Data.DefaultOutfit;
        if (p.IsHost()) p.Shapeshift(target, false);
        else RpcV3.Immediate(p.NetId, (byte)RpcCalls.Shapeshift).Write(target).Write(false).Send();

        yield return new WaitForSeconds(NetUtils.DeriveDelay(0.05f));
        RpcChangeSkin(p, outfit, p.GetClientId());
        // update name for player
        yield return new WaitForSeconds(NetUtils.DeriveDelay(0.05f));
        INameModel nameModel = p.NameModel();
        nameModel.Render(force: true);
    }
}