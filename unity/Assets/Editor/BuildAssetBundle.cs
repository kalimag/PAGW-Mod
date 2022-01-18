using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
	[MenuItem("Assets/Build AssetBundle")]
	static void BuildAllAssetBundles()
	{
		string assetBundleDirectory = "AssetBundles";

		if (!Directory.Exists(assetBundleDirectory))        
			Directory.CreateDirectory(assetBundleDirectory);

		BuildPipeline.BuildAssetBundles(assetBundleDirectory,
			BuildAssetBundleOptions.StrictMode,
			BuildTarget.StandaloneWindows);
	}
} 