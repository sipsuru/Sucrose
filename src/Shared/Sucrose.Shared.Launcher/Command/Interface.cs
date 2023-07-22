﻿using SSDECT = Sucrose.Shared.Dependency.Enum.CommandsType;
using SMR = Sucrose.Memory.Readonly;
using SSSHP = Sucrose.Shared.Space.Helper.Processor;
using SSSMI = Sucrose.Shared.Space.Manage.Internal;

namespace Sucrose.Shared.Launcher.Command
{
    internal static class Interface
    {
        public static void Command()
        {
            SSSHP.Run(SSSMI.Commandog, $"{SMR.StartCommand}{SSDECT.Interface}{SMR.ValueSeparator}{SSSMI.Portal}");
        }
    }
}