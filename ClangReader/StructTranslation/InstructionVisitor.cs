using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ClangReader.Lib.Ast.Models;
using ClangReader.Utilities;

namespace ClangReader.StructTranslation
{
    public class InstructionVisitor
    {
        private readonly AstToken _token;
        private PacketWriteEntry _resultObject = null;

        //packetShiftToken token.children[2]
        public InstructionVisitor(AstToken token)
        {
            _token = token;
        }

        public bool Execute()
        {
            if (TryGetSimpleWriteFromVar(_token, out var varDecl))
            {
                Debugger.Break();
                return true;
            }

            if (TryGetMethodCallWrite(_token, out var name))
            {
                Debugger.Break();
                return true;
            }

            //Write type : BYTE Name: Vector found: TSKILLTARGET Name: vTarget.Length
            if (TryParseSizeAccessor(_token))
            {
                return true;
            }

            return false;
        }

        private T GetResultObject<T>() where T : PacketWriteEntry, new()
        {
            if (_resultObject is null)
            {
                var result = new T();
                _resultObject = result;
                return result;
            }

            if (_resultObject is T resultObject)
            {
                return resultObject;
            }

            throw new InvalidOperationException("ResultObject has been set and cannot be overwritten");
        }

        /*
       // reference accessor (pInfo->wNpcID) SendCS_AUCTIONREG_REQ

        |   | | | | | | | `-ImplicitCastExpr 0x1f38c61d1e8 <line:2201:6, col:13> 'WORD':'unsigned short' <LValueToRValue>
        |   | | | | | | |   `-MemberExpr 0x1f38c61c578 <col:6, col:13> 'WORD':'unsigned short' lvalue ->wNpcID 0x1f38a41b8f0
        |   | | | | | | |     `-ImplicitCastExpr 0x1f38c61c560 <col:6> 'LPTAUCTIONREGINFO':'struct tagTAUCTIONREGINFO *' <LValueToRValue>
        |   | | | | | | |       `-DeclRefExpr 0x1f38c61c540 <col:6> 'LPTAUCTIONREGINFO':'struct tagTAUCTIONREGINFO *' lvalue ParmVar 0x1f38c61c1a0 'pInfo' 'LPTAUCTIONREGINFO':'struct tagTAUCTIONREGINFO *'

       / Size Accessor
            `-CXXFunctionalCastExpr 0x1f38c6b7060 <line:3008:6, col:25> 'BYTE':'unsigned char' functional cast to BYTE <NoOp>
       |   |   `-ImplicitCastExpr 0x1f38c6b7048 <col:11, col:24> 'BYTE':'unsigned char' <IntegralCast> part_of_explicit_cast
       |   |     `-CXXMemberCallExpr 0x1f38c6b6ff8 <col:11, col:24> 'std::vector<tagTSKILLTARGET *, std::allocator<tagTSKILLTARGET *>>::size_type':'unsigned long long'
       |   |       `-MemberExpr 0x1f38c6b6fc8 <col:11, col:19> '<bound member function type>' .size 0x1f38a7d6710
       |   |         `-DeclRefExpr 0x1f38c6b6fa8 <col:11> 'const VTSKILLTARGET':'const std::vector<tagTSKILLTARGET *, std::allocator<tagTSKILLTARGET *>>' lvalue ParmVar 0x1f38c6ae888 'vTarget' 'const VTSKILLTARGET &'

       //DeclRefExpr
               [0]	"const VTSKILLTARGET':'const std::vector<tagTSKILLTARGET *, std::allocator<tagTSKILLTARGET *>>"	string
               [1]	"lvalue"	string
               [2]	"ParmVar"	string
               [3]	"0x1f38c6ae888"	string
               [4]	"vTarget"	string
               [5]	"const VTSKILLTARGET &"	string

       //MemberExpr
               [0]	"<bound member function type>"	string
               [1]	".size"	string
               [2]	"0x1f38a7d6710"	string

       //Parses size accessor like Vector.size() and converts it to Array.Length;
       */

        private bool TryParseSizeAccessor(AstToken token)
        {
            var found = token.children.Select(TryParseSizeAccessor).FirstOrDefault(x => x == true);
            if (token.children.Any() && !found)
            {
                return false;
            }

            switch (token.Type)
            {
                case AstType.CXXFunctionalCastExpr:
                case AstType.ImplicitCastExpr:

                    break;

                case AstType.CXXMemberCallExpr:
                    break;

                case AstType.MemberExpr:
                    if (token.properties[1].EndsWith("size"))
                    {
                        //Console.Write("Length");
                        GetResultObject<SecondaryObjectPacketWriteEntry>().FieldName = "Length";
                        return true;
                    }

                    var toWrite = token.properties[1] == "lvalue" ? token.properties[2] : token.properties[1];
                    //->wNpcID
                    GetResultObject<SecondaryObjectPacketWriteEntry>().FieldName = toWrite.TrimStart("->");

                    Console.Write($" {toWrite}");
                    return true;
                    //Debugger.Break();
                    break;

                case AstType.DeclRefExpr:
                    var vectorRegex = new Regex("std\\:\\:vector\\<(.*?) .*?\\,");
                    var matches = vectorRegex.Match(token.properties[0]);
                    if (matches.Success)
                    {
                        //its a vector
                        var structName = matches.Groups[1].Value.TrimStart("tag");
                        GetResultObject<SecondaryObjectPacketWriteEntry>().ObjectType = structName;
                        GetResultObject<SecondaryObjectPacketWriteEntry>().IsArray = true;
                        //GetResultObject<SecondaryObjectPacketWriteEntry>().ObjectName = match.Groups[2].Value;
                        //Console.Write($"Vector found: {structName}");
                    }
                    else
                    {
                        var structTypeRegex = new Regex("':'(.*?) (.*?) ");
                        var match = structTypeRegex.Match(token.properties[0]);
                        if (match.Success)
                        {
                            //struct found: tagTAUCTIONREGINFO
                            //Console.Write($"{match.Groups[1].Value} found: {match.Groups[2].Value}");

                            GetResultObject<SecondaryObjectPacketWriteEntry>().ObjectType = match.Groups[1].Value;
                            GetResultObject<SecondaryObjectPacketWriteEntry>().ObjectName = match.Groups[2].Value.TrimStart("tag");
                        }
                    }

                    //pInfo
                    //Console.Write($" Name: {token.properties[4]}");
                    //Debugger.Break();
                    return true;

                    break;

                default:
                    Debugger.Break();
                    return found;
            }

            return found;
        }

        private bool TryGetSimpleWriteFromVar(AstToken token, out DeclRefExprProperties properties)
        {
            properties = null;
            switch (token.Type)
            {
                case AstType.DeclRefExpr when token.children.Count == 0:
                    properties = new DeclRefExprProperties(token);
                    return true;

                case AstType.ImplicitCastExpr:
                    foreach (var tokenChild in token.children)
                    {
                        if (TryGetSimpleWriteFromVar(tokenChild, out properties))
                        {
                            return true;
                        }
                    }
                    break;

                default:
                    break;
            }
            return false;
        }

        private bool TryGetMethodCallWrite(AstToken token, out string name)
        {
            //ImplicitCastExpr

            name = string.Empty;

            DeclRefExprProperties properties = null;
            switch (token.Type)
            {
                case AstType.MemberExpr when token.properties[0] == "<bound member function type>":
                    name = token.properties[1];
                    name = name.TrimStart("->").TrimStart("Get");
                    return true;

                case AstType.CXXMemberCallExpr:
                    foreach (var tokenChild in token.children)
                    {
                        if (TryGetMethodCallWrite(tokenChild, out name))
                        {
                            return true;
                        }
                    }
                    break;

                default:
                    break;
            }
            return false;
            return false;
        }
    }
}