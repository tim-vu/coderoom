using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces.Compiler;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Compiler
{
    public interface ICompiler
    {
        IReadOnlyCollection<Language> Languages { get; }

        Task<CompilationResult> Compile(IList<SourceFile> sourceFiles);
    }
}