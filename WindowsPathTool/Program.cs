using WinPathTool;

string varName = "path";

Console.WriteLine("WinPathTool — Read or update the Windows path environment variable");
Console.WriteLine("(C) Bjornar Egede-Nissen 2023\n");

//string[] _args = args.Length > 0 ? args : new string[] { "-c", "-s" };

PathTools pathTools = new PathTools(varName, args);
string target = pathTools.System ? "system" : "user";

if (!pathTools.WriteMode)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("[]\t- Show current paths (defaults to user paths)");
    Console.WriteLine("[string [...string]]\t- Add space separated string(s) to path (defaults to user paths)");
    Console.WriteLine("[-c]\t- Add current directory to path");
    Console.WriteLine("[-s]\t- Target system path variable");
    Console.WriteLine($"\nCurrent {target} paths:");

    int i = 0;
    foreach (string path in pathTools.GetPathArray())
    {
        i++;
        Console.WriteLine($"{i.ToString().PadLeft(2)}) {path}");
    }

    Console.WriteLine();
}
else
{
    Console.WriteLine($"Updating the {target} path variable...\n");

    try
    {
        if (pathTools.System && !ProcessManager.IsUserAnAdmin())
        {
            Console.WriteLine("Administrator privileges are required to write to the system path variable.\nThe program will relaunch with elevated rights in a different window.");
            ProcessManager.RelaunchAsElevated(pathTools.Args);
        }
        else
        {
            List<string> pathsAdded;

            try
            {
                pathsAdded = pathTools.UpdatePath();
            }
            catch (Exception pathErr)
            {
                Console.WriteLine(pathErr.Message);
                return;
            }

            if (pathsAdded.Count > 0)
            {
                Console.WriteLine("Paths added:");
                foreach (string path in pathsAdded)
                {
                    Console.WriteLine(path);
                }
                Console.WriteLine();
            }
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"\n\n--- An error occurred, operation aborted ---\n{e.Message}\n");
    }
}
