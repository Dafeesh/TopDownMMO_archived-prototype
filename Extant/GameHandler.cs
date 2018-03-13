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

            //TESTING
            games[0] = new Game_Survival("test1", new Player("Player1", "plr1", "Extant", 7));
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
                if (varifiedClient != null)
                {
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
                                        DebugLogger.GlobalDebug.LogNetworking("Sorted varified client: " + varifiedClient.VerifyUsername + ".");
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
                }

                if (!found)
                {
                    varifiedClient.SendPacket(new Ping_sp(Ping_sp.INCORRECT_ACTION));
                    varifiedClient.Stop();
                    DebugLogger.GlobalDebug.LogNetworking("Varified client not found in any game: " + varifiedClient.VerifyUsername + ".");
                }
            }
        }
    }
}
