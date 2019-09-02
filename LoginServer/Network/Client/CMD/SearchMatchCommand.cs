using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine.Enums;
using LoginServer.Engine.Managers;
using LoginServer.Network.Data;
using LoginServer.Utils;

namespace LoginServer.Network.CMD
{
    public class SearchMatchCommand : IJHSInterface
    {
        protected AccountManager DbManager;
        protected PlayerQueueManager matchQueue;

        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null)
                DbManager = AccountManager.Instance;

            if (matchQueue == null)
                matchQueue = PlayerQueueManager.Instance;

            SearchMatch packet = netMsg.ReadMessage<SearchMatch>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ user = DbManager.GetOnlineByConnectionId(connectionId);
                if (user != null)
                {

                    switch (packet.op)
                    {
                        case SearchMatchOperations.Search:
                            if (user.InQueue)
                            {
                                netMsg.conn.Send(NetworkConstants.START_SEARCH_MATCH, new SearchMatch() { op = SearchMatchOperations.NO_ERROR_SEARCHING });
                                return true;
                            }
                            CharacterOBJ player = user.GetPlayer(packet.value);
                            if (player != null)
                            {
                                lock (user)
                                {
                                    user.SelectedCharacer = player.PlayerId;
                                    user.InQueue = true;
                                    user.ResetNotification();
                                    matchQueue.AddPlayer(new MatchPlayer() { League = user.League, userId = user.Id, ConnectionId = connectionId, PlayerId = player.PlayerId });
                                    netMsg.conn.Send(NetworkConstants.START_SEARCH_MATCH, new SearchMatch() { op = SearchMatchOperations.SEARCHING_INIT, value = (uint)matchQueue.AvregeWaitTime() });
                                }
                            }
                            else
                            {
                                netMsg.conn.Send(NetworkConstants.START_SEARCH_MATCH, new SearchMatch() { op = SearchMatchOperations.ERR_CHARATER_NOT_THERE });
                            }
                            break;
                        case SearchMatchOperations.Cancel:
                            lock (user)
                            {
                                user.SelectedCharacer = 0;
                                user.InQueue = false;
                                netMsg.conn.Send(NetworkConstants.START_SEARCH_MATCH, new SearchMatch() { op = SearchMatchOperations.Cancel });
                            }
                            break;
                        case SearchMatchOperations.CHECK_START:
                            matchQueue.CheckStart();
                            break;
                    }
                }
                else
                {
                    netMsg.conn.Send(NetworkConstants.START_SEARCH_MATCH, new SearchMatch() { op = SearchMatchOperations.ERR_ACCOUNT_NOT_FOUND });
                }
            }

            return true;
        }
    }
}
