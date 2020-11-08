using System;

namespace ClangReader.Lib.Ast.Models
{
    public class SimpleWrite
    {
        public string Type { get; set; }
        public DeclRefExprProperties Variable { get; set; }
    }

    public class DeclRefExprProperties : IEquatable<DeclRefExprProperties>
    {
        public DeclRefExprProperties(AstToken token)
        {
            Token = token;
            Type = token.properties[0];
            InstanceId = token.properties[3];
            Name = token.properties[4];
        }

        public AstToken Token { get;  }

        public string Type { get; }
        public string InstanceId { get; }
        public string Name { get; }

        public bool Equals(DeclRefExprProperties other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && InstanceId == other.InstanceId && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DeclRefExprProperties) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (InstanceId != null ? InstanceId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}