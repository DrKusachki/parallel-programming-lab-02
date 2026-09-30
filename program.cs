using System.Diagnostics;

class Program
{
    static readonly object lockObj = new object();


    static int Knapsack(List<Tuple<int, int>> items, int W)
    {
        int n = items.Count;
        int max = 1 << n;
        int best = 0;

        for (int mask = 1; mask < max; ++mask)
        {
            int weightSum = 0;
            int valueSum = 0;

            for (int i = 0; i < n; ++i)
            {
                if ((mask & (1 << i)) != 0)
                {
                    weightSum += items[i].Item1;
                    valueSum += items[i].Item2;
                }
            }
            if (weightSum <= W && valueSum > best)
            {
                best = valueSum;
            }
        }
        return best;
    }

    public static int ParallelKnapsack(List<Tuple<int, int>> items, int W)
    {
        int n = items.Count;
        int max = 1 << n;
        int best = 0;

        Parallel.For(0, max, mask =>
        {
            int weightSum = 0, valueSum = 0;

            for (int i = 0; i < n; ++i)
            {
                if ((mask & (1 << i)) != 0)
                {
                    weightSum += items[i].Item1;
                    valueSum += items[i].Item2;
                }
            }
            if (weightSum <= W && valueSum > best)
            {
                lock (lockObj)
                {
                    best = valueSum;
                }
            }
        });
        return best;
    }

    static void Main() 
    {
        // Базовый пример для проверки
        List<Tuple<int, int>> items = [Tuple.Create(2, 3), Tuple.Create(3, 4), Tuple.Create(4, 5), Tuple.Create(5, 6)];
        int capacity = 5;

        Console.WriteLine("Example: ");
        Console.WriteLine($"Weights: [{string.Join(", ", items.Select(x => x.Item1).ToList())}]");
        Console.WriteLine($"Values: [{string.Join(", ", items.Select(x => x.Item2).ToList())}]");
        Console.WriteLine($"Max weight: {capacity}");

        Stopwatch timer = new Stopwatch();
        timer.Start();
        int bestValue = Knapsack(items, capacity);
        timer.Stop();
        Console.WriteLine($"Result, ST: {bestValue}, Time: {timer.ElapsedMilliseconds} ms");
        timer.Restart();
        bestValue = ParallelKnapsack(items, capacity);
        timer.Stop();
        Console.WriteLine($"Result, MT: {bestValue}, Time: {timer.ElapsedMilliseconds} ms");

        Console.WriteLine("\n\nRandom tests:\n");

        Random rand = new Random();

        int testsCount = 10;

        int minCount = 10, maxCount = 20;
        int minW = 10, maxW = 40;
        int minWeight = 5, maxWeight = 20;
        int minValue = 5, maxValue = 20;

        for (int i = 0; i < testsCount; i++)
        {
            int n = rand.Next(minCount, maxCount);
            items = new List<Tuple<int, int>>();
            for (int j = 0; j < n; j++)
                items.Add(Tuple.Create(rand.Next(minWeight, maxWeight), rand.Next(minValue, maxValue)));
            
            capacity = rand.Next(minW, maxW);
            Console.WriteLine($"Test {i + 1}. {items.Count} items:");
            Console.WriteLine($"Weights: [{string.Join(", ", items.Select(x => x.Item1).ToList())}]");
            Console.WriteLine($"Values: [{string.Join(", ", items.Select(x => x.Item2).ToList())}]");
            Console.WriteLine($"Capacity: {capacity}");
            timer.Restart();
            bestValue = Knapsack(items, capacity);
            timer.Stop();
            Console.WriteLine($"Result, ST: {bestValue}, Time: {timer.ElapsedMilliseconds} ms");
            timer.Restart();
            bestValue = ParallelKnapsack(items, capacity);
            timer.Stop();
            Console.WriteLine($"Result, MT: {bestValue}, Time: {timer.ElapsedMilliseconds} ms\n");
        }

    }
}
