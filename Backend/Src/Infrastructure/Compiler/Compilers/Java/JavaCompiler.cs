using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces.Compiler;
using CliWrap;
using CliWrap.Buffered;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Compiler.Compilers.Java
{
    public class JavaCompiler : BaseCompiler, ICompiler
    {
        
        public IReadOnlyCollection<Language> Languages { get; } = new List<Language> {Language.Java };

        private readonly ILogger<JavaCompiler> _logger;

        public JavaCompiler(ILogger<JavaCompiler> logger)
        {
            _logger = logger;

            Directory.CreateDirectory(TempDirectory);
        }

        public async Task<CompilationResult> Compile(IList<SourceFile> sourceFiles)
        {
            if (!sourceFiles.Any(s => s.Name.Equals("Solution.java")))
                return new CompilationResult("The source files do not contain Solution.java");

            BufferedCommandResult output = null;
            
            try
            {
                await WriteFilesToTempDirectory(sourceFiles);

                var command = Cli.Wrap("javac")
                    .WithWorkingDirectory(TempDirectory)
                    .WithArguments(sourceFiles.Select(f => f.Name).ToArray())
                    .WithValidation(CommandResultValidation.None);

                output = await command.ExecuteBufferedAsync();

                if (output.ExitCode != 0)
                {
                    return new CompilationResult(output.StandardError);
                }

                var files = new List<BinaryFile>();
                foreach (var file in Directory.EnumerateFiles(TempDirectory, "*.class", SearchOption.TopDirectoryOnly))
                {
                    files.Add(new BinaryFile(Path.GetFileName(file), await File.ReadAllBytesAsync(file)));
                }

                Cleanup();
                
                return new CompilationResult(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to compile");

                return output != null ? new CompilationResult(output.StandardError) : new CompilationResult("Unknown error occurred");
            }
        }

    }
}