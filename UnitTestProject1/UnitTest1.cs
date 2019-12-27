using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using rm_MakeVariablesList;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string text;
            List<string> lines;
            List<DebugInfo> debugList;

            text = @"
 <1><ac>: Abbrev Number: 8 (DW_TAG_variable)
    <ad>   DW_AT_name        : (indirect string, offset: 0x256f): g_ADigitalPinMap
    <b1>   DW_AT_decl_file   : 4
    <b2>   DW_AT_decl_line   : 27
    <b3>   DW_AT_decl_column : 23
    <b4>   DW_AT_type        : <0xa1>
    <b8>   DW_AT_external    : 1
    <b8>   DW_AT_declaration : 1
            ";

            lines = new List<string>(text.Split(Environment.NewLine.ToCharArray()));

            lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            debugList = new List<DebugInfo>();
            ReadElf.ConstractDebugList(lines, ref debugList);

            if(debugList.Count != 1)
            {
                Assert.Fail();
            }
            else
            {
                Assert.AreEqual(debugList[0].TagName, DebugInfo.Tag.Variable);
                Assert.AreEqual(debugList[0].TagAddress, (uint)0xac);
                Assert.AreEqual(debugList[0].Name, "g_ADigitalPinMap");
                Assert.AreEqual(debugList[0].NextAddress, (uint)0xa1);
            }

            text = @"
 <1><4a719>: Abbrev Number: 41 (DW_TAG_variable)
    <4a71a>   DW_AT_name        : (indirect string, offset: 0x4e43d): m_flags
    <4a71e>   DW_AT_decl_file   : 1
    <4a71f>   DW_AT_decl_line   : 77
    <4a720>   DW_AT_decl_column : 29
    <4a721>   DW_AT_type        : <0x49224>
    <4a725>   DW_AT_location    : 5 byte block: 3 c0 26 0 20 	(DW_OP_addr: 200026c0)
            ";

            lines = new List<string>(text.Split(Environment.NewLine.ToCharArray()));

            lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            debugList = new List<DebugInfo>();
            ReadElf.ConstractDebugList(lines, ref debugList);

            if (debugList.Count != 1)
            {
                Assert.Fail();
            }
            else
            {
                Assert.AreEqual(debugList[0].TagName, DebugInfo.Tag.Variable);
                Assert.AreEqual(debugList[0].TagAddress, (uint)0x4a719);
                Assert.AreEqual(debugList[0].Name, "m_flags");
                Assert.AreEqual(debugList[0].NextAddress, (uint)0x49224);
                Assert.AreEqual(debugList[0].Location, (uint)0x200026c0);
            }

            text = @"
 <2><8fa60>: Abbrev Number: 13 (DW_TAG_variable)
     DW_AT_name        : RcvSLIPLastChar	
     DW_AT_decl_file   : 1	
     DW_AT_decl_line   : 1379	
     DW_AT_type        : <8f215>	
     DW_AT_location    : 5 byte block: 3 77 10 0 0 	(DW_OP_addr: 1077)
            ";

            lines = new List<string>(text.Split(Environment.NewLine.ToCharArray()));

            lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            debugList = new List<DebugInfo>();
            ReadElf.ConstractDebugList(lines, ref debugList);

            if (debugList.Count != 1)
            {
                Assert.Fail();
            }
            else
            {
                Assert.AreEqual(debugList[0].TagName, DebugInfo.Tag.Variable);
                Assert.AreEqual(debugList[0].TagAddress, (uint)0x8fa60);
                Assert.AreEqual(debugList[0].Name, "RcvSLIPLastChar");
                Assert.AreEqual(debugList[0].NextAddress, (uint)0x8f215);
                Assert.AreEqual(debugList[0].Location, (uint)0x1077);
            }
        }
    }
}
