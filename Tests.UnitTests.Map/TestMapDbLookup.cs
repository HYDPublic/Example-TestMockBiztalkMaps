using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Map;
using Microsoft.XLANGs.BaseTypes;
using Moq;
using NUnit.Framework;

namespace Tests.UnitTests.Map
{
    [TestFixture]
    public class TestMapDbLookup
    {
        //Step 1: Define interface containing the signatures for all the functions you want to mock
        /// <summary>
        ///     Defines the signature for all functions need to be mocked for unit testing MapDbLookup.
        ///     You can workout the signature of the assembly in the xslt output when you validate the map in Visual Studio
        /// </summary>
        public interface IMockSignaturesMapDbLookup
        {
            /// <summary>
            ///     Mock signature for Biztalk DbLookup functiod
            /// </summary>
            string DBLookup(int indexAutoGenInternal, string valueToLookUp, string connectionString, string tableName,
                string columnToSearchLookupValueIn);

            /// <summary>
            ///     Mock signature for Biztalk DB Value Extract functiod
            /// </summary>
            string DBValueExtract(string dbLookUpReturnIndex, string columnContainingTranslatedValue);

            /// <summary>
            ///     Mock signature for Biztalk DB error functiod
            /// </summary>
            string DBErrorExtract(string dbLookUpReturnIndex);


            /// <summary>
            ///     Mock signature for Biztalk internal DBLookupShutdown functiod
            /// </summary>
            string DBLookupShutdown();
        }



        [SetUp]
        public void TestInit()
        {
            _sut = new MapDbLookup();
            _mocksForMapDb = new Mock<IMockSignaturesMapDbLookup>();
            //Step 2: Set up your mock object and map the namespace defined in the extentions xml file to this mock object
            var nameSpace = GetNameSpaceForClass("Microsoft.BizTalk.BaseFunctoids.FunctoidScripts",
                _sut.XsltArgumentListContent);
            _namespaceToInstanceObjectsMap = new Dictionary<string, object> {{nameSpace, _mocksForMapDb.Object}};
        }

        //Step 3: The core work magic here
        private static void Map(TransformBase map, string xmlInputFile, string outputXmlFile,
            Dictionary<string, object> namespaceToInstanceObjectsMap)
        {
            //Construct XsltArgumentList mapping the namespace to the mocked object
            var argumentsInstanceExtensionObjects = new XsltArgumentList();
            foreach (var entry in namespaceToInstanceObjectsMap)
            {
                argumentsInstanceExtensionObjects.AddExtensionObject(entry.Key, entry.Value);
            }

            //Do the transform using XslCompiledTransform
            var transform = new XslCompiledTransform();
            var setting = new XsltSettings(false, true);
            transform.Load(XmlReader.Create(new StringReader(map.XmlContent)), setting, new XmlUrlResolver());
            using (var stream = new FileStream(outputXmlFile, FileMode.CreateNew))
            {
                //using XslCompiledTransform to transform the source to output messahe
                transform.Transform(xmlInputFile, argumentsInstanceExtensionObjects, stream);
            }
        }


        private Mock<IMockSignaturesMapDbLookup> _mocksForMapDb;
        private MapDbLookup _sut;
        private Dictionary<string, object> _namespaceToInstanceObjectsMap;

        private const string ConnStrDoNotHardCodeThisInYourMap =
            "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=WAREHOUSE;Data Source=localhost";


        [TestCase("SampleTesttData.xml", "18991231", "1899-12-31 00:00:00.000")]
        public void ShouldMap(string inputXmlFile, string lookupValue, string mockDataTranslatedLookupValue)
        {
            //Arrange
            inputXmlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, inputXmlFile);
            const int index = 0;
            const string tableName = "dbo.CALENDAR";
            const string searchColumnName = "CALENDAR_ID";
            const string translatedValueColumnName = "CALENDAR_DATE";
            //Set up mock.. 
            //mock dblookup
            _mocksForMapDb.Setup(
                x => x.DBLookup(index, lookupValue, ConnStrDoNotHardCodeThisInYourMap, tableName, searchColumnName))
                .Returns(index.ToString);
            //mock dbvalue extract
            _mocksForMapDb.Setup(x => x.DBValueExtract(index.ToString(), translatedValueColumnName))
                .Returns(mockDataTranslatedLookupValue);
            var actualOutputXmlFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            //mock dberrror
            _mocksForMapDb.Setup(x => x.DBErrorExtract(index.ToString()))
                .Returns("");
            _mocksForMapDb.Setup(x => x.DBLookupShutdown())
                .Returns("");


            //Act
            Map(_sut, inputXmlFile, actualOutputXmlFile, _namespaceToInstanceObjectsMap);

            //Assert
            //Can also do  xml compare of the actual vs expected xml file..
            Assert.IsTrue(File.Exists(actualOutputXmlFile));
        }

        private static string GetNameSpaceForClass(string classFullName, string xsltExtensionXml)
        {
            var root = XElement.Parse(xsltExtensionXml);
            var namespaceOfScript =
                from el in root.Descendants("ExtensionObject")
                where (string) el.Attribute("ClassName") == classFullName
                select el.Attribute("Namespace").Value;

            return namespaceOfScript.First();
        }

    
    }
}