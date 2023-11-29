using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace SlayLosers;

public class SlayLosers : BasePlugin
{
    public override string ModuleName => "SlayLosers";
    public override string ModuleVersion => "0.0.1";
    public override string ModuleAuthor => "NiGHT";

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventRoundEnd>((@event, info) =>
        {
            if(@event.Winner < 2)
                return HookResult.Continue;

            var winner = @event.Winner;
            AddTimer(0.1f, () =>
            {
                var players = Utilities.GetPlayers().Where(x => x.TeamNum == (winner == 2 ? 3 : 2) && x.PawnIsAlive);
            
                if(!players.Any())
                    return;
            
                foreach (var player in players)
                    player.CommitSuicide(false, true);
                
                Server.PrintToChatAll($"[{ChatColors.Red}SlayLosers{ChatColors.Default}] Slayed {ChatColors.Red}{players.Count()} {(players.Count() == 1 ? "player" : "players")}{ChatColors.Default}.");
                return;
            });
            
            return HookResult.Continue;
        });
    }
}