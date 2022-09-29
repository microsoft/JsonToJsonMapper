using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JsonToJsonMapper;
using System.IO;
using Newtonsoft.Json;

namespace MipUnitTest
{
  [TestClass]
  public class AutoMapperTest
  {
    //Test
    [TestMethod, Description("Validate that the exception is thrown by auto mapper if mapping json is invalid")]
    public void Validate_AutoMapper_ExceptionThrownByInvalidMappingJson()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MAPUnitTest.LeadRecord, MAPUnitTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable1\": [{\"SourceColumn\": \"Name\",\"DestinationColumn\": \"Name\",\"DataType\": \"string\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": \"int\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": \"bool\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\": \"Guid\",\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson = "{\"name\": \"Rohit\",\"allowEmail\": false,\"SalesId\": null,\"Leadscore\":100}";

      //Act            
      try
      {
        var mapper = new AutoMapper(mappingJson);
        var lead = mapper.TransformIntoJson(inputJson, true);
        Assert.Fail();
      }
      catch (Exception ex)
      {
        //Assert
        Assert.IsTrue(string.Equals(ex.Message, string.Format("Invalid mapping json")));
      }
    }

    [TestMethod, Description("Validate that the property is not set if value datatype is different that the datatype mentioned in the config.")]
    public void Validate_AutoMapper_InvalidDatatype()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MAPUnitTest.LeadRecord, MAPUnitTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable1\": [{\"SourceColumn\": \"Name\",\"DestinationColumn\": \"Name\",\"DataType\": \"string\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": \"int\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": \"bool\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\": \"Guid\",\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson =
        "{\"name\": \"Rohit\",\"allowEmail\": false,\"SalesId\": null,\"Leadscore\":100a}";

      try
      {
        //Act
        var mapper = new AutoMapper(mappingJson);
        var lead = (LeadRecord)mapper.Transform(inputJson);
        Assert.Fail();
      }
      catch (Exception ex)
      {
        //Assert
        Assert.AreEqual(ex.Message, "Invalid mapping json");
      }
    }

    [TestMethod, Description("Validate that the property is not set if the value is null")]
    public void Validate_AutoMapper_NullValue()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MipUnitTest.LeadRecord, JsonToJsonMapper.Core.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable\": [{\"SourceColumn\": \"Name\",\"DestinationColumn\": \"Name\",\"DataType\": \"string\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": \"int\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": \"bool\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\": \"Guid\",\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson =
        "{\"name\": null,\"allowEmail\": false,\"SalesId\": null,\"Leadscore\":100}";

      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var lead = (LeadRecord)mapper.Transform(inputJson);
        Assert.IsNull(lead.Name);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod, Description("Validate that the property is not set if the value is empty string except if the datatype is string.")]
    public void Validate_AutoMapper_EmptyString()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MipUnitTest.LeadRecord, JsonToJsonMapper.Core.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable\": [{\"SourceColumn\": \"Name\",\"DestinationColumn\": \"Name\",\"DataType\": \"string\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": \"int\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": \"bool\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\": \"Guid\",\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson =
        "{\"name\": \"\",\"allowEmail\": false,\"SalesId\": null,\"Leadscore\":\"101\"}";

      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var lead = (LeadRecord)mapper.Transform(inputJson);
        Assert.IsTrue(lead.AllowEmail == false);
        Assert.IsTrue(lead.LeadScore == 101);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod, Description("Validate that the datetime value is not modified due to transformation.")]
    public void Validate_AutoMapper_DateTimeValidation()
    {
      // Arrange
      string mappingJson =
        @"{  ""MappingRuleConfig"": {    ""TruthTable"": [      {        ""SourceColumn"": """",        ""DestinationColumn"": ""EventId"",        ""DataType"": ""string"",        ""ComplexType"": null,        ""TransformValue"": {          ""DefaultValue"": ""-1""        }      },      {        ""SourceColumn"": ""$.MifContext.correlationId"",        ""DestinationColumn"": ""Token"",        ""DataType"": ""string"",        ""ComplexType"": null,        ""TransformValue"": null      },      {        ""SourceColumn"": """",        ""DestinationColumn"": ""SchemaName"",        ""DataType"": ""string"",        ""ComplexType"": null,        ""TransformValue"": {          ""DefaultValue"": ""JournalEvents.EventCore.SessionScanData""        }      },      {        ""SourceColumn"": """",        ""DestinationColumn"": ""Content"",        ""DataType"": ""string"",        ""ComplexType"": null,        ""TransformValue"": null      },      {        ""SourceColumn"": """",        ""DestinationColumn"": ""ContentObject"",        ""DataType"": ""string"",        ""ComplexType"": {          ""DataType"": ""JArray"",          ""Node"": ""$.GetReportResponse[*].GetReportResult[*].diffgr:diffgram[*].DocumentElement[*].['Session Scan Data']"",          ""TruthTable"": [            {              ""SourceColumn"": ""$.[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].ForEachContext.id"",              ""DestinationColumn"": ""EventKey"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""$.[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].ForEachContext.name"",              ""DestinationColumn"": ""EventName"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Event ID"",              ""DestinationColumn"": ""EventID"",              ""DataType"": ""int""            },            {              ""SourceColumn"": ""RegStatusID"",              ""DestinationColumn"": ""RegStatusID"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Registrant ID"",              ""DestinationColumn"": ""RegistrantID"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Type"",              ""DestinationColumn"": ""Type"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""First Name"",              ""DestinationColumn"": ""FirstName"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Last Name"",              ""DestinationColumn"": ""LastName"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Email"",              ""DestinationColumn"": ""Email"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""SignedUp"",              ""DestinationColumn"": ""SignedUp"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""SignupInsertDate"",              ""DestinationColumn"": ""SignupInsertDate"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""PresentationSignupModifiedDate"",              ""DestinationColumn"": ""PresentationSignupModifiedDate"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Session Code"",              ""DestinationColumn"": ""SessionCode"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Session Title"",              ""DestinationColumn"": ""SessionTitle"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Start Time"",              ""DestinationColumn"": ""StartTime"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""End Time"",              ""DestinationColumn"": ""EndTime"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""Track"",              ""DestinationColumn"": ""Track"",              ""DataType"": ""string""            },            {              ""SourceColumn"": ""PresentationCheckedInDate"",              ""DestinationColumn"": ""PresentationCheckedInDate"",              ""DataType"": ""string""            }          ]        }      },      {        ""SourceColumn"": """",        ""DestinationColumn"": ""EventDate"",        ""DataType"": ""formattedDatetime"",        ""Format"": ""yyyy-MM-ddTHH:mm:ssZ"",        ""TransformValue"": {          ""DefaultValue"": ""utcNow""        }      },      {        ""SourceColumn"": """",        ""DestinationColumn"": ""CreatedDate"",        ""DataType"": ""formattedDatetime"",        ""Format"": ""yyyy-MM-ddTHH:mm:ssZ"",        ""TransformValue"": {          ""DefaultValue"": ""utcNow""        }      }    ]  }}";

      string inputJson =
        @"{  ""GetReportResponse"": [    {      ""@xmlns"": ""https://api.eventcore.com/"",      ""GetReportResult"": [        {                    ""diffgr:diffgram"": [            {              ""@xmlns:msdata"": ""urn:schemas-microsoft-com:xml-msdata"",              ""@xmlns:diffgr"": ""urn:schemas-microsoft-com:xml-diffgram-v1"",              ""DocumentElement"": [                {                  ""@xmlns"": """",                  ""Session Scan Data"": [                    {                      ""@diffgr:id"": ""Session Scan Data1"",                      ""@msdata:rowOrder"": ""0"",                      ""@diffgr:hasChanges"": ""inserted"",                      ""Event ID"": ""14337"",                      ""RegStatusID"": ""7"",                      ""Registrant ID"": ""2017054"",                      ""Type"": ""General Attendee"",                      ""First Name"": ""emad"",                      ""Last Name"": ""AbuAljazer "",                      ""Email"": ""jazer313@hotmail.com"",                      ""SignedUp"": ""true"",                      ""SignupInsertDate"": ""2015-10-29T07:48:27.767-07:00"",                      ""PresentationSignupModifiedDate"": ""2015-10-29T07:48:27.767-07:00"",                      ""Session Code"": ""ITPRO17"",                      ""Session Title"": ""Deploy virtual machines in the cloud part II"",                      ""Start Time"": ""11/02/2015 15:15:00 -08:00"",                      ""End Time"": ""11/02/2015 16:30:00 -08:00"",                      ""Track"": ""IT Professional""                    }                  ]                }              ]            }          ]        }      ],      ""recordCount"": ""-1""    }  ],  ""MifContext"": {    ""runImmediate"": true,    ""runId"": ""b8cb0be4-2375-4de2-9781-15dbbf8bc9b3"",    ""TrackingEnabled"": false,    ""WorkFlowName"": ""EventCoreSessionScanDataToEDP"",    ""moreData"": ""true"",    ""start_Index"": 501,    ""end_Index"": 1000,    ""correlationId"": ""8ab25b61-ec8a-4b40-8189-4b77f4f3cbf5""  },  ""ForEachContext"": {    ""id"": ""8707"",    ""name"": ""MGX FY14""  }}";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var lead = mapper.TransformIntoJson(inputJson, true);

        Assert.IsTrue(lead.Contains("2015-10-29T07:48:27.767-07:00") && lead.Contains("2015-10-29T07:48:27.767-07:00")
                                                                     && lead.Contains("11/02/2015 15:15:00 -08:00") && lead.Contains("11/02/2015 16:30:00 -08:00") && lead.Contains("8707"));
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, Description("Validate that the property is set to default value if the value is not passed.")]
    public void Validate_AutoMapper_DefaultValue1()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MipUnitTest.LeadRecord, JsonToJsonMapper.Core.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable\": [{\"SourceColumn\": \"$.Name\",\"DestinationColumn\": \"Name\",\"DataType\": \"string\",\"ComplexType\": null,\"TransformValue\": {\"ValueMapping\":null,\"DefaultValue\":\"NA\"}},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": \"int\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": \"bool\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\": \"Guid\",\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson =
        "{\"name\": null,\"allowEmail\": false,\"SalesId\": null,\"Leadscore\":\"100\"}";

      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var lead = (LeadRecord)mapper.Transform(inputJson);
        Assert.IsTrue(lead.Name == "NA");
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod, Description("Validate that the property is mapped according to input value.")]
    public void Validate_AutoMapper_DefaultValue2()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MipUnitTest.LeadRecord, JsonToJsonMapper.Core.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable\": [{\"SourceColumn\": \"$.Name\",\"DestinationColumn\": \"Name\",\"DataType\": \"string\",\"ComplexType\": null,\"TransformValue\": {\"ValueMapping\":[{\"ExistingValue\": \"1\",\"NewValue\": true},{\"ExistingValue\": \"2\",\"NewValue\": false}],\"DefaultValue\":\"NA\"}},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": \"int\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": \"bool\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\": \"Guid\",\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson =
        "{\"Name\": \"1\",\"allowEmail\": false,\"SalesId\": null,\"Leadscore\":\"100\"}";

      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var lead = (LeadRecord)mapper.Transform(inputJson);
        Assert.IsTrue(lead.Name == "true");
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod, Description("Validate that the property is  set to default value if the value is not passed.")]
    public void Validate_AutoMapper_DefaultValue3()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MipUnitTest.LeadRecord, JsonToJsonMapper.Core.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable\": [{\"SourceColumn\": \"$.Name\",\"DestinationColumn\": \"Name\",\"DataType\": \"string\",\"ComplexType\": null,\"TransformValue\": {\"ValueMapping\":[{\"ExistingValue\": \"1\",\"NewValue\": true},{\"ExistingValue\": \"2\",\"NewValue\": false}],\"DefaultValue\":\"NA\"}},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": \"int\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": \"bool\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\": \"Guid\",\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson =
        "{\"Name\": \"22\",\"allowEmail\": false,\"SalesId\": null,\"Leadscore\":\"100\"}";

      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var lead = (LeadRecord)mapper.Transform(inputJson);
        Assert.IsTrue(lead.Name == "NA");
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod, Description("Validate that the property is  set to custom value if the value passed is empty.")]
    public void Validate_AutoMapper_EmptyValue()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MipUnitTest.LeadRecord, JsonToJsonMapper.Core.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable\": [{\"SourceColumn\": \"$.Name\",\"DestinationColumn\": \"Name\",\"DataType\": \"string\",\"ComplexType\": null,\"TransformValue\": {\"ValueMapping\":[{\"ExistingValue\": \"2\",\"NewValue\": false},{\"ExistingValue\": \"\",\"NewValue\": true}],\"DefaultValue\":\"NA\"}},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": \"int\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": \"bool\",\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\": \"Guid\",\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson =
        "{\"Name\": \"\",\"allowEmail\": false,\"SalesId\": null,\"Leadscore\":\"100\"}";

      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var lead = (LeadRecord)mapper.Transform(inputJson);
        Assert.IsTrue(lead.Name == "true");
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod, Description("Complex transformation test - Validate that the value returned is a complex type.")]
    public void Complex2FlatTransformationTest()
    {
      //Arrange - Certain input json with complex object
      string inputJson =
        "{\"dateCreated\": \"2015-08-31T14:30:00\",       \"accountCode\": \"Cross-Product\",       \"isActive\": true,       \"isHotel\": false,       \"isPreferred\": false,       \"brandFrnId\": null,       \"chainFrnId\": null,       \"locationType\": \"Venue\",       \"largestSpace\": null,       \"numberOfMeetRooms\": null,       \"numberOfRooms\": null,       \"totalSpace\": null,       \"address\": {         \"line1\": \"750 Main Street\",         \"line2\": null,         \"line3\": null,         \"line4\": null,         \"city\": \"Moncton\",         \"state\": \"New Brunswick\",         \"postalCode\": \"E1C 1E6\",         \"country\": \"Canada\",         \"intlState\": null       },       \"locationThirdParty\": null,       \"locationCode\": \"msft_can_delta_monc\",       \"desc\": \"\",       \"directions\": \"\",       \"email\": \"\",       \"externalFrnKey\": null,       \"fax\": \"\",       \"imgAttributes\": \"\",       \"imgSrc\": null,       \"label\": \"\",       \"name\": \"Delta Beausejour Hotel\",       \"notes\": \"\",       \"organization\": null,       \"phone\": \"\",       \"tollfree\": \"\",       \"url\": \"http://binged.it/1O4l53S\"     } ";
      string mapping = "{\"MappingRuleConfig\":{\"DestinationType\":\"\",\"SourceObject\":\"\",\"DestinationObject\":\"\",\"TruthTable\":[{\"SourceColumn\":\"$.address['city']\",\"DestinationColumn\":\"City\",\"DataType\":\"string\",\"ComplexType\":null,\"TransformValue\":null},{\"SourceColumn\":\"$.address['state']\",\"DestinationColumn\":\"State\",\"DataType\":\"string\",\"ComplexType\":null,\"TransformValue\":null},{\"SourceColumn\":\"$.address['postalCode']\",\"DestinationColumn\":\"PostalCode\",\"DataType\":\"string\",\"ComplexType\":null,\"TransformValue\":null},{\"SourceColumn\":\"$.address['country']\",\"DestinationColumn\":\"Country\",\"DataType\":\"string\",\"ComplexType\":null,\"TransformValue\":null}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var flatObject = JObject.Parse(mapper.TransformIntoJson(inputJson, false));

      //Assert
      Assert.AreEqual(flatObject.HasValues, true);
      Assert.AreEqual(flatObject.Count, 4);
    }

    [TestMethod, Description("Transpose handler test - Validate that the value returned is transposed.")]
    public void PromoteAttributesToPropertyTestWithoutPrependKeyText()
    {
      //Arrange - Certain input json with complex object
      string inputJson =
        "{\r\n  \"requestId\": \"a9ae#148add1e53d\",\r\n  \"success\": true,\r\n  \"nextPageToken\": \"GIYDAOBNGEYS2MBWKQYDAORQGA5DAMBOGAYDAKZQGAYDALBRGA3TQ===\",\r\n  \"moreResult\": true,\r\n  \"result\": [\r\n    {\r\n      \"id\": 2,\r\n      \"leadId\": 6,\r\n      \"activityTypeId\": 12,\r\n      \"primaryAttributeValueId\": 6,\r\n      \"primaryAttributeValue\": \"Owyliphys Iledil\",\r\n      \"attributes\": [\r\n        {\r\n          \"name\": \"Source Type\",\r\n          \"value\": \"Web page visit\"\r\n        },\r\n        {\r\n          \"name\": \"Source Info\",\r\n          \"value\": \"http://search.yahoo.com/search?p=train+cappuccino+army\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"id\": 3,\r\n      \"leadId\": 9,\r\n      \"activityTypeId\": 1,\r\n      \"primaryAttributeValueId\": 4,\r\n      \"primaryAttributeValue\": \"anti-phishing\",\r\n      \"attributes\": [\r\n        {\r\n          \"name\": \"Query Parameters\",\r\n          \"value\": null\r\n        },\r\n        {\r\n          \"name\": \"Client IP Address\",\r\n          \"value\": \"203.141.7.100\"\r\n        },\r\n        {\r\n          \"name\": \"User Agent\",\r\n          \"value\": \"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14\"\r\n        },\r\n        {\r\n          \"name\": \"Webpage ID\",\r\n          \"value\": 4\r\n        },\r\n        {\r\n          \"name\": \"Webpage URL\",\r\n          \"value\": \"/anti-phishing.html\"\r\n        },\r\n        {\r\n          \"name\": \"Referrer URL\",\r\n          \"value\": null\r\n        },\r\n        {\r\n          \"name\": \"Search Engine\",\r\n          \"value\": null\r\n        },\r\n        {\r\n          \"name\": \"Search Query\",\r\n          \"value\": null\r\n        }\r\n      ]\r\n    }\r\n  ]\r\n}";
      string mapping = "{\"MappingRuleConfig\":{\"TruthTable\":[{\"SourceColumn\":\"\",\"DestinationColumn\":\"Content\",\"DataType\":\"JArray\",\"ComplexType\":{\"DataType\":\"JArray\",\"Node\":\"$.result[*]\",\"TruthTable\":[{\"SourceColumn\":\"$.id\",\"DestinationColumn\":\"id\",\"DataType\":\"long\"},{\"SourceColumn\":\"$.leadId\",\"DestinationColumn\":\"leadId\",\"DataType\":\"long\"},{\"SourceColumn\":\"$.attributes[*]\",\"DestinationColumn\":\"\",\"DataType\":\"JArray\",\"TransformValue\":{\"Type\":\"PromoteArrayToProperty\",\"KeyLookupField\":\"$.name\",\"ValueLookupField\":\"$.value\"}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = @"{""Content"":[{""id"":2,""leadId"":6,""SourceType"":""Webpagevisit"",""SourceInfo"":""http://search.yahoo.com/search?p=train+cappuccino+army""},{""id"":3,""leadId"":9,""ClientIPAddress"":""203.141.7.100"",""UserAgent"":""Mozilla/5.0(Windows;U;WindowsNT5.1;en-US;rv:1.8.1.14)Gecko/20080404Firefox/2.0.0.14"",""WebpageID"":4,""WebpageURL"":""/anti-phishing.html""}]}";
      var value = response.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
      //Assert
      Assert.AreEqual(value, expectedResponse);
    }

    [TestMethod, Description("Transpose handler test - Validate that the value returned is transposed with prepend key text.")]
    public void PromoteAttributesToPropertyTestWithPrependKeyText()
    {
      //Arrange - Certain input json with complex object
      string inputJson =
        "{\r\n  \"requestId\": \"a9ae#148add1e53d\",\r\n  \"success\": true,\r\n  \"nextPageToken\": \"GIYDAOBNGEYS2MBWKQYDAORQGA5DAMBOGAYDAKZQGAYDALBRGA3TQ===\",\r\n  \"moreResult\": true,\r\n  \"result\": [\r\n    {\r\n      \"id\": 2,\r\n      \"leadId\": 6,\r\n      \"activityTypeId\": 12,\r\n      \"primaryAttributeValueId\": 6,\r\n      \"primaryAttributeValue\": \"Owyliphys Iledil\",\r\n      \"attributes\": [\r\n        {\r\n          \"name\": \"Source Type\",\r\n          \"value\": \"Web page visit\"\r\n        },\r\n        {\r\n          \"name\": \"Source Info\",\r\n          \"value\": \"http://search.yahoo.com/search?p=train+cappuccino+army\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"id\": 3,\r\n      \"leadId\": 9,\r\n      \"activityTypeId\": 1,\r\n      \"primaryAttributeValueId\": 4,\r\n      \"primaryAttributeValue\": \"anti-phishing\",\r\n      \"attributes\": [\r\n        {\r\n          \"name\": \"Query Parameters\",\r\n          \"value\": null\r\n        },\r\n        {\r\n          \"name\": \"Client IP Address\",\r\n          \"value\": \"203.141.7.100\"\r\n        },\r\n        {\r\n          \"name\": \"User Agent\",\r\n          \"value\": \"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14\"\r\n        },\r\n        {\r\n          \"name\": \"Webpage ID\",\r\n          \"value\": 4\r\n        },\r\n        {\r\n          \"name\": \"Webpage URL\",\r\n          \"value\": \"/anti-phishing.html\"\r\n        },\r\n        {\r\n          \"name\": \"Referrer URL\",\r\n          \"value\": null\r\n        },\r\n        {\r\n          \"name\": \"Search Engine\",\r\n          \"value\": null\r\n        },\r\n        {\r\n          \"name\": \"Search Query\",\r\n          \"value\": null\r\n        }\r\n      ]\r\n    }\r\n  ]\r\n}";
      string mapping = "{\"MappingRuleConfig\":{\"TruthTable\":[{\"SourceColumn\":\"\",\"DestinationColumn\":\"Content\",\"DataType\":\"JArray\",\"ComplexType\":{\"DataType\":\"JArray\",\"Node\":\"$.result[*]\",\"TruthTable\":[{\"SourceColumn\":\"$.id\",\"DestinationColumn\":\"id\",\"DataType\":\"long\"},{\"SourceColumn\":\"$.leadId\",\"DestinationColumn\":\"leadId\",\"DataType\":\"long\"},{\"SourceColumn\":\"$.attributes[*]\",\"DestinationColumn\":\"\",\"DataType\":\"JArray\",\"TransformValue\":{\"Type\":\"PromoteArrayToProperty\",\"PrependKeyText\":\"Attribute\",\"KeyLookupField\":\"$.name\",\"ValueLookupField\":\"$.value\"}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = @"{""Content"":[{""id"":2,""leadId"":6,""AttributeSourceType"":""Webpagevisit"",""AttributeSourceInfo"":""http://search.yahoo.com/search?p=train+cappuccino+army""},{""id"":3,""leadId"":9,""AttributeClientIPAddress"":""203.141.7.100"",""AttributeUserAgent"":""Mozilla/5.0(Windows;U;WindowsNT5.1;en-US;rv:1.8.1.14)Gecko/20080404Firefox/2.0.0.14"",""AttributeWebpageID"":4,""AttributeWebpageURL"":""/anti-phishing.html""}]}";
      var value = response.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
      //Assert
      Assert.IsTrue(value.Contains("AttributeWebpageURL"));
      Assert.AreEqual(value, expectedResponse);
    }

    [TestMethod, Description("Roslyn script handler test - Validate that the value returned is after executing the script.")]
    public void RoslynTest()
    {
      //Arrange - Certain input json with complex object
      string inputJson =
        "{\r\n  \"eventCode\": \"APAC-1PWBNR3-0331-16-HQ\",\r\n  \"pkEventId\": \"0x449740001\",\r\n  \"eventId\": \"2353\",\r\n  \"externalKey\": \"AP-Azure-WBNR-FY16-03Mar-31-Win10_Device_Management\",\r\n  \"accountCode\": \"C-and-E\",\r\n  \"eventName\": \"Windows 10 for device management\",\r\n  \"url\": null,\r\n  \"autoCalcCode\": false,\r\n  \"isTestMode\": false,\r\n  \"isActive\": true,\r\n  \"isTemplate\": false,\r\n  \"isLocked\": true,\r\n  \"isClassic\": true,\r\n  \"eventStatus\": \"Live\",\r\n  \"isPending\": false,\r\n  \"isDesign\": false,\r\n  \"isTesting\": false,\r\n  \"isOnsite\": false,\r\n  \"isOffline\": false,\r\n  \"isClosed\": false,\r\n  \"isNotificationEnabled\": false,\r\n  \"notes\": \"\",\r\n  \"contact\": {\r\n    \"contactName\": \"Microsoft SQL Team\",\r\n    \"organization\": \"\",\r\n    \"email\": \"b-dikurn@microsoft.com\",\r\n    \"phone\": \"\",\r\n    \"tollfree\": \"\",\r\n    \"fax\": \"\",\r\n    \"url\": \"\",\r\n    \"notes\": \"\"\r\n  },\r\n  \"location\": {\r\n    \"locationName\": \"Webinar\",\r\n    \"locationCode\": \"Webinar\",\r\n    \"email\": null,\r\n    \"phone\": null,\r\n    \"tollfree\": null,\r\n    \"fax\": null,\r\n    \"url\": null,\r\n    \"notes\": null\r\n  },\r\n  \"dateCreated\": \"2016-02-17T15:59:42\",\r\n  \"dateModified\": \"2016-02-19T14:21:21\",\r\n  \"startDate\": \"2016-03-31T08:00:00\",\r\n  \"endDate\": \"2016-03-31T16:00:00\",\r\n  \"dateClosed\": null,\r\n  \"txtEvtCreatedBy\": \"Samuel Pak\",\r\n  \"txtEvtModifiedBy\": \"Maha Pasha\",\r\n  \"glNumber\": \"\",\r\n  \"timezone\": \"(GMT+05:00) Islamabad, Karachi, Tashkent\",\r\n  \"timezoneMapping\": \"Asia/Karachi\",\r\n  \"timezoneId\": 85,\r\n  \"proposedLocation\": null,\r\n  \"plannedAttendance\": null,\r\n  \"proposedBudget\": null,\r\n  \"plannedCurrency\": null,\r\n  \"travelAccoCurrency\": null,\r\n  \"eventQuestions\": {\r\n    \"question\": [\r\n      {\r\n        \"questionField\": \"evt_ans_field19\",\r\n        \"questionId\": 638,\r\n        \"questionCode\": \"Design-LPHeroImg\",\r\n        \"questionName\": \"Design-LPHeroImg\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"https://info.microsoft.com/rs/157-GQE-382/images/ms-win10-webinar-banners-3000x300-04.png\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field24\",\r\n        \"questionId\": 996,\r\n        \"questionCode\": \"Design-EmailBannerImg\",\r\n        \"questionName\": \"Design-EmailBannerImg \",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field16\",\r\n        \"questionId\": 398,\r\n        \"questionCode\": \"Program-BannerText\",\r\n        \"questionName\": \"Program-BannerText\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"Microsft Event Banner Title\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field20\",\r\n        \"questionId\": 639,\r\n        \"questionCode\": \"Design-BannerTextColor\",\r\n        \"questionName\": \"Design-BannerTextColor\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"#ffffff\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field17\",\r\n        \"questionId\": 633,\r\n        \"questionCode\": \"Design-HeaderTextColor\",\r\n        \"questionName\": \"Design-HeaderTextColor\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"#333333\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field06\",\r\n        \"questionId\": 152,\r\n        \"questionCode\": \"Program-HeaderText\",\r\n        \"questionName\": \"Program-HeaderText\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"Windows 10 for device management\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field07\",\r\n        \"questionId\": 153,\r\n        \"questionCode\": \"Program-Description\",\r\n        \"questionName\": \"Program-Description\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"A\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field09\",\r\n        \"questionId\": 229,\r\n        \"questionCode\": \"Program-AdditionalInfo\",\r\n        \"questionName\": \"Program-AdditionalInfo\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"B\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field21\",\r\n        \"questionId\": 640,\r\n        \"questionCode\": \"Design-HighlightColor\",\r\n        \"questionName\": \"Design-HighlightColor\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"#ffffff\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field22\",\r\n        \"questionId\": 641,\r\n        \"questionCode\": \"Design-HighlightTextColor\",\r\n        \"questionName\": \"Design-HighlightTextColor\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"#ffffff\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field01\",\r\n        \"questionId\": 147,\r\n        \"questionCode\": \"Hero Banner Image\",\r\n        \"questionName\": \"Hero Banner Image\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"EN-BASICE-Banner-PeopleWithDevices.png\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field02\",\r\n        \"questionId\": 37143,\r\n        \"questionCode\": \"Modern Tele-Readiness Hotsheet URL\",\r\n        \"questionName\": \"Modern Tele-Readiness Hotsheet URL\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"\"\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"questionField\": \"evt_ans_field30\",\r\n        \"questionId\": 649,\r\n        \"questionCode\": \"DELETED-Program-Description\",\r\n        \"questionName\": \"DELETED-Program-Description\",\r\n        \"answers\": {\r\n          \"answer\": [\r\n            {\r\n              \"answerId\": null,\r\n              \"answerCode\": null,\r\n              \"value\": \"\"\r\n            }\r\n          ]\r\n        }\r\n      }\r\n    ]\r\n  },\r\n  \"groups\": null,\r\n  \"rotations\": null,\r\n  \"forms\": null,\r\n  \"websites\": null,\r\n  \"primaryFormURL\": \"/profile/form/index.cfm?PKformID=0x1084150001\",\r\n  \"questionAssignments\": null\r\n}";
      string mapping =
        "{\r\n  \"Scripts\": [\r\n    {\r\n      \"Name\": \"GetTitle\",\r\n      \"Code\": \"string GetTitle(string args){string[] input = args.Split(new string[] {\\\"[delimiter]\\\"}, System.StringSplitOptions.None);string[] data = null; var title = string.Empty;for (int i = 0; i < input.Length; i++){if (!string.IsNullOrEmpty(input[i]) && !string.IsNullOrWhiteSpace(input[i]) && !input[i].Contains(\\\"tokenDelimiter\\\")){title = input[i];break;}if (input[i].Contains(\\\"tokenDelimiter\\\") == true){data = input[i].Split(new string[] { \\\"[tokenDelimiter]\\\" }, System.StringSplitOptions.None);for (int x = 0; x < data.Length; x++){if (!string.IsNullOrEmpty(data[x]) && !string.IsNullOrWhiteSpace(data[x])){title = data[x].ToString();break;}}}else{continue;}} return title;}GetTitle(Args)\"\r\n    },\r\n    {\r\n      \"Name\": \"GetDescription\",\r\n      \"Code\": \"string GetEventDesc(string args) { string[] input = args.Split(new string[] { \\\"[delimiter]\\\" }, System.StringSplitOptions.None); string[] data = null; var eventDesc = string.Empty; for (int i = 0; i < input.Length; i++) { if (!string.IsNullOrEmpty(input[i]) && !string.IsNullOrWhiteSpace(input[i]) && !input[i].Contains(\\\"tokenDelimiter\\\")) { eventDesc = input[i]; eventDesc = System.Text.RegularExpressions.Regex.Replace(input[i], @\\\"<(.|\\n)*?>\\\", string.Empty); break; } if (input[i].Contains(\\\"tokenDelimiter\\\") == true) { data = input[i].Split(new string[] {\\\"[tokenDelimiter]\\\" }, System.StringSplitOptions.None); for (int x = 0; x < data.Length; x++) { if (!string.IsNullOrEmpty(data[x]) && !string.IsNullOrWhiteSpace(data[x])) { eventDesc = data[x].ToString(); eventDesc = System.Text.RegularExpressions.Regex.Replace(data[x], @\\\"<(.|\\n)*?>\\\", string.Empty); break;} } } else { continue; }} return eventDesc;}GetEventDesc(Args)\",\r\n      \"Reference\": {\r\n        \"Assembly\": \"System\",\r\n        \"NameSpace\": \"System.Text.RegularExpressions\"\r\n      }\r\n    },\r\n    {\r\n      \"Name\": \"GetURL\",\r\n      \"Code\": \"string GetURL(string args) { string[] input = args.Split(new string[] { \\\"[delimiter]\\\" }, System.StringSplitOptions.None); string[] data = null; var eventURL = string.Empty;for (int i = 0; i < input.Length; i++) { if (!string.IsNullOrEmpty(input[i]) && !string.IsNullOrWhiteSpace(input[i]) && !input[i].Contains(\\\"tokenDelimiter\\\")) { eventURL = input[i]; break; } if (input[i].Contains(\\\"tokenDelimiter\\\") == true) { data = input[i].Split(new string[] { \\\"[tokenDelimiter]\\\" }, System.StringSplitOptions.None); for (int x = 0; x < data.Length; x++) { if (!string.IsNullOrEmpty(data[x]) && !string.IsNullOrWhiteSpace(data[x])) { eventURL = data[x].ToString(); break; } } } else { continue; } } if (string.IsNullOrEmpty(eventURL)) return eventURL = \\\"www.microsoft.com\\\"; else return eventURL; }GetURL(Args)\"\r\n    },\r\n    {\r\n      \"Name\": \"GetJsonArray\",\r\n      \"Code\": \"string GetJsonArray(string args) { System.Text.StringBuilder sb = new System.Text.StringBuilder(); sb.Append(\\\"[\\\"); foreach (var item in args.Split(',')) { sb.Append(@\\\"\\\"\\\"\\\" + item + @\\\"\\\"\\\",\\\"); } sb.Remove(sb.Length - 1, 1); sb.Append(\\\"]\\\"); return sb.ToString();}GetJsonArray(Args)\"\r\n    }\r\n  ],\r\n  \"MappingRuleConfig\": {\r\n    \"DestinationType\": \"EventInformation\",\r\n    \"SourceObject\": \"CertainEvents\",\r\n    \"TruthTable\": [\r\n      {\r\n        \"SourceColumn\": \"\",\r\n        \"DestinationColumn\": \"EventList\",\r\n        \"DataType\": \"jarray\",\r\n        \"ComplexType\": {\r\n          \"TruthTable\": [\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"title\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetTitle\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Program-HeaderText')].answers.answer.[0].value\",\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Program-BannerText')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"accountCode\",\r\n              \"DestinationColumn\": \"accountCode\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"eventCode\",\r\n              \"DestinationColumn\": \"Id\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"eventStatus\",\r\n              \"DestinationColumn\": \"Status\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"timezone\",\r\n              \"DestinationColumn\": \"timezone\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"source\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"DefaultValue\": \"Certain\"\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"EventCategory\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"DefaultValue\": \"Onsite Event\"\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"startDate\",\r\n              \"DestinationColumn\": \"StartDate\",\r\n              \"DataType\": \"formattedDatetime\",\r\n              \"Format\": \"yyyy-MM-ddTHH:mm:ssZ\",\r\n              \"TransformValue\": { \"DefaultValue\": \"utcNow\" }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"endDate\",\r\n              \"DestinationColumn\": \"EndDate\",\r\n              \"DataType\": \"formattedDatetime\",\r\n              \"Format\": \"yyyy-MM-ddTHH:mm:ssZ\",\r\n              \"TransformValue\": { \"DefaultValue\": \"utcNow\" }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"$.location['locationName']\",\r\n              \"DestinationColumn\": \"locationCode\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"Description\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetDescription\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Search-EventAbstract')].answers.answer.[0].value\",\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Program-Description')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"PrimaryLanguage\",\r\n              \"DataType\": \"jarray\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetJsonArray\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Localization-Form-and-Email-Language')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"PrimaryTargetAudience\",\r\n              \"DataType\": \"jarray\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetJsonArray\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Search-EventsFor')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"Product\",\r\n              \"DataType\": \"jarray\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetJsonArray\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Search-EventProduct')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"EventSource\",\r\n              \"DestinationColumn\": \"EventSource\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"$.eventQuestions.question[?(@.questionCode == 'Search-EventIcon')].answers.answer.[0].value\",\r\n              \"DestinationColumn\": \"EventIcon\",\r\n              \"DataType\": \"string\"\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"Category\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"DefaultValue\": \"TBD\"\r\n              }\r\n            },\r\n            {\r\n              \"SourceColumn\": \"\",\r\n              \"DestinationColumn\": \"URL\",\r\n              \"DataType\": \"string\",\r\n              \"TransformValue\": {\r\n                \"Type\": \"script\",\r\n                \"ScriptName\": \"GetURL\",\r\n                \"Params\": [\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'Search-AlternateRegURL')].answers.answer.[0].value\",\r\n                  \"$.eventQuestions.question[?(@.questionCode == 'primaryFormURL')].answers.answer.[0].value\"\r\n                ]\r\n              }\r\n\r\n            }\r\n          ]\r\n        }\r\n      },\r\n      {\r\n        \"SourceColumn\": \"\",\r\n        \"DestinationColumn\": \"RequestId\",\r\n        \"DataType\": \"guid\",\r\n        \"TransformValue\": {\r\n          \"DefaultValue\": \"4bfaac9c-1e0d-4620-b9e6-66095376e99a\"\r\n        }\r\n      }\r\n    ]\r\n  }\r\n}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = @"{""EventList"":[{""title"":""Windows 10 for device management"",""accountCode"":""C-and-E"",""Id"":""APAC-1PWBNR3-0331-16-HQ"",""Status"":""Live"",""timezone"":""(GMT+05:00) Islamabad, Karachi, Tashkent"",""source"":""Certain"",""EventCategory"":""Onsite Event"",""StartDate"":""2016-03-31T08:00:00Z"",""EndDate"":""2016-03-31T16:00:00Z"",""locationCode"":""Webinar"",""Description"":""A"",""PrimaryLanguage"":[""""],""PrimaryTargetAudience"":[""""],""Product"":[""""],""Category"":""TBD"",""URL"":""www.microsoft.com""}],""RequestId"":""4bfaac9c-1e0d-4620-b9e6-66095376e99a""}";
      var value = response.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
      //Assert
      Assert.AreEqual(value, expectedResponse.Replace(" ", string.Empty));
    }

    [TestMethod, Description("Parent data test - Validate that the value returned is from the parent JObject.")]
    public void ParentDataTest()
    {
      //Arrange - Certain input json with complex object
      string inputJson =
        "{\r\n  \"events\": [\r\n    {\r\n      \"eventId\": \"100\",\r\n      \"eventName\": \"A\",\r\n      \"location\": {\r\n        \"locationName\": \"Lincoln Square\",\r\n        \"locationCode\": \"Lincoln Square\"\r\n      },\r\n      \"eventQuestions\": {\r\n        \"question\": [\r\n          {\r\n            \"questionId\": \"Q1\", \r\n            \"questionName\": \"1Design-LPHeroImg\",\r\n            \"answers\": {\r\n              \"answer\": [\r\n                {\r\n                  \"answerId\": null,\r\n                  \"value\": \"1http://na-ab11.marketo.com/rs/113-EDC-810/images/MSC Test Banner.jpg\"\r\n                },\r\n{\r\n                  \"answerId\": null,\r\n                  \"value\": \"2http://na-ab11.marketo.com/rs/113-EDC-810/images/MSC Test Banner.jpg\"\r\n                }\r\n              ]\r\n            }\r\n          },\r\n          {\r\n            \"questionId\": \"Q2\", \r\n            \"questionName\": \"2Search-EventsFor\",\r\n            \"answers\": {\r\n              \"answer\": [\r\n                {\r\n                  \"answerId\": null,\r\n                  \"value\": \"3Consumers,Partners\"\r\n                }\r\n              ]\r\n            }\r\n          }\r\n        ]\r\n      }\r\n    },\r\n{\r\n      \"eventId\": \"200\",\r\n      \"eventName\": \"B\",\r\n      \"location\": {\r\n        \"locationName\": \"Bel\",\r\n        \"locationCode\": \"Bel\"\r\n      },\r\n      \"eventQuestions\": {\r\n        \"question\": [\r\n          {\r\n            \"questionId\": \"Q3\", \r\n            \"questionName\": \"3Design-LPHeroImg\",\r\n            \"answers\": {\r\n              \"answer\": [\r\n               \r\n              ]\r\n            }\r\n          },\r\n          {\r\n            \"questionId\": \"Q4\", \r\n            \"questionName\": \"4Search-EventsFor\",\r\n            \"answers\": {\r\n              \"answer\": [\r\n                {\r\n                  \"answerId\": null,\r\n                  \"value\": \"4Klo\"\r\n                }\r\n              ]\r\n            }\r\n          }\r\n        ]\r\n      }\r\n    }\r\n  ]\r\n}";
      string mapping = "{\"MappingRuleConfig\":{\"TruthTable\":[{\"SourceColumn\":\"\",\"DestinationColumn\":\"Answers\",\"DataType\":\"string\",\"ComplexType\":{\"DataType\":\"JArray\",\"Node\":\"$.events[*].eventQuestions.question[*].answers.answer\",\"TruthTable\":[{\"SourceColumn\":\"$.[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].eventId\",\"DestinationColumn\":\"eventId\",\"DataType\":\"string\"},{\"SourceColumn\":\"$.[{parent}].[{parent}].[{parent}].[{parent}].[{parent}].questionId\",\"DestinationColumn\":\"questionId\",\"DataType\":\"string\"},{\"SourceColumn\":\"answerId\",\"DestinationColumn\":\"answerId\",\"DataType\":\"string\"},{\"SourceColumn\":\"answerCode\",\"DestinationColumn\":\"answerCode\",\"DataType\":\"string\"},{\"SourceColumn\":\"value\",\"DestinationColumn\":\"value\",\"DataType\":\"string\"}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = @"{""Answers"":[{""eventId"":""100"",""questionId"":""Q1"",""value"":""1http://na-ab11.marketo.com/rs/113-EDC-810/images/MSC Test Banner.jpg""},{""eventId"":""100"",""questionId"":""Q1"",""value"":""2http://na-ab11.marketo.com/rs/113-EDC-810/images/MSC Test Banner.jpg""},{""eventId"":""100"",""questionId"":""Q2"",""value"":""3Consumers,Partners""},{""eventId"":""200"",""questionId"":""Q3""},{""eventId"":""200"",""questionId"":""Q4"",""value"":""4Klo""}]}";
      var value = response.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
      //Assert
      Assert.AreEqual(value, expectedResponse.Replace(" ", string.Empty));
    }

    [TestMethod, Description("ValueMappingHandler test - Validate that the default value is returned.")]
    public void Validate_ValueMappingHandler_DefaultValue()
    {
      // Arrange
      string config = "{\"Type\":null,\"PrependKeyText\":null,\"ValueMapping\":null,\"DefaultValue\":\"Certain\",\"ScriptName\":null,\"Params\":null,\"KeyLookupField\":null,\"ValueLookupField\":null}";
      string input = "{\"value\":null}";

      //Act
      ITransformationHandler handler = new ValueMappingHandler();
      try
      {
        var response = handler.Run(JObject.Parse(config), JObject.Parse(input));
        Assert.IsTrue(response == "Certain");
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, Description("ValueMappingHandler test - Validate that the default value is returned is UTC now.")]
    public void Validate_ValueMappingHandler_DefaultValue_UtcNow()
    {
      // Arrange
      string config = "{\"Type\":null,\"PrependKeyText\":null,\"ValueMapping\":null,\"DefaultValue\":\"UtcNow\",\"ScriptName\":null,\"Params\":null,\"KeyLookupField\":null,\"ValueLookupField\":null}";
      string input = "{\"value\":null}";

      //Act
      ITransformationHandler handler = new ValueMappingHandler();
      try
      {
        var response = handler.Run(JObject.Parse(config), JObject.Parse(input));
        DateTime date;
        Assert.IsTrue(DateTime.TryParse(response, out date));
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, Description("ValueMappingHandler test - Validate that the default value is returned is new Guid.")]
    public void Validate_ValueMappingHandler_DefaultValue_Guid()
    {
      // Arrange
      string config = "{\"Type\":null,\"PrependKeyText\":null,\"ValueMapping\":null,\"DefaultValue\":\"NewGuid\",\"ScriptName\":null,\"Params\":null,\"KeyLookupField\":null,\"ValueLookupField\":null}";
      string input = "{\"value\":null}";

      //Act
      ITransformationHandler handler = new ValueMappingHandler();
      try
      {
        var response = handler.Run(JObject.Parse(config), JObject.Parse(input));
        Guid guid;
        Assert.IsTrue(Guid.TryParse(response, out guid));
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, Description("ValueMappingHandler test - Validate that the default value is returned is mapped.")]
    public void Validate_ValueMappingHandler_DefaultValue_Mapping()
    {
      // Arrange
      string config = "{\"Type\":null,\"PrependKeyText\":null,\"ValueMapping\":[{\"ExistingValue\": \"2\",\"NewValue\": 1},{\"ExistingValue\": \"1\",\"NewValue\": 2}, {\"ExistingValue\": \"0\",\"NewValue\": -1}],\"DefaultValue\":\"UtcNow\",\"ScriptName\":null,\"Params\":null,\"KeyLookupField\":null,\"ValueLookupField\":null}";
      string input = "{\"value\":\"1\"}";

      //Act
      ITransformationHandler handler = new ValueMappingHandler();
      try
      {
        var response = handler.Run(JObject.Parse(config), JObject.Parse(input));
        Assert.IsTrue(response == "2");
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is concatenated.")]
    public void FunctionTest_ConCat()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""eventId"": ""2353"",""externalKey"": ""AP-Azure"",""accountCode"": ""C-and-E"",""eventName"": ""Windows 10"",""eventQuestions"": {""question"": [{""questionField"": ""evt_ans_field19"",""questionId"": 638,""questionCode"": ""Design-LPHeroImg"",""questionName"": ""Design-LPHeroImg"",""answers"": {""answer"": [{""answerId"": null,""answerCode"": null,""value"": ""https://info.microsoft.com""}]}},{""questionField"": ""evt_ans_field24"",""questionId"": 996,""questionCode"": ""Design-EmailBannerImg"",""questionName"": ""Design-EmailBannerImg "",""answers"": {""answer"": [{""answerId"": null,""answerCode"": null,""value"": ""First answer""},{""answerId"": null,""answerCode"": null,""value"": ""Last answer""}]}},{""questionField"": ""evt_ans_field24"",""questionId"": 996,""questionCode"": ""Design-EmailBannerImg"",""questionName"": ""Design-EmailBannerImg "",""answers"": {""answer"": [{""answerId"": null,""answerCode"": null,""value"": """"}]}}]}}";
      string mapping = @"{""MappingRuleConfig"": {""TruthTable"": [{""SourceColumn"": """",""DestinationColumn"": ""EventList"",""DataType"": ""string"",""ComplexType"": {""DataType"": ""JArray"",""Node"": ""$.eventQuestions.question[*]"",""TruthTable"": [{""SourceColumn"": """",""DestinationColumn"": ""title"",""DataType"": ""string"",""TransformValue"": {""Type"": ""function"",""Delimeter"": "","",""Function"": ""ConCat"",""Params"": [""$.answers.answer[*].value"",""Hello world!""]}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"EventList\":[{\"title\":\"https://info.microsoft.com,Hello world!\"},{\"title\":\"First answer,Last answer,Hello world!\"},{\"title\":\",Hello world!\"}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is concatenated when delimeter is null.")]
    public void FunctionTest_ConCat_WithoutDelimeter()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{'eventId': '2353','externalKey': 'AP-Azure','accountCode': 'C-and-E','eventName': 'Windows 10','eventQuestions': {'question': [{'questionField': 'evt_ans_field19','questionId': 638,'questionCode': 'Design-LPHeroImg','questionName': 'Design-LPHeroImg','answers': {'answer': [{'answerId': null,'answerCode': null,'value': 'https://info.microsoft.com'}]}},{'questionField': 'evt_ans_field24','questionId': 996,'questionCode': 'Design-EmailBannerImg','questionName': 'Design-EmailBannerImg ','answers': {'answer': [{'answerId': null,'answerCode': null,'value': 'First answer'},{'answerId': null,'answerCode': null,'value': 'Last answer'}]}},{'questionField': 'evt_ans_field24','questionId': 996,'questionCode': 'Design-EmailBannerImg','questionName': 'Design-EmailBannerImg ','answers': {'answer': [{'answerId': null,'answerCode': null,'value': ''}]}}]}}";
      string mapping = @"{'MappingRuleConfig': {'TruthTable': [{'SourceColumn': '','DestinationColumn': 'EventList','DataType': 'string','ComplexType': {'DataType': 'JArray','Node': '$.eventQuestions.question[*]','TruthTable': [{'SourceColumn': '','DestinationColumn': 'title','DataType': 'string','TransformValue': {'Type': 'function','Function': 'ConCat','Params': ['$.answers.answer[*].value','Hello world!']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"EventList\":[{\"title\":\"https://info.microsoft.comHello world!\"},{\"title\":\"First answerLast answerHello world!\"},{\"title\":\"Hello world!\"}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is concatenated when params collection is null.")]
    public void FunctionTest_ConCat_EmptyParamCollection()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{'eventId': '2353','externalKey': 'AP-Azure','accountCode': 'C-and-E','eventName': 'Windows 10','eventQuestions': {'question': [{'questionField': 'evt_ans_field19','questionId': 638,'questionCode': 'Design-LPHeroImg','questionName': 'Design-LPHeroImg','answers': {'answer': [{'answerId': null,'answerCode': null,'value': 'https://info.microsoft.com'}]}},{'questionField': 'evt_ans_field24','questionId': 996,'questionCode': 'Design-EmailBannerImg','questionName': 'Design-EmailBannerImg ','answers': {'answer': [{'answerId': null,'answerCode': null,'value': 'First answer'},{'answerId': null,'answerCode': null,'value': 'Last answer'}]}},{'questionField': 'evt_ans_field24','questionId': 996,'questionCode': 'Design-EmailBannerImg','questionName': 'Design-EmailBannerImg ','answers': {'answer': [{'answerId': null,'answerCode': null,'value': ''}]}}]}}";
      string mapping = @"{'MappingRuleConfig': {'TruthTable': [{'SourceColumn': '','DestinationColumn': 'EventList','DataType': 'string','ComplexType': {'DataType': 'JArray','Node': '$.eventQuestions.question[*]','TruthTable': [{'SourceColumn': '','DestinationColumn': 'title','DataType': 'string','TransformValue': {'Type': 'function','Function': 'ConCat','Params': ['$.answers.answer[*].value1','Hello world!']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"EventList\":[{\"title\":\"Hello world!\"},{\"title\":\"Hello world!\"},{\"title\":\"Hello world!\"}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned has no empty array.")]
    public void IgnoreEmptyArrayTest_True()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""eventId"": ""2353"",""externalKey"": ""AP-Azure"",""accountCode"": ""C-and-E"",""eventName"": ""Windows 10"",""eventQuestions"": {""question"": []}}";
      string mapping = @"{""MappingRuleConfig"": {""TruthTable"": [{""SourceColumn"": """",""DestinationColumn"": ""EventList"",""DataType"": ""string"",""ComplexType"": {""DataType"": ""JArray"",""IgnoreEmptyArray"":true, ""Node"": ""$.eventQuestions.question[*]"",""TruthTable"": [{""SourceColumn"": """",""DestinationColumn"": ""title"",""DataType"": ""string"",""TransformValue"": {""Type"": ""function"",""Delimeter"": "","",""Function"": ""ConCat"",""Params"": [""$.answers.answer[*].value"",""Hello world!""]}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is empty array.")]
    public void IgnoreEmptyArrayTest_False()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""eventId"": ""2353"",""externalKey"": ""AP-Azure"",""accountCode"": ""C-and-E"",""eventName"": ""Windows 10"",""eventQuestions"": {""question"": []}}";
      string mapping = @"{""MappingRuleConfig"": {""TruthTable"": [{""SourceColumn"": """",""DestinationColumn"": ""EventList"",""DataType"": ""string"",""ComplexType"": {""DataType"": ""JArray"",""IgnoreEmptyArray"":false, ""Node"": ""$.eventQuestions.question[*]"",""TruthTable"": [{""SourceColumn"": """",""DestinationColumn"": ""title"",""DataType"": ""string"",""TransformValue"": {""Type"": ""function"",""Delimeter"": "","",""Function"": ""ConCat"",""Params"": [""$.answers.answer[*].value"",""Hello world!""]}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"EventList\":[]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is empty array.")]
    public void IgnoreEmptyArrayTest_NotSet()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""eventId"": ""2353"",""externalKey"": ""AP-Azure"",""accountCode"": ""C-and-E"",""eventName"": ""Windows 10"",""eventQuestions"": {""question"": []}}";
      string mapping = @"{""MappingRuleConfig"": {""TruthTable"": [{""SourceColumn"": """",""DestinationColumn"": ""EventList"",""DataType"": ""string"",""ComplexType"": {""DataType"": ""JArray"", ""Node"": ""$.eventQuestions.question[*]"",""TruthTable"": [{""SourceColumn"": """",""DestinationColumn"": ""title"",""DataType"": ""string"",""TransformValue"": {""Type"": ""function"",""Delimeter"": "","",""Function"": ""ConCat"",""Params"": [""$.answers.answer[*].value"",""Hello world!""]}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"EventList\":[]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Value handler test - Validate transformation if default value is empty.")]
    public void Automapper_EmptyDefaultValueTest()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""eventId"": ""2353"",""externalKey"": ""AP-Azure"",""accountCode"": ""C-and-E"",""eventName"": ""Windows 10"",""eventQuestions"": {""question"": []}}";
      string mapping = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"__meta__Entity\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"DefaultValue\": \"GenericPresales\" } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"EncryptedFields\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"DefaultValue\": \" \" } } ] } } ";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"__meta__Entity\":\"GenericPresales\",\"EncryptedFields\":\" \"}";

      //Assert
      Assert.AreEqual(expectedResponse, response);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is replaced value.")]
    public void FunctionTest_ReplaceValue()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{'eventId':'2353','externalKey':'AP-Azure','accountCode':'C-and-E','eventName':'Windows10','eventQuestions':{'question':[{'questionField':'evt_ans_field19','questionId':638,'questionCode':'Design-LPHeroImg','questionName':'Design-LPHeroImg','answers':{'answer':[{'answerId':null,'answerCode':null,'value':'https://info.microsoft.com'}]}},{'questionField':'evt_ans_field24','questionId':996,'questionCode':'Design-EmailBannerImg','questionName':'Design-EmailBannerImg','answers':{'answer':[{'answerId':null,'answerCode':null,'value':'Firstanswer'},{'answerId':null,'answerCode':null,'value':'Lastanswer'}]}},{'questionField':'evt_ans_field24','questionId':996,'questionCode':'Design-EmailBannerImg','questionName':'Design-EmailBannerImg','answers':{'answer':[{'answerId':null,'answerCode':null,'value':''}]}}]}}";
      string mapping = @"{'MappingRuleConfig':{'TruthTable':[{'SourceColumn':'','DestinationColumn':'EventList','DataType':'string','ComplexType':{'DataType':'JArray','Node':'$.eventQuestions.question[*]','TruthTable':[{'SourceColumn':'','DestinationColumn':'title','DataType':'bool','TransformValue':{'Type':'function','CompareToValue':'httPS://info.microsoft.com','ReturnValue':'false','DefaultValue':'true','Function':'ReplaceValue','Params':['$.answers.answer[*].value']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"EventList\":[{\"title\":false},{\"title\":true},{\"title\":true}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is replaced value When Path is given.")]
    public void FunctionTest_ReplaceValueNode_Failure()
    {
      //Arrange - Certain input json with complex object

      string inputJson = "{ \"mipmsxPreferenceTopic\": \"MOD\", \"JobTitle\": \"Webmaster and Records Manager\", \"BusinessPhone\": \"+15563213567\", \"MobilePhone\": 1234565430, \"CountryOrRegion\": \"United Sates\", \"mipmsxPreferenceType\": \"Topic\", }";

      string mapping = @"{'MappingRuleConfig':{'TruthTable':[{'SourceColumn':'','DestinationColumn':'Output','DataType':'string','ComplexType':{'DataType':'string','Node':'$','TruthTable':[{'SourceColumn':'','DestinationColumn':'PreferenceTopic','DataType':'string','TransformValue':{'Type':'function','CompareToValue':'CNE','ReturnValue':'','DefaultValue':'NA','Function':'ReplaceValue','Params':['$.mipmsxPreferenceType']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Output\":\"{\\r\\n  \\\"PreferenceTopic\\\": \\\"NA\\\"\\r\\n}\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is replaced value When Path is given.")]
    public void FunctionTest_ReplaceValueNode_Sucess()
    {
      //Arrange - Certain input json with complex object

      string inputJson = "{ \"mipmsxPreferenceTopic\": \"MOD\", \"JobTitle\": \"Webmaster and Records Manager\", \"BusinessPhone\": \"+15563213567\", \"MobilePhone\": 1234565430, \"CountryOrRegion\": \"United Sates\", \"mipmsxPreferenceType\": \"Topic\", }";

      string mapping = @"{'MappingRuleConfig':{'TruthTable':[{'SourceColumn':'','DestinationColumn':'Output','DataType':'string','ComplexType':{'DataType':'string','Node':'$','TruthTable':[{'SourceColumn':'','DestinationColumn':'PreferenceTopic','DataType':'string','TransformValue':{'Type':'function','CompareToValue':'Topic','ReturnValue':'RV','DefaultValue':'NA','Function':'ReplaceValue','Params':['$.mipmsxPreferenceType']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Output\":\"{\\r\\n  \\\"PreferenceTopic\\\": \\\"RV\\\"\\r\\n}\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is replaced value When Path is given.")]
    public void FunctionTest_ReplaceValueNode_JsonPath()
    {
      //Arrange - Certain input json with complex object

      string inputJson = "{ \"mipmsxPreferenceTopic\": \"MOD\", \"JobTitle\": \"Webmaster and Records Manager\", \"BusinessPhone\": \"+15563213567\", \"MobilePhone\": 1234565430, \"CountryOrRegion\": \"United Sates\", \"mipmsxPreferenceType\": \"Topic\", }";

      string mapping = @"{'MappingRuleConfig':{'TruthTable':[{'SourceColumn':'','DestinationColumn':'Output','DataType':'string','ComplexType':{'DataType':'string','Node':'$','TruthTable':[{'SourceColumn':'','DestinationColumn':'PreferenceTopic','DataType':'string','TransformValue':{'Type':'function','CompareToValue':'$.JobTitle','ReturnValue':'$.MobilePhone','DefaultValue':'$.mipmsxPreferenceTopic','Function':'ReplaceValue','Params':['$.mipmsxPreferenceType']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Output\":\"{\\r\\n  \\\"PreferenceTopic\\\": \\\"MOD\\\"\\r\\n}\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is replaced value When Path is given.")]
    public void FunctionTest_ReplaceValueNode_JsonPath_EmptyCompareValue()
    {
      //Arrange - Certain input json with complex object

      string inputJson = "{ \"mipmsxPreferenceTopic\": \"MOD\", \"JobTitle\": \"Webmaster and Records Manager\", \"BusinessPhone\": \"+15563213567\", \"MobilePhone\": 1234565430, \"CountryOrRegion\": \"United Sates\", \"mipmsxPreferenceType\": null, }";

      string mapping = @"{'MappingRuleConfig':{'TruthTable':[{'SourceColumn':'','DestinationColumn':'Output','DataType':'string','ComplexType':{'DataType':'string','Node':'$','TruthTable':[{'SourceColumn':'','DestinationColumn':'PreferenceTopic','DataType':'string','TransformValue':{'Type':'function','CompareToValue':'$.JobTitle','ReturnValue':'$.MobilePhone','DefaultValue':'$.mipmsxPreferenceTopic','Function':'ReplaceValue','Params':['$.mipmsxPreferenceType']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Output\":\"{\\r\\n  \\\"PreferenceTopic\\\": \\\"MOD\\\"\\r\\n}\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is replaced value When Path is given.")]
    public void FunctionTest_ReplaceValue_ReturnValue_Node()
    {
      //Arrange - Certain input json with complex object

      string inputJson = "{ \"mipmsxPreferenceTopic\": \"MOD\", \"JobTitle\": \"Webmaster and Records Manager\", \"BusinessPhone\": \"+15563213567\", \"MobilePhone\": 1234565430, \"CountryOrRegion\": \"United Sates\", \"mipmsxPreferenceType\": \"Topic\", }";

      string mapping = @"{'MappingRuleConfig':{'TruthTable':[{'SourceColumn':'','DestinationColumn':'Output','DataType':'string','ComplexType':{'DataType':'string','Node':'$','TruthTable':[{'SourceColumn':'','DestinationColumn':'PreferenceTopic','DataType':'string','TransformValue':{'Type':'function','CompareToValue':'$.JobTitle','ReturnValue':'$.MobilePhone','DefaultValue':'$.mipmsxPreferenceTopic','Function':'ReplaceValue','Params':['$.mipmsxPreferenceType']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Output\":\"{\\r\\n  \\\"PreferenceTopic\\\": \\\"MOD\\\"\\r\\n}\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is replaced value When Path is given.")]
    public void FunctionTest_ReplaceValueNodeTwo()
    {
      //Arrange - Certain input json with complex object

      string inputJson = "{ \"mipmsxPreferenceTopic\": \"MOD\", \"JobTitle\": \"Webmaster and Records Manager\", \"BusinessPhone\": \"+15563213567\", \"MobilePhone\": \"+1234565430\", \"CountryOrRegion\": \"United Sates\", \"mipmsxPreferenceType\": \"Mswide\", }";

      string mapping = @"{'MappingRuleConfig':{'TruthTable':[{'SourceColumn':'','DestinationColumn':'Output','DataType':'string','ComplexType':{'DataType':'string','Node':'$','TruthTable':[{'SourceColumn':'','DestinationColumn':'PreferenceTopic','DataType':'string','TransformValue':{'Type':'function','CompareToValue':'MSWide','ReturnValue':'','DefaultValue':'$.mipmsxPreferenceTopic','Function':'ReplaceValue','Params':['$.mipmsxPreferenceType']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Output\":\"{\\r\\n  \\\"PreferenceTopic\\\": \\\"\\\"\\r\\n}\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate the value returned when compareToValue and input value are both null.")]
    public void FunctionTest_ReplaceValue_NullCompareToValueAndInputValue_RegexTest()
    {
      //Arrange - Certain input json 
      string inputJson = @"{'MessageBody': {'address': null}}";
      string mapping = @"{'MappingRuleConfig': {  'TruthTable': [{'SourceColumn': '$.MessageBody.address','DestinationColumn': 'address','DataType': 'String','ComplexType': null,'TransformValue': {'Type': 'function','CompareToValue': null ,'ReturnValue': 'def' ,'DefaultValue': 'fgh','Function': 'ReplaceValueWithRegexComparison','Params': ['$.MessageBody.address']}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"address\":\"def\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate the value returned when compareToValue is null and inputvalue is not null.")]
    public void FunctionTest_ReplaceValue_NulllCompareToValue_RegexTest()
    {
      //Arrange - Certain input json 
      string inputJson = @"{'MessageBody': {'address': 'abc'}}";
      string mapping = @"{'MappingRuleConfig': {  'TruthTable': [{'SourceColumn': '$.MessageBody.address','DestinationColumn': 'address','DataType': 'String','ComplexType': null,'TransformValue': {'Type': 'function','CompareToValue': null ,'ReturnValue': null ,'DefaultValue': '$.MessageBody.address','Function': 'ReplaceValueWithRegexComparison','Params': ['$.MessageBody.address']}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"address\":\"abc\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate the value returned when compareToValue is empty.")]
    public void FunctionTest_ReplaceValue_EmptyCompareToValue_RegexTest()
    {
      //Arrange - Certain input json 
      string inputJson = @"{'MessageBody': {'address': 'abc'}}";
      string mapping = @"{'MappingRuleConfig': {  'TruthTable': [{'SourceColumn': '$.MessageBody.address','DestinationColumn': 'address','DataType': 'String','ComplexType': null,'TransformValue': {'Type': 'function','CompareToValue': '' ,'ReturnValue': 'def' ,'DefaultValue': '$.MessageBody.address','Function': 'ReplaceValueWithRegexComparison','Params': ['$.MessageBody.address']}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"address\":\"def\"}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate the value returned when compareToValue is a nonempty,non null regex.")]
    public void FunctionTest_ReplaceValue_RegexTest()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{'eventId':'2353','externalKey':'AP-Azure','accountCode':'C-and-E','eventName':'Windows10','eventQuestions':{'question':[{'questionField':'evt_ans_field19','questionId':638,'questionCode':'Design-LPHeroImg','questionName':'Design-LPHeroImg','answers':{'answer':[{'answerId':null,'answerCode':null,'value':'https://info.microsoft.com'}]}},{'questionField':'evt_ans_field24','questionId':996,'questionCode':'Design-EmailBannerImg','questionName':'Design-EmailBannerImg','answers':{'answer':[{'answerId':null,'answerCode':null,'value':'Firstanswer'},{'answerId':null,'answerCode':null,'value':'Lastanswer'}]}},{'questionField':'evt_ans_field24','questionId':996,'questionCode':'Design-EmailBannerImg','questionName':'Design-EmailBannerImg','answers':{'answer':[{'answerId':null,'answerCode':null,'value':''}]}}]}}";
      string mapping = @"{'MappingRuleConfig':{'TruthTable':[{'SourceColumn':'','DestinationColumn':'EventList','DataType':'string','ComplexType':{'DataType':'JArray','Node':'$.eventQuestions.question[*]','TruthTable':[{'SourceColumn':'','DestinationColumn':'title','DataType':'bool','TransformValue':{'Type':'function','CompareToValue':'[a-z]+.microsoft.com','ReturnValue':'false','DefaultValue':'true','Function':'ReplaceValueWithRegexComparison','Params':['$.answers.answer[*].value']}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"EventList\":[{\"title\":false},{\"title\":true},{\"title\":true}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Value handler test - Validate transformation if ignoreNull is set as false.")]
    public void Automapper_IgnoreNullFalseTest()
    {
      //Arrange - Certain input json with complex object
      string inputJson = "{ \"etag\": \"1-null\", \"transactionId\": null, \"completeCollectionSize\": 25, \"size\": 3, \"startingIndex\": 1, \"maxResults\": 3, \"questions\": [ { \"accountCode\": \"C-and-E\", \"eventCode\": \"tmpl-polish-devcl\", \"questionName\": \"Hero Banner Image\", \"questionLabel\": \"Hero Banner Image\", \"questionCode\": \"Hero Banner Image\", \"questionType\": \"Image\", \"answer\": [ { \"answerName\": \"answerNameXYZ\", \"answerLabel\": \"answerLabelXYZ\", \"answerCode\": \"answerCodeXYZ\" } ] }, { \"accountCode\": \"C-and-E\", \"eventCode\": \"tmpl-polish-devcl\", \"questionName\": \"Program-HeaderText\", \"questionLabel\": \"Program-HeaderText\", \"questionCode\": \"Program-HeaderText\", \"questionType\": \"Text\", \"answer\": [ ] }, { \"accountCode\": \"C-and-E\", \"eventCode\": \"tmpl-polish-devcl\", \"questionName\": \"Program-Description\", \"questionLabel\": \"Program-Description (Registration Form - Paragraph 1)\", \"questionCode\": \"Program-Description\", \"questionType\": \"Textarea\", \"answer\": [ ] } ] }";
      string mapping =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"EventId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"DefaultValue\": \"-1\" } }, { \"SourceColumn\": \"$.MifContext.BatchId\", \"DestinationColumn\": \"BatchId\", \"DataType\": \"guid\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.correlationId\", \"DestinationColumn\": \"RequestId\", \"DataType\": \"guid\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.moreEventQuestions\", \"DestinationColumn\": \"MoreResults\", \"DataType\": \"bool\", \"ComplexType\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"SchemaName\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"DefaultValue\": \"JournalEvents.Certain.EventQuestionAnswersDetail\" } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"Content\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"ContentObject\", \"DataType\": \"string\", \"ComplexType\": { \"DataType\": \"JArray\", \"IgnoreNullValue\": false, \"Node\": \"$.questions[*].answer\", \"TruthTable\": [ { \"SourceColumn\": \"$.[{parent}].[{parent}].[{parent}].accountCode\", \"DestinationColumn\": \"accountCode\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.[{parent}].[{parent}].[{parent}].eventCode\", \"DestinationColumn\": \"eventCode\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.[{parent}].[{parent}].[{parent}].questionName\", \"DestinationColumn\": \"questionName\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.[{parent}].[{parent}].[{parent}].questionLabel\", \"DestinationColumn\": \"questionLabel\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.[{parent}].[{parent}].[{parent}].questionCode\", \"DestinationColumn\": \"questionCode\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.[{parent}].[{parent}].[{parent}].questionType\", \"DestinationColumn\": \"questionType\", \"DataType\": \"string\" }, { \"SourceColumn\": \"answerLabel\", \"DestinationColumn\": \"answerLabel\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": null } }, { \"SourceColumn\": \"answerCode\", \"DestinationColumn\": \"answerCode\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": null } }, { \"SourceColumn\": \"answerName\", \"DestinationColumn\": \"answerName\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": null } } ] } } ] } }";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(JObject.Parse(inputJson), true);
      string expectedResponse = "{ \"EventId\": \"-1\", \"SchemaName\": \"JournalEvents.Certain.EventQuestionAnswersDetail\", \"ContentObject\": [ { \"accountCode\": \"C-and-E\", \"eventCode\": \"tmpl-polish-devcl\", \"questionName\": \"Hero Banner Image\", \"questionLabel\": \"Hero Banner Image\", \"questionCode\": \"Hero Banner Image\", \"questionType\": \"Image\", \"answerLabel\": \"answerLabelXYZ\", \"answerCode\": \"answerCodeXYZ\", \"answerName\": \"answerNameXYZ\" }, { \"accountCode\": \"C-and-E\", \"eventCode\": \"tmpl-polish-devcl\", \"questionName\": \"Program-HeaderText\", \"questionLabel\": \"Program-HeaderText\", \"questionCode\": \"Program-HeaderText\", \"questionType\": \"Text\", \"answerLabel\": null, \"answerCode\": null, \"answerName\": null }, { \"accountCode\": \"C-and-E\", \"eventCode\": \"tmpl-polish-devcl\", \"questionName\": \"Program-Description\", \"questionLabel\": \"Program-Description (Registration Form - Paragraph 1)\", \"questionCode\": \"Program-Description\", \"questionType\": \"Textarea\", \"answerLabel\": null, \"answerCode\": null, \"answerName\": null } ] }";
      //Assert
      Assert.IsTrue(JToken.DeepEquals(response, JObject.Parse(expectedResponse)));
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is first index after split")]
    public void FunctionTest_SplitWithOneIndex()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""LeadId"":""2353"",""FirstName"":""AP-Azure"",""LastName"":""C-and-E"",""EmailAddress"":""Windows 10"",""Key"":""12121212333_AKJKKAJDKAKJKKKDKSKAK""}";
      string mapping = @"{""MappingRuleConfig"":{""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""Key"",""DataType"":""string"",""ComplexType"":{""DataType"":""jArray"",""Node"":""$"",""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""Key"",""DataType"":""string"",""TransformValue"":{""Type"":""function"",""Delimeter"":""_"",""Index"":0,""Function"":""Split"",""Params"":[""$.Key""]}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Key\":[{\"Key\":\"12121212333\"}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate that the value returned is first and second index after split")]
    public void FunctionTest_SplitWithTwoIndex()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""LeadId"":""2353"",""FirstName"":""AP-Azure"",""LastName"":""C-and-E"",""EmailAddress"":""Windows 10"",""Key"":""12121212333_AKJKKAJDKAKJKKKDKSKAK""}";
      string mapping = @"{""MappingRuleConfig"":{""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""Data"",""DataType"":""string"",""ComplexType"":{""DataType"":""jArray"",""Node"":""$"",""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""Key"",""DataType"":""string"",""TransformValue"":{""Type"":""function"",""Delimeter"":""_"",""Index"":0,""Function"":""Split"",""Params"":[""$.Key""]}},{""SourceColumn"":"""",""DestinationColumn"":""Message"",""DataType"":""string"",""TransformValue"":{""Type"":""function"",""Delimeter"":""_"",""Index"":1,""Function"":""Split"",""Params"":[""$.Key""]}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Data\":[{\"Key\":\"12121212333\",\"Message\":\"AKJKKAJDKAKJKKKDKSKAK\"}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate Upper case functionality")]
    public void FunctionTest_TransformToUpperCase()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""LeadId"":""2353"",""FirstName"":""AP-Azure"",""LastName"":""C-and-E"",""EmailAddress"":""Windows 10"",""Key"":""12121212333_AKJKKAJDKAKJKKKDKSKAK""}";
      string mapping = @"{""MappingRuleConfig"":{""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""Data"",""DataType"":""string"",""ComplexType"":{""DataType"":""jArray"",""Node"":""$"",""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""EmailAddress"",""DataType"":""string"",""TransformValue"":{""Type"":""function"",""Function"":""TOUPPERCASE"",""Params"":[""$.EmailAddress""]}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Data\":[{\"EmailAddress\":\"WINDOWS 10\"}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate Upper case functionality in Parent Node")]
    public void FunctionTest_TransformToUpperCaseParent()
    {
      //Arrange - Certain input json with complex object
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
      var expectedResponse = "{\"Data\":[{\"EmailAddress\":\"WINDOWS 10\",\"FirstName\":\"AP - Azure\",\"LastName\":\"C - AND - E\"}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Function handler test - Validate Upper case functionality")]
    public void FunctionTest_TransformToLowerCase()
    {
      //Arrange - Certain input json with complex object
      string inputJson = @"{""LeadId"":""2353"",""FirstName"":""AP-Azure"",""LastName"":""C-and-E"",""EmailAddress"":""Windows 10"",""Key"":""12121212333_AKJKKAJDKAKJKKKDKSKAK""}";
      string mapping = @"{""MappingRuleConfig"":{""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""Data"",""DataType"":""string"",""ComplexType"":{""DataType"":""jArray"",""Node"":""$"",""TruthTable"":[{""SourceColumn"":"""",""DestinationColumn"":""EmailAddress"",""DataType"":""string"",""TransformValue"":{""Type"":""function"",""Function"":""TOLOWERCASE"",""Params"":[""$.EmailAddress""]}}]}}]}}";

      var mapper = new AutoMapper(mapping);

      //Act
      var response = mapper.TransformIntoJson(inputJson, true);
      var expectedResponse = "{\"Data\":[{\"EmailAddress\":\"windows 10\"}]}";

      //Assert
      Assert.AreEqual(response, expectedResponse);
    }

    [TestMethod, Description("Validate that the datatype is set to input type if mapping datatype is set to null.")]
    public void Validate_AutoMapper_NullDataType()
    {
      // Arrange
      string mappingJson =
        "{\"MappingRuleConfig\":{\"DestinationType\": \"MipUnitTest.LeadRecord, JsonToJsonMapper.Core.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"TruthTable\": [{\"SourceColumn\": \"$.Name\",\"DestinationColumn\": \"Name\",\"DataType\": null,\"ComplexType\": null,\"TransformValue\": {\"ValueMapping\":[{\"ExistingValue\": \"1\",\"NewValue\": true},{\"ExistingValue\": \"2\",\"NewValue\": false}],\"DefaultValue\":\"NA\"}},{\"SourceColumn\": \"LeadScore\",\"DestinationColumn\": \"LeadScore\",\"DataType\": null,\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"$.AllowEmail\",\"DestinationColumn\": \"AllowEmail\",\"DataType\": null,\"ComplexType\": null,\"TransformValue\": null},{\"SourceColumn\": \"CrmId\",\"DestinationColumn\": \"CrmId\",\"DataType\":  null,\"ComplexType\": null,\"TransformValue\": null}]}}";

      string inputJson =
        "{\"Name\": \"22\",\"AllowEmail\": false,\"CrmId\": null,\"LeadScore\":100}";

      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        LeadRecord lead = (LeadRecord)mapper.Transform(inputJson);
        Assert.IsTrue(lead.LeadScore == 100);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod, Description("Validate that the JObject and JArray are created With same Flattene Object.")]
    public void AutoMappperMappingForJObjectAndJarray()
    {
      // Arrange
      string inputJson =
        "{ \"ProgramName\": \"test4\", \"ProgramFolderId\": \"49\", \"ProgramFolderType\": \"Folder\", \"ProgramDescription\": \"xcvxbv\", \"ProgramType\": \"Default\", \"ProgramChannel\": \"Email Blast\", \"ProgramCosts\": \"8360\", \"ProgramStartDate\": \"2015-01-01\" }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"DestinationType\": \"JObject\", \"TruthTable\": [ { \"SourceColumn\": \"$.ProgramName\", \"DestinationColumn\": \"ProgramName\", \"DataType\": \"string\" }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"Folder\", \"DataType\": \"JObject\", \"ComplexType\": { \"Node\": \"$\", \"TruthTable\": [ { \"SourceColumn\": \"$.ProgramFolderId\", \"DestinationColumn\": \"id\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.ProgramFolderType\", \"DestinationColumn\": \"type\", \"DataType\": \"string\" } ] } }, { \"SourceColumn\": \"$.ProgramDescription\", \"DestinationColumn\": \"ProgramDescription\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.ProgramType\", \"DestinationColumn\": \"ProgramType\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.ProgramChannel\", \"DestinationColumn\": \"ProgramChannel\", \"DataType\": \"string\" }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"Costs\", \"ComplexType\": { \"DataType\": \"JArray\", \"Node\": \"$\", \"IgnoreEmptyArray\": true, \"TruthTable\": [ { \"SourceColumn\": \"$.ProgramStartDate\", \"DestinationColumn\": \"startDate\", \"DataType\": \"string\" }, { \"SourceColumn\": \"$.ProgramCosts\", \"DestinationColumn\": \"cost\", \"DataType\": \"string\" } ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(inputJson);
        string expectedOutput = "{\"ProgramName\":\"test4\",\"Folder\":{\"id\":\"49\",\"type\":\"Folder\"},\"ProgramDescription\":\"xcvxbv\",\"ProgramType\":\"Default\",\"ProgramChannel\":\"Email Blast\",\"Costs\":[{\"startDate\":\"2015-01-01\",\"cost\":\"8360\"}]}";

        Assert.AreEqual(output, expectedOutput);
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, Description("Validate that the datetime format is not changed after transformation.")]
    public void AutoMappperMapping_ValidateDateTimeFormat()
    {
      // Arrange
      string inputJson =
        "{'Content':[{'Date':'2015-11-30T00:00:00','LongFormattedDate':'Monday,November30','ShortFormattedDate':'11/30/2015','DateString':null,'DateIndex':1}],'MifContext':{'runImmediate':true,'runId':'97216631-3453-4326-9a83-d5c83c62c2e6','TrackingEnabled':false,'WorkFlowName':'EPDaysToEDP','programId':'52aa95a6-831a-e511-ab0e-00155d5066d7','programCode':'conv2015emea','correlationId':'e62812d0-7d19-4496-bd9a-8a5202fa8750'},'ForEachContext':{'Url':'https://eventpoint-conv2015emea-services.azurewebsites.net','APIKey':'050e1a02033e48319ac9fd051452a484'}";

      string mappingJson =
        "{'MappingRuleConfig': {'TruthTable': [{'SourceColumn': '','DestinationColumn': 'EventId','DataType': 'string','ComplexType': null,'TransformValue': {'DefaultValue': '-1'}},{'SourceColumn': '$.MifContext.correlationId','DestinationColumn': 'BatchId','DataType': 'guid','ComplexType': null,'TransformValue': null},{'SourceColumn': '','DestinationColumn': 'RequestId','DataType': 'guid','ComplexType': null,'TransformValue': {'DefaultValue': 'newGuid'}},{'SourceColumn': '','DestinationColumn': 'MoreResults','DataType': 'bool','ComplexType': null,'TransformValue': {'DefaultValue': 'false'}},{'SourceColumn': '','DestinationColumn': 'SchemaName','DataType': 'string','ComplexType': null,'TransformValue': {'DefaultValue': 'JournalEvents.EventPoint.ProgramDay'}},{'SourceColumn': '','DestinationColumn': 'ContentObject','DataType': 'JArray','ComplexType': {'DataType': 'JArray','Node': '$.Content[*]','TruthTable': [{'SourceColumn': '$.[{parent}].[{parent}].[{parent}].MifContext.programId','DestinationColumn': 'ProgramId','DataType': 'string'},{'SourceColumn': '$.[{parent}].[{parent}].[{parent}].MifContext.programCode','DestinationColumn': 'ProgramCode','DataType': 'string'},{'SourceColumn': '$.Date','DestinationColumn': 'ProgramDayDate','DataType': 'string'},{'SourceColumn': '$.Date','DestinationColumn': 'LongFormattedDate','DataType': 'formattedDatetime','Format': 'yyyy-MM-ddTHH:mm:ssZ',},{'SourceColumn': '$.ShortFormattedDate','DestinationColumn': 'ShortFormattedDate','DataType': 'string'},{'SourceColumn': '$.DateString','DestinationColumn': 'DateString','DataType': 'string'},{'SourceColumn': '$.DateIndex','DestinationColumn': 'DateIndex','DataType': 'string'}]}},{'SourceColumn': '','DestinationColumn': 'CreatedDate','DataType': 'formattedDatetime','Format': 'yyyy-MM-ddTHH:mm:ssZ','TransformValue': {'DefaultValue': 'utcNow'}},{'SourceColumn': '','DestinationColumn': 'EventDate','DataType': 'formattedDatetime','Format': 'yyyy-MM-ddTHH:mm:ssZ','TransformValue': {'DefaultValue': 'utcNow'}}]}}";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);

        Assert.AreEqual(output["ContentObject"][0]["ProgramDayDate"].ToString(), "2015-11-30T00:00:00");
        Assert.AreEqual(output["ContentObject"][0]["ShortFormattedDate"].ToString(), "11/30/2015");
      }
      catch (Exception)
      {
        Assert.Fail();
      }
    }

    [TestMethod, Description("Validate that the concatenation where nulls are present.")]
    public void AutoMappperMapping_ConcatWithNull()
    {
      // Arrange
      string inputJson =
        "{ \"batchId\": \"d7620d3b-188a-49a0-b540-29de82b6ae1c\", \"messageCorrelationId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"responseId\": \"bcf0f3d0-7053-4924-b773-11776daf7692\", \"success\": false, \"email\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"data\": { \"email\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"success\": false, \"errors\": { \"message\": \"Validation Failed.\", \"type\": \"Validation Error\", \"validationErrors\": [ { \"message\": \"PropertyRequired for action at #/action\" } ], \"innerValidationErrors\":[] } }, \"MifContext\": { \"runImmediate\": true, \"msitTelemetryConfig\": { \"eventProperties\": { \"BusinessProcessName\": \"SMIT.MKTGLeadFlow.AquireLead\", \"SenderId\": \"MOD\" } }, \"runId\": \"5852ac81-690a-40df-817d-b1c3a4a19687\", \"TrackingEnabled\": true, \"WorkFlowName\": \"ModLeadIngestion\", \"Request\": { \"batchId\": \"d7620d3b-188a-49a0-b540-29de82b6ae1c\", \"input\": [ { \"requestId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"data\": { \"o365TrialUsageScoreProbability\": 29, \"o365TrialUsageScorePercentile\": 18, \"originatingTenantID\": \"1e9130f9-cccd-4ee2-8b3a-9774ce790519\", \"mOSEmailPref\": true, \"mOSPhonePref\": true, \"mOSPartnerEmailPref\": false, \"mOSPartnerPhonePref\": false, \"firstName\": \"John\", \"lastName\": \"Doe\", \"mobilePhone\": \"\", \"phone\": \"4257056728\", \"leadImportSource\": \"Active Trial\", \"leadSource\": \"Active Trial\", \"address\": \"Line1\", \"city\": \"Bellevue\", \"company\": \"TEST_TEST_OCP_22b3da66microsoft\", \"countryCode\": \"US\", \"postalCode\": \"98005\", \"state\": \"WA\", \"originatingOfferID\": \"B07A1127-DE83-4A6D-9F85-2C104BDAE8B4\", \"originatingProductSKU\": \"Office 365 Enterprise E3 Trial\", \"originatingSubscriptionID\": \"491015935db94a51a0510bdf80dd2a1e\", \"mSPartner\": \"True\", \"numberOfEmployees\": \"12\", \"messageType\": \"ActiveTrialUserInfoV2\", \"email\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"originSystemName\": \"Mod\", \"asmUpdatedAt\": \"2016-09-18T01:14:14.4503257Z\", \"mosSubscriptions\": [ { \"subscriptionId\": \"819aa449-f3c5-4e21-ab4e-370dc859000bFT\", \"globalAdminLogin\": \"admintest\", \"hasPOR\": true, \"includedQuantity\": 1, \"isEXO\": false, \"isLYO\": true, \"isODB\": true, \"isProject\": false, \"isProPlus\": true, \"isSPO\": false, \"isTrialSubscription\": true, \"isVisio\": true, \"isYammer\": true, \"offerId\": \"32456\", \"offerName\": \"testOffer\", \"offerProductFamily\": \"testFamily\", \"offerProductName\": \"testProduct1\", \"orderId\": \"34532\", \"partnerId\": \"35675\", \"subscriptionCreatedDate\": \"2016-08-11T10:00:00Z\", \"subscriptionCurrentStatus\": \"2016-08-14T10:00:00Z\", \"subscriptionEndDate\": \"2016-08-14T10:00:00Z\", \"subscriptionStartDate\": \"2016-08-11T10:00:00Z\", \"subscriptionTypeName\": \"testSubscription\", \"subscriptionUpdatedDate\": \"2016-08-14T10:00:00Z\", \"tenantId\": \"34683\" } ], \"tenants\": [ { \"tenantid\": \"c9674688-99dd-4a96-a191-2bed92255b15FT\", \"hascustomdomain\": false, \"firstdomaincreateddate\": \"2016-08-11T10:00:00Z\", \"domaincount\": 200, \"haseducation\": true, \"hasexchange\": true, \"haslync\": true, \"hassharepoint\": true, \"hasyammer\": false, \"hassubscription\": true, \"hastrial\": true, \"haspaid\": false, \"hasproject\": true, \"hasvisio\": true, \"tenantcreateddate\": \"2016-08-11T10:00:00Z\", \"tenantcurrentstatus\": \"testing\", \"tenantlastupdateddate\": \"2016-08-11T10:00:00Z\", \"tenantcountrycode\": \"IN\", \"office365instance\": \"testingOffice\", \"haspor\": false, \"paidincludedquantity\": 1 } ] }, \"XCV\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\" } ], \"MifContext\": { \"runImmediate\": true, \"msitTelemetryConfig\": { \"eventProperties\": { \"BusinessProcessName\": \"SMIT.MKTGLeadFlow.AquireLead\", \"SenderId\": \"MOD\" } }, \"runId\": \"5852ac81-690a-40df-817d-b1c3a4a19687\", \"TrackingEnabled\": true, \"WorkFlowName\": \"ModLeadIngestion\" } }, \"messageId\": \"bcf0f3d0-7053-4924-b773-11776daf7692\", \"requestId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\" }, \"ForEachContext\": { \"requestId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"data\": { \"o365TrialUsageScoreProbability\": 29, \"o365TrialUsageScorePercentile\": 18, \"originatingTenantID\": \"1e9130f9-cccd-4ee2-8b3a-9774ce790519\", \"mOSEmailPref\": true, \"mOSPhonePref\": true, \"mOSPartnerEmailPref\": false, \"mOSPartnerPhonePref\": false, \"firstName\": \"John\", \"lastName\": \"Doe\", \"mobilePhone\": \"\", \"phone\": \"4257056728\", \"leadImportSource\": \"Active Trial\", \"leadSource\": \"Active Trial\", \"address\": \"Line1\", \"city\": \"Bellevue\", \"company\": \"TEST_TEST_OCP_22b3da66microsoft\", \"countryCode\": \"US\", \"postalCode\": \"98005\", \"state\": \"WA\", \"originatingOfferID\": \"B07A1127-DE83-4A6D-9F85-2C104BDAE8B4\", \"originatingProductSKU\": \"Office 365 Enterprise E3 Trial\", \"originatingSubscriptionID\": \"491015935db94a51a0510bdf80dd2a1e\", \"mSPartner\": \"True\", \"numberOfEmployees\": \"12\", \"messageType\": \"ActiveTrialUserInfoV2\", \"email\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"originSystemName\": \"Mod\", \"asmUpdatedAt\": \"2016-09-18T01:14:14.4503257Z\", \"mosSubscriptions\": [ { \"subscriptionId\": \"819aa449-f3c5-4e21-ab4e-370dc859000bFT\", \"globalAdminLogin\": \"admintest\", \"hasPOR\": true, \"includedQuantity\": 1, \"isEXO\": false, \"isLYO\": true, \"isODB\": true, \"isProject\": false, \"isProPlus\": true, \"isSPO\": false, \"isTrialSubscription\": true, \"isVisio\": true, \"isYammer\": true, \"offerId\": \"32456\", \"offerName\": \"testOffer\", \"offerProductFamily\": \"testFamily\", \"offerProductName\": \"testProduct1\", \"orderId\": \"34532\", \"partnerId\": \"35675\", \"subscriptionCreatedDate\": \"2016-08-11T10:00:00Z\", \"subscriptionCurrentStatus\": \"2016-08-14T10:00:00Z\", \"subscriptionEndDate\": \"2016-08-14T10:00:00Z\", \"subscriptionStartDate\": \"2016-08-11T10:00:00Z\", \"subscriptionTypeName\": \"testSubscription\", \"subscriptionUpdatedDate\": \"2016-08-14T10:00:00Z\", \"tenantId\": \"34683\" } ], \"tenants\": [ { \"tenantid\": \"c9674688-99dd-4a96-a191-2bed92255b15FT\", \"hascustomdomain\": false, \"firstdomaincreateddate\": \"2016-08-11T10:00:00Z\", \"domaincount\": 200, \"haseducation\": true, \"hasexchange\": true, \"haslync\": true, \"hassharepoint\": true, \"hasyammer\": false, \"hassubscription\": true, \"hastrial\": true, \"haspaid\": false, \"hasproject\": true, \"hasvisio\": true, \"tenantcreateddate\": \"2016-08-11T10:00:00Z\", \"tenantcurrentstatus\": \"testing\", \"tenantlastupdateddate\": \"2016-08-11T10:00:00Z\", \"tenantcountrycode\": \"IN\", \"office365instance\": \"testingOffice\", \"haspor\": false, \"paidincludedquantity\": 1 } ] }, \"XCV\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"batchId\": \"d7620d3b-188a-49a0-b540-29de82b6ae1c\" } }  ";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"batchId\", \"DestinationColumn\": \"batchId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"messageCorrelationId\", \"DestinationColumn\": \"messageCorrelationId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"responseId\", \"DestinationColumn\": \"responseId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"success\", \"DestinationColumn\": \"success\", \"DataType\": \"bool\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"email\", \"DestinationColumn\": \"EmailAddress\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"data\", \"DataType\": \"JObject\", \"ComplexType\": { \"DataType\": \"JObject\", \"Node\": \"$\", \"TruthTable\": [ { \"SourceColumn\": \"email\", \"DestinationColumn\": \"EmailAddress\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"success\", \"DataType\": \"bool\", \"ComplexType\": null, \"TransformValue\": { \"DefaultValue\": false } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"errors\", \"DataType\": \"JArray\", \"ComplexType\": { \"DataType\": \"JArray\", \"Node\": \"$\", \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": \"Validation Failed.\" } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"type\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": \"Validation Error\" } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"validationErrors\", \"DataType\": \"JArray\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \",\", \"Function\": \"Concat\", \"Params\": [ \"$.data.errors.validationErrors[*].message\", \"$.data.errors.innerValidationErrors[*].message\" ] } } ] } } ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"batchId\": \"d7620d3b-188a-49a0-b540-29de82b6ae1c\", \"messageCorrelationId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"responseId\": \"bcf0f3d0-7053-4924-b773-11776daf7692\", \"success\": false, \"EmailAddress\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"data\": { \"EmailAddress\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"success\": false, \"errors\": [ { \"message\": \"Validation Failed.\", \"type\": \"Validation Error\", \"validationErrors\": [ \"PropertyRequired for action at #/action\" ] } ] } } ";
        Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate that the concatenation where nulls are present.")]
    public void AutoMappperMapping_ConcatWithEmptyList()
    {
      // Arrange
      string inputJson =
        "{ \"batchId\": \"d7620d3b-188a-49a0-b540-29de82b6ae1c\", \"messageCorrelationId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"responseId\": \"bcf0f3d0-7053-4924-b773-11776daf7692\", \"success\": false, \"email\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"data\": { \"email\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"success\": false, \"errors\": { \"message\": \"Validation Failed.\", \"type\": \"Validation Error\", \"validationErrors1\": [ { \"message1\": \"PropertyRequired for action at #/action\" } ], \"innerValidationErrors\":[] } }, \"MifContext\": { \"runImmediate\": true, \"msitTelemetryConfig\": { \"eventProperties\": { \"BusinessProcessName\": \"SMIT.MKTGLeadFlow.AquireLead\", \"SenderId\": \"MOD\" } }, \"runId\": \"5852ac81-690a-40df-817d-b1c3a4a19687\", \"TrackingEnabled\": true, \"WorkFlowName\": \"ModLeadIngestion\", \"Request\": { \"batchId\": \"d7620d3b-188a-49a0-b540-29de82b6ae1c\", \"input\": [ { \"requestId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"data\": { \"o365TrialUsageScoreProbability\": 29, \"o365TrialUsageScorePercentile\": 18, \"originatingTenantID\": \"1e9130f9-cccd-4ee2-8b3a-9774ce790519\", \"mOSEmailPref\": true, \"mOSPhonePref\": true, \"mOSPartnerEmailPref\": false, \"mOSPartnerPhonePref\": false, \"firstName\": \"John\", \"lastName\": \"Doe\", \"mobilePhone\": \"\", \"phone\": \"4257056728\", \"leadImportSource\": \"Active Trial\", \"leadSource\": \"Active Trial\", \"address\": \"Line1\", \"city\": \"Bellevue\", \"company\": \"TEST_TEST_OCP_22b3da66microsoft\", \"countryCode\": \"US\", \"postalCode\": \"98005\", \"state\": \"WA\", \"originatingOfferID\": \"B07A1127-DE83-4A6D-9F85-2C104BDAE8B4\", \"originatingProductSKU\": \"Office 365 Enterprise E3 Trial\", \"originatingSubscriptionID\": \"491015935db94a51a0510bdf80dd2a1e\", \"mSPartner\": \"True\", \"numberOfEmployees\": \"12\", \"messageType\": \"ActiveTrialUserInfoV2\", \"email\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"originSystemName\": \"Mod\", \"asmUpdatedAt\": \"2016-09-18T01:14:14.4503257Z\", \"mosSubscriptions\": [ { \"subscriptionId\": \"819aa449-f3c5-4e21-ab4e-370dc859000bFT\", \"globalAdminLogin\": \"admintest\", \"hasPOR\": true, \"includedQuantity\": 1, \"isEXO\": false, \"isLYO\": true, \"isODB\": true, \"isProject\": false, \"isProPlus\": true, \"isSPO\": false, \"isTrialSubscription\": true, \"isVisio\": true, \"isYammer\": true, \"offerId\": \"32456\", \"offerName\": \"testOffer\", \"offerProductFamily\": \"testFamily\", \"offerProductName\": \"testProduct1\", \"orderId\": \"34532\", \"partnerId\": \"35675\", \"subscriptionCreatedDate\": \"2016-08-11T10:00:00Z\", \"subscriptionCurrentStatus\": \"2016-08-14T10:00:00Z\", \"subscriptionEndDate\": \"2016-08-14T10:00:00Z\", \"subscriptionStartDate\": \"2016-08-11T10:00:00Z\", \"subscriptionTypeName\": \"testSubscription\", \"subscriptionUpdatedDate\": \"2016-08-14T10:00:00Z\", \"tenantId\": \"34683\" } ], \"tenants\": [ { \"tenantid\": \"c9674688-99dd-4a96-a191-2bed92255b15FT\", \"hascustomdomain\": false, \"firstdomaincreateddate\": \"2016-08-11T10:00:00Z\", \"domaincount\": 200, \"haseducation\": true, \"hasexchange\": true, \"haslync\": true, \"hassharepoint\": true, \"hasyammer\": false, \"hassubscription\": true, \"hastrial\": true, \"haspaid\": false, \"hasproject\": true, \"hasvisio\": true, \"tenantcreateddate\": \"2016-08-11T10:00:00Z\", \"tenantcurrentstatus\": \"testing\", \"tenantlastupdateddate\": \"2016-08-11T10:00:00Z\", \"tenantcountrycode\": \"IN\", \"office365instance\": \"testingOffice\", \"haspor\": false, \"paidincludedquantity\": 1 } ] }, \"XCV\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\" } ], \"MifContext\": { \"runImmediate\": true, \"msitTelemetryConfig\": { \"eventProperties\": { \"BusinessProcessName\": \"SMIT.MKTGLeadFlow.AquireLead\", \"SenderId\": \"MOD\" } }, \"runId\": \"5852ac81-690a-40df-817d-b1c3a4a19687\", \"TrackingEnabled\": true, \"WorkFlowName\": \"ModLeadIngestion\" } }, \"messageId\": \"bcf0f3d0-7053-4924-b773-11776daf7692\", \"requestId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\" }, \"ForEachContext\": { \"requestId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"data\": { \"o365TrialUsageScoreProbability\": 29, \"o365TrialUsageScorePercentile\": 18, \"originatingTenantID\": \"1e9130f9-cccd-4ee2-8b3a-9774ce790519\", \"mOSEmailPref\": true, \"mOSPhonePref\": true, \"mOSPartnerEmailPref\": false, \"mOSPartnerPhonePref\": false, \"firstName\": \"John\", \"lastName\": \"Doe\", \"mobilePhone\": \"\", \"phone\": \"4257056728\", \"leadImportSource\": \"Active Trial\", \"leadSource\": \"Active Trial\", \"address\": \"Line1\", \"city\": \"Bellevue\", \"company\": \"TEST_TEST_OCP_22b3da66microsoft\", \"countryCode\": \"US\", \"postalCode\": \"98005\", \"state\": \"WA\", \"originatingOfferID\": \"B07A1127-DE83-4A6D-9F85-2C104BDAE8B4\", \"originatingProductSKU\": \"Office 365 Enterprise E3 Trial\", \"originatingSubscriptionID\": \"491015935db94a51a0510bdf80dd2a1e\", \"mSPartner\": \"True\", \"numberOfEmployees\": \"12\", \"messageType\": \"ActiveTrialUserInfoV2\", \"email\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"originSystemName\": \"Mod\", \"asmUpdatedAt\": \"2016-09-18T01:14:14.4503257Z\", \"mosSubscriptions\": [ { \"subscriptionId\": \"819aa449-f3c5-4e21-ab4e-370dc859000bFT\", \"globalAdminLogin\": \"admintest\", \"hasPOR\": true, \"includedQuantity\": 1, \"isEXO\": false, \"isLYO\": true, \"isODB\": true, \"isProject\": false, \"isProPlus\": true, \"isSPO\": false, \"isTrialSubscription\": true, \"isVisio\": true, \"isYammer\": true, \"offerId\": \"32456\", \"offerName\": \"testOffer\", \"offerProductFamily\": \"testFamily\", \"offerProductName\": \"testProduct1\", \"orderId\": \"34532\", \"partnerId\": \"35675\", \"subscriptionCreatedDate\": \"2016-08-11T10:00:00Z\", \"subscriptionCurrentStatus\": \"2016-08-14T10:00:00Z\", \"subscriptionEndDate\": \"2016-08-14T10:00:00Z\", \"subscriptionStartDate\": \"2016-08-11T10:00:00Z\", \"subscriptionTypeName\": \"testSubscription\", \"subscriptionUpdatedDate\": \"2016-08-14T10:00:00Z\", \"tenantId\": \"34683\" } ], \"tenants\": [ { \"tenantid\": \"c9674688-99dd-4a96-a191-2bed92255b15FT\", \"hascustomdomain\": false, \"firstdomaincreateddate\": \"2016-08-11T10:00:00Z\", \"domaincount\": 200, \"haseducation\": true, \"hasexchange\": true, \"haslync\": true, \"hassharepoint\": true, \"hasyammer\": false, \"hassubscription\": true, \"hastrial\": true, \"haspaid\": false, \"hasproject\": true, \"hasvisio\": true, \"tenantcreateddate\": \"2016-08-11T10:00:00Z\", \"tenantcurrentstatus\": \"testing\", \"tenantlastupdateddate\": \"2016-08-11T10:00:00Z\", \"tenantcountrycode\": \"IN\", \"office365instance\": \"testingOffice\", \"haspor\": false, \"paidincludedquantity\": 1 } ] }, \"XCV\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"batchId\": \"d7620d3b-188a-49a0-b540-29de82b6ae1c\" } }  ";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"batchId\", \"DestinationColumn\": \"batchId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"messageCorrelationId\", \"DestinationColumn\": \"messageCorrelationId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"responseId\", \"DestinationColumn\": \"responseId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"success\", \"DestinationColumn\": \"success\", \"DataType\": \"bool\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"email\", \"DestinationColumn\": \"EmailAddress\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"data\", \"DataType\": \"JObject\", \"ComplexType\": { \"DataType\": \"JObject\", \"Node\": \"$\", \"TruthTable\": [ { \"SourceColumn\": \"email\", \"DestinationColumn\": \"EmailAddress\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"success\", \"DataType\": \"bool\", \"ComplexType\": null, \"TransformValue\": { \"DefaultValue\": false } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"errors\", \"DataType\": \"JArray\", \"ComplexType\": { \"DataType\": \"JArray\", \"Node\": \"$\", \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": \"Validation Failed.\" } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"type\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": \"Validation Error\" } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"validationErrors\", \"DataType\": \"JArray\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \",\", \"Function\": \"Concat\", \"Params\": [ \"$.data.errors.validationErrors[*].message\", \"$.data.errors.innerValidationErrors[*].message\" ] } } ] } } ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"batchId\": \"d7620d3b-188a-49a0-b540-29de82b6ae1c\", \"messageCorrelationId\": \"6a87a56f-618c-4d0b-ac96-09adad1f99d5\", \"responseId\": \"bcf0f3d0-7053-4924-b773-11776daf7692\", \"success\": false, \"EmailAddress\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"data\": { \"EmailAddress\": \"P75LIOT4USIPA7RXVMPG@mipFT.com\", \"success\": false, \"errors\": [ { \"message\": \"Validation Failed.\", \"type\": \"Validation Error\", \"validationErrors\": [ \"\" ] } ] } } ";

        Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate that the DateTime offset is removed from the value to be returned.")]
    public void AutoMappperMapping_RemoveDateTimeOffsetTest_WithOffset()
    {
      // Arrange
      string inputJson =
        "{ \"Results\": [ { \"$id\": \"2\", \"Comments\": null, \"MarketingTactic\": null, \"MarketingVehicle\": null, \"PromotionCode\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\", \"Status\": { \"$id\": \"3\", \"OptionSetId\": 0, \"DisplayValue\": \"Active\", \"TypeName\": null, \"Id\": null, \"CreatedOn\": null, \"ModifiedOn\": null, \"CreatedBy\": null, \"ModifiedBy\": null, \"ExtendedId\": null }, \"StatusReason\": { \"$id\": \"4\", \"OptionSetId\": 0, \"DisplayValue\": \"Proposed\", \"TypeName\": null, \"Id\": null, \"CreatedOn\": null, \"ModifiedOn\": null, \"CreatedBy\": null, \"ModifiedBy\": null, \"ExtendedId\": null }, \"TacticEndDate\": \"2015-06-26T15:55:00Z\", \"TacticStartDate\": \"2015-06-26T15:00:00Z\", \"TacticName\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\", \"Owner\": { \"$id\": \"5\", \"Id\": \"4654eabc-090a-e711-8104-5065f38aab31\", \"DisplayValue\": \"CCCMDev #\", \"TypeName\": \"systemuser\" }, \"Origin\": { \"$id\": \"6\", \"OptionSetId\": 861980001, \"DisplayValue\": \"Marketing\", \"TypeName\": null, \"Id\": null, \"CreatedOn\": null, \"ModifiedOn\": null, \"CreatedBy\": null, \"ModifiedBy\": null, \"ExtendedId\": null }, \"GEPName\": \"Azure Platform\", \"GEPScenario\": null, \"ProgramId\": \"V1|OneGDC|10585\", \"GlobalCrmId\": null, \"Id\": \"55a11e81-7e0a-4628-b489-b9db3ff8acac\", \"CreatedOn\": \"2017-04-16T08:22:28Z\", \"ModifiedOn\": \"2017-07-16T16:52:34Z\", \"CreatedBy\": { \"$id\": \"7\", \"Id\": \"4654eabc-090a-e711-8104-5065f38aab31\", \"DisplayValue\": \"CCCMDev #\", \"TypeName\": \"systemuser\" }, \"ModifiedBy\": { \"$id\": \"8\", \"Id\": \"63fb5996-5821-e711-8105-5065f38aab31\", \"DisplayValue\": \"MAP PROD Corp #\", \"TypeName\": \"systemuser\" }, \"ExtendedId\": null } ], \"MoreRecords\": false, \"EntityName\": \"campaign\", \"PagingCookie\": \"<cookie page=\\\"1\\\"><name last=\\\"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\\\" first=\\\"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\\\" /><campaignid last=\\\"{55A11E81-7E0A-4628-B489-B9DB3FF8ACAC}\\\" first=\\\"{55A11E81-7E0A-4628-B489-B9DB3FF8ACAC}\\\" /></cookie>\", \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PushProgramsToSales\", \"MessageBody\": { \"Program\": { \"id\": \"10585\", \"name\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\", \"description\": \"\", \"createdAt\": \"2015-06-29T20:32:10Z+0000\", \"updatedAt\": \"2017-06-13T23:20:41Z+0000\", \"startDate\": \"2015-06-26T15:00:00.000Z+0000\", \"endDate\": \"2015-06-26T15:55:00.000Z-0000\", \"url\": \"https://app-sj18.marketo.com/#ME10585A1\", \"type\": \"EventWithWebinar\", \"channel\": \"Webinar\", \"status\": \"\", \"workspace\": \"1GDC - Marketing Programs\", \"folder\": { \"type\": \"Folder\", \"value\": 35150, \"folderName\": \"AADP July Sessions\" }, \"tags\": [ { \"tagType\": \"Area\", \"tagValue\": \"Corporate\" }, { \"tagType\": \"Global Engagement Program\", \"tagValue\": \"Azure Platform\" }, { \"tagType\": \"Product\", \"tagValue\": \"Azure\" }, { \"tagType\": \"Program Owner\", \"tagValue\": \"Colleen Colley\" } ], \"MarketoInstanceName\": \"OneGDC\", \"UniqueProgramIdentifier\": \"V1|OneGDC|10585\" }, \"id\": \"52c6abaa-ed84-4d28-9f03-851af208db41\", \"createdAt\": \"2017-08-05T09:10:25\" } }, \"ForEachContext\": { } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.MifContext.MessageBody.Program.UniqueProgramIdentifier\", \"DestinationColumn\": \"ProgramId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"TacticName\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.MifContext.MessageBody.Program.tokens[?(@.name == 'Program-Name')].value\", \"ReturnValue\": \"$.MifContext.MessageBody.Program.name\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.MifContext.MessageBody.Program.tokens[?(@.name == 'Program-Name')].value\" ] } }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.name\", \"DestinationColumn\": \"PromotionCode\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.tags[?(@.tagType == 'Global Engagement Program')].tagValue\", \"DestinationColumn\": \"GEPName\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.tags[?(@.tagType == 'Global Engagement Program Scenario')].tagValue\", \"DestinationColumn\": \"GEPScenario\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.startDate\", \"DestinationColumn\": \"TacticStartDate\", \"DataType\": \"removeDateTimeOffset\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.endDate\", \"DestinationColumn\": \"TacticEndDate\", \"DataType\": \"removeDateTimeOffset\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"Source\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"ValueMapping\": null, \"DefaultValue\": \"Marketing\" } }, { \"SourceColumn\": \"$.Results[0].Id\", \"DestinationColumn\": \"Id\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"Origin\", \"DataType\": \"JObject\", \"ComplexType\": { \"DataType\": \"JObject\", \"Node\": \"\", \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"DisplayValue\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": \"Marketing\" } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"OptionSetId\", \"DataType\": \"int\", \"TransformValue\": { \"DefaultValue\": 861980001 } } ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{\r\n  \"ProgramId\": \"V1|OneGDC|10585\",\r\n  \"TacticName\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\",\r\n  \"PromotionCode\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\",\r\n  \"GEPName\": \"Azure Platform\",\r\n  \"TacticStartDate\": \"2015-06-26T15:00:00Z\",\r\n  \"TacticEndDate\": \"2015-06-26T15:55:00Z\",\r\n  \"Source\": \"Marketing\",\r\n  \"Id\": \"55a11e81-7e0a-4628-b489-b9db3ff8acac\",\r\n  \"Origin\": {\r\n    \"DisplayValue\": \"Marketing\",\r\n    \"OptionSetId\": 861980001\r\n  }\r\n}";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate that the DateTime is sent as it is if no offset is present.")]
    public void AutoMappperMapping_RemoveDateTimeOffsetTest_WithoutOffset()
    {
      // Arrange
      string inputJson =
        "{ \"Results\": [ { \"$id\": \"2\", \"Comments\": null, \"MarketingTactic\": null, \"MarketingVehicle\": null, \"PromotionCode\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\", \"Status\": { \"$id\": \"3\", \"OptionSetId\": 0, \"DisplayValue\": \"Active\", \"TypeName\": null, \"Id\": null, \"CreatedOn\": null, \"ModifiedOn\": null, \"CreatedBy\": null, \"ModifiedBy\": null, \"ExtendedId\": null }, \"StatusReason\": { \"$id\": \"4\", \"OptionSetId\": 0, \"DisplayValue\": \"Proposed\", \"TypeName\": null, \"Id\": null, \"CreatedOn\": null, \"ModifiedOn\": null, \"CreatedBy\": null, \"ModifiedBy\": null, \"ExtendedId\": null }, \"TacticEndDate\": \"2015-06-26T15:55:00Z\", \"TacticStartDate\": \"2015-06-26T15:00:00Z\", \"TacticName\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\", \"Owner\": { \"$id\": \"5\", \"Id\": \"4654eabc-090a-e711-8104-5065f38aab31\", \"DisplayValue\": \"CCCMDev #\", \"TypeName\": \"systemuser\" }, \"Origin\": { \"$id\": \"6\", \"OptionSetId\": 861980001, \"DisplayValue\": \"Marketing\", \"TypeName\": null, \"Id\": null, \"CreatedOn\": null, \"ModifiedOn\": null, \"CreatedBy\": null, \"ModifiedBy\": null, \"ExtendedId\": null }, \"GEPName\": \"Azure Platform\", \"GEPScenario\": null, \"ProgramId\": \"V1|OneGDC|10585\", \"GlobalCrmId\": null, \"Id\": \"55a11e81-7e0a-4628-b489-b9db3ff8acac\", \"CreatedOn\": \"2017-04-16T08:22:28Z\", \"ModifiedOn\": \"2017-07-16T16:52:34Z\", \"CreatedBy\": { \"$id\": \"7\", \"Id\": \"4654eabc-090a-e711-8104-5065f38aab31\", \"DisplayValue\": \"CCCMDev #\", \"TypeName\": \"systemuser\" }, \"ModifiedBy\": { \"$id\": \"8\", \"Id\": \"63fb5996-5821-e711-8105-5065f38aab31\", \"DisplayValue\": \"MAP PROD Corp #\", \"TypeName\": \"systemuser\" }, \"ExtendedId\": null } ], \"MoreRecords\": false, \"EntityName\": \"campaign\", \"PagingCookie\": \"<cookie page=\\\"1\\\"><name last=\\\"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\\\" first=\\\"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\\\" /><campaignid last=\\\"{55A11E81-7E0A-4628-B489-B9DB3FF8ACAC}\\\" first=\\\"{55A11E81-7E0A-4628-B489-B9DB3FF8ACAC}\\\" /></cookie>\", \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PushProgramsToSales\", \"MessageBody\": { \"Program\": { \"id\": \"10585\", \"name\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\", \"description\": \"\", \"createdAt\": \"2015-06-29T20:32:10Z+0000\", \"updatedAt\": \"2017-06-13T23:20:41Z+0000\", \"startDate\": \"2015-06-26T15:00:00.000\", \"endDate\": \"2015-06-26T15:55:00.000\", \"url\": \"https://app-sj18.marketo.com/#ME10585A1\", \"type\": \"EventWithWebinar\", \"channel\": \"Webinar\", \"status\": \"\", \"workspace\": \"1GDC - Marketing Programs\", \"folder\": { \"type\": \"Folder\", \"value\": 35150, \"folderName\": \"AADP July Sessions\" }, \"tags\": [ { \"tagType\": \"Area\", \"tagValue\": \"Corporate\" }, { \"tagType\": \"Global Engagement Program\", \"tagValue\": \"Azure Platform\" }, { \"tagType\": \"Product\", \"tagValue\": \"Azure\" }, { \"tagType\": \"Program Owner\", \"tagValue\": \"Colleen Colley\" } ], \"MarketoInstanceName\": \"OneGDC\", \"UniqueProgramIdentifier\": \"V1|OneGDC|10585\" }, \"id\": \"52c6abaa-ed84-4d28-9f03-851af208db41\", \"createdAt\": \"2017-08-05T09:10:25\" } }, \"ForEachContext\": { } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.MifContext.MessageBody.Program.UniqueProgramIdentifier\", \"DestinationColumn\": \"ProgramId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"TacticName\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.MifContext.MessageBody.Program.tokens[?(@.name == 'Program-Name')].value\", \"ReturnValue\": \"$.MifContext.MessageBody.Program.name\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.MifContext.MessageBody.Program.tokens[?(@.name == 'Program-Name')].value\" ] } }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.name\", \"DestinationColumn\": \"PromotionCode\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.tags[?(@.tagType == 'Global Engagement Program')].tagValue\", \"DestinationColumn\": \"GEPName\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.tags[?(@.tagType == 'Global Engagement Program Scenario')].tagValue\", \"DestinationColumn\": \"GEPScenario\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.startDate\", \"DestinationColumn\": \"TacticStartDate\", \"DataType\": \"removeDateTimeOffset\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"$.MifContext.MessageBody.Program.endDate\", \"DestinationColumn\": \"TacticEndDate\", \"DataType\": \"removeDateTimeOffset\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"Source\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"ValueMapping\": null, \"DefaultValue\": \"Marketing\" } }, { \"SourceColumn\": \"$.Results[0].Id\", \"DestinationColumn\": \"Id\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"Origin\", \"DataType\": \"JObject\", \"ComplexType\": { \"DataType\": \"JObject\", \"Node\": \"\", \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"DisplayValue\", \"DataType\": \"string\", \"TransformValue\": { \"DefaultValue\": \"Marketing\" } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"OptionSetId\", \"DataType\": \"int\", \"TransformValue\": { \"DefaultValue\": 861980001 } } ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{\r\n  \"ProgramId\": \"V1|OneGDC|10585\",\r\n  \"TacticName\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\",\r\n  \"PromotionCode\": \"CO-Azure-WBNR-FY16-07Jul-AADP Quick Wins AM1\",\r\n  \"GEPName\": \"Azure Platform\",\r\n  \"TacticStartDate\": \"2015-06-26T15:00:00\",\r\n  \"TacticEndDate\": \"2015-06-26T15:55:00\",\r\n  \"Source\": \"Marketing\",\r\n  \"Id\": \"55a11e81-7e0a-4628-b489-b9db3ff8acac\",\r\n  \"Origin\": {\r\n    \"DisplayValue\": \"Marketing\",\r\n    \"OptionSetId\": 861980001\r\n  }\r\n}";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_HappyPath()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\", \"cDVSBusinessPhoneE164Format\": \"12345\", \"phone\": \"67890\" } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\", \"MainPhone\":\"12345\" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_ParamNull()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\", \"cDVSBusinessPhoneE164Format\": null, \"phone\": \"67890\" } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\", \"MainPhone\":\"67890\" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_ParamNotPresent()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\",  \"phone\": \"67890\" } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\", \"MainPhone\":\"67890\" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_ReturnValueNullParamNull()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\", \"cDVSBusinessPhoneE164Format\": null, \"phone\":null } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_ParamEmptyString()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\", \"cDVSBusinessPhoneE164Format\": \"\", \"phone\": \"67890\" } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\", \"MainPhone\":\"\" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_ParamWhiteSpace()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\", \"cDVSBusinessPhoneE164Format\": \" \", \"phone\": \"67890\" } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\", \"MainPhone\":\" \" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_ParamNullReturnValueEmpty()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\", \"cDVSBusinessPhoneE164Format\": null, \"phone\": \"\" } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\", \"MainPhone\":\"\" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_ParamEmptyStringWithIgnoreEmptyParamEnabled()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\", \"cDVSBusinessPhoneE164Format\": \"\", \"phone\": \"67890\" } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"IgnoreEmptyParams\": \"true\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\", \"MainPhone\":\"67890\" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Validate ReplaceValueFunction.")]
    public void AutoMappperMapping_ReplaceValueFunction_ParamWhiteSpaceStringWithIgnoreEmptyParamEnabled()
    {
      // Arrange
      string inputJson =
        "{ \"test\": 1, \"MifContext\": { \"description\": \"ThisWorkflowisusedforPullingProgramsfromMIPTopicandInserting/UpdatingtheminMSX\", \"runId\": \"1a3fdea2-2a68-4078-81a4-419d8ba869c7\", \"TrackingEnabled\": true, \"WorkFlowName\": \"PullLeadActivitiesFromMarketo_VisitWebpage\" }, \"ForEachContext\": { \"leadId\": \"leadId1\", \"cDVSBusinessPhoneE164Format\": \" \", \"phone\": \"67890\" } }";

      string mappingJson =
        "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"$.ForEachContext.leadId\", \"DestinationColumn\": \"LeadId\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": null }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"MainPhone\", \"DataType\": \"string\", \"ComplexType\": null, \"TransformValue\": { \"Type\": \"function\", \"CompareToValue\": null, \"DefaultValue\": \"$.ForEachContext.cDVSBusinessPhoneE164Format\", \"ReturnValue\": \"$.ForEachContext.phone\", \"Function\": \"ReplaceValue\", \"IgnoreEmptyParams\": \"true\", \"Params\": [ \"$.ForEachContext.cDVSBusinessPhoneE164Format\" ] } } ] } }";


      //Act
      var mapper = new AutoMapper(mappingJson);
      try
      {
        var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
        string expectedOutput = "{ \"LeadId\":\"leadId1\", \"MainPhone\":\"67890\" }";
        JObject j = JObject.Parse(expectedOutput);
        Assert.IsTrue(JToken.DeepEquals(JObject.Parse(output.ToString()), JObject.Parse(expectedOutput)));
      }
      catch (Exception ex)
      {
        Assert.Fail("Test failed: " + ex.Message);
      }
    }

    [TestMethod, Description("Test one to one mapping functionality")]
    public void OneToOneMappingTest()
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

      //Assert
      Assert.AreEqual("Financial Services", output.SelectToken("$.Industry").ToString());
    }

    [TestMethod, Description("Test range mapping functionality")]
    public void RangeMappingTest()
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

      //Assert
      Assert.AreEqual("51to250employees", output.SelectToken("$.CompanySize").ToString());
    }

    [TestMethod, Description("Test for UriEscapeDataString HappyPath With Escape character as Ampersand")]
    public void FunctionHanlder_UriEscapeDataString_HappyPath_Ampersand()
    {
      // Arrange
      string inputJson =
        "{ \"data\": { \"companyName\": \"Procter & Gamble Company\", \"countryName\": \"Bulgaria\", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";
      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": [ \"$.data.companyName\", \"$.data.countryName\", \"$.data.state\", \"$.data.city\", \"$.data.zipCode\", \"$.data.DunnsId\" ] } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \"\", \"Function\": \"Concat\", \"Params\": [ \"https://social.sprinklr.com/selling/search/company?companyName=\", \"$.data.companyName\", \"&countryName=\", \"$.data.countryName\", \"&state=\", \"$.data.state\", \"&city=\", \"$.data.city\", \"&zipCode=\", \"$.data.zipCode\", \"&DunnsId=\", \"$.data.DunnsId\" ] } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%20%26%20Gamble%20Company&countryName=Bulgaria&state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
    }

    [TestMethod, Description("Test for UriEscapeDataString HappyPath With Escape character as Dollar")]
    public void FunctionHanlder_UriEscapeDataString_HappyPath_Dollar()
    {
      // Arrange
      string inputJson =
        "{ \"data\": { \"companyName\": \"Procter $ Gamble Company\", \"countryName\": \"Bulgaria\", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";
      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": [ \"$.data.companyName\", \"$.data.countryName\", \"$.data.state\", \"$.data.city\", \"$.data.zipCode\", \"$.data.DunnsId\" ] } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \"\", \"Function\": \"Concat\", \"Params\": [ \"https://social.sprinklr.com/selling/search/company?companyName=\", \"$.data.companyName\", \"&countryName=\", \"$.data.countryName\", \"&state=\", \"$.data.state\", \"&city=\", \"$.data.city\", \"&zipCode=\", \"$.data.zipCode\", \"&DunnsId=\", \"$.data.DunnsId\" ] } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%20%24%20Gamble%20Company&countryName=Bulgaria&state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
    }

    [TestMethod, Description("Test for UriEscapeDataString HappyPath With Escape character as Plus")]
    public void FunctionHanlder_UriEscapeDataString_HappyPath_Plus()
    {
      // Arrange
      string inputJson =
        "{ \"data\": { \"companyName\": \"Procter + Gamble Company\", \"countryName\": \"Bulgaria+\", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";
      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": [ \"$.data.companyName\", \"$.data.countryName\", \"$.data.state\", \"$.data.city\", \"$.data.zipCode\", \"$.data.DunnsId\" ] } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \"\", \"Function\": \"Concat\", \"Params\": [ \"https://social.sprinklr.com/selling/search/company?companyName=\", \"$.data.companyName\", \"&countryName=\", \"$.data.countryName\", \"&state=\", \"$.data.state\", \"&city=\", \"$.data.city\", \"&zipCode=\", \"$.data.zipCode\", \"&DunnsId=\", \"$.data.DunnsId\" ] } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%20%2B%20Gamble%20Company&countryName=Bulgaria%2B&state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
    }

    [TestMethod, Description("Test for UriEscapeDataString HappyPath With Escape character as Space")]
    public void FunctionHanlder_UriEscapeDataString_HappyPath_Space()
    {
      // Arrange
      string inputJson =
        "{ \"data\": { \"companyName\": \"Procter Gamble Company\", \"countryName\": \"Bulgaria \", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";
      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": [ \"$.data.companyName\", \"$.data.countryName\", \"$.data.state\", \"$.data.city\", \"$.data.zipCode\", \"$.data.DunnsId\" ] } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \"\", \"Function\": \"Concat\", \"Params\": [ \"https://social.sprinklr.com/selling/search/company?companyName=\", \"$.data.companyName\", \"&countryName=\", \"$.data.countryName\", \"&state=\", \"$.data.state\", \"&city=\", \"$.data.city\", \"&zipCode=\", \"$.data.zipCode\", \"&DunnsId=\", \"$.data.DunnsId\" ] } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%20Gamble%20Company&countryName=Bulgaria%20&state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
    }

    [TestMethod, Description("Test for UriEscapeDataString HappyPath With Escape character as Percent")]
    public void FunctionHanlder_UriEscapeDataString_HappyPath_Percent()
    {
      // Arrange
      string inputJson =
        "{ \"data\": { \"companyName\": \"Procter%Gamble Company\", \"countryName\": \"Bulgaria \", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";
      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": [ \"$.data.companyName\", \"$.data.countryName\", \"$.data.state\", \"$.data.city\", \"$.data.zipCode\", \"$.data.DunnsId\" ] } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \"\", \"Function\": \"Concat\", \"Params\": [ \"https://social.sprinklr.com/selling/search/company?companyName=\", \"$.data.companyName\", \"&countryName=\", \"$.data.countryName\", \"&state=\", \"$.data.state\", \"&city=\", \"$.data.city\", \"&zipCode=\", \"$.data.zipCode\", \"&DunnsId=\", \"$.data.DunnsId\" ] } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%25Gamble%20Company&countryName=Bulgaria%20&state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
    }

    [TestMethod, Description("Test for UriEscapeDataString HappyPath With Escape character as Exclamation")]
    public void FunctionHanlder_UriEscapeDataString_HappyPath_Exclamation()
    {
      // Arrange
      string inputJson =
        "{ \"data\": { \"companyName\": \"Procter!Gamble Company\", \"countryName\": \"Bulgaria \", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";
      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": [ \"$.data.companyName\", \"$.data.countryName\", \"$.data.state\", \"$.data.city\", \"$.data.zipCode\", \"$.data.DunnsId\" ] } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \"\", \"Function\": \"Concat\", \"Params\": [ \"https://social.sprinklr.com/selling/search/company?companyName=\", \"$.data.companyName\", \"&countryName=\", \"$.data.countryName\", \"&state=\", \"$.data.state\", \"&city=\", \"$.data.city\", \"&zipCode=\", \"$.data.zipCode\", \"&DunnsId=\", \"$.data.DunnsId\" ] } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%21Gamble%20Company&countryName=Bulgaria%20&state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
    }

    [TestMethod, Description("Test for UriEscapeDataString Null Paramters")]
    public void FunctionHanlder_UriEscapeDataString_Null_Parameters()
    {
      // Arrange
      string inputJson = "{ \"data\": { \"companyName\": \"Procter!Gamble Company\", \"countryName\": \"Bulgaria \", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";

      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": null } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      // string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%21Gamble%20Company&countryName=Bulgaria%20&state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, new JObject()));
    }

    [TestMethod, Description("Test for UriEscapeDataString One Null Paramter")]
    public void FunctionHanlder_UriEscapeDataString_One_Null_Parameter()
    {
      // Arrange
      string inputJson = "{ \"data\": { \"companyName\": \"Procter!Gamble Company\", \"countryName\": \"Bulgaria \", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";

      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": [ \"$.data.companyName\", null, \"$.data.state\", \"$.data.city\", \"$.data.zipCode\", \"$.data.DunnsId\" ] } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \"\", \"Function\": \"Concat\", \"Params\": [ \"https://social.sprinklr.com/selling/search/company?companyName=\", \"$.data.companyName\", \"&countryName=\", \"$.data.countryName\", \"&state=\", \"$.data.state\", \"&city=\", \"$.data.city\", \"&zipCode=\", \"$.data.zipCode\", \"&DunnsId=\", \"$.data.DunnsId\" ] } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%21Gamble%20Company&countryName=Bulgaria &state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
    }

    [TestMethod, Description("Test for UriEscapeDataString With Invalid Parameter Field")]
    public void FunctionHanlder_UriEscapeDataString_Invalid_Parameter_Field()
    {
      // Arrange
      string inputJson =
        "{ \"data\": { \"companyName\": \"Procter!Gamble Company\", \"countryName\": \"Bulgaria \", \"state\": \"\", \"city\": \"Sofia\", \"zipCode\": \"1510\", \"DunnsId\": null } } ";
      string mappingJson = "{ \"MappingRuleConfig\": { \"TruthTable\": [ { \"SourceColumn\": \"\", \"DestinationColumn\": \"dummy\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Function\": \"UriEscapeDataString\", \"Params\": [ \"$.data.companyName\", \"$.data.countryName\", \"$.data.state1\", \"$.data.city\", \"$.data.zipCode\", \"$.data.DunnsId\" ] } }, { \"SourceColumn\": \"\", \"DestinationColumn\": \"message\", \"DataType\": \"string\", \"TransformValue\": { \"Type\": \"function\", \"Delimeter\": \"\", \"Function\": \"Concat\", \"Params\": [ \"https://social.sprinklr.com/selling/search/company?companyName=\", \"$.data.companyName\", \"&countryName=\", \"$.data.countryName\", \"&state=\", \"$.data.state\", \"&city=\", \"$.data.city\", \"&zipCode=\", \"$.data.zipCode\", \"&DunnsId=\", \"$.data.DunnsId\" ] } } ] } } ";
      //Act
      var mapper = new AutoMapper(mappingJson);
      var output = mapper.TransformIntoJson(JsonConvert.DeserializeObject<JObject>(inputJson), true);
      string expectedOutput = "{ \"message\": \"https://social.sprinklr.com/selling/search/company?companyName=Procter%21Gamble%20Company&countryName=Bulgaria%20&state=&city=Sofia&zipCode=1510&DunnsId=\" }";
      Assert.IsTrue(JToken.DeepEquals(output, JObject.Parse(expectedOutput)));
    }
  }
}
