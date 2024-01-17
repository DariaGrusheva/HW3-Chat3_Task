namespace Chat3_Task
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await Server.AcceptMsg();
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    await Client.SendMsg(args[0]);
                }
            }
        }
    }
}