// Upgrade NOTE: upgraded instancing buffer 'AmplifyShadersUIEmissiveDissolve' to new syntax.

// Made with Amplify Shader Editor v1.9.2.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/UI EmissiveDissolve"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        [Toggle]_FlipX("FlipX", Float) = 0
        [Toggle]_UseRectUV("UseRectUV", Float) = 0
        [HDR]_FillColor("FillColor", Color) = (0,0,0,1)
        _NoiseTex("NoiseTex", 2D) = "white" {}
        _NoiseTiling1("NoiseTiling", Float) = 1
        _DissolvePanSpeed("DissolvePanSpeed", Vector) = (0,0,0,0)
        _NoiseSpeed1("NoiseSpeed", Vector) = (1,1,0,0)
        _NoiseMin1("NoiseMin", Float) = -1.25
        _NoiseMax1("NoiseMax", Float) = 1
        _DissolveSoftness("DissolveSoftness", Float) = 1
        _PatternScale("PatternScale", Float) = 1
        [KeywordEnum(Radius,TexBased)] _DissolveType("DissolveType", Float) = 0
        _RadiusCenterX("RadiusCenterX", Float) = 0.5
        _RadiusCenterY("RadiusCenterY", Float) = 0.5
        _DissolveRadius("DissolveRadius", Float) = 0.5
        _DissolveLength("DissolveLength", Float) = 0.1
        _DissolveGuideTex("DissolveGuideTex", 2D) = "white" {}
        _EdgeValue("EdgeValue", Float) = 0
        _EdgeSoftness("EdgeSoftness", Float) = 1
        [HDR]_EdgeColor("EdgeColor", Color) = (2.828427,2.029812,1.209014,1)
        [KeywordEnum(AlphaBlend,Additive)] _EdgeBlend("EdgeBlend", Float) = 0
        [PerRendererData]_SpriteRect("SpriteRect", Vector) = (1,1,1,1)
        [HideInInspector] _texcoord( "", 2D ) = "white" {}

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
        	Ref [_Stencil]
        	ReadMask [_StencilReadMask]
        	WriteMask [_StencilWriteMask]
        	Comp [_StencilComp]
        	Pass [_StencilOp]
        }


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityShaderVariables.cginc"
            #define ASE_NEEDS_FRAG_COLOR
            #pragma shader_feature_local _EDGEBLEND_ALPHABLEND _EDGEBLEND_ADDITIVE
            #pragma multi_compile_instancing
            #pragma shader_feature_local _DISSOLVETYPE_RADIUS _DISSOLVETYPE_TEXBASED


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                float4 ase_texcoord3 : TEXCOORD3;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform float _NoiseMin1;
            uniform float _NoiseMax1;
            uniform sampler2D _NoiseTex;
            uniform float2 _NoiseSpeed1;
            uniform float _NoiseTiling1;
            uniform float4 _FillColor;
            uniform float4 _EdgeColor;
            uniform float _DissolveSoftness;
            uniform float _PatternScale;
            uniform float2 _DissolvePanSpeed;
            float4 _MainTex_TexelSize;
            uniform float _FlipX;
            uniform float _UseRectUV;
            uniform float _RadiusCenterX;
            uniform float _RadiusCenterY;
            uniform float _DissolveRadius;
            uniform float _DissolveLength;
            uniform sampler2D _DissolveGuideTex;
            uniform float _EdgeValue;
            uniform float _EdgeSoftness;
            UNITY_INSTANCING_BUFFER_START(AmplifyShadersUIEmissiveDissolve)
            	UNITY_DEFINE_INSTANCED_PROP(float4, _SpriteRect)
#define _SpriteRect_arr AmplifyShadersUIEmissiveDissolve
            UNITY_INSTANCING_BUFFER_END(AmplifyShadersUIEmissiveDissolve)

            
            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
                OUT.ase_texcoord3.xyz = ase_worldPos;
                
                
                //setting value to unused interpolator channels and avoid initialization warnings
                OUT.ase_texcoord3.w = 0;

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float3 ase_worldPos = IN.ase_texcoord3.xyz;
                float2 appendResult112 = (float2(ase_worldPos.x , ase_worldPos.y));
                float2 panner109 = ( 1.0 * _Time.y * _NoiseSpeed1 + ( appendResult112 * _NoiseTiling1 ));
                float smoothstepResult110 = smoothstep( _NoiseMin1 , _NoiseMax1 , tex2D( _NoiseTex, panner109 ).r);
                float noisePattern117 = smoothstepResult110;
                float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST);
                float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
                float4 tex2DNode5 = tex2D( _MainTex, uv_MainTex );
                float4 temp_output_78_0 = ( _FillColor * tex2DNode5 );
                float2 appendResult90 = (float2(ase_worldPos.x , ase_worldPos.y));
                float2 panner126 = ( 1.0 * _Time.y * _DissolvePanSpeed + appendResult90);
                float2 texCoord87 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
                float2 appendResult132 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
                float4 _SpriteRect_Instance = UNITY_ACCESS_INSTANCED_PROP(_SpriteRect_arr, _SpriteRect);
                float4 break21_g4 = _SpriteRect_Instance;
                float2 appendResult4_g4 = (float2(break21_g4.x , break21_g4.y));
                float2 appendResult6_g4 = (float2(break21_g4.z , break21_g4.w));
                float2 break9_g4 = ( ( ( IN.texcoord.xy * appendResult132 ) - appendResult4_g4 ) / appendResult6_g4 );
                float lerpResult12_g4 = lerp( break9_g4.x , ( 1.0 - break9_g4.x ) , _FlipX);
                float2 appendResult10_g4 = (float2(lerpResult12_g4 , break9_g4.y));
                float2 lerpResult138 = lerp( texCoord87 , appendResult10_g4 , _UseRectUV);
                float2 uv143 = lerpResult138;
                float2 appendResult141 = (float2(_RadiusCenterX , _RadiusCenterY));
                float dissolveRadius151 = _DissolveRadius;
                float dissolveLength152 = _DissolveLength;
                float temp_output_21_0_g5 = dissolveLength152;
                float2 appendResult96 = (float2(ase_worldPos.x , ase_worldPos.y));
                float2 panner124 = ( 1.0 * _Time.y * _DissolvePanSpeed + ( appendResult96 * _PatternScale ));
                #if defined(_DISSOLVETYPE_RADIUS)
                float staticSwitch93 = ( tex2D( _NoiseTex, ( _PatternScale * panner126 ) ).r + -( ( length( ( uv143 - appendResult141 ) ) - ( dissolveRadius151 - temp_output_21_0_g5 ) ) / temp_output_21_0_g5 ) );
                #elif defined(_DISSOLVETYPE_TEXBASED)
                float staticSwitch93 = ( tex2D( _NoiseTex, panner124 ).r + ( ( tex2D( _DissolveGuideTex, uv143 ).r - ( dissolveRadius151 - dissolveLength152 ) ) / dissolveLength152 ) );
                #else
                float staticSwitch93 = ( tex2D( _NoiseTex, ( _PatternScale * panner126 ) ).r + -( ( length( ( uv143 - appendResult141 ) ) - ( dissolveRadius151 - temp_output_21_0_g5 ) ) / temp_output_21_0_g5 ) );
                #endif
                float smoothstepResult42 = smoothstep( 0.0 , _DissolveSoftness , staticSwitch93);
                float temp_output_41_0 = saturate( smoothstepResult42 );
                float smoothstepResult49 = smoothstep( _EdgeValue , ( _EdgeValue + _EdgeSoftness ) , staticSwitch93);
                float temp_output_59_0 = saturate( ( temp_output_41_0 * ( 1.0 - smoothstepResult49 ) ) );
                float4 lerpResult91 = lerp( temp_output_78_0 , _EdgeColor , temp_output_59_0);
                #if defined(_EDGEBLEND_ALPHABLEND)
                float4 staticSwitch92 = lerpResult91;
                #elif defined(_EDGEBLEND_ADDITIVE)
                float4 staticSwitch92 = ( temp_output_78_0 + ( temp_output_59_0 * _EdgeColor ) );
                #else
                float4 staticSwitch92 = lerpResult91;
                #endif
                float4 appendResult40 = (float4(staticSwitch92.rgb , ( tex2DNode5.a * temp_output_41_0 )));
                float4 appendResult122 = (float4(( noisePattern117 * (appendResult40).xyz ) , (appendResult40).w));
                

                half4 color = ( IN.color * appendResult122 );

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
    CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19201
Node;AmplifyShaderEditor.CommentaryNode;116;-1802.621,-1648.134;Inherit;False;1803.927;660.815;Comment;11;114;113;112;111;110;109;108;107;106;105;117;NoisePattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;94;-3157.627,-1255.662;Inherit;False;1124.282;713.9672;Comment;9;81;35;86;89;90;126;153;154;157;Radius Dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.SmoothstepOpNode;49;-883.8997,65.50784;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;53;-668.2249,66.04282;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1389.426,149.443;Inherit;False;Property;_EdgeSoftness;EdgeSoftness;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1103.601,124.47;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1358.425,76.44285;Inherit;False;Property;_EdgeValue;EdgeValue;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-297.0995,-263.5509;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;59;-332.6537,-34.4746;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-486.3017,-599.6647;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;41;-610.0541,-237.8656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-476.1532,-34.67459;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;54;-370.3588,172.0972;Inherit;False;Property;_EdgeColor;EdgeColor;20;1;[HDR];Create;True;0;0;0;False;0;False;2.828427,2.029812,1.209014,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;91;226.4028,-137.1306;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;154.945,-584.6371;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;992.0846,-317.2522;Inherit;False;FLOAT4;4;0;FLOAT3;1,0,0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;92;718.0056,-337.5866;Inherit;False;Property;_EdgeBlend;EdgeBlend;21;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;AlphaBlend;Additive;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;93;-1776.477,-544.877;Inherit;False;Property;_DissolveType;DissolveType;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;Radius;TexBased;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;89;-3044.399,-961.9592;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;90;-2857.703,-937.5826;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-2616.896,-1007.878;Inherit;False;Property;_PatternScale;PatternScale;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-2866.725,-175.4979;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-1409.373,-1513.346;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;110;-714.1506,-1368.626;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-916.1506,-1347.626;Inherit;False;Property;_NoiseMin1;NoiseMin;8;0;Create;True;0;0;0;False;0;False;-1.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;112;-1572.772,-1574.346;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;113;-1752.621,-1598.134;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;114;-947.1506,-1272.626;Inherit;False;Property;_NoiseMax1;NoiseMax;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;99;-2431.665,-202.8896;Inherit;True;Property;_TextureSample0;Texture Sample 0;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;81;-2663.767,-1205.662;Inherit;True;Property;_NoiseTex;NoiseTex;4;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;106;-1055.47,-1536.778;Inherit;True;Property;_NoiseMask1;NoiseMask;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;-1605.373,-1438.346;Inherit;False;Property;_NoiseTiling1;NoiseTiling;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-435.785,-1361.999;Inherit;False;noisePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;120;1172.593,-383.2383;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;121;1186.608,-222.857;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;1183.636,-464.3752;Inherit;False;117;noisePattern;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;1398.373,-437.7367;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;122;1591.454,-334.9681;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;109;-1248.63,-1512.362;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;108;-1439.63,-1362.362;Inherit;False;Property;_NoiseSpeed1;NoiseSpeed;7;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;125;-2885.366,11.54383;Inherit;False;Property;_DissolvePanSpeed;DissolvePanSpeed;6;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.WorldPosInputsNode;95;-3236.811,-199.6975;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;96;-3050.115,-175.3209;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;124;-2658.49,-60.9002;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;126;-2599.434,-926.1683;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;-828.062,-495.7421;Inherit;True;Property;_test;test;0;0;Create;True;0;0;0;False;0;False;-1;7a170cdb7cc88024cb628cfcdbb6705c;7a170cdb7cc88024cb628cfcdbb6705c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;86;-3037.334,-775.61;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-15.49266,145.7077;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-3495.956,-433.099;Inherit;False;Property;_RadiusCenterX;RadiusCenterX;13;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-3489.956,-352.099;Inherit;False;Property;_RadiusCenterY;RadiusCenterY;14;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;141;-3311.956,-409.099;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexelSizeNode;131;-4747.739,-678.9379;Inherit;False;-1;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;132;-4565.736,-606.9378;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;133;-4590.742,-466.0577;Inherit;False;InstancedProperty;_SpriteRect;SpriteRect;22;1;[PerRendererData];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;127;-4372.401,-607.4377;Inherit;False;SRectUV;0;;4;8a9219883f02c404d93cd88cc4103060;0;2;16;FLOAT2;0,0;False;20;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;138;-3867.3,-638.0725;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-4121.498,-587.6378;Inherit;False;Property;_UseRectUV;UseRectUV;2;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-4151.442,-711.9469;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;143;-3706.556,-638.9611;Inherit;False;uv;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1039.924,-167.9791;Inherit;False;Property;_DissolveSoftness;DissolveSoftness;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-2310.35,-722.5093;Inherit;False;151;dissolveRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-2306.35,-641.5093;Inherit;False;152;dissolveLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-4996.393,159.066;Inherit;False;Property;_DissolveRadius;DissolveRadius;15;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-4958.093,238.4658;Inherit;False;Property;_DissolveLength;DissolveLength;16;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;151;-4755.904,160.8437;Inherit;False;dissolveRadius;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;-3231.391,290.8321;Inherit;False;143;uv;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-3121.179,594.422;Inherit;False;152;dissolveLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;-3138.179,467.4219;Inherit;False;151;dissolveRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;148;-2863.3,496.7604;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;145;-2434.809,573.3646;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;147;-2644.507,409.5522;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;157;-2307.345,-934.6534;Inherit;False;SRadiusDissolve;-1;;5;a318621bd65470b43ac73e4fd821eb8a;0;6;22;FLOAT2;0,0;False;17;SAMPLER2D;;False;18;FLOAT;1;False;19;FLOAT2;0,0;False;20;FLOAT;0.5;False;21;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;158;-2065.331,-13.0172;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;162;-5084.323,-525.2783;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-4733.191,241.2049;Inherit;False;dissolveLength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-813.7785,-235.7707;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;101;-3011.913,264.7142;Inherit;True;Property;_DissolveGuideTex;DissolveGuideTex;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;79;-770.9401,-679.2653;Inherit;False;Property;_FillColor;FillColor;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;163;1608.22,-549.5604;Inherit;False;0;0;_Color;Shader;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;160;1927.984,-341.6814;Float;False;True;-1;2;ASEMaterialInspector;0;3;AmplifyShaders/UI EmissiveDissolve;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;1763.782,-436.2062;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;165;1558.782,-731.2062;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;49;0;93;0
WireConnection;49;1;51;0
WireConnection;49;2;57;0
WireConnection;53;0;49;0
WireConnection;57;0;51;0
WireConnection;57;1;52;0
WireConnection;39;0;5;4
WireConnection;39;1;41;0
WireConnection;59;0;58;0
WireConnection;78;0;79;0
WireConnection;78;1;5;0
WireConnection;41;0;42;0
WireConnection;58;0;41;0
WireConnection;58;1;53;0
WireConnection;91;0;78;0
WireConnection;91;1;54;0
WireConnection;91;2;59;0
WireConnection;56;0;78;0
WireConnection;56;1;55;0
WireConnection;40;0;92;0
WireConnection;40;3;39;0
WireConnection;92;1;91;0
WireConnection;92;0;56;0
WireConnection;93;1;157;0
WireConnection;93;0;158;0
WireConnection;90;0;89;1
WireConnection;90;1;89;2
WireConnection;98;0;96;0
WireConnection;98;1;35;0
WireConnection;105;0;112;0
WireConnection;105;1;107;0
WireConnection;110;0;106;1
WireConnection;110;1;111;0
WireConnection;110;2;114;0
WireConnection;112;0;113;1
WireConnection;112;1;113;2
WireConnection;99;0;81;0
WireConnection;99;1;124;0
WireConnection;106;0;81;0
WireConnection;106;1;109;0
WireConnection;117;0;110;0
WireConnection;120;0;40;0
WireConnection;121;0;40;0
WireConnection;123;0;118;0
WireConnection;123;1;120;0
WireConnection;122;0;123;0
WireConnection;122;3;121;0
WireConnection;109;0;105;0
WireConnection;109;2;108;0
WireConnection;96;0;95;1
WireConnection;96;1;95;2
WireConnection;124;0;98;0
WireConnection;124;2;125;0
WireConnection;126;0;90;0
WireConnection;126;2;125;0
WireConnection;5;0;162;0
WireConnection;86;0;143;0
WireConnection;86;1;141;0
WireConnection;55;0;59;0
WireConnection;55;1;54;0
WireConnection;141;0;139;0
WireConnection;141;1;140;0
WireConnection;131;0;162;0
WireConnection;132;0;131;3
WireConnection;132;1;131;4
WireConnection;127;16;132;0
WireConnection;127;20;133;0
WireConnection;138;0;87;0
WireConnection;138;1;127;0
WireConnection;138;2;137;0
WireConnection;143;0;138;0
WireConnection;151;0;25;0
WireConnection;148;0;156;0
WireConnection;148;1;155;0
WireConnection;145;0;147;0
WireConnection;145;1;155;0
WireConnection;147;0;101;1
WireConnection;147;1;148;0
WireConnection;157;22;126;0
WireConnection;157;17;81;0
WireConnection;157;18;35;0
WireConnection;157;19;86;0
WireConnection;157;20;153;0
WireConnection;157;21;154;0
WireConnection;158;0;99;1
WireConnection;158;1;145;0
WireConnection;152;0;26;0
WireConnection;42;0;93;0
WireConnection;42;2;43;0
WireConnection;101;1;144;0
WireConnection;160;0;164;0
WireConnection;164;0;165;0
WireConnection;164;1;122;0
ASEEND*/
//CHKSM=815D24EDF6A547F77EBE4AA2FD1DE03A52E33ECC