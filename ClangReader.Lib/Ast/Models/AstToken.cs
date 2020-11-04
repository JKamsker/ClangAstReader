using System;
using System.Collections.Generic;

namespace ClangReader
{
    public class AstToken
    {
        public string name;

        public string offset;
        public string relationOffset;
        public string fileContext;
        public string filePointer;

        public AstTokenContext context;
        public string[] properties = Array.Empty<string>();
        public string[] attributes = Array.Empty<string>();

        public List<AstToken> children = new List<AstToken>();
        public AstToken parent;

        public AstToken() : this(true)
        {
            
        }

        public AstToken(bool initializeChildren  = true)
        {
            if (initializeChildren)
            {
                children = new List<AstToken>();
            }
        }

        public override string ToString()
        {
            return name;
        }

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
                throw new ArgumentException("This child already has an parrent", nameof(token.parent));
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
                Name = name,
                Attributes = attributes,
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