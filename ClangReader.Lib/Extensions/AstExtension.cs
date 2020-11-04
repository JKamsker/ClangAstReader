using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClangReader.Utilities
{
    public static class AstExtension
    {
        public static Expression AsExpression(this AstToken token)
        {
            var name = token.name;
            if (name == "BinaryOperator")
            {
                ExpressionType type = token.properties[1] switch
                {
                    "+" => ExpressionType.Add,
                    "'+'" => ExpressionType.Add,
                    _ => throw new NotImplementedException(token.properties[1])
                };

                return Expression.MakeBinary(type, token.children[0].AsExpression(), token.children[1].AsExpression());
            }

            if (name == "ParenExpr")
            {
                if (token.children.Count == 1)
                {
                    return token.children[0].AsExpression();
                }
                throw new NotSupportedException("ParenExpr with many children is Not supported");
            }

            if (name == "IntegerLiteral")
            {
                if (token.properties[0] == "int")
                {
                    return Expression.Constant(int.Parse(token.properties[1]));
                }
                throw new NotSupportedException("This literal is not supported");
            }

            throw new NotSupportedException("This exp type is not supported");
        }

        public static IEnumerable<AstToken> VisitEnumerable(this AstToken token, Func<AstToken, bool> predicate)
        {
            foreach (var tokenChild in token.children)
            {
                if (predicate(tokenChild))
                {
                    yield return tokenChild;
                }
                var results = VisitEnumerable(tokenChild, predicate);
                foreach (var astToken in results)
                {
                    yield return astToken;
                }
            }
        }

        public static void Visit(this AstToken token, Func<AstToken, bool> predicate, Action<AstToken> onFound)
        {
            foreach (var tokenChild in token.children)
            {
                if (predicate(tokenChild))
                {
                    onFound(tokenChild);
                }
                Visit(tokenChild, predicate, onFound);
            }
        }

        public static string SerializeFriendly(this AstToken token)
        {
            return JsonConvert.SerializeObject(token.AsTokenDto(), Formatting.Indented);
        }
    }
}