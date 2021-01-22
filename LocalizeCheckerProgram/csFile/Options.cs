using CommandLine;

namespace LocalizeChecker
{
    class Options
    {
        [Option('r', "revert", SetName = "revert", Required = true, HelpText = "파일 변환")]
        public bool Revert { get; set; }

        [Option('s', "stretch", SetName = "stretch", Required = true, HelpText = "파일 변환")]
        public bool Stretch { get; set; }

        [Option('t', "target", Required = true, HelpText = "솔루션(.sln) 파일 경로 \nex) --target \"d:\\sample\\red.sln\"")]
        public string FilePath { get; set; }

    }
}

