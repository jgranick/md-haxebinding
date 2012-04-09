using System;
using System.IO;
using System.Xml;
using MonoDevelop.Projects;


namespace MonoDevelop.HaxeBinding.Projects
{

	public class NMEProjectBinding : IProjectBinding
	{
		
		public string Name { get { return "NME"; } }

		
		public bool CanCreateSingleFileProject (string sourceFile)
		{
			return sourceFile.EndsWith (".hx", StringComparison.OrdinalIgnoreCase)
                || sourceFile.EndsWith (".nmml", StringComparison.OrdinalIgnoreCase);
		}
		
		
		public Project CreateProject (ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new NMEProject (info, projectOptions);
		}


		public Project CreateSingleFileProject (string sourceFile)
		{
			ProjectCreateInformation info = new ProjectCreateInformation ();
			info.ProjectName = Path.GetFileNameWithoutExtension (sourceFile);
			info.SolutionPath = Path.GetDirectoryName (sourceFile);
			info.ProjectBasePath = Path.GetDirectoryName (sourceFile);

			Project project = null;
			project = new NMEProject (info, null);
			project.Files.Add (new ProjectFile (sourceFile));

			return project;
		}
		
	}
	
}