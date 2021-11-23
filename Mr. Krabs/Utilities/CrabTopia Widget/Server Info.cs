using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Krabs.Utilities.CrabTopia_Widget {
    public class ServerInfo {


        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Channel {
            public string id { get; set; }
            public string name { get; set; }
            public int position { get; set; }
        }

        public class Game {
            public string name { get; set; }
        }

        public class Member {
            public string id { get; set; }
            public string username { get; set; }
            public string discriminator { get; set; }
            public object avatar { get; set; }
            public string status { get; set; }
            public string avatar_url { get; set; }
            public Game game { get; set; }
        }

        public class Root {
            public string id { get; set; }
            public string name { get; set; }
            public string instant_invite { get; set; }
            public List<Channel> channels { get; set; }
            public List<Member> members { get; set; }
            public int presence_count { get; set; }
        }


    }
}
