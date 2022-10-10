/*
This calculates the optimum arrangement of different coloured magnetic balls,
to create concentric hexagon shaped rings, where each ring contains only one colour
of magnetic balls.
E.G.     r r r 
        r g g r
       r g b g r
        r g g r
         r r r
It's called BeanHexagoniser, because you could use coloured beans
and it avoids names like updateBalls and hexMyBalls etc.
*/
internal class Program
{
    private static void Main(string[] args)
    {
        List<BeanType> beansAvailable = new List<BeanType>();
        
        // hard coded  for convenience, but could be user inputs.
         beansAvailable.Add(new BeanType("red", 16 * 5 + 2));
         beansAvailable.Add(new BeanType("silver/turquise", 6 * 5 + 1));
         beansAvailable.Add(new BeanType("blue", 25 + 3));
         beansAvailable.Add(new BeanType("brown", 25));
         beansAvailable.Add(new BeanType("black", 15));
         beansAvailable.Add(new BeanType("pink", 5));
         beansAvailable.Add(new BeanType("orange", 4));
         beansAvailable.Add(new BeanType("yellow", 13 * 5 + 1));
        
        int beansAvailableQuantity = 0;
        int beansUsedQuantity = 0;

        Console.WriteLine($"You have {beansAvailable.Count} types of bean these are they:");
        for (int i = 0; i < beansAvailable.Count; i++)
        {
            Console.WriteLine(
                $"{i}: {beansAvailable[i].getColour()} x {beansAvailable[i].getQuantity()} ");
            beansAvailableQuantity += beansAvailable[i].getQuantity();
        }

        List<string> colourList = Hexagoniser.hexMyBeans(beansAvailable);

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
    }
}

/**
Given a list of beans/balls and thier quantities, finds the arrangement
of concentric hexagons of beans, each ring/layer in the hexagon must be of one color, 
that uses the most beans. 
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
            beansRemaining += beanType.getQuantity();
        }

        bool success = recursiveBuildLayers(
            ref beansAvailable,
            beansRemaining,
            currentAttempt,
            bestArrangement
        );

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

        while  (! complete && beansAvailableIndex < beansAvailable.Count) { 

            if (currentLayer == 0) {
                Console.WriteLine($"Processing {((double)beansAvailableIndex / beansAvailable.Count):P1}");
            }

            if (beansAvailable[beansAvailableIndex].getQuantity() >= layerQuantity) {
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
        currentAttempt.Add(beansAvailable[beansAvailableIndex].getColour());
        beansAvailable[beansAvailableIndex] = new BeanType(
            beansAvailable[beansAvailableIndex].getColour(),
            beansAvailable[beansAvailableIndex].getQuantity() - layerQuantity
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
            beansAvailable[beansAvailableIndex].getColour(),
            beansAvailable[beansAvailableIndex].getQuantity() + layerQuantity
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
    private string colour;
    private int quantity;

    public BeanType(string colour, int quantity) {
        this.colour = colour;
        this.quantity = quantity; 
    }

    public string getColour() {
        return this.colour;
    }
    public int getQuantity() {
        return this.quantity;
    }

    public void reduceQuantity(int reduction) {
        this.quantity = this.quantity - reduction;
    }
}

