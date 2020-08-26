import re

class Language:

    def __init__(self, name: str, key: int, main_file: str, run_command: str):
        self.name = name
        self.key = key
        self.main_file = main_file
        self.run_command = run_command

    @staticmethod
    def process_output(output):
        return output


class DenoLanguage(Language):

    FILENAME_REGEX = "Check file.+\n"

    @staticmethod
    def process_output(output):

        output = re.subn(DenoLanguage.FILENAME_REGEX, '', output)[0]

        if 'error' in output:
            return output.partition('\n')[0]

        return output


class Languages:

    JAVA = Language('Java', 0, 'Solution.class', 'java -cp . Solution')
    CSHARP = Language('C#', 1, 'Solution.dll', 'dotnet Solution.dll')
    PYTHON3 = Language('Python3', 2, 'Solution.py', 'python3 Solution.py')
    JAVASCRIPT = DenoLanguage('JavaScript', 3, 'Solution.js', '(export NO_COLOR=true ; deno run Solution.js)')
    TYPESCRIPT = DenoLanguage('TypeScript', 4, 'Solution.ts', '(export NO_COLOR=true ; deno run Solution.ts)')

    ALL = [JAVA, CSHARP, PYTHON3, JAVASCRIPT, TYPESCRIPT]

    @staticmethod
    def get_language_by_key(key: str) -> Language:

        for language in Languages.ALL:

            if language.key == key:
                return language
