using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangReader.Pipelined
{
    public class BatchLineReadResult : IDisposable, IEnumerable<BatchLineItem>
    {
        public List<BatchLineItem> Items { get; set; }
        public IDisposable Disposable { get; set; }

        public void Dispose()
        {
            Disposable?.Dispose();
            Disposable = null;
        }

        public IEnumerator<BatchLineItem> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Items).GetEnumerator();
    }
    public readonly struct BatchLineItem
    {
        public Memory<char> LineCopy { get; }

        public BatchLineItem(Memory<char> lineCopy)
        {
            this.LineCopy = lineCopy;
        }
    }

}