using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoClient
{
    static class Settings
    {
        public static string storageKey { get; set; }

        public static string storageAccount { get; set; }

        public static string batchKey { get; set; }

        public static string batchAccount { get; set; }

        public static string batchEndpoint { get; set; }

        public static string inputContainer { get; set; }
        
        public static string outputContainer { get; set; }
        
        public static string resourceContainer { get; set; }

        public static string commands { get; set; }

        public const string poolname = "demo";

        public static batchaccount[] accounts = new[] {
            new batchaccount("yidingadf", "t8RiYvO4yjxHtJBSCGYKzW6MzHBiUgEU+Qaxr2wLS9Hud4Hk5pbF/3RBjSiNTFubjSEDURaNdx/abDenrS80kw==", "https://yidingadf.westeurope.batch.azure.com" ),
            new batchaccount("resourcemgmt", "", "https://batch-test.windows-int.net/" )
        };

        public static void LoadSettings()
        {
            storageKey = "GZZ9g/NjGUK2DP1DaG4DlafQgfYI6tvWFJLGvrTTqdi/WK1MqDYPKrxgt8MJWIiQ5rsEuFS7bIDWf75g5scW4A==";
            storageAccount = "mvpdemo15";

            inputContainer = "https://mvpdemo15.blob.core.windows.net/input";
            outputContainer = "https://mvpdemo15.blob.core.windows.net/output";
            resourceContainer = "https://mvp15demo.blob.core.windows.net/resource";
        }
    }

    struct batchaccount
    {
        public string key;
        public string account;
        public string url;

        public batchaccount(string account, string key, string url) { this.key = key; this.account = account; this.url = url; }
    }
}
