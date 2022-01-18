Shader "Kalimag/Mod/Overlay" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags {
			"Queue" = "Overlay"
			"RenderType"="Opaque"
			"ForceNoShadowCasting" = "True"
		}
		Offset -1, 1
		ZTest Always

		Cull Front
		ZWrite Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard noshadow

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color.rgb;
			o.Metallic = 1;
			o.Smoothness = 0.5;
			o.Normal = o.Normal - 1;
		}
		ENDCG

		Cull Back
		ZWrite On
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard noshadow

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color.rgb;
			o.Metallic = 1;
			o.Smoothness = 0.5;
		}
		ENDCG
	}
}
