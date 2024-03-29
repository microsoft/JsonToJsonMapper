﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonToJsonMapper;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

namespace JsonToJsonMapper
{
    class RoslynScriptHandler : ITransformationHandler
    {
        public Dictionary<string, Script> Scripts { get; set; }
        public RoslynScriptHandler(Dictionary<string, Script> scripts)
        {
            Scripts = scripts;
        }

        /// <summary>
        /// Executes the script using Roslyn and returns the value.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public dynamic Run(JObject transform, JObject input)
        {
            StringBuilder inputParam = new StringBuilder();
            List<string> parameters = transform["Params"].ToObject<List<string>>();
            string scriptName = transform["ScriptName"].Value<string>();

            foreach (var item in parameters)
            {
                if (item.StartsWith("$"))
                {
                    if (input.SelectTokens(item) != null)
                    {
                        var tokens = input.SelectTokens(item);
                        foreach (var i in tokens)
                        {
                            inputParam.Append(i.ToString());
                            if (tokens.Count() > 1)
                                inputParam.Append("[tokenDelimiter]");
                        }
                    }
                    if (parameters.Count > 1)
                        inputParam.Append(string.Empty + "[delimiter]");
                }
                else
                    inputParam.Append(item + "[delimiter]");
            }

            var result = Scripts[scriptName].RunAsync(new ScriptHost { Args = inputParam.ToString() });
            return result.Result.ReturnValue.ToString();
        }
    }
}
