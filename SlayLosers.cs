using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace SlayLosers;

public class SlayLosersConfig : BasePluginConfig
{
    public bool FixPlayerScore { get; set; } = true;
}

public class SlayLosers : BasePlugin, IPluginConfig<SlayLosersConfig>
{
    public override string ModuleName => "SlayLosers";
    public override string ModuleVersion => "0.0.2";
    public override string ModuleAuthor => "NiGHT";
    
    public required SlayLosersConfig Config { get; set; }
    
    public void OnConfigParsed(SlayLosersConfig config)
    {
        Config = config;
    }
    
    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventRoundEnd>((@event, info) =>
        {
            if(@event.Winner < 2)
                return HookResult.Continue;

            var winner = @event.Winner;
            AddTimer(0.1f, () =>
            {
                var players = Utilities.GetPlayers().Where(x => x.TeamNum == (winner == 2 ? 3 : 2) && x.PawnIsAlive).ToList();
            
                if(players.Count == 0)
                    return;
            
                foreach (var player in players)
                {
                    player.CommitSuicide(false, true);

                    if (!Config.FixPlayerScore)
                        continue;
                    
                    player.ActionTrackingServices!.MatchStats.Deaths -= 1;
                    player.ActionTrackingServices.MatchStats.Kills += 1;
                }
                
                Server.PrintToChatAll(Localizer["SlayMessage"].Value.Replace("{count}", players.Count.ToString()));
            });
            
            return HookResult.Continue;
        });
    }
}