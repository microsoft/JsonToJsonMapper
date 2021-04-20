using JsonToJsonMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Samples
{
	class Program
	{
		static void Main(string[] args)
		{
			Complex2ComplexTransformationSample();
			//Complex2FlatTransformationSample();
			//RoslynSample();
			//TransformToUpperCaseParent();
			//TransformToLowerCase();
			//OneToOneMappingTest();
			RangeMappingTest();
		}

		public static void Complex2ComplexTransformationSample()
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

		public static void Complex2FlatTransformationSample()
		{
			//Arrange - Certain input json with complex object
			string inputJson = "{\"dateCreated\": \"2015-08-31T14:30:00\",       \"accountCode\": \"Cross-Product\",       \"isActive\": true,       \"isHotel\": false,       \"isPreferred\": false,       \"brandFrnId\": null,       \"chainFrnId\": null,       \"locationType\": \"Venue\",       \"largestSpace\": null,       \"numberOfMeetRooms\": null,       \"numberOfRooms\": null,       \"totalSpace\": null,       \"address\": {         \"line1\": \"750 Main Street\",         \"line2\": null,         \"line3\": null,         \"line4\": null,         \"city\": \"Moncton\",         \"state\": \"New Brunswick\",         \"postalCode\": \"E1C 1E6\",         \"country\": \"Canada\",         \"intlState\": null       },       \"locationThirdParty\": null,       \"locationCode\": \"msft_can_delta_monc\",       \"desc\": \"\",       \"directions\": \"\",       \"email\": \"\",       \"externalFrnKey\": null,       \"fax\": \"\",       \"imgAttributes\": \"\",       \"imgSrc\": null,       \"label\": \"\",       \"name\": \"Delta Beausejour Hotel\",       \"notes\": \"\",       \"organization\": null,       \"phone\": \"\",       \"tollfree\": \"\",       \"url\": \"http://binged.it/1O4l53S\"     } ";
			string mapping = "{\"MappingRuleConfig\":{\"DestinationType\":\"\",\"SourceObject\":\"\",\"DestinationObject\":\"\",\"TruthTable\":[{\"SourceColumn\":\"$.address['city']\",\"DestinationColumn\":\"City\",\"DataType\":\"string\",\"ComplexType\":null,\"TransformValue\":null},{\"SourceColumn\":\"$.address['state']\",\"DestinationColumn\":\"State\",\"DataType\":\"string\",\"ComplexType\":null,\"TransformValue\":null},{\"SourceColumn\":\"$.address['postalCode']\",\"DestinationColumn\":\"PostalCode\",\"DataType\":\"string\",\"ComplexType\":null,\"TransformValue\":null},{\"SourceColumn\":\"$.address['country']\",\"DestinationColumn\":\"Country\",\"DataType\":\"string\",\"ComplexType\":null,\"TransformValue\":null}]}}";

			var mapper = new AutoMapper(mapping);
			var flatObject = JObject.Parse(mapper.TransformIntoJson(inputJson, false));
			Console.WriteLine("Input" + inputJson);
			Console.WriteLine("\n");
			Console.WriteLine("Output" + flatObject);
			Console.ReadLine();

		}

		public static void RoslynSample()
		{
			//Arrange - Certain input json with complex object
			string inputJson = "{\r\n  \"eventCode\": \"APAC-1PWBNR3-0331-16-HQ\",\r\n  \"pkEventId\": \"0x449740001\",\r\n  \"eventId\": \"2353\",\r\n  \"externalKey\": \"AP-Azure-WBNR-FY16-03Mar-31-Win10_Device_Management\",\r\n  \"accountCode\": \"C-and-E\",\r\n  \"eventName\": \"Windows 10 for device management\",\r\n  \"url\": null,\r\n  \"autoCalcCode\": false,\r\n  \"isTestMode\": false,\r\n  \"isActive\": true,\r\n  \"isTemplate\": false,\r\n  \"isLocked\": true,\r\n  \"isClassic\": true,\r\n  \"eventStatus\": \"Live\",\r\n  \"isPending\": false,\r\n  \"isDesign\": false,\r\n  \"isTesting\": false,\r\n  \"isOnsite\": false,\r\n  \"isOffline\": false,\r\n  \"isClosed\": false,\r\n  \"isNotificationEnabled\": false,\r\n  \"notes\": \"\",\r\n  \"contact\": {\r\n    \"contactName\": \"Microsoft SQL Team\",\r\n    \"organization\": \"\",\r\n    \"email\": \"b-dikurn@microsoft.com\",\r\n    \"phone\": \"\",\r\n    \"tollfree\": \"\",\r\n    \"fax\": \"\",\r\n    \"url\": \"\",\r\n    \"notes\": \"\"\r\n  },\r\n  \"location\": {\r\n    \"locationName\": \"Webinar\",\r\n    \"locationCode\": \"Webinar\",\r\n    \"email\": null,\r\n    \"phone\": null,\r\n    \"tollfree\": null,\r\n    \"fax\": null,\r\n    \"url\": null,\r\n    \"notes\": null\r\n  },\r\n  \"dateCreated\": \"2016-02-17T15:59:42\",\r\n  \"dateModified\": \"2016-02-19T14:21:21\",\r\n  \"startDate\": \"2016-03-31T08:00:00\",\r\n  \"endDate\": \"2016-03-31T16:00:00\",\r\n  \"dateClosed\": null,\r\n  \"txtEvtCreatedBy\": \"Samuel Pak\",\r\n  \"txtEvtModifiedBy\": \"Maha Pasha\",\r\n  \"glNumber\": \"\",\r\n  \"timezone\": \"(GMT+05:00) Islamabad, Karachi, Tashkent\",\r\n  \"timezoneMapping\": \"Asia/Karachi\",\r\n  \"timezoneId\": 85,\r\n  \"proposedLocation\": null,\r\n  \"plannedAttendance\": null,\r\n  \"proposedBudget\": null,\r\n  \"plannedCurrency\": null,\r\n  \"travelAccoCurrency\": null,\r\n  \"eventQuestions\": {\r\n    \"question\": [\r\n      {\r\n        \"questionField\": \"evt_ans_field19\",\r\n        \"questionId\": 638,\r\n        \"questionCode\": \"Design-LPHeroImg\",\r\n        \"questionName\": \"Design-LPHeroImg\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"https://info.microsoft.com/rs/157-GQE-382/images/ms-win10-webinar-banners-3000x300-04.png\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field24\",\r\n        \"questionId\": 996,\r\n        \"questionCode\": \"Design-EmailBannerImg\",\r\n        \"questionName\": \"Design-EmailBannerImg \",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field16\",\r\n        \"questionId\": 398,\r\n        \"questionCode\": \"Program-BannerText\",\r\n        \"questionName\": \"Program-BannerText\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"Microsft Event Banner Title\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field20\",\r\n        \"questionId\": 639,\r\n        \"questionCode\": \"Design-BannerTextColor\",\r\n        \"questionName\": \"Design-BannerTextColor\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"#ffffff\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field17\",\r\n        \"questionId\": 633,\r\n        \"questionCode\": \"Design-HeaderTextColor\",\r\n        \"questionName\": \"Design-HeaderTextColor\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"#333333\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field06\",\r\n        \"questionId\": 152,\r\n        \"questionCode\": \"Program-HeaderText\",\r\n        \"questionName\": \"Program-HeaderText\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"Windows 10 for device management\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field07\",\r\n        \"questionId\": 153,\r\n        \"questionCode\": \"Program-Description\",\r\n        \"questionName\": \"Program-Description\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"A\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field09\",\r\n        \"questionId\": 229,\r\n        \"questionCode\": \"Program-AdditionalInfo\",\r\n        \"questionName\": \"Program-AdditionalInfo\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"B\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field21\",\r\n        \"questionId\": 640,\r\n        \"questionCode\": \"Design-HighlightColor\",\r\n        \"questionName\": \"Design-HighlightColor\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"#ffffff\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field22\",\r\n        \"questionId\": 641,\r\n        \"questionCode\": \"Design-HighlightTextColor\",\r\n        \"questionName\": \"Design-HighlightTextColor\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"#ffffff\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field01\",\r\n        \"questionId\": 147,\r\n        \"questionCode\": \"Hero Banner Image\",\r\n        \"questionName\": \"Hero Banner Image\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"EN-BASICE-Banner-PeopleWithDevices.png\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field02\",\r\n        \"questionId\": 37143,\r\n        \"questionCode\": \"Modern Tele-Readiness Hotsheet URL\",\r\n        \"questionName\": \"Modern Tele-Readiness Hotsheet URL\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field30\",\r\n        \"questionId\": 649,\r\n        \"questionCode\": \"DELETED-Program-Description\",\r\n        \"questionName\": \"DELETED-Program-Description\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"\"\r\n            }\r\n          ]\r\n        }\r\n      }\r\n    ]\r\n  },\r\n  \"groups\": null,\r\n  \"rotations\": null,\r\n  \"forms\": null,\r\n  \"websites\": null,\r\n  \"primaryFormURL\": \"/profile/form/index.cfm?PKformID=0x1084150001\",\r\n  \"questionAssignments\": null\r\n}";
			string mapping = "{\r\n  \"Scripts\": [\r\n    {\r\n      \"Name\": \"GetTitle\",\r\n      \"Code\": \"string GetTitle(string args){string[] input = args.Split(new string[] {\\\"[delimiter]\\\"}, System.StringSplitOptions.None);string[] data = null; var title = string.Empty;for (int i = 0; i < input.Length; i++){if (!string.IsNullOrEmpty(input[i]) && !string.IsNullOrWhiteSpace(input[i]) && !input[i].Contains(\\\"tokenDelimiter\\\")){title = input[i];break;}if (input[i].Contains(\\\"tokenDelimiter\\\") == true){data = input[i].Split(new string[] { \\\"[tokenDelimiter]\\\" }, System.StringSplitOptions.None);for (int x = 0; x < data.Length; x++){if (!string.IsNullOrEmpty(data[x]) && !string.IsNullOrWhiteSpace(data[x])){title = data[x].ToString();break;}}}else{continue;}} return title;}GetTitle(Args)\"\r\n    },\r\n    {\r\n      \"Name\": \"GetDescription\",\r\n      \"Code\": \"string GetEventDesc(string args) { string[] input = args.Split(new string[] { \\\"[delimiter]\\\" }, System.StringSplitOptions.None); string[] data = null; var eventDesc = string.Empty; for (int i = 0; i < input.Length; i++) { if (!string.IsNullOrEmpty(input[i]) && !string.IsNullOrWhiteSpace(input[i]) && !input[i].Contains(\\\"tokenDelimiter\\\")) { eventDesc = input[i]; eventDesc = System.Text.RegularExpressions.Regex.Replace(input[i], @\\\"<(.|\\n)*?>\\\", string.Empty); break; } if (input[i].Contains(\\\"tokenDelimiter\\\") == true) { data = input[i].Split(new string[] {\\\"[tokenDelimiter]\\\" }, System.StringSplitOptions.None); for (int x = 0; x < data.Length; x++) { if (!string.IsNullOrEmpty(data[x]) && !string.IsNullOrWhiteSpace(data[x])) { eventDesc = data[x].ToString(); eventDesc = System.Text.RegularExpressions.Regex.Replace(data[x], @\\\"<(.|\\n)*?>\\\", string.Empty); break;} } } else { continue; }} return eventDesc;}GetEventDesc(Args)\",\r\n      \"Reference\": {\r\n        \"Assembly\": \"System\",\r\n        \"NameSpace\": \"System.Text.RegularExpressions\"\r\n      }\r\n    },\r\n    {\r\n      \"Name\": \"GetURL\",\r\n      \"Code\": \"string GetURL(string args) { string[] input = args.Split(new string[] { \\\"[delimiter]\\\" }, System.StringSplitOptions.None); string[] data = null; var eventURL = string.Empty;for (int i = 0; i < input.Length; i++) { if (!string.IsNullOrEmpty(input[i]) && !string.IsNullOrWhiteSpace(input[i]) && !input[i].Contains(\\\"tokenDelimiter\\\")) { eventURL = input[i]; break; } if (input[i].Contains(\\\"tokenDelimiter\\\") == true) { data = input[i].Split(new string[] { \\\"[tokenDelimiter]\\\" }, System.StringSplitOptions.None); for (int x = 0; x < data.Length; x++) { if (!string.IsNullOrEmpty(data[x]) && !string.IsNullOrWhiteSpace(data[x])) { eventURL = data[x].ToString(); break; } } } else { continue; } } if (string.IsNullOrEmpty(eventURL)) return eventURL = \\\"www.microsoft.com\\\"; else return eventURL; }GetURL(Args)\"\r\n    },\r\n    {\r\n      \"Name\": \"GetJsonArray\",\r\n      \"Code\": \"string GetJsonArray(string args) { System.Text.StringBuilder sb = new System.Text.StringBuilder(); sb.Append(\\\"[\\\"); foreach (var item in args.Split(',')) { sb.Append(@\\\"\\\"\\\"\\\" + item + @\\\"\\\"\\\",\\\"); } sb.Remove(sb.Length - 1, 1); sb.Append(\\\"]\\\"); return sb.ToString();}GetJsonArray(Args)\"\r\n    }\r\n  ],\r\n  \"MappingRuleConfig\": {\r\n    \"DestinationType\": \"EventInformation\",\r\n    \"SourceObject\": \"CertainEvents\",\r\n    \"TruthTable\": [\r\n      {\r\n        \"SourceColumn\": \"\",\r\n        \"DestinationColumn\": \"EventList\",\r\n        \"DataType\": \"jarray\",\r\n        \"ComplexType\": {\r\n          \"TruthTable\": [\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"title\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetTitle\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Program-HeaderText')].answers.answer.[0].value\",\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Program-BannerText')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"accountCode\",\r\n              \"DestinationColumn\": \"accountCode\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"eventCode\",\r\n              \"DestinationColumn\": \"Id\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"eventStatus\",\r\n              \"DestinationColumn\": \"Status\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"timezone\",\r\n              \"DestinationColumn\": \"timezone\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"source\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"DefaultValue\": \"Certain\"\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"EventCategory\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"DefaultValue\": \"Onsite Event\"\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"startDate\",\r\n              \"DestinationColumn\": \"StartDate\",\r\n              \"DataType\": \"formattedDatetime\",\r\n              \"Format\": \"yyyy-MM-ddTHH:mm:ssZ\",\r\n              \"TransformValue\": { \"DefaultValue\": \"utcNow\" }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"endDate\",\r\n              \"DestinationColumn\": \"EndDate\",\r\n              \"DataType\": \"formattedDatetime\",\r\n              \"Format\": \"yyyy-MM-ddTHH:mm:ssZ\",\r\n              \"TransformValue\": { \"DefaultValue\": \"utcNow\" }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"$.location['locationName']\",\r\n              \"DestinationColumn\": \"locationCode\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"Description\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetDescription\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Search-EventAbstract')].answers.answer.[0].value\",\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Program-Description')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"PrimaryLanguage\",\r\n              \"DataType\": \"jarray\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetJsonArray\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Localization-Form-and-Email-Language')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"PrimaryTargetAudience\",\r\n              \"DataType\": \"jarray\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetJsonArray\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Search-EventsFor')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"Product\",\r\n              \"DataType\": \"jarray\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetJsonArray\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Search-EventProduct')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"EventSource\",\r\n              \"DestinationColumn\": \"EventSource\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"$.eventQuestions.question[?(@.questionCode == 'Search-EventIcon')].answers.answer.[0].value\",\r\n              \"DestinationColumn\": \"EventIcon\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"Category\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"DefaultValue\": \"TBD\"\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"URL\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetURL\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Search-AlternateRegURL')].answers.answer.[0].value\",\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'primaryFormURL')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"SourceColumn\": \"\",\r\n        \"DestinationColumn\": \"RequestId\",\r\n        \"DataType\": \"guid\",\r\n        \"TransformValue\": {\r\n          \"DefaultValue\": \"4bfaac9c-1e0d-4620-b9e6-66095376e99a\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}";

			var mapper = new AutoMapper(mapping);

			var response = mapper.TransformIntoJson(inputJson, true);
			var OutputJson = @"{""EventList"":[{""title"":""Windows 10 for device management"",""accountCode"":""C-and-E"",""Id"":""APAC-1PWBNR3-0331-16-HQ"",""Status"":""Live"",""timezone"":""(GMT+05:00) Islamabad, Karachi, Tashkent"",""source"":""Certain"",""EventCategory"":""Onsite Event"",""StartDate"":""2016-03-31T08:00:00Z"",""EndDate"":""2016-03-31T16:00:00Z"",""locationCode"":""Webinar"",""Description"":""A"",""PrimaryLanguage"":[""""],""PrimaryTargetAudience"":[""""],""Product"":[""""],""Category"":""TBD"",""URL"":""www.microsoft.com""}],""RequestId"":""4bfaac9c-1e0d-4620-b9e6-66095376e99a""}";
			var value = response.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);

			if (value.Equals(OutputJson.Replace(" ", string.Empty)))
			{
				Console.WriteLine("Input" + inputJson);
				Console.WriteLine("\n");
				Console.WriteLine("Output" + OutputJson);
				Console.ReadLine();
			}
			else
			{
				Console.WriteLine("Sample Imput not working");
				Console.ReadLine();
			}
		}

		public static void TransformToUpperCaseParent()
		{
			string mapping;
			using (StreamReader r = new StreamReader(@"Jsons\ToUpper\Upper_Mapping.json"))
			{
				mapping = (JToken.ReadFrom(new JsonTextReader(r))).ToString();
			}

			string inputJson;
			using (StreamReader r = new StreamReader(@"Jsons\ToUpper\Transformation_Input.json"))
			{
				inputJson = (JToken.ReadFrom(new JsonTextReader(r))).ToString();
			}

			var mapper = new AutoMapper(mapping);

			//Act
			var response = mapper.TransformIntoJson(inputJson, true);

			Console.WriteLine("Input" + inputJson);
			Console.WriteLine("\n");
			Console.WriteLine("Output" + response);
			Console.ReadLine();

		}

		public static void TransformToLowerCase()
		{
			string inputJson = @"{""LeadId"":""2353"",""FirstName"":""AP-Azure"",""LastName"":""C-and-E"",""EmailAddress"":""Windows 10"",""Key"":""12121212333_AKJKKAJDKAKJKKKDKSKAK""}";
			string mapping = @"{""MappingRuleConfig"":{""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""Data"",""DataType"":""string"",""ComplexType"":{""DataType"":""jArray"",""Node"":""$"",""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""EmailAddress"",""DataType"":""string"",""TransformValue"":{""Type"":""function"",""Function"":""TOLOWERCASE"",""Params"":[""$.EmailAddress""]}}]}}]}}";

			var mapper = new AutoMapper(mapping);

			var response = mapper.TransformIntoJson(inputJson, true);
			Console.WriteLine("Input" + inputJson);
			Console.WriteLine("\n");
			Console.WriteLine("Output" + response);
			Console.ReadLine();
		
	}

		public static void OneToOneMappingTest()
		{
			//Arrange
			string mappingJson;
			using (StreamReader r = new StreamReader(@"Jsons\OneToOneMapping\OneToOneMappingRules.json"))
			{
				mappingJson = (JToken.ReadFrom(new JsonTextReader(r))).ToString();
			}

			string input;
			using (StreamReader r = new StreamReader(@"Jsons\OneToOneMapping\Transformation_Input.json"))
			{
				input = (JToken.ReadFrom(new JsonTextReader(r))).ToString();
			}

			//Act            
			var mapper = new AutoMapper(mappingJson);
			var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(input), true);

			if("Financial Services".Equals(output.SelectToken("$.Industry").ToString()))
					{
				Console.WriteLine("Financial Services: " + output.SelectToken("$.Industry").ToString());
				Console.ReadLine();
			}

		}

		public static void RangeMappingTest()
		{
			//Arrange
			string mappingJson;
			using (StreamReader r = new StreamReader(@"Jsons\RangeMapping\RangeMappingRules.json"))
			{
				mappingJson = (JToken.ReadFrom(new JsonTextReader(r))).ToString();
			}

			string input;
			using (StreamReader r = new StreamReader(@"Jsons\RangeMapping\Transformation_Input.json"))
			{
				input = (JToken.ReadFrom(new JsonTextReader(r))).ToString();
			}

			//Act            
			var mapper = new AutoMapper(mappingJson);
			var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(input), true);

			if ("51to250employees".Equals(output.SelectToken("$.CompanySize").ToString()))
			{
				Console.WriteLine("CompanySize: " + output.SelectToken("$.CompanySize").ToString());
				Console.ReadLine();
			}
		}
	}
}
