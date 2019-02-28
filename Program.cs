using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ping
{
    class Program
    {
        static async Task Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine($"使用方法：dotnet ping 127.0.0.1");
                return;
            }


            long totalTime = 0;
            int timeout = 120;
            int echoNum = 4;

            Ping pingSender = new Ping();
            var data = new byte[64];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)i;
            }

            var options = new PingOptions(64, true);
            for (int i = 0; i < echoNum; i++)
            {
                PingReply reply = null;
                try
                {
                    reply = await pingSender.SendPingAsync(args[0], timeout, data, options);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.Delay(1000);
                    continue;
                }

                if (reply.Status == IPStatus.Success)
                {
                    totalTime += reply.RoundtripTime;
                    if (reply.Options == null)  //mac下测试 options=null，windows下正常。
                        Console.WriteLine($"{data.Length} bytes from {reply.Address}: icmp_seq={i} time={reply.RoundtripTime} ms");
                    else
                        Console.WriteLine($"{reply.Buffer.Length} bytes from {reply.Address}: icmp_seq={i} ttl={reply.Options.Ttl} time={reply.RoundtripTime} ms");
                }

                await Task.Delay(1000);
            }

            Console.WriteLine($"平均延时：{totalTime / echoNum} ms");
        }
    }
}
