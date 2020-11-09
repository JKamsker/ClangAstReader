using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ClangReader.LanguageTranslation;
using ClangReader.Lib.Ast;
using ClangReader.Lib.Ast.Models;
using ClangReader.Lib.Extensions;
using ClangReader.Utilities;

namespace ClangReader
{
    internal class FourSPacketTranslationVisitor
    {
        public void VisitTest(AstToken token)
        {
            if (token.Type == AstType.ForStmt)
            {
                Console.WriteLine("For statement");
            }

            foreach (var child in token.children)
            {
                VisitTest(child);
            }

            if (IsPacketShiftExpression(token))
            {
                //Type from token.children[0]
                //
                //|-CXXOperatorCallExpr 0x1f38c6b5468 <line:2997:2, line:3005:6> 'CPacket' lvalue '<<'
                //| |-ImplicitCastExpr 0x1f38c6b5450 <col:3> 'CPacket &(*)(int)' <FunctionToPointerDecay>
                //| | `-DeclRefExpr 0x1f38c6b5430 <col:3> 'CPacket &(int)' lvalue CXXMethod 0x1f386e73ac8 'operator<<' 'CPacket &(int)'
                if (TryGetPacketWriteOverloadType(token.children[0], out var typeName))
                {
                    Console.Write($"Write type : {typeName} Name: ");
                }
                else
                {
                    Console.WriteLine("Write type not found!");
                    Debugger.Break();
                    return;
                }

                var instructionvisitor = 


                if (token.children.Count > 1 && TryGetSimpleWriteFromVar(token.children[2], out var prop))
                {
                    Console.Write(prop.Name);

                    //var callPart = token.children[0].SerializeFriendly();

                    //Debugger.Break();
                }
                else if (TryGetMethodCallWrite(token.children[2], out var name))
                {
                    //Debugger.Break();

                    Console.Write(name);
                }
                else if (TryParseSizeAccessor(token.children[2]))
                {
                    //Debugger.Break();
                }
                else
                {
                    Console.Write("Unknown");

                    var bdy = AstTranslator.GetFunctionBody(token.children[2]);
                    var relevant = token.AsTokenDto();
                    relevant.Children[1] = new AstTokenDto();
                    var reser = relevant.SerializeFriendly();

                    Debugger.Break();
                }

                //var accessor = GetChildAccesor(token.children[2]);
                Console.WriteLine();
            }
        }

        // reference accessor (pInfo->wNpcID) SendCS_AUCTIONREG_REQ
        /*
         |   | | | | | | | `-ImplicitCastExpr 0x1f38c61d1e8 <line:2201:6, col:13> 'WORD':'unsigned short' <LValueToRValue>
         |   | | | | | | |   `-MemberExpr 0x1f38c61c578 <col:6, col:13> 'WORD':'unsigned short' lvalue ->wNpcID 0x1f38a41b8f0
         |   | | | | | | |     `-ImplicitCastExpr 0x1f38c61c560 <col:6> 'LPTAUCTIONREGINFO':'struct tagTAUCTIONREGINFO *' <LValueToRValue>
         |   | | | | | | |       `-DeclRefExpr 0x1f38c61c540 <col:6> 'LPTAUCTIONREGINFO':'struct tagTAUCTIONREGINFO *' lvalue ParmVar 0x1f38c61c1a0 'pInfo' 'LPTAUCTIONREGINFO':'struct tagTAUCTIONREGINFO *'
        */

        // Size Accessor
        /*     `-CXXFunctionalCastExpr 0x1f38c6b7060 <line:3008:6, col:25> 'BYTE':'unsigned char' functional cast to BYTE <NoOp>
        |   |   `-ImplicitCastExpr 0x1f38c6b7048 <col:11, col:24> 'BYTE':'unsigned char' <IntegralCast> part_of_explicit_cast
        |   |     `-CXXMemberCallExpr 0x1f38c6b6ff8 <col:11, col:24> 'std::vector<tagTSKILLTARGET *, std::allocator<tagTSKILLTARGET *>>::size_type':'unsigned long long'
        |   |       `-MemberExpr 0x1f38c6b6fc8 <col:11, col:19> '<bound member function type>' .size 0x1f38a7d6710
        |   |         `-DeclRefExpr 0x1f38c6b6fa8 <col:11> 'const VTSKILLTARGET':'const std::vector<tagTSKILLTARGET *, std::allocator<tagTSKILLTARGET *>>' lvalue ParmVar 0x1f38c6ae888 'vTarget' 'const VTSKILLTARGET &'
        */

        //DeclRefExpr
        /*		[0]	"const VTSKILLTARGET':'const std::vector<tagTSKILLTARGET *, std::allocator<tagTSKILLTARGET *>>"	string
		        [1]	"lvalue"	string
		        [2]	"ParmVar"	string
		        [3]	"0x1f38c6ae888"	string
		        [4]	"vTarget"	string
		        [5]	"const VTSKILLTARGET &"	string
        */

        //MemberExpr
        /*		[0]	"<bound member function type>"	string
		        [1]	".size"	string
		        [2]	"0x1f38a7d6710"	string
        */

        //Parses size accessor like Vector.size() and converts it to Array.Length;
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
                        Console.Write(".Length");
                        return true;
                    }

                    var toWrite = token.properties[1] == "lvalue" ? token.properties[2] : token.properties[1];

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
                        Console.Write($"Vector found: {structName}");
                    }
                    else
                    {
                        var structTypeRegex = new Regex("':'(.*?) (.*?) ");
                        var match = structTypeRegex.Match(token.properties[0]);
                        if (match.Success)
                        {
                            Console.Write($"{match.Groups[1].Value} found: {match.Groups[2].Value}");
                        }
                    }

                    Console.Write($" Name: {token.properties[4]}");
                    //Debugger.Break();
                    return true;

                    break;

                default:
                    return found;
            }

            return found;
        }

        private bool TryGetPacketWriteOverloadType(AstToken token, out string typeName)
        {
            switch (token.Type)
            {
                case AstType.ImplicitCastExpr:
                    foreach (var tokenChild in token.children)
                    {
                        if (TryGetPacketWriteOverloadType(tokenChild, out typeName))
                        {
                            return true;
                        }
                    }
                    break;

                case AstType.DeclRefExpr:
                    var match = Regex.Match(token.properties[0], "CPacket.*?\\((.*?)\\)");
                    if (match.Success)
                    {
                        typeName = match.Groups[1].Value;
                        return true;
                    }
                    break;

                default:
                    break;
            }

            typeName = string.Empty;
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

        private bool IsPacketShiftExpression(AstToken token)
        {
            return token.Type == AstType.CXXOperatorCallExpr && token.properties.Length <= 3 && (token.properties[2] == "<<" || token.properties[2] == "'<<'");
        }

        private bool TryGetSimpleWriteFromVar(AstToken token, out DeclRefExprProperties properties)
        {
            properties = null;
            switch (token.Type)
            {
                case AstType.DeclRefExpr when token.children.Count == 0:
                    properties = new DeclRefExprProperties(token);
                    return true;

                // case AstType.CXXOperatorCallExpr when IsPacketShiftExpression(token):
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
    }
}