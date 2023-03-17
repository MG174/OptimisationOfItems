using System.Collections.Concurrent;
using System.Diagnostics;

public class Item
{
    public string name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}
public class PackOfItems
{
    public Item A { get; set; }
    public Item B { get; set; }
    public Item C { get; set; }
    public Item D { get; set; }
    public Item E { get; set; }
    public int maxValue { get; set; }
}

class Program
{
    public static void Main(string[] args)
    {
        List<Item> A = new List<Item>();
        List<Item> B = new List<Item>();
        List<Item> C = new List<Item>();
        List<Item> D = new List<Item>();
        List<Item> E = new List<Item>();

        //Real data 
        //A.Add(new Item { name = "a1", X = 5000, Y = 1000 });
        //A.Add(new Item { name = "a2", X = 2000, Y = 1000 });
        //B.Add(new Item { name = "b1", X = 1400, Y = 1000 });
        //B.Add(new Item { name = "b2", X = 2000, Y = 900 });
        //C.Add(new Item { name = "c1", X = 1300, Y = 1000 });
        //C.Add(new Item { name = "c2", X = 2000, Y = 900 });
        //D.Add(new Item { name = "d1", X = 6000, Y = 1000 });
        //D.Add(new Item { name = "d2", X = 2000, Y = 1500 });
        //E.Add(new Item { name = "e1", X = 1100, Y = 1000 });
        //E.Add(new Item { name = "e2", X = 2000, Y = 2000 });

        //Random mock data
        for (int i = 0; i < 1000000; i++)
        {
            A.Add(new Item { name = $"{RandomGenerator.RandomString(10)}", X = RandomGenerator.RandomInt(500, 5000), Y = RandomGenerator.RandomInt(500, 5000) });
            B.Add(new Item { name = $"{RandomGenerator.RandomString(10)}", X = RandomGenerator.RandomInt(500, 5000), Y = RandomGenerator.RandomInt(500, 5000) });
            C.Add(new Item { name = $"{RandomGenerator.RandomString(10)}", X = RandomGenerator.RandomInt(500, 5000), Y = RandomGenerator.RandomInt(500, 5000) });
            D.Add(new Item { name = $"{RandomGenerator.RandomString(10)}", X = RandomGenerator.RandomInt(500, 5000), Y = RandomGenerator.RandomInt(500, 5000) });
            E.Add(new Item { name = $"{RandomGenerator.RandomString(10)}", X = RandomGenerator.RandomInt(500, 5000), Y = RandomGenerator.RandomInt(500, 5000) });
        }

        A = A.OrderByDescending(x => x.X / x.Y).ToList();
        B = B.OrderByDescending(x => x.X / x.Y).ToList();
        C = C.OrderByDescending(x => x.X / x.Y).ToList();
        D = D.OrderByDescending(x => x.X / x.Y).ToList();
        E = E.OrderByDescending(x => x.X / x.Y).ToList();

        var minY = 4500;
        var maxY = 5500;

        var maxX = 0;
        var interation = 0;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var query = from itemA in A.AsParallel()
                    from itemB in B.AsParallel()
                    from itemC in C.AsParallel()
                    from itemD in D.AsParallel()
                    from itemE in E.AsParallel()
                    let totalX = itemA.X + itemB.X + itemC.X + itemD.X + itemE.X
                    let totalY = itemA.Y + itemB.Y + itemC.Y + itemD.Y + itemE.Y
                    where minY <= totalY && totalY <= maxY && totalX >= maxX
                    select new PackOfItems { A = itemA, B = itemB, C = itemC, D = itemD, E = itemE, maxValue = totalX };

        var solutions = new ConcurrentBag<PackOfItems>();

        try { 
        query.ForAll(result =>
        {
            if (stopwatch.Elapsed.TotalSeconds > 3)
                throw new Exception();

            Interlocked.Exchange(ref maxX, result.maxValue);

            if (result.maxValue == maxX)
            {
                solutions.Add(result);
                Console.WriteLine("Valid solution: Y: " + (result.A.Y + result.B.Y + result.C.Y + result.D.Y + result.E.Y) + ", X: " + result.maxValue);
            }
            else if (result.maxValue > maxX)
            {
                maxX = result.maxValue;
                solutions.Clear();
                solutions.Add(result);
                Console.WriteLine("Valid solution: Y: " + (result.A.Y + result.B.Y + result.C.Y + result.D.Y + result.E.Y) + ", X: " + result.maxValue);
            }
        });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Query cancelled.");
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine("Elapsed time: " + stopwatch.Elapsed.ToString("g"));
        }


        var best = solutions.MaxBy(x => x.maxValue);
        Console.WriteLine("Ended with solution: " + (best.A.Y + best.B.Y + best.C.Y + best.D.Y + best.E.Y) + ", X: " + best.maxValue);
    }
}

public static class RandomGenerator
{
    public static string RandomString(int length)
    {
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public static int RandomInt(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max);
    }
}