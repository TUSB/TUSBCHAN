using Discord;
using Discord.WebSocket;

namespace TUSBCHAN.Functions
{
    class Decision
    {
        private const long rootid = 290039249149886464;
        private const long creatorid = 290015220519403520;
        private const long englishid = 365469339278639105;

        public bool IsRoot(SocketMessage context)
        {
            bool root = false;
            var userid = context.Author.Id;
            var user = context.Channel.GetUserAsync(userid).Result as IGuildUser;
            foreach (var role in user.RoleIds)
            {
                if (role == rootid)
                {
                    root = true;
                    break;
                }

            }
            return root;
        }

        public bool IsCreator(SocketMessage context)
        {
            bool creator = false;
            var userid = context.Author.Id;
            var user = context.Channel.GetUserAsync(userid).Result as IGuildUser;
            foreach (var role in user.RoleIds)
            {
                if (role == creatorid)
                {
                    creator = true;
                    break;
                }

            }
            return creator;
        }

        public bool IsEnglish (SocketMessage context)
        {
            bool english = false;
            var userid = context.Author.Id;
            var user = context.Channel.GetUserAsync(userid).Result as IGuildUser;
            foreach (var role in user.RoleIds)
            {
                if (role == englishid)
                {
                    english = true;
                    break;
                }

            }
            return english;
        }
    }
}
