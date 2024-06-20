﻿using System.Collections.Generic;
using Lotus.Roles;
using Lotus.Roles.Operations;
using Lotus.Victory;
using VentLib.Options.Game.Tabs;

namespace Lotus.GameModes.Standard;

public class StandardGameMode : GameMode
{
    public static StandardGameMode Instance;

    public override string Name { get; set; } = "Standard";
    public new StandardRoleOperations RoleOperations { get; }
    public new StandardRoleManager RoleManager { get; }

    public StandardGameMode()
    {
        Instance = this;

        RoleOperations = new(this);
        RoleManager = new();
    }

    public override void Assign(PlayerControl player, CustomRole role)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerable<GameOptionTab> EnabledTabs()
    {
        throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        throw new System.NotImplementedException();
    }

    public override void SetupWinConditions(WinDelegate winDelegate)
    {
        throw new System.NotImplementedException();
    }

    public override void AssignRoles(List<PlayerControl> players)
    {
        throw new System.NotImplementedException();
    }
}