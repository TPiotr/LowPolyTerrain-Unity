// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "TerrainShader"
{
	Properties
	{
		_NoiseTexture("Noise texture", 2D) = "white" {}
		_NoisePower("Noise power", Float) = 0.1
		_ShadowIntensity("Shadow intensity", Float) = 0.1
	}
		SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target es3.0 

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			#include "Lighting.cginc"
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#include "AutoLight.cginc"

			sampler2D _NoiseTexture;
			fixed _NoisePower;
			fixed _ShadowIntensity;

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 vertex_color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD2;
				SHADOW_COORDS(1)

				nointerpolation fixed2 uv : TEXCOORD0;
				nointerpolation fixed4 color : COLOR;
			};

			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				half3 world_normal = UnityObjectToWorldNormal(v.normal);
				
				half light_factor = max(0, dot(world_normal, _WorldSpaceLightPos0.xyz));
				half4 light_color = light_factor * _LightColor0;
				light_color.rgb += ShadeSH9(half4(world_normal, 1));

				o.color = v.vertex_color * light_color;
				TRANSFER_SHADOW(o)
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				/*half3 x = ddx(i.worldPos);
				half3 y = ddy(i.worldPos);

				half3 normal = -normalize(cross(x,y));
				
				half light_factor = max(0, dot(normal, _WorldSpaceLightPos0.xyz));
				half4 light_color = light_factor * _LightColor0;
				light_color.rgb += ShadeSH9(half4(normal, 1)); LOW POLY SHADING */ 
				
				fixed shadow = SHADOW_ATTENUATION(i);
				
				fixed4 noise = fixed4(tex2D(_NoiseTexture, i.uv).rgb, 1.0) * _NoisePower;
				fixed4 color = (i.color + noise) * shadow; // LOW POLY SHADING ((i.color * light_color) + noise) * shadow;
			
				return color;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}