using Server;
using System.Data.SqlClient;

internal class Program
{

    private static void Main(string[] args)
    {
        SQL.Initialize();
        Network.Initialize();
        Network.Start();
        Security.Initialize();
        Security.Run();
        Service.Initialize();

        while (true)
        {
            //主循环
            Thread.Sleep(1000);
        }
    }
}