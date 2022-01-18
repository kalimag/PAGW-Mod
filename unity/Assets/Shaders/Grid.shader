Shader "Kalimag/Mod/Grid" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[IntRange] _GridSize("Grid Size", Range(1,100)) = 10
		_LineSize("Line Size", Range(0,1)) = 0.15
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"RenderType"="TransparentCutout"
			"ForceNoShadowCasting" = "True"
		}
		Offset -1, 1
		Cull Off
	

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard noshadow

		struct Input {
			float2 uv_MainTex;
			fixed facing : VFACE;
		};

		float4 _Color;

		float _GridSize;
		float _LineSize;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 uv = IN.uv_MainTex;

			float gsize = floor(_GridSize);

			gsize += _LineSize;

			if (frac(uv.x * gsize) <= _LineSize || frac(uv.y * gsize) <= _LineSize)
			{	
				o.Albedo = _Color.rgb;
				o.Metallic = 0.1;
				o.Smoothness = 0.2;
				o.Alpha = 1.0;
			}
			else
			{
				clip(-1.0);
			}
		}
		ENDCG
	}
}
