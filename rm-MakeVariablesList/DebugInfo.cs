using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace rm_MakeVariablesList
{
    public class DebugInfo
    {
        public enum Tag
        {
            Nothing,
            BaseType,
            TypeDef,
            StructureType,
            Member,
            Union,
            Variable,
            VolatileType,
            EnumerationType,
            ArrayType,
            SubrangeType,
            PointerType,
            ConstType,
            ClassType,
            Inheritance,
            FormalParameter,
            Subprogram
        }

        public enum Attribute
        {
            Nothing,
            Name,
            ByteSize,
            Type,
            DataMemberLocation,
            BitSize,
            UpperBound,
            Location,
            Specification
        }

        public uint TagAddress { set; get; }
        public Tag TagName { set; get; }
        public string Name { set; get; }
        public int ByteSize { set; get; }
        public uint NextAddress { set; get; }
        public int DataMemberLocation { set; get; }
        public int UpperBound { set; get; }
        public uint Location { set; get; }
        public uint Specification { set; get; }
        public List<DebugInfo> Belongs { set; get; }

        public DebugInfo()
        {
            TagName = Tag.Nothing;
            Belongs = new List<DebugInfo>();
        }

        public DebugInfo(DebugInfo info)
        {
            TagAddress = info.TagAddress;
            TagName = info.TagName;

            Name = info.Name;
            ByteSize = info.ByteSize;
            NextAddress = info.NextAddress;
            DataMemberLocation = info.DataMemberLocation;

            Belongs = new List<DebugInfo>(info.Belongs);
        }

        public static Tag GetTag(string name)
        {
            if(name == "DW_TAG_base_type")
            {
                return Tag.BaseType;
            }
            else if (name == "DW_TAG_typedef")
            {
                return Tag.TypeDef;
            }
            else if (name == "DW_TAG_structure_type")
            {
                return Tag.StructureType;
            }
            else if (name == "DW_TAG_member")
            {
                return Tag.Member;
            }
            else if (name == "DW_TAG_union_type")
            {
                return Tag.Union;
            }
            else if (name == "DW_TAG_variable")
            {
                return Tag.Variable;
            }
            else if (name == "DW_TAG_volatile_type")
            {
                return Tag.VolatileType;
            }
            else if(name == "DW_TAG_enumeration_type")
            {
                return Tag.EnumerationType;
            }
            else if (name == "DW_TAG_array_type")
            {
                return Tag.ArrayType;
            }
            else if (name == "DW_TAG_subrange_type")
            {
                return Tag.SubrangeType;
            }
            else if (name == "DW_TAG_pointer_type")
            {
                return Tag.PointerType;
            }
            else if (name == "DW_TAG_const_type")
            {
                return Tag.ConstType;
            }
            else if (name == "DW_TAG_class_type")
            {
                return Tag.ClassType;
            }
            else if (name == "DW_TAG_inheritance")
            {
                return Tag.Inheritance;
            }
            else if (name == "DW_TAG_formal_parameter")
            {
                return Tag.FormalParameter;
            }
            else if (name == "DW_TAG_subprogram")
            {
                return Tag.Subprogram;
            }
            else
            {
                return Tag.Nothing;
            }

        }

        public static Attribute GetAttribute(string name)
        {
            if (name == "DW_AT_name")
            {
                return Attribute.Name;
            }
            else if (name == "DW_AT_byte_size")
            {
                return Attribute.ByteSize;
            }
            else if (name == "DW_AT_type")
            {
                return Attribute.Type;
            }
            else if (name == "DW_AT_data_member_location")
            {
                return Attribute.DataMemberLocation;
            }
            else if (name == "DW_AT_bit_size")
            {
                return Attribute.BitSize;
            }
            else if (name == "DW_AT_upper_bound")
            {
                return Attribute.UpperBound;
            }
            else if (name == "DW_AT_location")
            {
                return Attribute.Location;
            }
            else if (name == "DW_AT_specification")
            {
                return Attribute.Specification;
            }
            else
            {
                return Attribute.Nothing;
            }

        }

    }

    public class VariableElement
    {
        public string Symbol { set; get; }
        public int Address { set; get; }
        public int Offset { set; get; }

        public int Size { set; get; }

        public VariableElement()
        {
            Symbol = "";
            Address = 0;
            Offset = 0;
            Size = 0;

        }

        public VariableElement(VariableElement data)
        {
            Symbol = data.Symbol;
            Address = data.Address;
            Offset = data.Offset;
            Size = data.Size;

        }

    }

}
