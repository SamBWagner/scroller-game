ConsoleKeyInfo key = new();
CancellationTokenSource cancelSource = new();
Task watchKeys = Task.Run(() => { WatchKeys(); });
// int counter = 0;

Console.Clear();

while (true)
{
    if (key.Key == ConsoleKey.Q)
    {
        Console.WriteLine($"actions: {key.Key}");
    }
    else
    {
        Console.WriteLine("actions: ");
    }
    // Console.WriteLine(counter);
    // counter++;
    Console.Clear();
}

void WatchKeys()
{
    do 
    {
        key = Console.ReadKey(true);
    } while (key.Key != ConsoleKey.Q);
    cancelSource.Cancel();
}
