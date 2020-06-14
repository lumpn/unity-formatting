//----------------------------------------
// MIT License
// Copyright(c) 2020 Jonas Boetel
//----------------------------------------
using System.IO;
using NUnit.Framework;
using UnityEditor;

namespace Lumpn.Formatting.Tests
{
    [TestFixture]
    public sealed class FormattingTest
    {
        [Test]
        public void TestLineEndings()
        {
            RunTest("Test line endings", TestLineEndings);
        }

        [Test]
        public void TestTabsVersusSpaces()
        {
            RunTest("Test spaces vs. tabs", TestTabsVersusSpaces);
        }

        [Test]
        public void TestIndentation()
        {
            RunTest("Test indentation", TestIndentation);
        }

        [Test]
        public void TestTrailingWhitespace()
        {
            RunTest("Test trailing whitespaces", TestTrailingWhitespaces);
        }

        [Test]
        public void TestFinalNewLine()
        {
            RunTest("Test final new line", TestFinalNewLine);
        }

        [Test]
        public void TestPlainASCII()
        {
            RunTest("Test plain ASCII", TestPlainASCII);
        }

        private static void TestLineEndings(string path)
        {
            using (var file = File.OpenRead(path))
            {
                int lineNumber = 1;
                int value;
                while ((value = file.ReadByte()) >= 0)
                {
                    if (value == 10) { lineNumber++; }
                    Assert.AreNotEqual(13, value, "File '{0}' has Windows style line ending in line {1}", path, lineNumber);
                }
            }
        }

        private static void TestTabsVersusSpaces(string path)
        {
            using (var file = File.OpenRead(path))
            {
                int lineNumber = 1;
                int value;
                while ((value = file.ReadByte()) >= 0)
                {
                    if (value == 10) { lineNumber++; }
                    Assert.AreNotEqual(9, value, "File '{0}' has a tab character in line {1}", path, lineNumber);
                }
            }
        }

        private static void TestIndentation(string path)
        {
            const int tabSize = FormattingUtils.tabSize;
            using (var file = File.OpenRead(path))
            {
                int lineNumber = 1;
                int consecutiveSpaces = 0;
                bool counting = true;
                int value;
                while ((value = file.ReadByte()) >= 0)
                {
                    if (counting)
                    {
                        if (value == 32)
                        {
                            consecutiveSpaces++;
                        }
                        else
                        {
                            Assert.AreEqual((consecutiveSpaces / tabSize) * tabSize, consecutiveSpaces, "File '{0}' is not using {1} spaces for indentation in line {2}", path, tabSize, lineNumber);
                            counting = false;
                        }
                    }
                    if (value == 10)
                    {
                        lineNumber++;
                        consecutiveSpaces = 0;
                        counting = true;
                    }
                }
            }
        }

        private static void TestTrailingWhitespaces(string path)
        {
            using (var file = File.OpenRead(path))
            {
                int lineNumber = 1;
                int consecutiveSpaces = 0;
                int value;
                while ((value = file.ReadByte()) >= 0)
                {
                    if (value == 10)
                    {
                        Assert.AreEqual(0, consecutiveSpaces, "File '{0}' has a trailing whitespaces in line {1}", path, lineNumber);
                        lineNumber++;
                    }
                    if (value == 32)
                    {
                        consecutiveSpaces++;
                    }
                    else
                    {
                        consecutiveSpaces = 0;
                    }
                }
            }
        }

        private static void TestFinalNewLine(string path)
        {
            using (var file = File.OpenRead(path))
            {
                int consecutiveLinefeeds = 0;
                int value;
                while ((value = file.ReadByte()) >= 0)
                {
                    if (value == 10)
                    {
                        consecutiveLinefeeds++;
                    }
                    else
                    {
                        consecutiveLinefeeds = 0;
                    }
                }
                Assert.AreEqual(1, consecutiveLinefeeds, "File '{0}' must have a final new line character at the end", path);
            }
        }

        private static void TestPlainASCII(string path)
        {
            using (var file = File.OpenRead(path))
            {
                int lineNumber = 1;
                int value;
                while ((value = file.ReadByte()) >= 0)
                {
                    if (value == 10) { lineNumber++; }
                    Assert.Less(value, 128, "File '{0}' has a non-ASCII character in line {1}", path, lineNumber);
                }
            }
        }

        private static void RunTest(string title, System.Action<string> test)
        {
            var guids = AssetDatabase.FindAssets("t:script t:shader");
            FormattingUtils.RunAction(guids, title, test);
        }
    }
}
