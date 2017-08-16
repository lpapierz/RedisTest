using System;
using StackExchange.Redis;

namespace RedisTest
{
    internal class Program
    {
        private static readonly Lazy<ConnectionMultiplexer> lazyConnection =
            new Lazy<ConnectionMultiplexer>(
                () =>
                {
                    return
                        ConnectionMultiplexer.Connect(
                            "lp2.redis.cache.windows.net:6380,password=mho5qAO27NIkKnWY2KVRSW7nasNFehfP4fZBM6DZuKc=,ssl=True,abortConnect=False,allowAdmin=true");
                });

        public static ConnectionMultiplexer Connection
        {
            get { return lazyConnection.Value; }
        }

        private static void Main(string[] args)
        {
            var entryCount = 1;

            var cache = Connection.GetDatabase();
            var input = new ConsoleKeyInfo();
            while (input.Key != ConsoleKey.Escape)
            {
                input = Console.ReadKey();

                switch (input.Key)
                {
                    case ConsoleKey.Enter:
                        var key = $"key{entryCount}";
                        var value = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        entryCount++;
                        Console.WriteLine($"Pushing new data to Redis {{key: {key}, value: {value}}}");
                        cache.StringSet(key, value);
                        Console.WriteLine($"Saved value for key {key}:");
                        string valueSaved = cache.StringGet(key);
                        Console.WriteLine(valueSaved);
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine("Clear all databases...");
                        foreach (var endPoint in Connection.GetEndPoints())
                        {
                            var server = Connection.GetServer(endPoint);
                            if (!server.IsSlave)
                            {
                                server.FlushAllDatabases();
                            }
                        }

                        break;
                }
            }
        }
    }
}