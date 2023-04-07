
using TOHTOR.API;
using TOHTOR.GUI;
using TOHTOR.GUI.Name;
using TOHTOR.Roles.Internals.Attributes;
using TOHTOR.Roles.RoleGroups.Vanilla;
using UnityEngine;
using VentLib.Options.Game;

namespace TOHTOR.Roles.RoleGroups.Impostors;

public class PickPocket : Impostor
{
    private int maximumVotes;
    private int currentVotes;

    [UIComponent(UI.Counter, GameStates = new [] { GameState.Roaming, GameState.InMeeting})]
    private string VoteCounter() => RoleUtils.Counter(currentVotes, color: new Color(1f, 0.45f, 0.25f));

    [RoleAction(RoleActionType.Attack)]
    public override bool TryKill(PlayerControl target)
    {
        bool killed = base.TryKill(target);
        if (!killed) return false;
        currentVotes++;
        return true;
    }

    protected override GameOptionBuilder RegisterOptions(GameOptionBuilder optionStream) =>
        base.RegisterOptions(optionStream)
            .SubOption(sub => sub.Name("Kill Cooldown")
                .AddFloatRange(2, 120, 2.5f, 16, "s")
                .BindFloat(f => KillCooldown = f)
                .Build())
            .SubOption(sub => sub.Name("Maximum Additional Votes")
                .AddIntRange(1, 14, 1, 4)
                .BindInt(i => maximumVotes = i)
                .Build());
}