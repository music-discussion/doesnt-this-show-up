using System.Collections.Generic;
using System.Linq;
using TOHTOR.API;
using TOHTOR.Extensions;
using TOHTOR.Factions;
using TOHTOR.Factions.Crew;
using TOHTOR.Factions.Interfaces;
using TOHTOR.FactionsOLD;
using TOHTOR.Roles;

namespace TOHTOR.Victory.Conditions;

public class VanillaCrewmateWin: IFactionWinCondition
{
    private static readonly List<IFaction> CrewmateFaction = new() { FactionInstances.Crewmates };
    private WinReason winReason = WinReason.TasksComplete;

    public bool IsConditionMet(out List<IFaction> factions)
    {
        factions = CrewmateFaction;
        winReason = WinReason.TasksComplete;

        // Any player that is really an impostor but is also not allied to the crewmates
        if (Game.GetAlivePlayers().Any(p => { CustomRole role = p.GetCustomRole(); return role.Faction is not Crewmates && role.RealRole.IsImpostor(); }))
            return GameData.Instance.TotalTasks == GameData.Instance.CompletedTasks;

        winReason = WinReason.FactionLastStanding;
        return true;
    }

    public WinReason GetWinReason() => winReason;

    public int Priority() => -1;
}