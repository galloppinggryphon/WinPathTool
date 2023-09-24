namespace WinPathTool
{
    public class PathExistsException : Exception
    {
        public PathExistsException(string message) : base("Path already exists: " + message) { }
    }
}
