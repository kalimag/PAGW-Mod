using System;
using UnityEngine;
using IO = System.IO;

namespace Kalimag.Modding.Unity.Visualization
{
	internal static class ModAssets
    {
        private static readonly Lazy<AssetBundle> Assets = new Lazy<AssetBundle>(() =>
            AssetBundle.LoadFromFile(
                IO.Path.Combine(IO.Path.GetDirectoryName(typeof(ModAssets).Assembly.Location), "modassets")
            )
        );

        private static readonly Lazy<Material> _transparentMaterial = new Lazy<Material>(() => Assets.Value.LoadAsset<Material>("Assets/Materials/Transparent.mat"));
        private static readonly Lazy<Material> _opaqueMaterial = new Lazy<Material>(() => Assets.Value.LoadAsset<Material>("Assets/Materials/Opaque.mat"));
        private static readonly Lazy<Material> _gridMaterial = new Lazy<Material>(() => Assets.Value.LoadAsset<Material>("Assets/Materials/Grid.mat"));
        private static readonly Lazy<Material> _overlayMaterial = new Lazy<Material>(() => Assets.Value.LoadAsset<Material>("Assets/Materials/Overlay.mat"));

        public static Material TransparentMaterial => _transparentMaterial.Value;
        public static Material OpaqueMaterial => _opaqueMaterial.Value;
        public static Material GridMaterial => _gridMaterial.Value;
        public static Material OverlayMaterial => _overlayMaterial.Value;
    }

    internal class Lazy<T>
	{
        private T _value;
        private bool _initialized;
        private readonly Func<T> _factory;

        public Lazy(Func<T> factory)
		{
            _factory = factory;
		}

		public T Value
		{
			get
			{
                if (!_initialized)
				{
                    _value = _factory();
                    _initialized = true;
				}
                return _value;
			}
		}
	}
}
