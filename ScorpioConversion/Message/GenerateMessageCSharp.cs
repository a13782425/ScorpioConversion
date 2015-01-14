﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
public class GenerateMessageCSharp : IGenerate
{
    public GenerateMessageCSharp() : base(PROGRAM.CSharp) {}
    protected override string Generate_impl()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(@"using System.Collections.Generic;
using Scorpio.Commons;
using Scorpio.Message;
namespace __Package {
public class __ClassName : IMessage {");
        builder.Append(GenerateMessageFields());
        builder.Append(GenerateMessageWrite());
        builder.Append(GenerateMessageRead());
        builder.Append(GenerateMessageDeserialize());
        builder.Append(@"
}
}");
        builder.Replace("__ClassName", m_ClassName);
        builder.Replace("__Package", m_Package);
        return builder.ToString();
    }
    string GenerateMessageFields()
    {
        StringBuilder builder = new StringBuilder();
        foreach (var field in m_Fields)
        {
            string str = "";
            if (field.Array) {
                str = @"
    private List<__Type> ___Name;
    public List<__Type> get__Name() { return ___Name; }
    public void set__Name(List<__Type> value) { ___Name = value; AddSign(__Index); } ";
            } else {
                str = @"
    private __Type ___Name;
    public __Type get__Name() { return ___Name; }
    public void set__Name(__Type value) { ___Name = value; AddSign(__Index); } ";
            }
            str = str.Replace("__Index", field.Index.ToString());
            str = str.Replace("__Name", field.Name);
            str = str.Replace("__Type", GetCodeType(field.Type));
            builder.Append(str);
        }
        return builder.ToString();
    }
    string GenerateMessageWrite()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(@"
    public override void Write(ScorpioWriter writer) {
        writer.WriteInt32(__Sign);");
        foreach (var field in m_Fields)
        {
            string str = "";
            if (field.Array) {
                str = @"
        if (HasSign(__Index)) {
            writer.WriteInt32(___Name.Count);
            for (int i = 0;i < ___Name.Count; ++i) { __FieldWrite; }
        }";
            } else {
                str = @"
        if (HasSign(__Index)) { __FieldWrite; }";
            }
            string name = field.Array ? "___Name[i]" : "___Name";
            string write = field.IsBasic ? "writer.__Write(___Name)" : (field.Enum ? "writer.__Write((int)___Name)" : "___Name.Write(writer)");
            write = write.Replace("___Name", name);
            str = str.Replace("__FieldWrite", write);
            str = str.Replace("__Write", field.IsBasic ? field.Info.WriteFunction : (field.Enum ? "WriteInt32" : ""));
            str = str.Replace("__Index", field.Index.ToString());
            str = str.Replace("__Name", field.Name);
            builder.Append(str);
        }
        builder.Append(@"
    }");
        return builder.ToString();
    }
    string GenerateMessageRead()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(@"
    public static __ClassName Read(ScorpioReader reader) {
        __ClassName ret = new __ClassName();
        ret.__Sign = reader.ReadInt32();");
        foreach (var field in m_Fields) {
            string str = "";
            if (field.Array) {
                str = @"
        if (ret.HasSign(__Index)) {
            int number = reader.ReadInt32();
            ret.___Name = new List<__Type>();
            for (int i = 0;i < number; ++i) { ret.___Name.Add(__FieldRead); }
        }";
            } else {
                str = @"
        if (ret.HasSign(__Index)) { ret.___Name = __FieldRead; }";
            }
            str = str.Replace("__FieldRead", field.IsBasic ? "reader.__Read()" : (field.Enum ? "(__Type)reader.ReadInt32()" : "__Type.Read(reader)"));
            str = str.Replace("__Read", field.IsBasic ? field.Info.ReadFunction : "");
            str = str.Replace("__Type", GetCodeType(field.Type));
            str = str.Replace("__Index", field.Index.ToString());
            str = str.Replace("__Name", field.Name);
            builder.Append(str);
        }
        builder.Append(@"
        return ret;
    }");
        return builder.ToString();
    }
    static string GenerateMessageDeserialize()
    {
        return @"
    public static __ClassName Deserialize(byte[] data) {
        return Read(new ScorpioReader(data));
    }";
    }
}
