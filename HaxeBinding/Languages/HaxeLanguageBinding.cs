using System;
using System.IO;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Projects.CodeGeneration;

namespace MonoDevelop.HaxeBinding.Languages
{
    public class HaxeLanguageBinding : ILanguageBinding
    {
        public string Language {
            get { return "Haxe"; }
        }

        // TODO: Remove MD < 2.1 compatibility first or later.
        public string CommentTag { get { return "//"; } }

        public string SingleLineCommentTag { get { return "//"; } }
        public string BlockCommentStartTag { get { return "/*"; } }
        public string BlockCommentEndTag { get { return "*/"; } }

        public bool IsSourceCodeFile(string fileName)
        {
            return fileName.EndsWith(".as", StringComparison.OrdinalIgnoreCase);
        }

        public IParser Parser {
            get { return null; }
        }

        public IRefactorer Refactorer {
            get { return null; }
        }

        public string GetFileName(string baseName)
        {
            return baseName + ".as";
        }
    }
}
