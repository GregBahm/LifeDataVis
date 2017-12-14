Shader "Unlit/BoxShader"
{
	Properties
	{
        _Max("Max", Float) = 1
        _Min("Min", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			 
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0; 
                float3 worldPos : TEXCOORD1;
                float3 objSpace : TEXCOORD2;
				float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
			};
			
            float _Max;
            float _Min;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.objSpace = v.vertex;
                o.normal = v.normal;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                float3 awake = float3(1, 0, 0);
                float3 asleep = float3(0, .5, 1);
                float param = (i.worldPos.y - _Min) / (_Max - _Min);
                float3 ret = lerp(asleep, awake, saturate(param));
                float mid = (_Min + _Max) / 2;
                float distToMid = abs(i.worldPos.y - mid); 
                distToMid = distToMid / (_Max - _Min);
                ret = lerp(1, ret, saturate(distToMid));

                float hiParam = (i.objSpace.y + .5);
                //ret = lerp(0.5, ret, hiParam);
                
                
                ret = lerp(0, ret, i.normal.y);

                return float4(ret, 1);
			}
			ENDCG
		}
	}
}
