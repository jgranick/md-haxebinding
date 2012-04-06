// This file is part of the MonoDevelop Flex Language Binding.
//
// Copyright (c) 2009 Studio Associato Di Nunzio e Di Gregorio
//
//  Authors:
//     Federico Di Gregorio <fog@initd.org>
//
// This source code is licenced under The MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using MonoDevelop.Core;
using MonoDevelop.Core.Collections;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Output;
using MonoDevelop.Projects.Dom.Parser;
//using MonoDevelop.Projects.Gui.Completion;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.CodeTemplates;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.HaxeBinding.Tools;
using MonoDevelop.HaxeBinding.Projects;

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
		
		
        public override void Initialize()
        {
			base.Initialize();
			
			if (Document.HasProject && Document.Project is NMEProject) {
				
				mCanRunCompletion = true;
				
				mTempBaseDirectory = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());
				mTempDirectory = Path.Combine (mTempBaseDirectory, GetPackage ().Replace (".", "/"));
				Directory.CreateDirectory (mTempDirectory);
				
				mTempFileName = Path.Combine (mTempDirectory, Path.GetFileName (Document.FileName));
				
			}
		}
		
		
		private string GetPackage () {
			
			int packageIndex = Document.Editor.Text.IndexOf ("package");
			
			if (packageIndex > -1) {
				
				int start = packageIndex + "package".Length;
				return Document.Editor.Text.Substring (start, Document.Editor.Text.IndexOf (';', start) - start).Trim ();
				
			}
			
			return "";
			
		}
		
		
		public override bool CanRunCompletionCommand ()
		{
			return mCanRunCompletion;
		}
		
		
		private void FetchCompletionData (CodeCompletionContext completionContext) {
			
			if (completionContext.TriggerOffset != mCacheTriggerOffset) {
				
				mCacheTriggerOffset = completionContext.TriggerOffset;
				
				File.WriteAllText (mTempFileName, Document.Editor.Text);
				
				string data = NMECommandLineToolsManager.GetCompletionData ((NMEProject)Document.Project, mTempDirectory, mTempFileName, mCacheTriggerOffset);
				
				try {
					
					var xml = new XmlDocument ();
					xml.LoadXml (data);
					
					if (xml.HasChildNodes) {
						
						if (xml.FirstChild.Name == "type") {
							
							mCacheIsObject = false;
							
						} else {
							
							mCacheIsObject = true;
							
						}
						
						mCacheXML = xml;
						mCacheXMLCurrent = true;
						mCacheTriggerLine = completionContext.TriggerLine;
						
					}
					
					return;
					
				} catch (Exception ex) {
				}
				
				mCacheXMLCurrent = false;
				//mCacheXML = null;
				return;
				
			}
			
		}
		
		
		private CompletionDataList GetCompletionList (CodeCompletionContext completionContext)
		{
			
			FetchCompletionData (completionContext);
			
			if (mCacheXML != null && mCacheXMLCurrent && mCacheIsObject) {
				
				try {
					
					XmlDocument xml = mCacheXML;
					
					CompletionDataList list = new CompletionDataList ();
					
					if (xml.HasChildNodes && xml.FirstChild.HasChildNodes) {
						
						string name;
						string type;
						string icon;
						
						switch (xml.FirstChild.Name) {
								
						case "list":
							
							foreach (XmlElement node in xml.FirstChild.ChildNodes) {
								
								name = node.GetAttribute("n");
								type = node.InnerText;
								icon = "md-property";
								
								if (type.IndexOf ("->") > -1) {
									
									icon = "md-method";
									//md-literal
									
								}
								
								if (type.IndexOf ("* @private") == -1) {
									
									list.Add (new CompletionData (name, icon, type));
									
								}
								
							}
							
							break;
							
						}
								
					}
					
					return list;
						
				} catch (Exception ex) {
				}
				
			}
			
			return null;
			
		}
		
		
		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			if (mCompletionEnabled && completionContext.TriggerOffset != mCacheTriggerOffset) {
				
				return GetCompletionList (completionContext);
				
			}
			
			return null;
		}
			
		
		public override ICompletionDataList HandleCodeCompletion (CodeCompletionContext completionContext, char completionChar, ref int triggerWordLength)
		{
			if (mCompletionEnabled && completionContext.TriggerOffset != mCacheTriggerOffset) {
				
				return GetCompletionList (completionContext);
				
			}
			
			return null;
		}
		
		
		public override IParameterDataProvider HandleParameterCompletion (CodeCompletionContext completionContext, char completionChar)
		{
			if (mCompletionEnabled) {
				
				// HandleCodeCompletion is always called first, so we don't need to fetch completion data
				
				if (completionChar == ')' || completionContext.TriggerLine != mCacheTriggerLine) {
					
					// invalidate cached completion
					
					mCacheXML = null;
					
				}
				
				if (!mCacheIsObject && mCacheXML != null) {
					
					if (parameterDataProvider == null) {
						
						parameterDataProvider = new HaxeParameterDataProvider ();
						parameterDataProvider.Update (completionContext, mCacheXML);
						return parameterDataProvider;
						
					}
					
				} else {
					
					if (parameterDataProvider != null) {
						
						parameterDataProvider.Clear ();
						parameterDataProvider = null;
						
					}
					
				}
					
			}
			
			return null;
		}
		
		
		public override void Dispose ()
		{
			try { Directory.Delete (mTempBaseDirectory, true); }
			catch {}
			base.Dispose ();
		}
		
		
		public override bool KeyPress (Gdk.Key key, char keyChar, Gdk.ModifierType modifier)
		{
			if (key == Gdk.Key.BackSpace) {
				
				mCompletionEnabled = false;
				
			} else {
				
				mCompletionEnabled = true;
				
			}
			
			return base.KeyPress (key, keyChar, modifier);
		}
		
    }
}
