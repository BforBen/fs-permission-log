using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using GuildfordBoroughCouncil.FsSecCheck.Data;

namespace GuildfordBoroughCouncil.FsSecCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(@"Please specify a path e.g. fsseccheck c:\");
                Console.ReadKey();
            }

            Console.WriteLine("File System Permission Checker");
            Console.WriteLine();

            var worker = new Worker(args[0]);
            worker.Start();
        }
    }

    public class Worker
    {
        internal FsSecCheckData _db = new FsSecCheckData();
        internal string _Root;
        internal Guid _Session;
        
        public Worker(string Root)
        {
            var di =  new DirectoryInfo(Root);
            if (!di.Exists)
            {
                throw new ArgumentOutOfRangeException("Path doesn't exist.");
            }

            _Root = di.FullName;
            _Session = Guid.NewGuid();
        }

        public void Start()
        {
            Console.WriteLine("The ID for this session is {0}.", _Session);
            Console.WriteLine();

            Dir(_Root);
        }

        internal void Dir(string Path)
        {
            CheckPermissions(Path);

            Console.WriteLine("Looking for subfolders in " + Path);

            var SubFolders = Directory.EnumerateDirectories(Path, "*", SearchOption.TopDirectoryOnly);

            foreach (var Folder in SubFolders)
            {
                // Check for access
                try
                {
                    Dir(Folder);
                }
                catch (UnauthorizedAccessException)
                {
                    _db.NoAuthorisations.Add(new NoAuthorisation
                    {
                        Machine = Environment.MachineName,
                        Path = Folder,
                        Session = _Session
                    });
                    _db.SaveChanges();
                }
            }
        }

        internal void CheckPermissions(string Path)
        {
            Console.WriteLine("Checking permissions on " + Path);

            var DirectoryAcl = Directory.GetAccessControl(Path);

            foreach (FileSystemAccessRule rule in DirectoryAcl.GetAccessRules(true, false, typeof(NTAccount)))
            {
                _db.Permissions.Add(new Permission
                {
                    Machine = System.Environment.MachineName,
                    Path = Path,
                    Object = rule.IdentityReference.Value,
                    Type = rule.AccessControlType.ToString(),
                    Rights = rule.FileSystemRights.ToString(),
                    Inherited = rule.IsInherited,
                    Inheritance = rule.InheritanceFlags.ToString(),
                    Propagation = rule.PropagationFlags.ToString(),
                    Session = _Session
                });
            }

            _db.SaveChanges();
        }
    }
}
