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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MonoDevelop.Core;
using MonoDevelop.Core.Collections;
using MonoDevelop.Projects;
using MonoDevelop.Projects.Dom;
using MonoDevelop.Projects.Dom.Output;
using MonoDevelop.Projects.Dom.Parser;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.CodeTemplates;

namespace FlexBinding.Syntax
{
    public partial class As3DocumentStateEngine : ICloneable, IDocumentStateEngine
    {

        public As3DocumentStateEngine()
        {
            mStack = new As3DocumentStateStack();
            mBuffer = new StringBuilder();
            Reset();
        }

        int mPosition;
        int mLineNumber;
        int mLineOffset;
        bool mNeedsReindent;

        As3DocumentStateStack mStack;
        StringBuilder mBuffer;

        public object Clone()
        {
            As3DocumentStateEngine clone = (As3DocumentStateEngine)this.MemberwiseClone();
            clone.mStack = (As3DocumentStateStack)mStack.Clone();
            clone.mBuffer = new StringBuilder(mBuffer.ToString(), mBuffer.Capacity);
            clone.mPosition = mPosition;
            clone.mLineNumber = mLineNumber;
            clone.mLineOffset = mLineOffset;
            clone.mNeedsReindent = mNeedsReindent;

            return clone;
        }

        /* IDocumentStateEngine implementation */

        public int Position {
            get { return mPosition; }
        }

        public void Reset()
        {
            mStack.Clear();
            mBuffer.Length = 0;
            mPosition = 0;
            mLineNumber = 1;
            mLineOffset = 0;
            mNeedsReindent = false;
        }

        public void Push (char c)
        {
            mPosition += 1;
            mLineOffset += 1;
            mNeedsReindent = false;

            As3DocumentStateInside inside = mStack.PeekInside();

            switch (c)
            {
                case '\n':
                    PushNewLine(inside);
                    break;

                case '{':
                    PushOpenBrace(inside);
                    break;

                case '}':
                    PushCloseBrace(inside);
                    break;

                default:
                    mBuffer.Append(c);
                    break;
            }

            //Console.WriteLine ("STATE: push: {0} {1} {2}", c, mStack.PeekInside(), mStack.PeekIndent());
        }

        void PushOpenBrace(As3DocumentStateInside inside)
        {
            mStack.Push(As3DocumentStateInside.Brace, 1);
        }

        void PushCloseBrace(As3DocumentStateInside inside)
        {
            mStack.PopOrDefault();
            mNeedsReindent = true;
        }

        void PushNewLine(As3DocumentStateInside inside)
        {
            mLineNumber += 1;
            mLineOffset = 0;
        }

        /* Useful properties and methods used by editor extensions */

        public bool NeedsReindent {
            get { return mNeedsReindent; }
        }

        public int LineNumber {
            get { return mLineNumber; }
        }

        public int LineOffset {
            get { return mLineOffset; }
        }

        public int Indent {
            get { return mStack.PeekIndent(); }
        }

    }
}
