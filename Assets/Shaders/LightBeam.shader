Shader "Unlit/LightBeam"
{
    Properties
    {
        [HDR]_Color ("Colour", Color) = (1,1,1,1)
        _MainTex ("Main texture", 2D) = "white" {}
        _RimPow ("Rim Power", Float) = 1
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        ZWrite Off
        //Blend SrcAlpha OneMinusSrcAlpha
        Blend One One
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                UNITY_FOG_COORDS(0)
                float3 worldPos: TEXCOORD1;
                float3 worldNorm: TEXCOORD2;
                float3 worldView: TEXCOORD3;
                float2 uv1: TEXCOORD4;
                float2 uv2: TEXCOORD5;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half _RimPow;
            half4 _Color;

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNorm = UnityObjectToWorldNormal(v.normal);
                o.worldView = normalize(_WorldSpaceCameraPos - o.worldPos);

                o.uv1 = v.texcoord.xy * float2(_MainTex_ST.x, 1) + float2(frac(_Time.y * _MainTex_ST.z), 0);
                o.uv2 = v.texcoord.xy * float2(_MainTex_ST.y, 1) + float2(frac(_Time.y * _MainTex_ST.w), 0);

                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half fres = saturate(dot(normalize(i.worldNorm), normalize(i.worldView)));
                fres = pow(fres, _RimPow);

                half worldMask = saturate(i.worldPos.y);
                // worldMask *= worldMask;

                half beam1 = tex2D(_MainTex, i.uv1).g;
                half beam2 = tex2D(_MainTex, i.uv2).g;
                half beam = lerp(0.3, 1, beam1 * beam2);

                half4 col = _Color * fres * worldMask * beam;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
