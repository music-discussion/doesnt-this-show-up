using System.Collections.Generic;
using System.Linq;
using Lotus.API.Odyssey;
using Lotus.API.Player;
using Lotus.Extensions;
using Lotus.Roles.Events;
using Lotus.Roles.Interactions;
using Lotus.Roles.Interfaces;
using Lotus.Roles.Internals;
using Lotus.Roles.Internals.Enums;
using Lotus.Roles.Internals.Attributes;
using Lotus.Roles.Overrides;
using Lotus.Roles.RoleGroups.Vanilla;
using Lotus.Options;
using VentLib.Localization.Attributes;
using VentLib.Options.Game;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace Lotus.Roles.RoleGroups.Impostors;

public class Vampire : Impostor, IVariableRole
{
    private static Vampiress _vampiress = new();

    private float killDelay;
    [NewOnSetup] private HashSet<byte> bitten = null!;

    [RoleAction(LotusActionType.Attack)]
    public override bool TryKill(PlayerControl target)
    {
        MyPlayer.RpcMark(target);
        InteractionResult result = MyPlayer.InteractWith(target, LotusInteraction.HostileInteraction.Create(this));
        if (result is InteractionResult.Halt) return false;

        bitten.Add(target.PlayerId);
        Game.MatchData.GameHistory.AddEvent(new BittenEvent(MyPlayer, target));

        Async.Schedule(() =>
        {
            MyPlayer.InteractWith(target, CreateInteraction(target));
            bitten.Remove(target.PlayerId);
        }, killDelay);

        return false;
    }

    [RoleAction(LotusActionType.RoundStart)]
    public void ResetBitten() => bitten.Clear();

    [RoleAction(LotusActionType.MeetingCalled)]
    public void KillBitten() => bitten.Filter(Players.PlayerById).Where(p => p.IsAlive()).ForEach(p => MyPlayer.InteractWith(p, CreateInteraction(p)));

    private DelayedInteraction CreateInteraction(PlayerControl target)
    {
        FatalIntent intent = new(true, () => new BittenDeathEvent(target, MyPlayer));
        return new DelayedInteraction(intent, killDelay, this);
    }

    public CustomRole Variation() => _vampiress;

    public bool AssignVariation() => RoleUtils.RandomSpawn(_vampiress);

    protected override GameOptionBuilder RegisterOptions(GameOptionBuilder optionStream) =>
        base.RegisterOptions(optionStream)
            .SubOption(sub => sub
                .Name("Kill Delay")
                .Bind(v => killDelay = (float)v)
                .AddFloatRange(2.5f, 60f, 2.5f, 2, GeneralOptionTranslations.SecondsSuffix)
                .Build());

    protected override RoleModifier Modify(RoleModifier roleModifier) =>
        base.Modify(roleModifier)
            .OptionOverride(new IndirectKillCooldown(KillCooldown))
            .LinkedRoles(_vampiress);

    [Localized(nameof(Vampire))]
    public static class VampireTranslations
    {
        [Localized(nameof(Options))]
        public static class Options
        {
            [Localized(nameof(KillDelay))]
            public static string KillDelay = "Kill Delay";
        }
    }
}