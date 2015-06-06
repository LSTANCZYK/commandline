﻿namespace CommandLine.FSharp.Tests

open CommandLine
open CommandLine.FSharp

type public FakeOptions() = class
    let mutable stringValue = ""
    let mutable intSequence = Seq.empty<int>
    let mutable boolValue = false
    let mutable longValue = 0L

    [<Option(HelpText = "Define a string value here.")>]
    member this.StringValue with get() = stringValue and set(value) = stringValue <- value

    [<Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.")>]
    member this.IntSequence with get() = intSequence and set(value) = intSequence <- value

    [<Option('x', HelpText = "Define a boolean or switch value here.")>]
    member this.BoolValue with get() = boolValue and set(value) = boolValue <- value

    [<Value(0)>]
    member this.LongValue with get() = longValue and set(value) = longValue <- value
end