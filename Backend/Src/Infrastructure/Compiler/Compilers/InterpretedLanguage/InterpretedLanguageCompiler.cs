using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces.Compiler;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Compiler.Compilers.InterpretedLanguage
{
    public class InterpretedLanguageCompiler : ICompiler
    {
        public IReadOnlyCollection<Language> Languages { get; } = new List<Language>() {Language.Python3, Language.JavaScript, Language.TypeScript};
        public Task<CompilationResult> Compile(IList<SourceFile> sourceFiles)
        {
            return Task.FromResult(new CompilationResult(sourceFiles.Select(s => new BinaryFile(s.Name, Encoding.UTF8.GetBytes(s.Content))).ToList()));
        }
    }
}