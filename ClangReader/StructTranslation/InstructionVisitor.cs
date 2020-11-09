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

            if (TryParseSizeAccessor(_token))
            {
                return true;
            }

            return false;
        }

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