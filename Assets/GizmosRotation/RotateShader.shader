Shader "RotateShader"
{
	SubShader
	{
		Tags { "Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off
		//ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off

		Pass 
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color;
			float _Scale;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
				float3 normal : TEXTURE0;
			};

			v2f vert (appdata v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				/*float3 pos = v.vertex.xyz + float3(0.01,0.01,0.0);
				pos.y = 0;
				o.normal = mul((float3x3)UNITY_MATRIX_MV, normalize(pos));*/

				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				/*float angle = dot(i.normal,float3(0,0,1));
				if (angle < 0)
				{
					i.color = i.color.r * 0.299 + i.color.g * 0.587 + i.color.b * 0.114;
				}*/
				return i.color;
			}

			ENDCG
		}
	}
}