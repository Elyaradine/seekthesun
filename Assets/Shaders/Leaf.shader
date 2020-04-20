Shader "Unlit/Leaf"
{
    Properties
    {
        [HDR]_Color ("Colour", Color) = (1,1,1,1)
        [HDR]_BackCol ("Backface Colour", Color) = (1,1,1,1)
        _ShadowCol ("Shadow Colour", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="OpaqueLeaf"
            // "RenderType"="Opaque"
        }
        LOD 100
        // ZWrite On

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase nodirlightmap nodynlightmap novertexlights

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "VertexWind.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 col: COLOR0;
                SHADOW_COORDS(1)
            };

            fixed4 _Color;
            fixed4 _ShadowCol;
            fixed4 _BackCol;

            v2f vert (appdata_full v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);

                v.vertex.xyz = WindOffset(v.vertex.xyz);
                
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 ldir = _WorldSpaceLightPos0.xyz;
                float nl = dot(worldNormal, ldir);
                nl = step(0.005, nl);

                o.col = half4(GammaToLinearSpace(v.color.rgb), 1) * lerp(_BackCol, _Color, saturate(nl));

                TRANSFER_SHADOW(o)

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                fixed shadow = SHADOW_ATTENUATION(i);
                return lerp(_ShadowCol, i.col, shadow);
            }
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode"="ShadowCaster"
            }
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "VertexWind.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                v.vertex.xyz = WindOffset(v.vertex.xyz);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i): SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }

            ENDCG
        }
    }
    // FallBack "VertexLit"
}