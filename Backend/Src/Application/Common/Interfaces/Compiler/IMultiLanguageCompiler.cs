using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Interfaces.Compiler
{
    public interface IMultiLanguageCompiler
    {
        Task<CompilationResult> Compile(Language language, IList<SourceFile> sourceFiles);
    }
}