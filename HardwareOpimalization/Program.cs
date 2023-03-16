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

        A.Add(new Item { name = "a1", X = 5000, Y = 1000 });
        A.Add(new Item { name = "a2", X = 2000, Y = 1000 });

        B.Add(new Item { name = "b1", X = 1400, Y = 1000 });
        B.Add(new Item { name = "b2", X = 2000, Y = 900 });

        C.Add(new Item { name = "c1", X = 1300, Y = 1000 });
        C.Add(new Item { name = "c2", X = 2000, Y = 900 });

        D.Add(new Item { name = "d1", X = 6000, Y = 1000 });
        D.Add(new Item { name = "d2", X = 2000, Y = 1500 });

        E.Add(new Item { name = "e1", X = 1100, Y = 1000 });
        E.Add(new Item { name = "e2", X = 2000, Y = 2000 });

        var minY = 4500;
        var maxY = 5500;

        List<PackOfItems> solutions = new List<PackOfItems>();

        var maxX = 0;

        foreach (var itemA in A)
        {
            foreach (var itemB in B)
            {
                foreach (var itemC in C)
                {
                    foreach (var itemD in D)
                    {
                        foreach (var itemE in E)
                        {
                            var totalX = itemA.X + itemB.X + itemC.X + itemD.X + itemE.X;
                            var totalY = itemA.Y + itemB.Y + itemC.Y + itemD.Y + itemE.Y;

                            if ((minY <= totalY) && (totalY <= maxY)) 
                            {
                                if (totalX > maxX)
                                {
                                    maxX = totalX;
                                    solutions.Clear();
                                    solutions.Add(new PackOfItems { A = itemA, B = itemB, C = itemC, D = itemD, E = itemE, maxValue = maxX });
                                } else if (totalX == maxX) 
                                {
                                    solutions.Add(new PackOfItems { A = itemA, B = itemB, C = itemC, D = itemD, E = itemE, maxValue = maxX });
                                }
                            }
                        }
                    }
                }
            }
        }

        var best = solutions.MaxBy(x => x.maxValue);

    } 
}