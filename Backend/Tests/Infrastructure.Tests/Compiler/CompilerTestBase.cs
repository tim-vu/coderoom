using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Compiler.Compilers.Java;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.Compiler
{
    public class CompilerTestBase
    {

        protected static async Task<List<SourceFile>> LoadSourceFiles(string path)
        {
            var files = new List<SourceFile>();

            foreach (var file in Directory.EnumerateFiles(path))
            {
                files.Add(new SourceFile
                {
                    Name = Path.GetFileName(file),
                    Content = await File.ReadAllTextAsync(file)
                });
            }

            return files;
        }
    }
}