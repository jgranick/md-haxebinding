using System;
using System.IO;
using System.Xml;
using MonoDevelop.Projects;


// This file was taken from the old "FlexBinding" add-in and has not been adapted yet


namespace MonoDevelop.HaxeBinding.Projects
{
    public class HaxeProjectBinding : IProjectBinding
    {
        public string Name {
            get { return "Haxe"; }
        }

        public Project CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
        {
            Console.WriteLine(projectOptions.OuterXml);
            string language = projectOptions.GetAttribute("MainLanguage");
            return new HaxeProject(info, projectOptions, language);
        }

        public Project CreateSingleFileProject(string sourceFile)
        {
            ProjectCreateInformation info = new ProjectCreateInformation();
            info.ProjectName = Path.GetFileNameWithoutExtension(sourceFile);
            //info.CombinePath = Path.GetDirectoryName(sourceFile);
			info.SolutionPath = Path.GetDirectoryName(sourceFile);
            info.ProjectBasePath = Path.GetDirectoryName(sourceFile);

            Project project = null;
			project = new HaxeProject(info, null, "Haxe");
            project.Files.Add(new ProjectFile(sourceFile));

            return project;
        }
        
        public bool CanCreateSingleFileProject(string sourceFile)
        {
            return sourceFile.EndsWith(".hx", StringComparison.OrdinalIgnoreCase)
                || sourceFile.EndsWith(".nmml", StringComparison.OrdinalIgnoreCase);
        }
    }
}
