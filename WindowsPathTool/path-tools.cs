using Microsoft.Win32;

namespace WinPathTool
{
    public class PathTools
    {
        private readonly string _varName;
        private readonly List<string> _pathArgList = new List<string>();

        public string Args { get; init; }

        public bool WriteMode { get; init; }

        public bool System { get; init; }

        public List<string> NewPaths
        {
            get; private set;
        } = new List<string>();

        public string PathString { get; private set; } = "";

        public PathTools(string varName, string[] args)
        {
            _varName = varName;
            Args = String.Join(" ", args);

            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg == "-s")
                    {
                        System = true;
                    }
                    else if (arg == "-c")
                    {
                        _pathArgList.Add(CurrentDir());
                        WriteMode = true;
                    }
                    else
                    {
                        _pathArgList.Add(arg);
                        WriteMode = true;
                    }
                }
            }
        }

        public static string CurrentDir()
        {
            return Environment.CurrentDirectory;
        }

        public List<string> UpdatePath()
        {
            return UpdatePath(_pathArgList.ToArray());
        }

        public List<string> UpdatePath(string[] newPaths)
        {
            string[] pathArr = GetPathArray();

            foreach (string path in newPaths)
            {
                string _path = path.Trim();
                bool pathExists = _path != "" && Array.Exists(pathArr,
                 element => element.Equals(_path));

                if (!pathExists)
                {
                    NewPaths.Add(_path);
                }
                else
                {
                    throw new PathExistsException(_path);
                }
            }

            if (NewPaths.Count > 0)
            {
                List<string> pathVarList = new List<string>(NewPaths);
                pathVarList.Insert(0, GetPathVariable());
                string newPathString = string.Join(';', pathVarList);

                SetPathVariable(newPathString);

                PathString = newPathString;
            }

            return NewPaths;
        }

        public RegistryKey GetRegistryPathKey()
        {
            string keyName = System
            ? @"System\CurrentControlSet\Control\Session Manager\Environment"
            : @"Environment";

            RegistryKey key = System
                ? Registry.LocalMachine.OpenSubKey(keyName, WriteMode)
                : Registry.CurrentUser.OpenSubKey(keyName, WriteMode);

            return key;
        }

        public string[] GetPathArray()
        {
            PathString = GetPathVariable();
            return PathString.Split(";");
        }

        public string GetPathVariable()
        {
            var key = GetRegistryPathKey();
            var pathFromRegistry = key.GetValue(_varName, "not found", RegistryValueOptions.DoNotExpandEnvironmentNames);
            return pathFromRegistry.ToString();
        }

        private void SetPathVariable(string newPath)
        {
            var key = GetRegistryPathKey();
            key.SetValue(_varName, newPath, RegistryValueKind.ExpandString);
        }
    }
}
