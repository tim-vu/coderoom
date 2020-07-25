using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces.Compiler;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Compiler.Compilers.CSharp;
using Infrastructure.Compiler.Compilers.InterpretedLanguage;
using Infrastructure.Compiler.Compilers.Java;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Compiler
{
    public class MultiLanguageCompiler : IMultiLanguageCompiler
    {
        
        private static readonly IReadOnlyCollection<Type> CompilerTypes = new List<Type>
        {
            typeof(JavaCompiler),
            typeof(CSharpCompiler),
            typeof(InterpretedLanguageCompiler)
        };

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MultiLanguageCompiler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task<CompilationResult> Compile(Language language, IList<SourceFile> sourceFiles)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                foreach (var type in CompilerTypes)
                {
                    var compiler = (ICompiler) ActivatorUtilities.CreateInstance(scope.ServiceProvider, type);
                
                    if (!compiler.Languages.Contains(language))
                        continue;

                    return compiler.Compile(sourceFiles);
                }
            }

            return Task.FromResult(new CompilationResult("Language not supported"));
        }
    }
}