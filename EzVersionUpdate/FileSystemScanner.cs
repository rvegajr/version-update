public interface IFileSystemScanner
{
    List<string> FindFiles(string path, string[] masks);
}
/// <summary>
/// Utility class for scanning and retrieving a list of files within a directory.
/// </summary>
public class FileSystemScanner : IFileSystemScanner
{
    /// <summary>
    /// Finds files matching specified file masks within the given directory path.
    /// </summary>
    /// <param name="path">The path of the directory to search for files in.</param>
    /// <param name="masks">An array of file masks (e.g., "*.txt", "*.csv") to filter the files.</param>
    /// <returns>
    /// A list of file paths matching the specified file masks within the given directory.
    /// </returns>
    public List<string> FindFiles(string path, string[] masks)
    {
        // Specify whether to search for files in subdirectories.
        SearchOption sopt = SearchOption.AllDirectories;
        List<string> listFiles = new List<string>();
        List<DirectoryInfo> dirs2scan = new List<DirectoryInfo>();
        bool bNoHidden = true;

        // Initialize the list of directories to scan with the provided directory path.
        dirs2scan.Add(new DirectoryInfo(path));

        for (; dirs2scan.Count != 0;)
        {
            int scanIndex = dirs2scan.Count - 1;

            // Iterate through the file masks and find matching files in the current directory.
            foreach (var mask in masks)
            {
                FileInfo[] filesInfo = dirs2scan[scanIndex].GetFiles(mask, SearchOption.TopDirectoryOnly);

                foreach (FileInfo fi in filesInfo)
                {
                    if (bNoHidden && fi.Attributes.HasFlag(FileAttributes.Hidden))
                        continue;

                    listFiles.Add(fi.FullName);
                }
            }

            if (sopt != SearchOption.AllDirectories)
                break;

            // Traverse subdirectories and add them to the list for further scanning.
            foreach (DirectoryInfo dir in dirs2scan[scanIndex].GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (bNoHidden && dir.Attributes.HasFlag(FileAttributes.Hidden))
                    continue;

                dirs2scan.Add(dir);
            }

            dirs2scan.RemoveAt(scanIndex);
        }

        // Return the list of file paths matching the specified masks.
        return listFiles;
    }
}
