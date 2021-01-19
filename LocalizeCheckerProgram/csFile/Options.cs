using CommandLine;

namespace LocalizeChecker
{
    class Options
    {
        [Option("target", Required = true, HelpText = "솔루션(.sln) 파일 경로 \nex) --target \"d:\\sample\\red.sln\"")]
        public string FilePath { get; set; }
    }
}

