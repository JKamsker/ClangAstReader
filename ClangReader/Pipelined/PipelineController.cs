using System;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

using ClangReader.Utilities;

namespace ClangReader.Pipelined
{
    public class PipelineController
    {
        private Channel<BatchLineReadResult> _lineInputChannel;
        private FileToLinesProcessor _reader;
        private LineBatchToMinimalInfoProcessor _minimalParser;

        public PipelineController(string sourcefile)
        {
            var lineReader = new FastLineReader(sourcefile);
            //@"C:\Users\Weirdo\source\repos\4Story\Agnares\4Story_5.0_Source\Client\astout\cssender-02.ast")
            _lineInputChannel = Channel.CreateBounded<BatchLineReadResult>(new BoundedChannelOptions(100)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true
            });

            _reader = new FileToLinesProcessor(lineReader, _lineInputChannel.Writer);
            _minimalParser = new LineBatchToMinimalInfoProcessor(_lineInputChannel.Reader);
        }

        public async Task Start()
        {
        }
    }
}