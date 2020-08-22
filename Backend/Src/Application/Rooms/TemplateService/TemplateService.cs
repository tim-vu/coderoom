using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Application.Rooms.RoomService;
using Domain.Enums;

namespace Application.Rooms.TemplateService
{
    public class TemplateService : ITemplateService
    {
        private readonly string _templateDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rooms/TemplateService/Templates");
        
        private static readonly IReadOnlyDictionary<Language, string> TemplatePaths = new Dictionary<Language, string>()
        {
            {Language.Java, "Solution.java"},
            {Language.CSharp, "Solution.cs"},
            {Language.Python3, "Solution.py"}
        };
        
        private static readonly ConcurrentDictionary<Language, string> Templates = new ConcurrentDictionary<Language, string>();
        
        public string GetLanguageTemplate(Language language)
        {

            if (Templates.TryGetValue(language, out var template))
            {
                return template;
            }

            if (!TemplatePaths.TryGetValue(language, out var relativePath))
            {
                Templates.TryAdd(language, string.Empty);
                return string.Empty;
            }
            var path = Path.Combine(_templateDirectory, relativePath);

            var content = File.ReadAllText(path);

            Templates.TryAdd(language, content);
            return content;
        }
    }
}