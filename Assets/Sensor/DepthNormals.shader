Shader "Custom/DepthNormals" 
{
	Properties
	{
		_MainTex("", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _CameraDepthNormalsTexture;

			struct v2f 
			{
				float4 pos : SV_POSITION;
				float4 scrPos: TEXCOORD1;
			};

			v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.pos);
				o.scrPos.y = 1 - o.scrPos.y;
				return o;
			}

			sampler2D _MainTex;

			half4 frag(v2f i) : COLOR
			{
				float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.scrPos.xy);

				float3 normal;
				float depth;
				DecodeDepthNormal(depthnormal, depth, normal);

				if (depth > 1)
				{
					// No normals BG.
					depthnormal.x = 0;
					depthnormal.y = 0;
				}
				// Depth is originally stored in z and w, ignore w for now.
				// https://github.com/Unity-Technologies/ml-agents/issues/5445
				depthnormal.z = 1 - depth; // invert, closer is brighter.
				depthnormal.w = 1;

				return depthnormal;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
