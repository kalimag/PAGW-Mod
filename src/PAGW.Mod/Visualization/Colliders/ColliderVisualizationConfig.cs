using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kalimag.Modding.Unity.Visualization.Colliders
{
	internal class ColliderVisualizationConfig : IEquatable<ColliderVisualizationConfig>
    {

        private static readonly Color DefaultColor = Color.magenta;
        private const float Alpha = 0.7f;



        public Type BehaviourSelector { get; private set; }
        public IEnumerable<Type> BehaviourSelectors { get; private set; }
        public string TagSelector { get; private set; }
        public int? LayerSelector { get; private set; }
        public ColliderTypes ColliderTypes { get; private set; }

        public string SelectorLabel { get; }

        public bool Enabled { get; set; }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (value != _color)
                {
                    _color = value;
                    ColorLabel = $"#{(int)Math.Round(value.r * 255):X2}{(int)Math.Round(value.g):X2}{(int)Math.Round(value.b):X2}";
                    UpdateMaterial();
                }
            }
        }

        private ShapeVisualizationShader _shader;
        public ShapeVisualizationShader Shader
        {
            get => _shader;
            set
            {
                if (value != _shader)
                {
                    _shader = value;
                    UpdateMaterial();
                }
            }
        }

        public Material Material { get; } = new Material(ModAssets.OpaqueMaterial);

        public string ColorLabel { get; private set; }

        //public event Action MaterialUpdated;




        private ColliderVisualizationConfig(string label, ColliderTypes colliderTypes, bool enabled, Color? color, ShapeVisualizationShader shader)
        {
            SelectorLabel = label;
            if (colliderTypes != ColliderTypes.Both)
                SelectorLabel += " " + colliderTypes.ToString();
            ColliderTypes = colliderTypes;
            Enabled = enabled;
            Color = color ?? DefaultColor;
            Shader = shader;
        }

        public static ColliderVisualizationConfig ForBehaviour(Type behaviour,
            ColliderTypes colliderTypes, bool enabled = false, Color? color = null, ShapeVisualizationShader shader = ShapeVisualizationShader.Transparent)
        {
            return new ColliderVisualizationConfig(behaviour.FullName, colliderTypes, enabled, color, shader) 
            {
                BehaviourSelector = behaviour
            };
        }

        public static ColliderVisualizationConfig ForBehaviours(IEnumerable<Type> behaviours, string label,
            ColliderTypes colliderTypes, bool enabled = false, Color? color = null, ShapeVisualizationShader shader = ShapeVisualizationShader.Transparent)
        {
            return new ColliderVisualizationConfig(label, colliderTypes, enabled, color, shader)
            {
                BehaviourSelectors = behaviours.ToList()
            };
        }

        public static ColliderVisualizationConfig ForTag(string tag,
            ColliderTypes colliderTypes, bool enabled = false, Color? color = null, ShapeVisualizationShader shader = ShapeVisualizationShader.Transparent)
        {
            return new ColliderVisualizationConfig("#" + tag, colliderTypes, enabled, color, shader) {
                TagSelector = tag
            };
        }

        public static ColliderVisualizationConfig ForLayer(int layer,
            ColliderTypes colliderTypes, bool enabled = false, Color? color = null, ShapeVisualizationShader shader = ShapeVisualizationShader.Transparent)
        {
            return new ColliderVisualizationConfig("L" + layer + " " + LayerMask.LayerToName(layer), colliderTypes, enabled, color, shader) {
                LayerSelector = layer
            };
        }

        public static ColliderVisualizationConfig ForOther(string label,
            ColliderTypes colliderTypes, bool enabled = false, Color? color = null, ShapeVisualizationShader shader = ShapeVisualizationShader.Transparent)
        {
            return new ColliderVisualizationConfig(label, colliderTypes, enabled, color, shader);
        }


        private void UpdateMaterial()
        {
            var color = Color;

            Material sourceMaterial;

            if (Shader == ShapeVisualizationShader.Opaque)
            {
                sourceMaterial = ModAssets.OpaqueMaterial;
            }
            else if (Shader == ShapeVisualizationShader.Transparent)
            {
                sourceMaterial = ModAssets.TransparentMaterial;
                color.a = Alpha;
            }
            else if (Shader == ShapeVisualizationShader.Grid)
            {
                sourceMaterial = ModAssets.GridMaterial;
            }
            else if (Shader == ShapeVisualizationShader.Overlay)
            {
                sourceMaterial = ModAssets.OverlayMaterial;
            }
            else
            {
                sourceMaterial = ModAssets.TransparentMaterial;
            }

            Material.shader = sourceMaterial.shader;
            Material.renderQueue = sourceMaterial.renderQueue;

            Material.color = color;

            //MaterialUpdated?.Invoke();
        }



        public  bool Equals(ColliderVisualizationConfig other)
        {
            return ColliderTypes == other.ColliderTypes &&
                   TagSelector == other.TagSelector &&
                   BehaviourSelector == other.BehaviourSelector &&
                   BehaviourSelectors == other.BehaviourSelectors &&
                   LayerSelector == other.LayerSelector;
        }

        public override bool Equals(object obj) => Equals(obj as ColliderVisualizationConfig);

        public override int GetHashCode()
        {
            return TagSelector?.GetHashCode() ?? 0 ^
                   BehaviourSelector?.GetHashCode() ?? 0 ^
                   BehaviourSelectors?.GetHashCode() ?? 0 ^
                   LayerSelector?.GetHashCode() ?? 0 ^
                   ColliderTypes.GetHashCode();
        }


    }
}
