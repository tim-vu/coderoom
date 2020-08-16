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

namespace Infrastructure.Compiler.Compilers.CSharp
{
    public class CSharpCompiler : BaseCompiler, ICompiler
    {
        private readonly string _projectFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Compiler/Compilers/CSharp/Resources/Solution.csproj");

        private const string BuildArgs =
            "build --configuration Release --nologo -consoleLoggerParameters:NoSummary --verbosity:quiet /p:GenerateFullPaths=false";

        private const string Runtime = "netcoreapp3.1";

        private readonly ILogger<CSharpCompiler> _logger;

        public CSharpCompiler(ILogger<CSharpCompiler> logger)
        {
            _logger = logger;
        }

        public IReadOnlyCollection<Language> Languages { get; } = new List<Language> {Language.CSharp};

        public async Task<CompilationResult> Compile(IList<SourceFile> sourceFiles)
        {
            if (!sourceFiles.Any(s => s.Name.Equals("Solution.cs")))
            {
                return new CompilationResult("The source files do not contain Solution.cs");
            }

            BufferedCommandResult output = null;
            
            try
            {
                var projectFile = new SourceFile
                {
                    Content = await File.ReadAllTextAsync(_projectFilePath),
                    Name = Path.GetFileName(_projectFilePath)
                };

                var allFiles = new List<SourceFile> {projectFile};
                allFiles.AddRange(sourceFiles);

                await WriteFilesToTempDirectory(allFiles);

                var command = Cli.Wrap("dotnet")
                    .WithWorkingDirectory(TempDirectory)
                    .WithArguments(BuildArgs)
                    .WithValidation(CommandResultValidation.None);

                output = await command.ExecuteBufferedAsync();

                if (output.ExitCode != 0)
                {
                    return new CompilationResult(RemoveFullPaths(output.StandardOutput));
                }

                var files = new List<BinaryFile>();
                var outputDir = Path.Combine(TempDirectory, "bin", "Release", Runtime);
                foreach (var file in Directory.EnumerateFiles(outputDir))
                {
                    var filename = Path.GetFileName(file);

                    if (!filename.EndsWith(".dll") && filename != "Solution.runtimeconfig.json")
                        continue;
                        

                    files.Add(new BinaryFile(filename, await File.ReadAllBytesAsync(file)));
                }
                
                Cleanup();
                
                return new CompilationResult(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to compile");
                
                return output != null ? new CompilationResult(output.StandardOutput) : new CompilationResult("Unknown error occurred");
            }
        }

        private static string RemoveFullPaths(string buildOutput)
        {
            return buildOutput;
        }
    }
}