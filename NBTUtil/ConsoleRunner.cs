using NBTExplorer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NBTUtil
{
    internal class ConsoleRunner
    {
        public ConsoleRunner()
        {
        }

        private static bool Debug = false;

        public bool Run(string[] args)
        {
            if (!args.Any())
            {
                PrintError("You must supply a path");
                return false;
            }
            var path = args[0];
            if (args.Length == 2)
            {
                Debug = bool.Parse(args[1]);
            }
            var nodes = new NbtPathEnumerator(path);
            foreach (var node in nodes)
            {
                Process(node);
            }

            return true;
        }

        private static void Process(DataNode targetNode)
        {
            if (NodeContainsRemovedTags(targetNode.NodeName))
            {
                targetNode.DeleteNode();
            }
            targetNode.Expand();
            var nodesToDelete = new List<DataNode>();
            foreach (var child in targetNode.Nodes)
            {
                if (child.NodeName == null && !child.Nodes.Any())
                {
                    continue;
                };

                if (child.NodeName != null && NodeContainsRemovedTags(child.NodeName))
                {
                    OutputDebug(child);
                    nodesToDelete.Add(child);
                }
                else
                {
                    Process(child);
                }
            }
            foreach (var node in nodesToDelete)
            {
                node.DeleteNode();
            }
            targetNode.Root.Save();
            targetNode.Release();
        }

        private static bool NodeContainsRemovedTags(string nodeName)
        {
            nodeName = nodeName.ToLower();
            return nodeName.Contains("arstheurgia") || nodeName.Contains("astromine");
        }

        private static void OutputDebug(DataNode node)
        {
            if (Debug)
            {
                var pathParts = node.NodePath.Split('/');
                var path = pathParts.Length > 6 ? string.Join("/", pathParts.Skip(pathParts.Length - 6)) : node.NodePath;
                Console.WriteLine($"Removing node {node.NodeName} from {path}");
            }
        }

        private bool PrintError(string error)
        {
            Console.WriteLine(error);

            return false;
        }
    }
}