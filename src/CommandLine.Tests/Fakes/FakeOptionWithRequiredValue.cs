﻿// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    public class FakeOptionWithRequiredValue
    {
        [Value(0, Required = true)]
        public string StringValue { get; set; }

        [Value(1)]
        public int IntValue { get; set; }
    }
}
