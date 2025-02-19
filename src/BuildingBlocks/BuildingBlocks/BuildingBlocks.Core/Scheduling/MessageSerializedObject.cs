﻿using System.Linq;

namespace BuildingBlocks.Core.Scheduling
{
    public class MessageSerializedObject
    {
        public MessageSerializedObject(string fullTypeName, string data, string additionalDescription,
            string assemblyName = "")
        {
            FullTypeName = fullTypeName;
            Data = data;
            AdditionalDescription = additionalDescription;
            AssemblyName = assemblyName == "" ? "Application.Commands" : assemblyName;
        }

        public string FullTypeName { get; private set; }

        public string Data { get; private set; }

        public string AdditionalDescription { get; private set; }
        public string AssemblyName { get; private set; }

        public override string ToString()
        {
            var commandName = FullTypeName.Split('.').Last();
            return $"{commandName} {AdditionalDescription}";
        }
    }
}