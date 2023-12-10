namespace rcetest;

internal static class RunThings
{
    public static void ProcessArgs(string[] args, string assembly, Dictionary<string, string> valuesPassed, Action returnHelpMessage)
    {
        if (args.Length == 1 && args[0].Contains("help"))
        {
            returnHelpMessage();
        }
        else if (args.Length == 1 && args[0].Contains("version"))
        {
            Console.WriteLine($"rcetest version {assembly}");
        }
        else if (args.Length <= 6 && args.Length > 1)
        {
            // capture all the values of each argument
            for(var idx = 0; idx < args.Length; idx++)
            {
                var keyidx = 0;
                var valueidx = 0;
                if (idx % 2 == 0 || idx == 0)
                {
                    keyidx = idx; 
                    valueidx = idx+1;
                }
                else
                {
                    valueidx = idx;
                }
                if (valueidx == idx)
                {
                    continue;
                }
                valuesPassed.Add(args[keyidx], args[valueidx]);
            }
            foreach(var v in valuesPassed)
            {
                Console.WriteLine($"{v.Key} {v.Value}");
            }
        }
        else if (args.Length > 6)
        {
            Console.WriteLine("Too many arguments. Use --help for more information.");
        }
        else
        {
            Console.WriteLine("No arguments provided. Use --help for more information.");
        }
    }
}