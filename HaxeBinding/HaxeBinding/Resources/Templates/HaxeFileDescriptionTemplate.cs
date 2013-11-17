using System;
using MonoDevelop.Ide.Templates;
using MonoDevelop.HaxeBinding.Projects;
using System.Collections.Generic;

namespace HaxeBinding
{
	public class HaxeFileDescriptionTemplate : TextFileDescriptionTemplate
	{

		public override void ModifyTags (MonoDevelop.Projects.SolutionItem policyParent, MonoDevelop.Projects.Project project, string language, string identifier, string fileName, ref Dictionary<string, string> tags)
		{
			base.ModifyTags (policyParent, project, language, identifier, fileName, ref tags);

			if (tags != null) {
				if (project is HaxeProject)
					tags ["ModuleName"] = (project as HaxeProject).ModuleName;
			}
		}
	}
}

