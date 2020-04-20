Shader "Skybox/GradientGround"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 0)
        _Color2 ("Color 2", Color) = (1, 1, 1, 0)
        _UpVector ("Up Vector", Vector) = (0, 1, 0, 0)
        _Intensity ("Intensity", Float) = 1.0
        _Exponent ("Exponent", Float) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Background" 
            "Queue"="Background" 
        }

        Pass
        {
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
    
            struct v2f
            {
                float4 position : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };
            
            half4 _Color1;
            half4 _Color2;
            half4 _UpVector;
            half _Intensity;
            half _Exponent;
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.position = UnityObjectToClipPos (v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }
            
            fixed4 frag (v2f i) : COLOR
            {
                half d = saturate(dot(normalize(i.texcoord), _UpVector));
                return lerp (_Color1, _Color2, pow (d, _Exponent)) * _Intensity;
            }

            ENDCG
        }
    }
}