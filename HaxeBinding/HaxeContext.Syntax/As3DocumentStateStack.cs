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
//
// Parts of this file have been taken from original MonoDevelop source code.
// Author: Mike Krüger <mkrueger@novell.com>
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace FlexBinding.Syntax
{
    public class As3DocumentStateStack : Stack<As3DocumentState>, ICloneable
    {
        public void Push(As3DocumentStateInside inside, int relativeIndent)
        {
            relativeIndent = Math.Max(0, relativeIndent + PeekIndent());
            base.Push(new As3DocumentState(inside, relativeIndent));
        }

        public As3DocumentState PopOrDefault()
        {
            if (Count > 0)
                return Pop();
            else
                return default(As3DocumentState);
        }

        public int PeekIndent()
        {
            if (Count > 0)
                return Peek().Indent;
            else
                return 0;
        }

        public As3DocumentStateInside PeekInside()
        {
            if (Count > 0)
                return Peek().Inside;
            else
                return As3DocumentStateInside.NoState;
        }

        public object Clone()
        {
            As3DocumentStateStack clone = new As3DocumentStateStack();
            foreach (As3DocumentState state in this.Reverse()) {
                Console.WriteLine("STACK: {0} {1}", state.Inside, state.Indent);
                clone.Push(state);
            }
            return clone;
        }
    }
}
