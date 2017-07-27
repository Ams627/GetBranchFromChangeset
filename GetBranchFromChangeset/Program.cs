using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace GetBranchFromChangeset
{
    internal partial class Program
    {

        private static void Main(string[] args)
        {
            try
            {
                var name = "http://vstfs2013:8080/tfs/vstfsimport";
                var uri = new Uri(name);
                var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri);
                string path = "$/UK Rail/TVM/Development Heavy Rail/TVM_UK";

                //varChangesetVersionSpec(changeset)
                var changesetDetails = tfs.GetService<VersionControlServer>().GetChangeset(116361);

                ItemIdentifier rootitem = new ItemIdentifier("$/UK Rail/TVM/Development Heavy Rail/TVM_UK/Source/TVM");
                BranchObject[] allBranches = tfs.GetService<VersionControlServer>().QueryBranchObjects(rootitem, RecursionType.Full);
                foreach (var branch in allBranches)
                {
                    bool isDeleted = branch.Properties.RootItem.IsDeleted;
                    Console.Write(isDeleted ? "deleted " : "active ");
                    Console.WriteLine(branch.Properties.RootItem.Item);
                }

                var branches = allBranches.Where(x => !x.Properties.RootItem.IsDeleted).Select(y => y.Properties.RootItem.Item).ToHashSet();

                foreach (var change in changesetDetails.Changes)
                {
                    var item = change.Item.ServerItem;
                    foreach (var branch in branches)
                    {
                        if (item.Contains(branch))
                        {
                            Console.WriteLine(branch);
                        }
                    }
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
