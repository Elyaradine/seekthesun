Shader "Unlit/Stem"
{
    Properties
    {
        _Color ("Colour", Color) = (1,1,1,1)
        _ShadowCol ("Shadow Colour", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
        }
        LOD 100

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

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 col: COLOR0;
                SHADOW_COORDS(1)
            };

            fixed4 _Color, _ShadowCol;

            v2f vert (appdata_full v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.col = half4(GammaToLinearSpace(v.color.rgb), 1) * _Color;

                TRANSFER_SHADOW(o)

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
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
            #include "UnityCG.cginc"

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
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
    FallBack "VertexLit"
}
