using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace UpdateUnityAssets
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Copy builds to Unity. Press 'y' for yes. Other key to exit");

            var key = Char.ToLower(Console.ReadKey().KeyChar);

            string[] assets = new string[]{
                "VRPN"
            };


            if (key == 'y')
            {

                var solutionFolder = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent;
                var unityAssetFolder = Path.Combine(solutionFolder.Parent.FullName, "IK - usage", "Assets", "TrackingLib", "Resources");


                foreach (var assetname in assets)
                {


                    var assetBuildPath = Path.Combine(solutionFolder.FullName, "TrackingRelay_" + assetname, "bin", "x86", "TrackingRelay_" + assetname);
#if DEBUG
                    assetBuildPath += "_debug";
#endif

                    var unityZippedAssetPath = Path.Combine(unityAssetFolder, "TrackingRelay_" + assetname + ".bytes");

                    if (File.Exists(unityZippedAssetPath))
                        File.Delete(unityZippedAssetPath);

                    ZipFile.CreateFromDirectory(assetBuildPath, unityZippedAssetPath);
                }
            }
        }
    }
}
