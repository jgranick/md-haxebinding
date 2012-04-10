using System;
using System.IO;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Projects.CodeGeneration;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;


namespace MonoDevelop.HaxeBinding.Languages
{

	public class NMMLLanguageBinding : ILanguageBinding
	{
		
		public string BlockCommentEndTag { get { return "-->"; } }
		public string BlockCommentStartTag { get { return "<!--"; } }
		public string CommentTag { get { return null; } }
		public string Language { get { return "NMML"; } }
		public string SingleLineCommentTag { get { return null; } }
		
		
		public string GetFileName (string baseName)
		{
			return baseName + ".nmml";
		}
		
		
		public bool IsSourceCodeFile (string fileName)
		{
			return fileName.EndsWith (".nmml", StringComparison.OrdinalIgnoreCase);
		}

		
		public IParser Parser {
			get { return null; }
		}


		public IRefactorer Refactorer {
			get { return null; }
		}
		
	}
	
}
