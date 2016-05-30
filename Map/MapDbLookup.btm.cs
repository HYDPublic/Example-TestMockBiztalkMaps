namespace Map {
    
    
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"Schema.SourceSchema", typeof(global::Schema.SourceSchema))]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"Schema.DestinationSchema", typeof(global::Schema.DestinationSchema))]
    public sealed class MapDbLookup : global::Microsoft.XLANGs.BaseTypes.TransformBase {
        
        private const string _strMap = @"<?xml version=""1.0"" encoding=""UTF-16""?>
<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:var=""http://schemas.microsoft.com/BizTalk/2003/var"" exclude-result-prefixes=""msxsl var s0 ScriptNS0"" version=""1.0"" xmlns:s0=""http://MockingBiztalkMaps/SourceSchema"" xmlns:ns0=""http://MockingBiztalkMaps/DestinationSchema"" xmlns:ScriptNS0=""http://schemas.microsoft.com/BizTalk/2003/ScriptNS0"">
  <xsl:output omit-xml-declaration=""yes"" method=""xml"" version=""1.0"" />
  <xsl:template match=""/"">
    <xsl:apply-templates select=""/s0:Root"" />
  </xsl:template>
  <xsl:template match=""/s0:Root"">
    <xsl:variable name=""var:v3"" select=""string(ValueToLookUp/text())"" />
    <ns0:Root>
      <xsl:variable name=""var:v1"" select=""ScriptNS0:DBLookup(0 , string(ValueToLookUp/text()) , &quot;Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=WAREHOUSE;Data Source=localhost&quot; , &quot;dbo.CALENDAR&quot; , &quot;CALENDAR_ID&quot;)"" />
      <xsl:variable name=""var:v2"" select=""ScriptNS0:DBValueExtract(string($var:v1) , &quot;CALENDAR_DATE&quot;)"" />
      <xsl:attribute name=""TranslatedValue"">
        <xsl:value-of select=""$var:v2"" />
      </xsl:attribute>
      <xsl:variable name=""var:v4"" select=""ScriptNS0:DBLookup(0 , $var:v3 , &quot;Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=WAREHOUSE;Data Source=localhost&quot; , &quot;dbo.CALENDAR&quot; , &quot;CALENDAR_ID&quot;)"" />
      <xsl:variable name=""var:v5"" select=""ScriptNS0:DBErrorExtract(string($var:v4))"" />
      <Error>
        <xsl:value-of select=""$var:v5"" />
      </Error>
    </ns0:Root>
    <xsl:variable name=""var:v6"" select=""ScriptNS0:DBLookupShutdown()"" />
  </xsl:template>
</xsl:stylesheet>";
        
        private const string _strArgList = @"<ExtensionObjects>
  <ExtensionObject Namespace=""http://schemas.microsoft.com/BizTalk/2003/ScriptNS0"" AssemblyName=""Microsoft.BizTalk.BaseFunctoids, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" ClassName=""Microsoft.BizTalk.BaseFunctoids.FunctoidScripts"" />
</ExtensionObjects>";
        
        private const string _strSrcSchemasList0 = @"Schema.SourceSchema";
        
        private const global::Schema.SourceSchema _srcSchemaTypeReference0 = null;
        
        private const string _strTrgSchemasList0 = @"Schema.DestinationSchema";
        
        private const global::Schema.DestinationSchema _trgSchemaTypeReference0 = null;
        
        public override string XmlContent {
            get {
                return _strMap;
            }
        }
        
        public override string XsltArgumentListContent {
            get {
                return _strArgList;
            }
        }
        
        public override string[] SourceSchemas {
            get {
                string[] _SrcSchemas = new string [1];
                _SrcSchemas[0] = @"Schema.SourceSchema";
                return _SrcSchemas;
            }
        }
        
        public override string[] TargetSchemas {
            get {
                string[] _TrgSchemas = new string [1];
                _TrgSchemas[0] = @"Schema.DestinationSchema";
                return _TrgSchemas;
            }
        }
    }
}
