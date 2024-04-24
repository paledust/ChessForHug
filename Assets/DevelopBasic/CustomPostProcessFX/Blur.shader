Shader "Hidden/Custom/Blur"
{
    HLSLINCLUDE
// StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _BlurSize;
    int _Sample;
    float4 Frag_Vertical(VaryingsDefault i) : SV_Target
    {
        float4 col = 0;
        for(float index = 0; index < _Sample; index++){
            float2 uv = i.texcoord + float2(0, (index/(_Sample-1) - 0.5) * _BlurSize);
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        }
        col /= _Sample;

        return col;
    }
    float4 Frag_Horizontal(VaryingsDefault i) : SV_Target
    {
        float invAspect = _ScreenParams.y/_ScreenParams.x;
        float4 col = 0;
        for(float index = 0; index < _Sample; index++){
            float2 uv = i.texcoord + float2((index/(_Sample-1) - 0.5f) * _BlurSize * invAspect, 0);
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        }
        col = col/_Sample;
        return col;
    }
    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag_Vertical
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag_Horizontal
            ENDHLSL
        }
    }
}