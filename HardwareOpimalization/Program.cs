using CsvHelper;
using Newtonsoft.Json;
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
    public int TotalCost { get; set; }
}

class Program
{
    public static void Main(string[] args)
    {

        #region Ładowanie danych

        Console.WriteLine("Ścieżka:");
        string path = Console.ReadLine();
        Console.WriteLine("Kwota:");
        string amount = Console.ReadLine();
        Console.WriteLine("Odchylenie:");
        string offset = Console.ReadLine();
        Console.WriteLine("Liczba rozwiązań:");
        string solutionNumber = Console.ReadLine();

        Console.WriteLine("Istotność podzespołów (zakres 0-10)");
        Console.WriteLine("Procesor:");
        string multiplierA = Console.ReadLine();
        Console.WriteLine("Karta Graficzna:");
        string multiplierB = Console.ReadLine();
        Console.WriteLine("Dysk:");
        string multiplierC = Console.ReadLine();
        Console.WriteLine("RAM:");
        string multiplierD = Console.ReadLine();
        Console.WriteLine("Płyta Główna:");
        string multiplierE = Console.ReadLine();

        Console.WriteLine("------------------");
        Console.WriteLine("Obliczanie rozwiązań");
        Console.WriteLine("------------------");

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
        var csvB = new CsvReader(readerB, CultureInfo.InvariantCulture);
        var csvC = new CsvReader(readerC, CultureInfo.InvariantCulture);
        var csvD = new CsvReader(readerD, CultureInfo.InvariantCulture);
        var csvE = new CsvReader(readerE, CultureInfo.InvariantCulture);
        A = csvA.GetRecords<Item>().ToList();
        B = csvB.GetRecords<Item>().ToList();
        C = csvC.GetRecords<Item>().ToList();
        D = csvD.GetRecords<Item>().ToList();
        E = csvE.GetRecords<Item>().ToList();

        #endregion

        #region Transformacja danych

        A.ForEach(x => x.X = (int)(Utility.Remap(x.X, A.MinBy(x => x.X).X, A.MaxBy(x => x.X).X, 1, 100) * (1 + (Double.Parse(multiplierA) / 10))));
        B.ForEach(x => x.X = (int)(Utility.Remap(x.X, B.MinBy(x => x.X).X, B.MaxBy(x => x.X).X, 1, 100) * (1 + (Double.Parse(multiplierB) / 10))));
        C.ForEach(x => x.X = (int)(Utility.Remap(x.X, C.MinBy(x => x.X).X, C.MaxBy(x => x.X).X, 1, 100) * (1 + (Double.Parse(multiplierC) / 10))));
        D.ForEach(x => x.X = (int)(Utility.Remap(x.X, D.MinBy(x => x.X).X, D.MaxBy(x => x.X).X, 1, 100) * (1 + (Double.Parse(multiplierD) / 10))));
        E.ForEach(x => x.X = (int)(Utility.Remap(x.X, E.MinBy(x => x.X).X, E.MaxBy(x => x.X).X, 1, 100) * (1 + (Double.Parse(multiplierE) / 10))));

        A = A.OrderByDescending(x => x.X / x.Y).ToList();
        B = B.OrderByDescending(x => x.X / x.Y).ToList();
        C = C.OrderByDescending(x => x.X / x.Y).ToList();
        D = D.OrderByDescending(x => x.X / x.Y).ToList();
        E = E.OrderByDescending(x => x.X / x.Y).ToList();

        #endregion

        #region Porównanie wszystkich możliwych kombinacji

        var minY = Double.Parse(amount) * (1 - (Double.Parse(offset) / 100));
        var maxY = Double.Parse(amount) * (1 + (Double.Parse(offset) / 100));
        var maxX = 0;
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
                    select new PackOfItems { A = itemA, B = itemB, C = itemC, D = itemD, E = itemE, maxValue = totalX, TotalCost = totalY };

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
                    Console.WriteLine("Znaleziono rozwiązanie: Y: " + (result.A.Y + result.B.Y + result.C.Y + result.D.Y + result.E.Y) + ", X: " + result.maxValue);
                }
                else if (result.maxValue > maxX)
                {
                    maxX = result.maxValue;
                    solutions.Clear();
                    solutions.Add(result);
                    Console.WriteLine("Znaleziono rozwiązanie: Y: " + (result.A.Y + result.B.Y + result.C.Y + result.D.Y + result.E.Y) + ", X: " + result.maxValue);
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Anulowano kwerendę.");
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine("Czas kwerendy: " + stopwatch.Elapsed.ToString("g"));
        }

        #endregion

        #region Wylistowanie wyników do pliku

        var best = solutions.OrderByDescending(x => x.maxValue).ToList().Take(Int32.Parse(solutionNumber));
        string fileName = path + @"\solution.json";
        string json = JsonConvert.SerializeObject(best, Formatting.Indented);
        File.WriteAllText(fileName, json);

        #endregion
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
    public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }
}