using System;
using System.Threading;
using System.Windows.Forms;

using Extant;

using GameServer.HostGame;
using GameServer.Networking;
using GameServer.Shared;

namespace GameServer
{
    public class GameHandler : ThreadRun
    {
        private Int32 numberOfGames;
        private Game[] games;

        private Networking.HostServer hostServer;

        public GameHandler(String ip, Int32 port, Int32 numGames)
            : base("<GameHandler>")
        {
            numberOfGames = numGames;
            games = new HostGame.Game[numberOfGames];

            hostServer = new Networking.HostServer(ip, port);
            hostServer.Start();
        }

        protected override void Begin()
        {
            //TESTING
            games[0] = new Game_Survival("test1", new Player[]{ new Player("Player1", "plr1", "Extant", 1, Player.TeamColor.Blue) ,
                                                                new Player("Player2", "plr2", "Extant", 2, Player.TeamColor.Blue) ,
                                                                new Player("Player3", "plr3", "Bullstrikers", 2, Player.TeamColor.Red) ,
                                                                new Player("Player4", "plr4", "Bullstrikers", 2, Player.TeamColor.Red) ,
                                                                new Player("Player5", "plr5", "Other", 2, Player.TeamColor.Spectator) });
            games[0].Start();
        }

        protected override void RunLoop()
        {
            SortVarifiedClients();
        }

        protected override void Finish()
        {
            hostServer.Stop();

            foreach (HostGame.Game g in games)
                if (g != null)
                    g.Stop();
            games = null;
        }

        private void SortVarifiedClients()
        {
            bool found;
            Client varifiedClient;

            while ((varifiedClient = hostServer.GetVarifiedClient()) != null)
            {
                found = false;
                foreach (Game g in games)
                {
                    if (g != null)
                    {
                        foreach (Player p in g.Players)
                        {
                            if (p.Username == varifiedClient.VerifyUsername)
                            {
                                if (p.Password == varifiedClient.VerifyPassword)
                                {
                                    p.SetClient(varifiedClient);
                                    p.NewlyConnected = true;
                                    DebugLogger.GlobalDebug.LogNetworking("Sorted verified client <" + varifiedClient.VerifyUsername + "> to game [" + g.GameID + "].");
                                }
                                else
                                {
                                    varifiedClient.SendPacket(new Ping_sp(Ping_sp.WRONG_INFO));
                                    varifiedClient.Stop();
                                    DebugLogger.GlobalDebug.LogNetworking("Client did not have correct password: " + varifiedClient.VerifyUsername + ".");
                                }
                                found = true;
                            }
                        }
                    }
                }

                if (!found)
                {
                    varifiedClient.SendPacket(new Ping_sp(Ping_sp.INCORRECT_ACTION));
                    varifiedClient.Stop();
                    DebugLogger.GlobalDebug.LogCatch("Varified client not found in any game: " + varifiedClient.VerifyUsername + ".");
                }
            }
        }
    }
}
