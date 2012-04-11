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
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Output;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.HaxeBinding.Projects;
using MonoDevelop.HaxeBinding.Tools;


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
		
		
		private void FetchCompletionData (CodeCompletionContext completionContext)
		{
			if (completionContext.TriggerOffset != mCacheTriggerOffset)
			{
				mCacheTriggerOffset = completionContext.TriggerOffset;
				File.WriteAllText (mTempFileName, Document.Editor.Text);
				
				string data = HaxeCompilerManager.GetCompletionData (Document.Project, mTempBaseDirectory, mTempFileName, mCacheTriggerOffset);
				
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
				if (mCacheXML.FirstChild.Name == "type")
				{
					mCacheIsObject = false;
				}
				else
				{
					mCacheIsObject = true;
				}
			}
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
						string name;
						string type;
						string icon;
						
						switch (xml.FirstChild.Name)
						{
							case "list":
								
								foreach (XmlElement node in xml.FirstChild.ChildNodes)
								{
									name = node.GetAttribute ("n");
									type = node.InnerText;
									icon = "md-property";
								
									if (type.IndexOf ("->") > -1)
									{
										icon = "md-method";
										//md-literal
									}
								
									if (type.IndexOf ("@private") == -1)
									{
										list.Add (new CompletionData (name, icon, type));
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
		
		
		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			if (mCompletionEnabled)
			{
				if (completionChar == '.' || completionChar == '[' || completionChar == '(')
				{
					return GetCompletionList (completionContext);
				}
			}
			
			return null;
		}
			
		
		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{
			if (mCompletionEnabled)
			{
				if (completionChar == '.' || completionChar == '[' || completionChar == '(')
				{
					return GetCompletionList (completionContext);
				}
			}
			
			return null;
		}
		
		
		public override IParameterDataProvider HandleParameterCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			if (completionContext.TriggerOffset < mCacheTriggerOffset || completionContext.TriggerLine != mCacheTriggerLine)
			{
				if (parameterDataProvider != null)
				{
					parameterDataProvider.Clear ();
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
						parameterDataProvider.Clear ();
						parameterDataProvider = null;
					}
					return null;
				}
				
				if (!mCacheIsObject && mCacheXML != null && completionContext.TriggerLine == mCacheTriggerLine)
				{
					if (parameterDataProvider == null)
					{
						parameterDataProvider = new HaxeParameterDataProvider ();
						parameterDataProvider.Update (completionContext, mCacheXML);
						return parameterDataProvider;
					}
				}
				else
				{
					if (parameterDataProvider != null)
					{
						parameterDataProvider.Clear ();
						parameterDataProvider = null;
					}
				}
			}
			
			return null;
		}
		
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			if (Document.HasProject && ((Document.Project is NMEProject) || (Document.Project is HaxeProject)))
			{
				mCanRunCompletion = true;
				
				mTempBaseDirectory = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
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
