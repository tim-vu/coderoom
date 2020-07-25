using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Compiler.Compilers
{
    public abstract class BaseCompiler
    {

        protected readonly string TempDirectory;
        
        protected BaseCompiler()
        {
            TempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(TempDirectory);
        }

        protected async Task WriteFilesToTempDirectory(ICollection<SourceFile> sourceFiles)
        {
            foreach (var file in sourceFiles)
            {
                await File.WriteAllTextAsync(Path.Combine(TempDirectory, file.Name), file.Content);
            }
        }
        
        protected void Cleanup()
        {
            Directory.Delete(TempDirectory, true);
        }
    }
}