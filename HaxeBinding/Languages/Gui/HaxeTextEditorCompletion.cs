using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using MonoDevelop.Core;
using MonoDevelop.Core.Collections;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.CodeTemplates;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects;
//using MonoDevelop.Projects.Dom;
//using MonoDevelop.Projects.Dom.Output;
//using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.HaxeBinding.Projects;
using MonoDevelop.HaxeBinding.Tools;
using ICSharpCode.NRefactory.Completion;


namespace MonoDevelop.HaxeBinding.Languages.Gui
{	

	public class HaxeTextEditorCompletion : CompletionTextEditorExtension
	{
		
		private XmlDocument mCacheXML = null;
		private bool mCacheXMLCurrent = true;
		private bool mCacheIsObject = true;
		private int mCacheTriggerLine = -1;
		private int mCacheTriggerOffset = -1;
		private bool mCanRunCompletion = false;
		private bool mCompletionEnabled = true;
		private string mTempBaseDirectory;
		private string mTempDirectory;
		private string mTempFileName;
		private HaxeParameterDataProvider parameterDataProvider;
		
		
		public override bool CanRunCompletionCommand ()
		{
			return mCanRunCompletion;
		}
		
		
		public override string CompletionLanguage {
			get {
				return "Haxe";
			}
		}
		
		
		public override ICompletionDataList CodeCompletionCommand (CodeCompletionContext completionContext)
		{
			//MonoDevelop.Ide.MessageService.ShowError ("CodeCompletionCommand");
			// This default implementation of CodeCompletionCommand calls HandleCodeCompletion providing
			// the char at the cursor position. If it returns a provider, just return it.
			
			int pos = completionContext.TriggerOffset;
			if (pos > 0) {
				char ch = Editor.GetCharAt (pos - 1);
				int triggerWordLength = completionContext.TriggerWordLength;
				ICompletionDataList completionList = HandleCodeCompletion (completionContext, ch, ref triggerWordLength);
				if (completionList != null)
					return completionList;
			}
			return null;
		}
		
		
		public override void Dispose ()
		{
			try
			{
				Directory.Delete (mTempBaseDirectory, true);
			}
			catch
			{
			}
			base.Dispose ();
		}
		
		
		public override int GetCurrentParameterIndex (int startOffset)
		{
			int cursor = document.Editor.Caret.Offset;
			int i = startOffset;
			
			if (i > cursor)
				return -1;
			else if (i == cursor)
				return 1;
			
			int parameterIndex = 1;
			
			while (i++ < cursor) {
				char ch = document.Editor.GetCharAt (i-1);
				if (ch == ',')
					parameterIndex++;
				else if (ch == ')')
					return -1;
			}
			
			return parameterIndex;
			
			
			//int index = 
			//return 0;
			//return base.GetCurrentParameterIndex (startOffset);
		}
		
		
		private void FetchCompletionData (CodeCompletionContext completionContext)
		{
			if (completionContext != null && completionContext.TriggerOffset != mCacheTriggerOffset)
			{
				mCacheTriggerOffset = completionContext.TriggerOffset;
				File.WriteAllText (mTempFileName, Document.Editor.Text);
				
				string data = HaxeCompilerManager.GetCompletionData (Document.Project, mTempBaseDirectory, mTempFileName, mCacheTriggerOffset);

				//MonoDevelop.Ide.MessageService.ShowMessage (data);

				try
				{
					var xml = new XmlDocument ();
					xml.LoadXml (data);
					
					if (xml.HasChildNodes)
					{
						if (xml.FirstChild.Name == "type")
						{
							mCacheIsObject = false;
						}
						else
						{
							mCacheIsObject = true;
						}
						
						mCacheXML = xml;
						mCacheXMLCurrent = true;
						mCacheTriggerLine = completionContext.TriggerLine;
					}
					
					return;
				}
				catch (Exception ex)
				{
					ex.ToString ();
				}
				
				mCacheXMLCurrent = false;
				//mCacheXML = null;
				return;
			}
			else 
			{
				// temp hack to reset the cacheIsObject value for method completion
				if (mCacheXML != null && mCacheXML.FirstChild.Name == "type")
				{
					mCacheIsObject = false;
				}
				else
				{
					mCacheIsObject = true;
				}
			}
		}


		private string FormatDocumentation (string documentation)
		{
			string[] lines = documentation.Split ('\n');

			documentation = "";
			bool newline = true;

			for (int i = 0; i < lines.Length; i++) {

				string line = "\n" + lines[i].Trim ();
				line = line.Replace ("\n* ", " ");
				line = line.Replace ("\n*", " ");
				line = line.Replace ("<p>", "");
				line = line.Replace ("</p>", "");
				line = line.Replace ("<code>", "");
				line = line.Replace ("</code>", "");
				line = line.Replace ("<b>", "");
				line = line.Replace ("</b>", "");

				if (newline) {

					line = line.TrimStart ();
					newline = false;

				}

				if (line.Trim () == "") {

					line = "\n\n";
					newline = true;

				}

				if (line.IndexOf ("@param") > -1 || line.IndexOf ("@throws") > -1 || line.IndexOf ("@return") > -1) {

					break;

				}

				documentation += line;

			}

			return "\n" + documentation;
		}


		private string FormatShortType (string type)
		{
			string[] keywords = type.Split (' ');
			type = "";

			for (int i = 0; i < keywords.Length; i++) {

				if (keywords[i].IndexOf (".") > -1) {

					string[] segments = keywords[i].Split ('.');

					if (keywords[0].IndexOf ("<") > -1) {

						type += keywords[0].Substring (0, keywords[0].IndexOf ("<") + 1);

					}

					type += segments[segments.Length - 1];
					
				} else {

					type += keywords[i];

				}

				if (i < keywords.Length - 1) {

					type += " ";

				}

			}

			return type;
		}


		private string FormatType (string name, string type)
		{
			if (type.IndexOf (":") > -1) {

				type = type.Replace (" ", "");

				string[] splitByOpenParen = type.Split ('(');
				type = "";

				for (int i = 0; i < splitByOpenParen.Length; i++) {

					string[] splitByCloseParen = splitByOpenParen[i].Split (')');

					for (int j = 0; j < splitByCloseParen.Length; j++) {

						if (i > 0 && j == 0) {

							type += splitByCloseParen[0].Replace ("->", "-&gt;");

						} else {

							type += splitByCloseParen[i];

						}

					}

				}

				string[] parameters = type.Split (new string[] { "->" }, StringSplitOptions.None);

				string display = "function " + name + " (";
				
				for (int i = 0; i < parameters.Length - 1; i++) {

					string param = parameters [i];
					param = param.Replace ("-&gt;", " -> ");
					int index = param.IndexOf (":");
					
					if (index > -1) {	
						display += param.Substring (0, index + 1) + FormatShortType (param.Substring (index + 1));
						
						if (i < parameters.Length - 2) {
							display += ", ";
						}
					}
				}
				
				display += ") : " + FormatShortType (parameters [parameters.Length - 1]) + " ...";
				
				type = display;

			} else {

				if (type != "") {

					type = "var " + name + " : " + FormatShortType (type) + " ...";

				}

			}

			//type = type.Replace (" : ", ":");

			return type;
		}
		
		
		private CompletionDataList GetCompletionList (CodeCompletionContext completionContext)
		{
			FetchCompletionData (completionContext);
			
			if (mCacheXML != null && mCacheXMLCurrent && mCacheIsObject)
			{
				try
				{
					XmlDocument xml = mCacheXML;
					CompletionDataList list = new CompletionDataList ();
					
					if (xml.HasChildNodes && xml.FirstChild.HasChildNodes)
					{
						string documentation;
						string[] lines;
						string name;
						string type;
						string icon;
						
						switch (xml.FirstChild.Name)
						{
							case "list":
								
								foreach (XmlElement node in xml.FirstChild.ChildNodes)
								{
									name = node.GetAttribute ("n");
									icon = "md-property";
									
									lines = node.InnerText.Replace("\r","").Split('\n');

									type = "";
									documentation = "";

									for (var i = 0; i < lines.Length; i++) {
										
										if (i == 0) {
											
											type = lines[0];
											
										} else {
											
											documentation += "\n" + lines[i];
											
										}
										
									}

									//MonoDevelop.Ide.MessageService.ShowMessage (documentation);
								
									if (type.IndexOf (":") > -1)
									{
										icon = "md-method";
										//md-literal
									}
								
									if (documentation.IndexOf ("@private") == -1)
									{
										list.Add (new CompletionData (name, icon, FormatType (name, type) + FormatDocumentation (documentation)));
									}
								}
							
								break;
						}	
					}
					
					return list;
				}
				catch (Exception ex)
				{
					ex.ToString ();
				}
			}
			
			return null;	
		}
		
		
		private string GetPackage ()
		{
			
			int packageIndex = Document.Editor.Text.IndexOf ("package");
			
			if (packageIndex > -1)
			{
				
				int start = packageIndex + "package".Length;
				return Document.Editor.Text.Substring (start, Document.Editor.Text.IndexOf (';', start) - start).Trim ();
				
			}
			
			return "";
			
		}
		
		
		/*public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			if (mCanRunCompletion && mCompletionEnabled)
			{
				if (completionChar == '.' || completionChar == '(')
				{
					return GetCompletionList (completionContext);
				}
			}
			
			return null;
		}*/
			
		
		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{
			if (mCanRunCompletion && mCompletionEnabled)
			{
				if (completionChar == '.' || completionChar == '(')
				{
					return GetCompletionList (completionContext);
				}
			}
			
			return null;
		}
		
		
		public override ParameterDataProvider HandleParameterCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			//MonoDevelop.Ide.MessageService.ShowError ("HandleParameterCompletion");
			if (mCanRunCompletion)
			{
				if (mCacheIsObject || completionContext.TriggerOffset < mCacheTriggerOffset || completionContext.TriggerLine != mCacheTriggerLine)
				{
					if (parameterDataProvider != null)
					{
						//parameterDataProvider.Clear ();
						parameterDataProvider = null;
					}
				}
				
				if (mCompletionEnabled)
				{
					// HandleCodeCompletion is always called first, so we don't need to fetch completion data
					
					if (completionChar == ')')
					{
						// invalidate cached completion
						//mCacheXML = null;
						mCacheIsObject = true;
						if (parameterDataProvider != null)
						{
							//parameterDataProvider.Clear ();
							parameterDataProvider = null;
						}
						//return null;
					}
					
					if (!mCacheIsObject && mCacheXML != null && completionContext.TriggerLine == mCacheTriggerLine)
					{
						if (parameterDataProvider == null)
						{
							//MonoDevelop.Ide.MessageService.ShowError ("Create Data Provider");
							parameterDataProvider = new HaxeParameterDataProvider (completionContext.TriggerOffset);
							parameterDataProvider.Update (completionContext, mCacheXML);
							return parameterDataProvider;
						}
					}
					else
					{
						if (parameterDataProvider != null)
						{
							//parameterDataProvider.Clear ();
							//parameterDataProvider = null;
						}
					}
				}
			}
			
			return parameterDataProvider;
			//return null;
		}
		
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			if (Document.HasProject && ((Document.Project is OpenFLProject) || (Document.Project is NMEProject) || (Document.Project is HaxeProject)))
			{
				mCanRunCompletion = true;
				
				mTempBaseDirectory = Path.Combine (Path.GetTempPath (), "md" + Guid.NewGuid ().ToString ().Replace ("-", ""));
				mTempDirectory = Path.Combine (mTempBaseDirectory, GetPackage ().Replace (".", "/"));
				Directory.CreateDirectory (mTempDirectory);
				
				mTempFileName = Path.Combine (mTempDirectory, Path.GetFileName (Document.FileName));
			}
		}
		
		
		public override bool KeyPress (Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{
			if (key == Gdk.Key.BackSpace)
			{
				mCompletionEnabled = false;
			}
			else
			{
				mCompletionEnabled = true;
			}
			
			return base.KeyPress (key, keyChar, modifier);
		}
		
	}
	
}
