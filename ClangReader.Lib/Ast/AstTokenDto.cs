using System;
using System.Collections.Generic;

using ClangReader.Lib.Ast.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ClangReader.Lib.Ast
{
    public class AstTokenDto
    {
        [JsonConverter(typeof(StringEnumConverter)), JsonProperty(Order = -1)]
        public AstType Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        public string[] Properties { get; set; } = Array.Empty<string>();
        public string[] Attributes { get; set; } = Array.Empty<string>();

        public List<AstTokenDto> Children { get; set; }

        public bool ShouldSerializeProperties() => Properties?.Length > 0;

        public bool ShouldSerializeAttributes() => Attributes?.Length > 0;

        public bool ShouldSerializeChildren() => Children?.Count > 0;
    }
}