using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module3
{
    public class FileSystemVisitor
    {
        private readonly string _startingPath;
        private readonly Func<string, bool> _filter;

        public delegate void FileSystemVisitorDelegate(string item, VisitorEventArgs args);
        public delegate void SimpleDelegate();

        public event SimpleDelegate Start;
        public event SimpleDelegate Finish;
        public event FileSystemVisitorDelegate FileFound;
        public event FileSystemVisitorDelegate DirectoryFound;
        public event FileSystemVisitorDelegate FilteredFileFound;
        public event FileSystemVisitorDelegate FilteredDirectoryFound;

        public FileSystemVisitor(string startingPath) : this(startingPath, null)
        {
        }

        public FileSystemVisitor(string startingPath, Func<string, bool> filter)
        {
            _startingPath = startingPath;
            _filter = filter;
        }

        public IEnumerable<string> GetFiles()
        {
            Start?.Invoke(); 

            if (!Directory.Exists(_startingPath))
            {
                Console.WriteLine("Directory does not exist.");
                Finish?.Invoke();
                yield break;
            }

            VisitorEventArgs args = new VisitorEventArgs();
            foreach (var item in Traverse(_startingPath, args))
            {
                if (args.Stop)
                    break;

                yield return item;
            }

            Finish?.Invoke(); 

        }

        private IEnumerable<string> Traverse(string path, VisitorEventArgs args)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                DirectoryFound?.Invoke(directory, args); 

                if (args.Stop) yield break;
                if (args.Exclude) continue;

                if (_filter == null || _filter(directory))
                {
                    FilteredDirectoryFound?.Invoke(directory, args); 

                    if (args.Stop) yield break;
                    if (!args.Exclude)
                    {
                        yield return directory; 
                    }
                }

                foreach (var file in Traverse(directory, args)) 
                {
                    if (args.Stop) yield break;
                    yield return file;
                }
            }

            foreach (var file in Directory.GetFiles(path))
            {
                FileFound?.Invoke(file, args); 

                if (args.Stop) yield break;
                if (args.Exclude) continue;

                if (_filter == null || _filter(file))
                {
                    FilteredFileFound?.Invoke(file, args);

                    if (args.Stop) yield break;
                    if (!args.Exclude)
                    {
                        yield return file; 
                    }
                }
            }
        }        
    }

    public class VisitorEventArgs : EventArgs
    {
        public bool Exclude { get; set; } = false;
        public bool Stop { get; set; } = false;
    }
}
