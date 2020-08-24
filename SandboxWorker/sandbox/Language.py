class Language:

    def __init__(self, name: str, key: int, main_file: str, run_command: str):
        self.name = name
        self.key = key
        self.main_file = main_file
        self.run_command = run_command


class Languages:

    JAVA = Language('Java', 0, 'Solution.class', 'java -cp . Solution')
    CSHARP = Language('C#', 1, 'Solution.dll', 'dotnet Solution.dll')
    PYTHON3 = Language('Python3', 2, 'Solution.py', 'python3 Solution.py')
    JAVASCRIPT = Language('JavaScript', 3, 'Solution.js', 'node Solution.js')

    ALL = [JAVA, CSHARP, PYTHON3, JAVASCRIPT]

    @staticmethod
    def get_language_by_key(key: str) -> Language:

        for language in Languages.ALL:

            if language.key == key:
                return language
