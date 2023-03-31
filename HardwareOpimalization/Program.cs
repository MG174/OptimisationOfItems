using CsvHelper;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
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
        //var path = @"C:\Users\Maciej Goryczko\source\repos\HardwareOpimalization\HardwareOpimalization\files";

        Console.WriteLine("Path:");
        string path = Console.ReadLine();
        Console.WriteLine("Amount:");
        string amount = Console.ReadLine();
        Console.WriteLine("Offset:");
        string offset = Console.ReadLine();



        //dev
        path = @"C:\Users\Maciej Goryczko\source\repos\HardwareOpimalization\HardwareOpimalization\files";
        amount = "5000";
        offset = "10";

        List<Item> A = new List<Item>();
        List<Item> B = new List<Item>();
        List<Item> C = new List<Item>();
        List<Item> D = new List<Item>();
        List<Item> E = new List<Item>();

        var readerA = new StreamReader(path + @"\inputA.csv");
        var readerB = new StreamReader(path + @"\inputB.csv");
        var readerC = new StreamReader(path + @"\inputC.csv");
        var readerD = new StreamReader(path + @"\inputD.csv");
        var readerE = new StreamReader(path + @"\inputE.csv");
        var csvA = new CsvReader(readerA, CultureInfo.InvariantCulture);
        var csvB = new CsvReader(readerA, CultureInfo.InvariantCulture);
        var csvC = new CsvReader(readerA, CultureInfo.InvariantCulture);
        var csvD = new CsvReader(readerA, CultureInfo.InvariantCulture);
        var csvE = new CsvReader(readerA, CultureInfo.InvariantCulture);
        A = csvA.GetRecords<Item>().ToList();
        B = csvB.GetRecords<Item>().ToList();
        C = csvC.GetRecords<Item>().ToList();
        D = csvD.GetRecords<Item>().ToList();
        E = csvE.GetRecords<Item>().ToList();

        //Random mock data
        for (int i = 0; i < 40; i++)
        {
            A.Add(new Item { name = $"{Utility.RandomString(10)}", X = Utility.RandomInt(500, 5000), Y = Utility.RandomInt(500, 5000) });
            B.Add(new Item { name = $"{Utility.RandomString(10)}", X = Utility.RandomInt(500, 5000), Y = Utility.RandomInt(500, 5000) });
            C.Add(new Item { name = $"{Utility.RandomString(10)}", X = Utility.RandomInt(500, 5000), Y = Utility.RandomInt(500, 5000) });
            D.Add(new Item { name = $"{Utility.RandomString(10)}", X = Utility.RandomInt(500, 5000), Y = Utility.RandomInt(500, 5000) });
            E.Add(new Item { name = $"{Utility.RandomString(10)}", X = Utility.RandomInt(500, 5000), Y = Utility.RandomInt(500, 5000) });
        }

        A = A.OrderByDescending(x => x.X / x.Y).ToList();
        B = B.OrderByDescending(x => x.X / x.Y).ToList();
        C = C.OrderByDescending(x => x.X / x.Y).ToList();
        D = D.OrderByDescending(x => x.X / x.Y).ToList();
        E = E.OrderByDescending(x => x.X / x.Y).ToList();

        var minY = Double.Parse(amount) * (1 - (Double.Parse(offset) / 100));
        var maxY = Double.Parse(amount) * (1 + (Double.Parse(offset) / 100));

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

        try
        {
            query.ForAll(result =>
            {
                if (stopwatch.Elapsed.TotalSeconds > 5)
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

public static class Utility
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