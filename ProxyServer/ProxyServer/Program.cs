using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ProxyServer
{
    public class Program
    {
        private const string EventHubConnectionString = "HostName=17m12puhub.azure-devices.net;DeviceId=myDeviceId;SharedAccessKey=v1LQGw7MoTYXkcF0lIc210/KH9O/dXy875uRSLzkqbQ=";
        private const string EventHubName = "17m12puhub.azure-devices.net";
        private const string StorageContainerName = "a17m12pukafka-2019-01-17t19-00-31-312z";
        private const string StorageAccountName = "17m12pustorage";
        private const string StorageAccountKey = "iGpIupWpZGc6xAEKrweNNdNbfeQVHojLDER4PwRqvMUHEn40RtrDypCQA1WLFMGQ42IIJbB1FVN5+dLNAKUc6g==";

        private static readonly string StorageConnectionString 
            = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
