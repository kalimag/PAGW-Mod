Shader "Kalimag/Mod/Transparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,0.5)
	}
	SubShader {
		Tags {
			"Queue" = "Transparent"
			"RenderType"="Transparent"
			"ForceNoShadowCasting" = "True"
		}
		Offset -1, 1
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard alpha:auto noshadow

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			o.Albedo = _Color.rgb;
			o.Metallic = 0.1;
			o.Smoothness = 0.2;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
}
