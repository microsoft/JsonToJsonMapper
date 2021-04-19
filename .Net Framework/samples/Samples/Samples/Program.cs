using JsonToJsonMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Samples
{
	class Program
	{
		static void Main(string[] args)
		{
			string mappingJson =
			"{\"MappingRuleConfig\":{\"TruthTable\":[{\"DestinationColumn\":\"ContentJSON\",\"ComplexType\":{\"DataType\":\"JArray\",\"Node\":\"$.result[*]\",\"TruthTable\":[{\"SourceColumn\":\"$.leadId\",\"DestinationColumn\":\"leadId\",\"DataType\":\"long\"},{\"SourceColumn\":\"$.attributes[*]\",\"DataType\":\"JArray\",\"TransformValue\":{\"Type\":\"PromoteArrayToProperty\",\"KeyLookupField\":\"$.name\",\"ValueLookupField\":\"$.value\"}}]}}]}}";


			string inputJson =
				"{\"result\":[{\"id\":2,\"leadId\":6,\"activityDate\":\"2013-09-26T06:56:35+0000\",\"activityTypeId\":12,\"attributes\":[{\"name\":\"Source Type\",\"value\":\"Web page visit\"},{\"name\":\"Source Info\",\"value\":\"http://search.yahoo.com\"}]}]}";
			AutoMapper autoMapper = new AutoMapper(mappingJson);

			var outputJson = autoMapper.TransformIntoJson(inputJson, true);
			Console.WriteLine("Input" + inputJson);
			Console.WriteLine("\n");
			Console.WriteLine("Output" + outputJson);
			Console.ReadLine();
		}
	}
}
