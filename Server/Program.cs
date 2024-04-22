using Server;
using System.Data.SqlClient;

internal class Program
{

    private static void Main(string[] args)
    {
        SQL.Initialize();
        Network.Initialize();
        Network.Start();

        while (true)
        {
            //主循环


        }
    }
}