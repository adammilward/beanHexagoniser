/*
This calculates the optimum arrangement of different coloured magnetic balls,
to create concentric hexagon shaped rings, where each ring contains only one colour
of magnetic balls.
E.G.     r r r 
        r g g r
       r g b g r
        r g g r
         r r r
*/

using System;
using System.Diagnostics;
using System.Collections.Generic;

internal class Program
{
    private static void Main(string[] args)
    {
        List<BeanType> beansAvailable = new List<BeanType>();
        
        // hard coded  for convenience, but could be user inputs.
        beansAvailable.Add(new BeanType("magenta", 252));
        beansAvailable.Add(new BeanType("black", 126));
        beansAvailable.Add(new BeanType("orange", 84));
        beansAvailable.Add(new BeanType("brown", 78));
        beansAvailable.Add(new BeanType("pink", 7));

        //  beansAvailable.Add(new BeanType("pink", 5));
        //  beansAvailable.Add(new BeanType("orange", 4));
        //  beansAvailable.Add(new BeanType("black", 15));
        //  beansAvailable.Add(new BeanType("brown", 25));
        //  beansAvailable.Add(new BeanType("blue", 25 + 3));
        //  beansAvailable.Add(new BeanType("silver/turquise", 6 * 5 + 1));
        //  beansAvailable.Add(new BeanType("yellow", 13 * 5 + 1));
        //  beansAvailable.Add(new BeanType("red", 16 * 5 + 2));

         
        
        int beansAvailableQuantity = 0;
        int beansUsedQuantity = 0;

        // can speed things up a lot
        //beansAvailable.Sort((BeanType a, BeanType b) => a.Quantity - b.Quantity);

        Console.WriteLine($"You have {beansAvailable.Count} types of bean these are they:");
        for (int i = 0; i < beansAvailable.Count; i++)
        {
            Console.WriteLine($"{i}: {beansAvailable[i].Colour} x {beansAvailable[i].Quantity} ");
            beansAvailableQuantity += beansAvailable[i].Quantity;
        }

        Timer timer = Timer.Instance;
        List<string> colourList = Hexagoniser.hexMyBeans(beansAvailable);
        Console.WriteLine($"finished calculation: {timer.elapsed}s");

        Console.WriteLine();

        for (int i = 0; i < colourList.Count; i++) {
            int beans = (i == 0) ? 1 : i * 6;
            Console.WriteLine($"Layer {i + 1} - {colourList[i]}, {beans}");
            beansUsedQuantity += beans;
        }

        Console.WriteLine();
        Console.WriteLine($"You started with {beansAvailableQuantity} beans");
        Console.WriteLine($"You can Use {beansUsedQuantity} beans");
        Console.WriteLine(
            $"{beansAvailableQuantity} - {beansUsedQuantity} = {beansAvailableQuantity - beansUsedQuantity} beans left over");
        Console.WriteLine($"Total elapsed {timer.elapsed} s");
    }
}

/**
Given a list of beans/balls and thier quantities, finds the largest possible arrangement
of concentric hexagons of beans, each ring/layer in the hexagon must be of one color.
**/
class Hexagoniser
{
    /**
    provided with a list of beans outputs a list of colours,
    the first color being the inner most hexagon (a single bean)
    **/
    public static List<string> hexMyBeans(List<BeanType> beansAvailable) {
        
        List<string> currentAttempt = new List<string>();
        List<string> bestArrangement = new List<string>(currentAttempt);

        int beansRemaining = 0;
        foreach(BeanType beanType in beansAvailable) {
            beansRemaining += beanType.Quantity;
        }

        bool success = recursiveBuildLayers(
            ref beansAvailable,
            beansRemaining,
            currentAttempt,
            bestArrangement
        );
        Console.WriteLine();

        return bestArrangement;
    }

    private static bool recursiveBuildLayers(
        ref List<BeanType> beansAvailable,
        int beansRemaining,
        List<string> currentAttempt,
        List<string> bestArrangement
    ){
        int currentLayer = currentAttempt.Count;
        int beansAvailableIndex = 0;
        
        int layerQuantity = (currentLayer == 0) ? 1 : currentLayer * 6;

        bool complete = layerQuantity > beansRemaining;

        Timer timer = Timer.Instance;

        while  (! complete && beansAvailableIndex < beansAvailable.Count) { 

            if (currentLayer < 2) {
                if (currentLayer == 0) {
                    Console.WriteLine();
                    Console.Write(
                        $"Processing {((double)beansAvailableIndex / beansAvailable.Count):P1} - {timer.elapsed}s ");
                } else if (currentLayer == 1) {
                    Console.Write("*");
                } else if (currentLayer == 2) {
                    Console.Write("-");
                } else {
                    Console.Write(".");
                }
            }

            if (beansAvailable[beansAvailableIndex].Quantity >= layerQuantity) {
                // found a beanType with sufficient quantity
                addBeans(
                    ref currentAttempt,
                    ref beansAvailable,
                    layerQuantity,
                    ref beansRemaining,
                    beansAvailableIndex
                );
                updateBest(currentAttempt, ref bestArrangement);
                
                //next layer
                complete = recursiveBuildLayers(
                    ref beansAvailable,
                    beansRemaining,
                    currentAttempt,
                    bestArrangement
                );

                if (! complete) {
                    removeBeans(
                        ref currentAttempt,
                        ref beansAvailable,
                        layerQuantity,
                        ref beansRemaining,
                        beansAvailableIndex
                    );
                }
            } 
            beansAvailableIndex ++;
        }
        return complete;
    }

    private static void addBeans(
        ref List<string> currentAttempt,
        ref List<BeanType> beansAvailable,
        int layerQuantity,
        ref int beansRemaining,
        int beansAvailableIndex
    ) {
        currentAttempt.Add(beansAvailable[beansAvailableIndex].Colour);
        beansAvailable[beansAvailableIndex] = new BeanType(
            beansAvailable[beansAvailableIndex].Colour,
            beansAvailable[beansAvailableIndex].Quantity - layerQuantity
        );
        beansRemaining -= layerQuantity;
    }

    private static void removeBeans(
        ref List<string> currentAttempt,
        ref List<BeanType> beansAvailable,
        int layerQuantity,
        ref int beansRemaining,
        int beansAvailableIndex
    ) {
        currentAttempt.RemoveAt(currentAttempt.Count - 1);
        beansAvailable[beansAvailableIndex] = new BeanType(
            beansAvailable[beansAvailableIndex].Colour,
            beansAvailable[beansAvailableIndex].Quantity + layerQuantity
        );
        beansRemaining += layerQuantity;
    }

    private static void updateBest(
        List<string> currentAttempt,
        ref List<string> bestArrangement
    ) {
        if (bestArrangement.Count < currentAttempt.Count) {
            bestArrangement.Clear();
            for(int i = 0; i < currentAttempt.Count; i ++) {
                bestArrangement.Add(currentAttempt[i]);
            }           
        }
    }
}

struct BeanType {
    public string Colour {get; private set;}
    public int Quantity {get; private set;}

    public BeanType(string colour, int quantity) {
        this.Colour = colour;
        this.Quantity = quantity; 
    }

    public void reduceQuantity(int reduction) {
        this.Quantity = this.Quantity - reduction;
    }
}

/*
Singleton class provides a single instace of timer,
which can be used to display time elapsed, after it was
first instantiated.
*/
class Timer
{
    private static Timer? instance = null;
    private static readonly object padlock = new object();

    private Stopwatch stopwatch;

    private Timer()
    {
        this.stopwatch = new Stopwatch();
        this.stopwatch.Start();
    }

    public static Timer Instance
    {
        get
        {
            if (instance == null)
            {
                lock (padlock) 
                {
                    instance = new Timer();
                }
            }
            return instance;
        }
    }

    public double elapsed {
        get {
            TimeSpan ts = stopwatch.Elapsed;
            return ts.Seconds + (double)ts.Milliseconds / 1000;            
        }
    }
}