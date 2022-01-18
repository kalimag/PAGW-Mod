Shader "Kalimag/Mod/Opaque" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Offset -1, 1
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard noshadow

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Albedo = _Color.rgb;
			o.Metallic = 0.1;
			o.Smoothness = 0.2;
		}
		ENDCG
	}
}
