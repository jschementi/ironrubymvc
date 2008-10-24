namespace IronRubyMvc.Tests {
    using System;
    using System.IO;
    using System.Web.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class UnitTestHelpers {
        public static void AssertThrows<TExpectedException>(Action method) where TExpectedException : Exception {
            try {
                method();
            }
            catch (TExpectedException) {

            }
        }

        public static void AssertThrows<TExpectedException>(Action method, string message) where TExpectedException : Exception {
            try {
                method();
            }
            catch (TExpectedException e) {
                Assert.AreEqual(message, e.Message);
            }
        }

        public static MemoryStream ToMemoryStream(this string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static StreamReader ToStreamReader(this string s) {
            Stream stream = s.ToMemoryStream();
            return new StreamReader(stream);
        }
    }

    //found a bug with Moq and MarshallByRef. This is my work around.
    internal class TestVirtualPathProvider : VirtualPathProvider {
        string path;
        VirtualFile file;

        public TestVirtualPathProvider(string path, VirtualFile file) {
            this.path = path;
            this.file = file;
        }

        public override VirtualFile GetFile(string virtualPath) {
            if (virtualPath == this.path)
                return this.file;
            throw new InvalidOperationException("wrong path passed in");
        }
    }

    internal class TestVirtualFile : VirtualFile {
        Stream stream;

        public TestVirtualFile(Stream stream, string virtualPath)
            : base(virtualPath) {
            this.stream = stream;
        }

        public override System.IO.Stream Open() {
            return this.stream;
        }
    }
}