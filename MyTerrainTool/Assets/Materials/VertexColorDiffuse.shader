Shader "Mobile/VertexColorDiffuse" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags{ "RenderType" = "Opaque" }
		LOD 150

		CGPROGRAM

		
		#pragma surface surf Lambert noforwardadd
		#pragma target es3.0 

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			nointerpolation float4 vertColor : COLOR;

		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = IN.vertColor;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Mobile/VertexLit" 
	}
