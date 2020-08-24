using System;

namespace Domain.Enums
{
    public enum Language
    {
        [LanguageAttribute("java", "Java")]
        Java,
        
        [LanguageAttribute("cs", "C#")]
        CSharp,
        
        [LanguageAttribute("py", "Python3")]
        Python3,
        
        [LanguageAttribute("js", "JavaScript")]
        JavaScript
    }

    public static class LanguageExtensions
    {
        public static string GetFileExtension(this Language language)
        {
            var attr = language.GetAttribute<LanguageAttribute>();
            return attr.FileExtension;
        }

        public static string GetName(this Language language)
        {
            var attr = language.GetAttribute<LanguageAttribute>();
            return attr.Name;
        }
    }
    
    public class LanguageAttribute : Attribute
    {
        public string FileExtension { get; }
        
        public string Name { get; }
        
        internal LanguageAttribute(string fileExtension, string name)
        {
            FileExtension = fileExtension;
            Name = name;
        }
    }
}