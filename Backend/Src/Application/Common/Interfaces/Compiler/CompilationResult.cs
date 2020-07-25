using System.Collections.Generic;
using Domain.Entities;

namespace Application.Common.Interfaces.Compiler
{
    public class CompilationResult
    {
        public bool Success { get; }

        public string Output { get; }
        
        public IReadOnlyList<BinaryFile> Files { get; }

        public CompilationResult(IEnumerable<BinaryFile> files)
        {
            Success = true;
            Output = string.Empty;
            Files = new List<BinaryFile>(files);
        }

        public CompilationResult(string output)
        {
            Output = output;
            Files = new List<BinaryFile>();
        }
    }
}