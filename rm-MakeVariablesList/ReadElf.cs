using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rm_MakeVariablesList
{
    public static class ReadElf
    {
        private enum TagType
        {
            Non,
            StructureOrUnion,
            Array,
            Class
        }
        public static List<SymbolFactor> Convert(List<VariableElement> test)
        {
            List<SymbolFactor> target = new List<SymbolFactor>();

            foreach(var item in test)
            {
                SymbolFactor tmp = new SymbolFactor();
                tmp.Symbol = item.Symbol;
                tmp.Address = "0x" + item.Address.ToString("X8");
                tmp.Offset = item.Offset.ToString();
                tmp.Size = item.Size.ToString();
                target.Add(tmp);
            }

            return target;
        }
        private static bool TagTryParse(string line, out DebugInfo info)
        {
            info = new DebugInfo();

            var value1 = line.IndexOf("<");
            var value2 = line.IndexOf(">");

            if (value1 == -1 || value2 == -1)
                return false;

            var number = line.Substring(value1+1, value2 - value1 - 1);

            // Need number ?

            line = line.Remove(0, value2+1);

            value1 = line.IndexOf("<");
            value2 = line.IndexOf(">");

            if (value1 == -1 || value2 == -1)
                return false;

            var addressText = line.Substring(value1+1, value2 - value1 - 1);

            if (!uint.TryParse(addressText, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out uint address))
                return false;

            info.TagAddress = address;


            value1 = line.IndexOf("(");
            value2 = line.IndexOf(")");

            if (value1 == -1 || value2 == -1)
                return false;

            var tag = line.Substring(value1+1, value2 - value1 - 1);

            if(!tag.Contains("DW_TAG"))
                return false;

            info.TagName = DebugInfo.GetTag(tag);

            return true;
        }

        private static bool AttributeTryParse(string line, ref DebugInfo info)
        {
            var firstOccurrence = line.IndexOf(':');

            if (firstOccurrence == -1)
                return false;

            var rawAttribute = line.Substring(0, firstOccurrence).Replace(" ", "");
            var rawContent = line.Substring(firstOccurrence+1, (line.Length - firstOccurrence)-1);

            DebugInfo.Attribute attribute;

            firstOccurrence = rawAttribute.IndexOf('>');

            if (firstOccurrence != -1)
            {
                rawAttribute = rawAttribute.Substring(firstOccurrence + 1, (rawAttribute.Length - firstOccurrence) - 1);
            }

            if (!rawAttribute.Contains("DW_AT"))
                return false;

            attribute = DebugInfo.GetAttribute(rawAttribute);

            switch(attribute)
            {
                case DebugInfo.Attribute.UpperBound:
                    if (int.TryParse(rawContent, out int upperboundValue))
                    {
                        info.UpperBound = upperboundValue;
                    }
                    break;

                case DebugInfo.Attribute.ByteSize:
                    if(int.TryParse(rawContent, out int sizeValue))
                    {
                        info.ByteSize = sizeValue;
                    }
                    break;

                case DebugInfo.Attribute.Type:
                    {
                        var value1 = rawContent.IndexOf("<");
                        var value2 = rawContent.IndexOf(">");

                        if (value1 != -1 && value2 != -1 && value2 > value1)
                        {
                            rawContent = rawContent.Substring(value1 + 1, value2 - value1 - 1);

                            if (rawContent.Substring(0, 2) == "0x")
                                rawContent = rawContent.Remove(0, 2);

                            if (uint.TryParse(rawContent,
                                             System.Globalization.NumberStyles.HexNumber,
                                             System.Globalization.CultureInfo.InvariantCulture,
                                             out uint typeValue))
                            {
                                info.NextAddress = typeValue;
                            }

                        }

                    }

                    break;

                case DebugInfo.Attribute.Name:
                    {
                        var delimiter = rawContent.IndexOf("):");

                        if(delimiter != -1)
                            rawContent = rawContent.Substring(delimiter+2, (rawContent.Length - delimiter) - 2);

                        info.Name = new string(rawContent.Where(c => !char.IsWhiteSpace(c)).ToArray());

                    }
                    break;

                case DebugInfo.Attribute.DataMemberLocation:
                    {
                        var value1 = rawContent.IndexOf("(");
                        var value2 = rawContent.IndexOf(")");

                        if (value1 != -1 && value2 != -1 && value2 > value1)
                        {
                            var locationText = rawContent.Substring(value1 + 1, value2 - value1 - 1);

                            var splitedLocationText = locationText.Split(':');

                            if(splitedLocationText.Count() == 2)
                            {
                                if (int.TryParse(splitedLocationText[1], out int value))
                                {
                                    info.DataMemberLocation = value;

                                }

                            }

                        }
                        else
                        {
                            if (int.TryParse(rawContent, out int value))
                                info.DataMemberLocation = value;


                        }

                    }

                    break;

                case DebugInfo.Attribute.Location:
                    {
                        var value1 = rawContent.IndexOf("(");
                        var value2 = rawContent.IndexOf(")");

                        if (value1 != -1 && value2 != -1 && value2 > value1)
                        {
                            var locationText = rawContent.Substring(value1 + 1, value2 - value1 - 1);

                            var splitedLocationText = locationText.Split(':');

                            if (splitedLocationText.Count() == 2)
                            {
                                if (splitedLocationText[0] != "DW_OP_addr")
                                {

                                }
                                else if (uint.TryParse(splitedLocationText[1],
                                                      System.Globalization.NumberStyles.HexNumber,
                                                      System.Globalization.CultureInfo.InvariantCulture,
                                                      out uint value))
                                {
                                    info.Location = value;

                                }

                            }

                        }

                    }

                    break;

                case DebugInfo.Attribute.Specification:
                    {
                        var value1 = rawContent.IndexOf("<");
                        var value2 = rawContent.IndexOf(">");

                        if (value1 != -1 && value2 != -1 && value2 > value1)
                        {
                            rawContent = rawContent.Substring(value1 + 1, value2 - value1 - 1);

                            if (rawContent.Substring(0, 2) == "0x")
                                rawContent = rawContent.Remove(0, 2);

                            if (uint.TryParse(rawContent,
                                             System.Globalization.NumberStyles.HexNumber,
                                             System.Globalization.CultureInfo.InvariantCulture,
                                             out uint specificationValue))
                            {
                                info.Specification = specificationValue;
                            }

                        }

                    }

                    break;

                default:
                    break;
            }

            return true;
        }


        public static void ConstractDebugList(List<string> lines, ref List<DebugInfo> debugList)
        {
            bool foundTag = false;
            DebugInfo.Tag lastTag = DebugInfo.Tag.Nothing;

            DebugInfo info = new DebugInfo();

            foreach (var line in lines)
            {
                if (TagTryParse(line, out DebugInfo tmpInfo))
                {
                    if (tmpInfo.TagName != DebugInfo.Tag.Nothing)
                        foundTag = true;
                    else
                        foundTag = false;

                    if (lastTag != DebugInfo.Tag.Nothing)
                        debugList.Add(info);

                    lastTag = tmpInfo.TagName;

                    info = new DebugInfo(tmpInfo);

                }
                else
                {
                    if (!foundTag)
                        continue;

                    AttributeTryParse(line, ref info);

                }

            }

            if (lastTag != DebugInfo.Tag.Nothing)
                debugList.Add(info);

        }


        public static void FormatDebugList(ref List<DebugInfo> originalDebugList, ref List<DebugInfo> debugList)
        {
            int lastIndex = 0;

            TagType tagType = TagType.Non;

            foreach (var item in originalDebugList)
            {
                if (tagType == TagType.Class)
                {
                    if (item.TagName == DebugInfo.Tag.Inheritance)
                    {
                        debugList[lastIndex].Belongs.Add(item);
                        continue;
                    }
                    else if (item.TagName == DebugInfo.Tag.Member)
                    {
                        debugList[lastIndex].Belongs.Add(item);
                        continue;
                    }
                    else if (item.TagName == DebugInfo.Tag.FormalParameter)
                    {
                        //ignore
                        continue;
                    }
                    else if (item.TagName == DebugInfo.Tag.Subprogram)
                    {
                        //ignore
                        continue;
                    }

                    tagType = TagType.Non;

                }

                if (tagType == TagType.Array)
                {
                    if (item.TagName == DebugInfo.Tag.SubrangeType)
                    {
                        debugList[lastIndex].Belongs.Add(item);
                    }

                    tagType = TagType.Non;

                }

                if (tagType == TagType.StructureOrUnion)
                {
                    if (item.TagName == DebugInfo.Tag.Member)
                    {
                        debugList[lastIndex].Belongs.Add(item);
                        continue;
                    }

                    tagType = TagType.Non;

                }

                if (item.TagName == DebugInfo.Tag.StructureType ||
                    item.TagName == DebugInfo.Tag.Union)
                {
                    tagType = TagType.StructureOrUnion;

                    debugList.Add(item);

                    lastIndex = debugList.Count - 1;
                    continue;

                }

                if (item.TagName == DebugInfo.Tag.ArrayType)
                {
                    tagType = TagType.Array;

                    debugList.Add(item);

                    lastIndex = debugList.Count - 1;
                    continue;

                }

                if (item.TagName == DebugInfo.Tag.ClassType)
                {
                    tagType = TagType.Class;

                    debugList.Add(item);

                    lastIndex = debugList.Count - 1;
                    continue;

                }

                if (item.TagName == DebugInfo.Tag.Variable)
                {
                    tagType = TagType.Non;

                    if (string.IsNullOrEmpty(item.Name) && item.Specification != 0)
                        UpdateLocation(ref originalDebugList, item);

                    debugList.Add(item);

                    lastIndex = debugList.Count - 1;
                    continue;

                }

                if (item.TagName != DebugInfo.Tag.Nothing)
                {
                    debugList.Add(item);

                }

            }

        }

        public static void ConstructVariablesList(ref List<DebugInfo> structures, ref List<VariableElement> elements)
        {
            foreach (var item in structures)
            {
                if (item.TagName == DebugInfo.Tag.Variable)
                {
                    if (!string.IsNullOrEmpty(item.Name) && item.Location != 0)
                    {
#if DEBUG
                        string keyword = "PIDSpeedHandle_M1";
                        if (item.Name == keyword)
                            item.Name = keyword;
#endif

                        VariableElement distElement = new VariableElement();
                        distElement.Symbol = item.Name;
                        distElement.Address = (int)item.Location;
                        SearchConnection(structures, item, ref distElement, ref elements);

                    }

                }

            }

        }
        private static void UpdateLocation(ref List<DebugInfo> structures, DebugInfo rootInfo)
        {
            foreach (var item in structures)
            {
                if(item.TagAddress == rootInfo.Specification)
                {
                    item.Location = rootInfo.Location;
                    break;
                }
            }

        }

        private static void SearchConnection(List<DebugInfo> structures, DebugInfo rootInfo, ref VariableElement distElement, ref List<VariableElement> elements)
        {
            distElement.Offset += rootInfo.DataMemberLocation;

            if (rootInfo.Belongs.Count != 0)
            {
                foreach (var info in rootInfo.Belongs)
                {
                    VariableElement newFactor = new VariableElement(distElement);
                    if (info.TagName == DebugInfo.Tag.Member)
                        newFactor.Symbol += "." + info.Name;

                    SearchConnection(structures, info, ref newFactor, ref elements);

                }

                return;
            }

            if (rootInfo.NextAddress == 0)
            {
                distElement.Size = rootInfo.ByteSize;
                elements.Add(distElement);
                return;
            }

            foreach (var item in structures)
            {
                if (item.TagAddress != rootInfo.NextAddress)
                    continue;

                if(item.TagName == DebugInfo.Tag.ArrayType)
                {
                    // Give up!
                    distElement.Size = 0;
                    elements.Add(distElement);
                }
                else if (item.TagName == DebugInfo.Tag.PointerType)
                {
                    // Give up!
                    distElement.Size = item.ByteSize;
                    elements.Add(distElement);
                }
                else if (item.TagName == DebugInfo.Tag.ClassType)
                {
                    if (item.NextAddress != 0)
                    {
                        SearchConnection(structures, item, ref distElement, ref elements);
                    }
                    else
                    {
                        foreach (var tmp in structures)
                        {
                            // Swap
                            if (tmp.Name == item.Name && tmp.ByteSize != 0)
                            {
                                SearchConnection(structures, tmp, ref distElement, ref elements);
                                break;
                            }

                        }

                    }

                }
                else
                {
                    SearchConnection(structures, item, ref distElement, ref elements);
                }

                break;


            }

        }


    }


}
