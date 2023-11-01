// The IProjectFileEditor interface defines a contract for editing properties within a Visual Studio project file (.csproj).
public interface IProjectFileEditor
{
    string Version { get; set; } // Gets or sets the version of the project.
    string AssemblyVersion { get; set; } // Gets or sets the AssemblyVersion of the project.
    string FileVersion { get; set; } // Gets or sets the FileVersion of the project.
    string ProjectFilePath { get; set; } // Gets or sets the path to the project file.
    void Save(); // Saves any changes made to the project file.
    void Load(string projectFilePath); // Loads a project file for editing.
}

// The ProjectFileEditor class implements the IProjectFileEditor interface and provides utility methods for manipulating and editing properties within a Visual Studio project file (.csproj).
public class ProjectFileEditor : IProjectFileEditor
{
    private XDocument projectFileDocument;
    private string projectFilePath;
    private bool isModified;

    // Loads the project file for editing.
    private void LoadProjectFile(string fileToLoad)
    {
        try
        {
            if (string.IsNullOrEmpty(fileToLoad))
            {
                throw new ArgumentNullException(nameof(fileToLoad));
            }

            projectFilePath = fileToLoad;
            projectFileDocument = XDocument.Load(projectFilePath);
            isModified = false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error loading project file: {ex.Message}");
        }
    }

    // Gets or sets the version of the project from the .csproj file.
    public string Version
    {
        get
        {
            var versionElement = projectFileDocument.Root
                .Elements("PropertyGroup")
                .Elements("Version")
                .FirstOrDefault();

            return versionElement?.Value;
        }
        set
        {
            UpdateProperty("Version", value);
        }
    }

    // Gets or sets the AssemblyVersion of the project from the .csproj file.
    public string AssemblyVersion
    {
        get
        {
            var versionElement = projectFileDocument.Root
                .Elements("PropertyGroup")
                .Elements("AssemblyVersion")
                .FirstOrDefault();

            return versionElement?.Value;
        }
        set
        {
            UpdateProperty("AssemblyVersion", value);
        }
    }

    // Gets or sets the FileVersion of the project from the .csproj file.
    public string FileVersion
    {
        get
        {
            var versionElement = projectFileDocument.Root
                .Elements("PropertyGroup")
                .Elements("FileVersion")
                .FirstOrDefault();

            return versionElement?.Value;
        }
        set
        {
            UpdateProperty("FileVersion", value);
        }
    }

    // Gets or sets the project file's path.
    public string ProjectFilePath { get => projectFilePath; set => projectFilePath = value; }

    // Updates the specified property in the project file.
    private void UpdateProperty(string propertyName, string value)
    {
        var propertyElement = projectFileDocument.Root
            .Elements("PropertyGroup")
            .Elements(propertyName)
            .FirstOrDefault();

        if (propertyElement != null)
        {
            propertyElement.Value = value;
        }
        else
        {
            // If the property element does not exist, create it.
            var propertyGroup = projectFileDocument.Root
                .Elements("PropertyGroup")
                .FirstOrDefault();

            if (propertyGroup == null)
            {
                propertyGroup = new XElement("PropertyGroup");
                projectFileDocument.Root.Add(propertyGroup);
            }

            propertyGroup.Add(new XElement(propertyName, value));
        }

        isModified = true;
    }

    // Saves the changes made to the project file if there are any modifications.
    public void Save()
    {
        if (isModified)
        {
            projectFileDocument.Save(projectFilePath);
            isModified = false;
        }
    }

    // Load a new project file for editing.
    public void Load(string projectFilePath)
    {
        LoadProjectFile(projectFilePath);
    }
}
