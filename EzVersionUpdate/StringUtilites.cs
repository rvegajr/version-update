public interface IStringUtilites
{
    string Pluck(string str, string leftString, string rightString);
    string VersionStringIncrement(string versionString, int segmentToIncrement);
    string VersionStringParts(string versionString, params int[] versionSegments);
}

/// <summary>
/// A utility class for various string manipulation operations.
/// </summary>
public class StringUtilites : IStringUtilites
{
    /// <summary>
    /// Extracts a substring between two specified substrings within a given string.
    /// </summary>
    /// <param name="str">The input string to search within.</param>
    /// <param name="leftString">The left delimiter to start extraction from.</param>
    /// <param name="rightString">The right delimiter to end extraction at.</param>
    /// <returns>
    /// The extracted substring between the left and right delimiters.
    /// Returns an empty string if the delimiters are not found or if an exception occurs.
    /// </returns>
    public string Pluck(string str, string leftString, string rightString)
    {
        try
        {
            var lpos = str.LastIndexOf(leftString);
            var rpos = str.IndexOf(rightString, lpos + 1);
            if (rpos > 0)
            {
                lpos = str.LastIndexOf(leftString, rpos);
                if ((lpos > 0) && (rpos > lpos))
                {
                    return str.Substring(lpos + leftString.Length, (rpos - lpos) - leftString.Length);
                }
            }
        }
        catch (Exception)
        {
            return "";
        }
        return "";
    }

    /// <summary>
    /// Extracts specific segments from a version string using dot ('.') as a separator.
    /// </summary>
    /// <param name="versionString">The input version string to extract segments from.</param>
    /// <param name="versionSegments">An array of segment indices to extract.</param>
    /// <returns>The extracted version segments concatenated as a string.</returns>
    public string VersionStringParts(string versionString, params int[] versionSegments)
    {
        var vArr = versionString.Split('.');
        if (vArr.Length == 3)
        {
            var lstArr = vArr.ToList();
            lstArr.Add("0");
            vArr = lstArr.ToArray();
        }
        string newVersion = "";
        foreach (var versionSegment in versionSegments)
        {
            newVersion += (newVersion.Length > 0 ? "." : "") + vArr[versionSegment].ToString();
        }
        return newVersion;
    }

    /// <summary>
    /// Increments a specific segment in a version string using dot ('.') as a separator.
    /// </summary>
    /// <param name="versionString">The input version string to modify.</param>
    /// <param name="segmentToIncrement">The index of the segment to increment.</param>
    /// <returns>The modified version string with the specified segment incremented.</returns>
    public string VersionStringIncrement(string versionString, int segmentToIncrement)
    {
        var vArr = versionString.Split('.');
        var valAsStr = vArr[segmentToIncrement];
        int valAsInt = 0;
        int.TryParse(valAsStr, out valAsInt);
        vArr[segmentToIncrement] = (valAsInt + 1).ToString();
        return string.Join(".", vArr);
    }
}
