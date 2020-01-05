// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace IxMilia.Stl
{
    public class StlReadException : Exception
    {
        public StlReadException()
            : base()
        {
        }

        public StlReadException(string message)
            : base(message)
        {
        }

        public StlReadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
