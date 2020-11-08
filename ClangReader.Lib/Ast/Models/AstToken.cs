using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ClangReader.Lib.Ast.Models
{
    [Flags]
    public enum AstAttributes
    {
        // ReSharper disable once InconsistentNaming
        Not_Defined,

        Implicit = 1 << 1,
        Used = 1 << 2,
        Referenced = 1 << 3,
        CInit = 1 << 4,
        Extern = 1 << 5,
        Callinit = 1 << 6,
        Static = 1 << 7,
        Definition = 1 << 8,
        Nrvo = 1 << 9,
        Struct = 1 << 10,
        Guid = 1 << 10,
        Default = 1 << 11,

        __int128 = 1 << 12,
        __int128_t = 1 << 13
    }

    public class AstToken
    {
        public string unknownName;

        public string offset;
        public string relationOffset;
        public string fileContext;
        public string filePointer;

        public AstTokenContext context;

        public AstAttributes Attributes { get; set; }
        public AstKnownSuffix Type { get; set; }

        public string[] properties = Array.Empty<string>();
        public string[] additionalAttributes = Array.Empty<string>();

        public List<AstToken> children = new List<AstToken>();
        public AstToken parent;

        public AstToken() : this(true)
        {
        }

        public AstToken(bool initializeChildren = true)
        {
            if (initializeChildren)
            {
                children = new List<AstToken>();
            }
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(unknownName) ? Type.ToString() : unknownName;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void AddChild(AstToken token, int depth)
        {
            if (depth <= 0)
            {
                AddChild(token);
                return;
            }

            children[^1].AddChild(token, depth - 1);
        }

        public void AddChild(AstToken token)
        {
            token = token ?? throw new Exception();
            if (token.parent != null && token.parent != this)
            {
                throw new ArgumentException("This child already has an parent", nameof(token.parent));
            }

            children.Add(token);
            token.parent = this;
        }

        public IEnumerable<AstToken> TraverseParents(bool includeThis = false)
        {
            if (includeThis)
            {
                yield return this;
            }

            if (parent == null)
            {
                yield break;
            }

            foreach (var parents in parent.TraverseParents(true))
            {
                yield return parents;
            }
        }

        public AstTokenDto AsTokenDto()
        {
            var thisCopy = new AstTokenDto
            {
                Name = unknownName,
                Type = Type,
                Attributes = additionalAttributes,
                Properties = properties,
                Children = new List<AstTokenDto>(children.Count),
            };

            foreach (var child in children)
            {
                thisCopy.Children.Add(child.AsTokenDto());
            }

            return thisCopy;
        }
    }
}